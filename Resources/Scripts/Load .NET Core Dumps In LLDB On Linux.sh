#!/bin/bash

# Initial Configuration
sudo apt-get install -y -qq lldb
lldb -P
sudo mkdir -p /usr/lib/local/lib/python3.11 && sudo ln -s /usr/lib/llvm-14/lib/python3.11/dist-packages /usr/lib/local/lib/python3.11/dist-packages
dotnet tool install --global dotnet-debugger-extensions
~/.dotnet/tools/dotnet-debugger-extensions install
dotnet tool install --global dotnet-sos
~/.dotnet/tools/dotnet-sos install
dotnet tool install --global dotnet-symbol

# Download Dump Symbols
~/.dotnet/tools/dotnet-symbol -o ~/Symbols <CoreDumpFile>
dotnet-symbol --host-only --debugging <CoreDumpFile>
rm -rf ~/.dotnet/symbolcache

# Load Dump In `LLDB`
lldb --core <CoreDumpFile> dotnet

## Load Dump Symbols In `LLDB`
setsymbolserver -directory /home/<user>/Symbols
loadsymbols
setsymbolserver -ms
loadsymbols
nano ~/.lldbinit