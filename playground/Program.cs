using System;
using System.Threading.Tasks;
using Valkey.Glide;
using Valkey.Glide.Pipeline;
using static Valkey.Glide.ConnectionConfiguration;

namespace ValkeyPlayground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Valkey GLIDE Playground ===");
            
            try
            {
                // Connect to local Valkey/Redis server
                var config = new StandaloneClientConfigurationBuilder()
                    .WithAddress("localhost", 6379)
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                    .Build();
                
                using var client = await GlideClient.CreateClient(config);
                Console.WriteLine("✓ Connected to localhost:6379");
                
                await RunDemo(client);
                
                Console.WriteLine("\n=== Playground Complete ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        static async Task RunDemo(GlideClient client)
        {
            Console.WriteLine("Testing LMPOP commands with cluster client...");
            
            try
            {
                // Create a cluster client to test
                var clusterConfig = new ClusterClientConfigurationBuilder()
                    .WithAddress("localhost", 6379)
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                    .Build();
                
                using var clusterClient = await GlideClusterClient.CreateClient(clusterConfig);
                Console.WriteLine("✓ Connected to cluster");
                
                // Test the multi-key ListLeftPop with hash slot prefix
                string prefix = "{listKey}-";
                string key1 = prefix + "key1-" + Guid.NewGuid();
                string key2 = prefix + "key2-" + Guid.NewGuid();
                
                // Clean up first
                await clusterClient.KeyDeleteAsync([key1, key2]);
                
                // Add some data to key2
                await clusterClient.ListRightPushAsync(key2, ["lmpop1", "lmpop2", "lmpop3"]);
                Console.WriteLine("Added 3 elements to key2");
                
                // Test LMPOP with cluster client
                var result1 = await clusterClient.ListLeftPopAsync([key1, key2], 2);
                Console.WriteLine($"Cluster ListLeftPop([key1, key2], 2): Key={result1.Key}, Values=[{string.Join(", ", result1.Values)}], IsNull={result1.IsNull}");
                
                await clusterClient.KeyDeleteAsync([key1, key2]);
                // Cluster client will be disposed by using statement
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cluster test: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            // Also test with standalone client
            try
            {
                string key1 = "test-lmpop-key1-" + Guid.NewGuid();
                string key2 = "test-lmpop-key2-" + Guid.NewGuid();
                
                await client.KeyDeleteAsync([key1, key2]);
                await client.ListRightPushAsync(key2, ["lmpop1", "lmpop2", "lmpop3"]);
                
                var result = await client.ListLeftPopAsync([key1, key2], 2);
                Console.WriteLine($"Standalone ListLeftPop([key1, key2], 2): Key={result.Key}, Values=[{string.Join(", ", result.Values)}], IsNull={result.IsNull}");
                
                await client.KeyDeleteAsync([key1, key2]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during standalone test: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
