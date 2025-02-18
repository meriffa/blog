#!/bin/bash

# Enable .NET Core Dump (https://learn.microsoft.com/dotnet/core/diagnostics/collect-dumps-crash)
export DOTNET_DbgEnableMiniDump=1
export DOTNET_DbgMiniDumpType=4
export DOTNET_DbgMiniDumpName=/tmp/CoreDump.%p
export DOTNET_CreateDumpDiagnostics=1

# Setup Linux Core Dump
echo "/tmp/CoreDump.%p" | sudo tee /proc/sys/kernel/core_pattern
sudo sysctl -p
cat /proc/sys/kernel/core_pattern
echo 0x0000003F > /proc/self/coredump_filter
cat /proc/self/coredump_filter
ulimit -c unlimited
ulimit -a