#!/bin/bash

# List .NET Core Processes
ps -ef | grep "[d]otnet"

# Create Dump Using `createdump`
dotnet --list-runtimes
sudo ln -s /usr/share/dotnet/shared/Microsoft.NETCore.App/9.0.0/createdump /usr/bin/createdump
createdump -f ~/coredump_Mini.%p --normal <PID>
createdump -f ~/coredump_Heap.%p --withheap <PID>
createdump -f ~/coredump_Triage.%p --triage <PID>
createdump -f ~/coredump_Full.%p --full <PID>

# Create Dump Using `dotnet-dump`
dotnet tool install -g dotnet-dump
dotnet-dump collect -o ~/coredump_Mini --type Mini -p <PID>
dotnet-dump collect -o ~/coredump_Heap --type Heap -p <PID>
dotnet-dump collect -o ~/coredump_Triage --type Triage -p <PID>
dotnet-dump collect -o ~/coredump_Full --type Full -p <PID>

# Create Dump On Crash
export DOTNET_DbgEnableMiniDump=1
export DOTNET_DbgMiniDumpName=/tmp/coredump_%t.%p
export DOTNET_DbgMiniDumpType=1
export DOTNET_DbgMiniDumpType=2
export DOTNET_DbgMiniDumpType=3
export DOTNET_DbgMiniDumpType=4

# Create Dump Using `ProcDump`
sudo apt-get install -y -qq wget apt-transport-https
wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get -qq update
sudo apt-get install -y -qq procdump
procdump -s 0 <PID> ~
procdump -s 5 -n 3 <PID> ~
procdump -s 0 -c 90 <PID> ~
procdump -s 0 -m 1024 <PID> ~
procdump -s 0 -tc 16 <PID> ~
procdump -s 0 -fc 256 <PID> ~
sudo procdump -s 0 -e -f System.* <PID> ~