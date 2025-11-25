#!/bin/zsh
# Streams all commands from Valkey and logs them to a file with timestamps in ./log directory.

LOGDIR="./log"
LOGFILE="$LOGDIR/valkey-monitor.log"

# Ensure directory exists
if [ ! -d "$LOGDIR" ]; then
    mkdir -p "$LOGDIR"
fi
# Ensure log file exists
if [ ! -f "$LOGFILE" ]; then
    touch "$LOGFILE"
fi

# Run MONITOR and prepend timestamps using date
valkey-cli MONITOR | while read -r line; do
    echo "$(date '+[%Y-%m-%d %H:%M:%S]') $line"
done >> "$LOGFILE"
