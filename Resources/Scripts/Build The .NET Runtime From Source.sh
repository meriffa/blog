#!/bin/bash

# Prerequisites
sudo apt-get update -qq
sudo apt-get install build-essential curl git gettext locales cmake llvm clang lld lldb liblldb-dev libunwind8-dev libicu-dev liblttng-ust-dev libssl-dev libkrb5-dev pigz python-is-python3 cpio -y
sudo localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8

# Download The .NET Runtime Source
git clone -b main https://github.com/dotnet/runtime.git ~/CLR

# Build The .NET Runtime From Source
cd ~/CLR
./build.sh -subset clr -configuration Debug
ls ./artifacts/bin/coreclr/linux.x64.Debug
./build.sh -subset clr -configuration Release
ls ./artifacts/bin/coreclr/linux.x64.Release
./build.sh -subset libs -configuration Debug
./build.sh -subset libs -configuration Release
ls ./artifacts/bin
./src/tests/build.sh -debug -p:LibrariesConfiguration=Debug -generatelayoutonly
ls ./artifacts/tests/coreclr/linux.x64.Debug/Tests/Core_Root
./src/tests/build.sh -release -p:LibrariesConfiguration=Release -generatelayoutonly
ls ./artifacts/tests/coreclr/linux.x64.Release/Tests/Core_Root
./build.sh -subset clr+libs+host+packs -configuration Release                                   # Generate Shipping Packs (all runtime components, installer packages, dotnet host)
ls ./artifacts/packages/Release/Shipping
./build.sh -clean                                                                               # Cleanup build

# Run Application Using The Local .NET Runtime Build
export CORE_ROOT=~/CLR/artifacts/tests/coreclr/linux.x64.Debug/Tests/Core_Root
export CORE_ROOT=~/CLR/artifacts/tests/coreclr/linux.x64.Release/Tests/Core_Root
$CORE_ROOT/corerun <Application.dll> [ApplicationArgument1] [ApplicationArgument2] ... [ApplicationArgumentN]

# Debugging The Local .NET Runtime Using LLDB
lldb -- $CORE_ROOT/corerun <Application.dll> [ApplicationArgument1] [ApplicationArgument2] ... [ApplicationArgumentN]
process launch -s
process continue
bpmd ByteZoo.Blog.App.dll ByteZoo.Blog.App.Controllers.Concepts.StringController.Execute
breakpoint set -n coreclr_execute_assembly
process handle -s false SIGUSR1

# Debugging The Local .NET Runtime Managed Code Using Visual Studio Code
cd ~/CLR/src/libraries/System.Console/src
code .

# Analyze Thread.Interrupt Source Code
# - Thread.Interrupt() -> [./src/coreclr/System.Private.CoreLib/src/System/Threading/Thread.CoreCLR.cs] -> ThreadNative_Interrupt
#   - cd ~/CLR/src/coreclr && grep -rl "ThreadNative_Interrupt" . -> [./src/coreclr/vm/comsynchronizable.cpp] -> thread->UserInterrupt -> Thread::UserInterrupt
#     - cd ~/CLR/src/coreclr && grep -rl "::UserInterrupt" . -> [./src/coreclr/vm/threads.cpp] -> Alert() -> QueueUserAPC(UserInterruptAPC)
#       - QueueUserAPC() -> [Linux] -> PAL QueueUserAPC Internal Implementation
#       - QueueUserAPC() -> [Windows] -> QueueUserAPC2() -> Asynchronous Procedure Call (APC) Queue

# .NET SDK
git clone -b main https://github.com/dotnet/sdk.git ~/SDK
cd ~/SDK
./build.sh
ls ./artifacts/bin/redist/Debug/dotnet
source ./eng/dogfood.sh
which dotnet
dotnet --version
MSBuild --version