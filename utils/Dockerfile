ARG SERVICE_TYPE
ARG SERVICE_VERSION
FROM ${SERVICE_TYPE}:${SERVICE_VERSION}

RUN apt-get update && apt-get install -y python3 python3-pip && rm -rf /var/lib/apt/lists/*

COPY valkey-glide/utils/cluster_manager.py /usr/local/bin/cluster_manager.py
RUN chmod +x /usr/local/bin/cluster_manager.py

CMD ["sh", "-c", "/usr/local/bin/cluster_manager.py $CLUSTER_MANAGER_ARGS && tail -f /dev/null"]
