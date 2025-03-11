# SOS Extension Commands

## Prerequisites

```
sudo apt-get update -qq
sudo apt-get install cmake clang gdb gettext git libicu-dev liblldb-dev libunwind8 lldb python3-lldb llvm make liblttng-ust-dev tar wget zip -y
```

## Download .NET Core Diagnostics Repository

```
git clone -b main https://github.com/meriffa/diagnostics.git ~/Development/SOS
```

## Build .NET Core Diagnostics

```
cd ~/Development/SOS
./build.sh -debug
ls ~/Development/SOS/artifacts/bin/linux.x64.Debug
./build.sh -release
ls ~/Development/SOS/artifacts/bin/linux.x64.Release
```

## Create New SOS Extension Command

* Start Visual Studio Code: ```./start-vscode.sh```
* Create new command class [~/Development/SOS/src/Microsoft.Diagnostics.ExtensionCommands/ByteZoo.Blog.Commands].

## Deploy New SOS Extension Command

```echo "plugin load /home/marian/Development/SOS/artifacts/bin/linux.x64.Debug/libsosplugin.so" >> ~/.lldbinit```
or
```cp /path/to/<ExtensionLibrary.dll> ~/.dotnet/sos/extensions```
or
```export DOTNET_DIAGNOSTIC_EXTENSIONS=/path/to/<ExtensionLibrary.dll>```

## Test New SOS Extension Command

```
lldb -c CoreDump_Full.<PID> dotnet -o "dhe -d FreeSummary" -o "exit"
~/Development/SOS/artifacts/bin/dotnet-dump/Debug/net8.0/publish/dotnet-dump analyze CoreDump_Full.<PID> -c "dhe -d FreeSummary" -c "exit"
```

## Troubleshooting

```
sosflush
logging enable
setsymbolserver -ms
loadsymbols
setsymbolserver -directory <SymbolsFolder>
setclrpath
setclrpath <TargetProcessRuntimeFolder|CoreDumpRuntimeFolder>
sethostruntime
sethostruntime -clear
sethostruntime -major <Number> <DebuggerHostRuntimeFolder>
```

## DumpHeapExport

```dhe -d String -min 300 -max 600```
```dhe -d StringSummary -min 300 -max 600```
```dhe -d Free -min 25```
```dhe -d FreeSummary -min 25```
```dhe -d Object -type ByteZoo.Blog.App -t json```
```dhe -d Object -type ByteZoo.Blog.App -t json -o ./output.json```
```dhe -d Object -type ByteZoo.Blog.App -t csv```
```dhe -d Object -type ByteZoo.Blog.App -t tab```
```dhe -d ObjectSummary -type ByteZoo.Blog.App```
```dhe -d ObjectFragmentationSummary -minFragmentationBlockSize 25```
```dhe -d ThinLock```

## DumpEnumValues

```dev -mt 7FC6497EEB80```
```dev -type ByteZoo.Blog.Common.Models.Business.EmployeeEventType```
```dev -type ByteZoo.Blog.Common.Models.Business.EmployeeEventType -module ByteZoo.Blog.Common.dll```

## References

* [Single File Diagnostic Tools](https://github.com/dotnet/diagnostics/blob/main/documentation/single-file-tools.md)
* [Private Runtime Build Testing](https://github.com/dotnet/diagnostics/blob/main/documentation/privatebuildtesting.md)