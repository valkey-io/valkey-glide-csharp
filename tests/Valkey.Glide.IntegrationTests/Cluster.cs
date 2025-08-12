// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Valkey.Glide.IntegrationTests;

public class Cluster : IDisposable
{
    public string ContainerName { get; }

    public List<(string host, ushort port)> Hosts { get; private set; } = [];

    public bool IsClusterMode { get; private set; }

    public bool IsTls { get; private set; }

    public Version ServerVersion { get; private set; } = new();

    private readonly string? _utilsDir;
    private static readonly object BuildLock = new();
    private static bool s_imageBuilt = false;
    private static int s_nextPort = 7000;

    public Cluster()
    {
        ContainerName = $"test-cluster-{Guid.NewGuid():N}";
        _utilsDir = Path.Combine(GetProjectDir(), "utils");
    }

    public Cluster(List<(string host, ushort port)> hosts)
    {
        ContainerName = "";
        Hosts = [.. hosts];
        _utilsDir = null;
    }

    public void Start(bool clusterMode, int replicas = 1, bool tls = false)
    {
        IsClusterMode = clusterMode;
        IsTls = tls;

        EnsureImageBuilt();

        List<int> internalPorts = GetInternalPorts(clusterMode, replicas);
        string clusterArgs = BuildClusterManagerArgs(clusterMode, replicas, tls, internalPorts);

        string portMappings = string.Join(" ", internalPorts.Select(p => $"-p {p}:{p}"));
        string output = RunProcess("docker", $"run --rm -d --name {ContainerName} {portMappings} " +
            $"-e CLUSTER_MANAGER_ARGS=\"{clusterArgs}\" utils-cache");

        Hosts = GetDockerAssignedPorts(internalPorts);

        WaitForClusterReady();
        ServerVersion = GetServerVersion();
    }

    public void Stop() => RunProcess("docker", $"stop {ContainerName}");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Stop();
    }

    private List<int> GetInternalPorts(bool clusterMode, int replicas)
    {
        int nodeCount = clusterMode ? 3 * (1 + replicas) : 1 + replicas;
        lock (BuildLock)
        {
            List<int> ports = [.. Enumerable.Range(s_nextPort, nodeCount)];
            s_nextPort += nodeCount;
            return ports;
        }
    }

    private string BuildClusterManagerArgs(bool clusterMode, int replicas, bool tls, List<int> ports)
    {
        string replicasArg = replicas > 0 ? $"--replicas {replicas}" : "";
        string modeArg = clusterMode ? "--cluster-mode" : "";
        string tlsArg = tls ? "--tls" : "";
        string portsArg = $"-p {string.Join(" ", ports)}";

        return $"start {modeArg} {replicasArg} {tlsArg} {portsArg} --prefix {ContainerName}".Trim();
    }

    private List<(string host, ushort port)> GetDockerAssignedPorts(List<int> internalPorts)
    {
        List<(string host, ushort port)> hosts = [];

        foreach (int internalPort in internalPorts)
        {
            string output = RunProcess("docker", $"port {ContainerName} {internalPort}");
            string externalPort = output.Trim().Split(':')[1];
            hosts.Add(("127.0.0.1", ushort.Parse(externalPort)));
        }

        return hosts;
    }

    private void WaitForClusterReady()
    {
        for (int i = 0; i < 120; i++) // 2 min timeout
        {
            if (IsClusterReady())
            {
                return;
            }

            Thread.Sleep(1000);
        }

        throw new TimeoutException("Cluster failed to become ready within 2 minutes");
    }

    private bool IsClusterReady()
    {
        foreach ((string host, ushort port) in Hosts)
        {
            try
            {
                using TcpClient client = new();
                client.Connect(host, port);
                using NetworkStream stream = client.GetStream();

                byte[] request = Encoding.UTF8.GetBytes("info server\r\n");
                stream.Write(request);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer);

                if (bytesRead == 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        return true;
    }

    private Version GetServerVersion()
    {
        if (Hosts.Count == 0)
        {
            throw new Exception("No servers available");
        }

        (string host, ushort port) = Hosts[0];
        using TcpClient client = new();
        client.Connect(host, port);
        using NetworkStream stream = client.GetStream();

        byte[] request = Encoding.UTF8.GetBytes("info server\r\n");
        stream.Write(request);

        byte[] buffer = new byte[4096];
        int bytesRead = stream.Read(buffer);
        string info = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        string[] lines = info.Split('\n');
        string line = lines.FirstOrDefault(l => l.Contains("valkey_version")) ??
                     lines.First(l => l.Contains("redis_version"));

        return new Version(line.Split(':')[1].Trim());
    }

    private void EnsureImageBuilt()
    {
        lock (BuildLock)
        {
            if (!s_imageBuilt)
            {
                try
                {
                    string containers = RunProcess("docker", "ps -aq --filter name=test-cluster-");
                    if (!string.IsNullOrWhiteSpace(containers))
                    {
                        _ = RunProcess("docker", $"stop {containers.Replace('\n', ' ').Trim()}");
                        _ = RunProcess("docker", $"rm {containers.Replace('\n', ' ').Trim()}");
                    }
                    _ = RunProcess("docker", "rmi utils-cache");
                }
                catch { }

                _ = RunProcess("docker-compose", "build cache", _utilsDir);
                _ = RunProcess("docker", "tag utils-cache utils-cache");
                s_imageBuilt = true;
            }
        }
    }

    private string RunProcess(string fileName, string arguments, string? workingDirectory = null)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        if (workingDirectory != null)
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
        }

        _ = process.Start();
        process.WaitForExit();

        return process.ExitCode != 0
            ? throw new Exception($"Process failed: {process.StandardError.ReadToEnd()}")
            : process.StandardOutput.ReadToEnd();
    }

    private static string GetProjectDir()
    {
        string? projectDir = Directory.GetCurrentDirectory();
        while (!(projectDir == null || Directory.EnumerateDirectories(projectDir).Any(d => Path.GetFileName(d) == "utils")))
        {
            projectDir = Path.GetDirectoryName(projectDir);
        }

        return projectDir ?? throw new DirectoryNotFoundException("Cannot find project directory with utils folder");
    }
}
