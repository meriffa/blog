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
setclrpath                                                                                      # DAC Runtime (Target Application / Core Dump)
setclrpath <TargetProcessRuntimeFolder|CoreDumpRuntimeFolder>
sethostruntime                                                                                  # Managed SOS Code Runtime
sethostruntime -clear
sethostruntime -major <Number> <DebuggerHostRuntimeFolder>
```

## DumpHeapExport

```dhe -d String -min 300 -max 600```
```dhe -d StringSummary -min 300 -max 600```
```dhe -d Free -min 25```
```dhe -d FreeSummary -min 25```
```dhe -d Object -type ByteZoo.Blog.App -t console```
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

## DumpModulesExport

```dme```
```dme -name ByteZoo.Blog```
```dme -name ByteZoo.Blog -types```

## DumpStringsExport

```dse -start "ByteZoo.Blog"```
```dse -end " completed."```
```dse -contain "Application" -i```
```dse "bytezoo.blog.app" -i```

## DumpSingle

```dumpsingle -value 42F6E979```
```dumpsingle 7f7e4606bf30```

## DumpDouble

```dumpdouble -value 4081BF1EB851EB85```
```dumpdouble 7f7e4606bf28```

## DumpDecimal

```dumpdecimal -valueLow 0000000000060000 -valueHigh 0000001A21A278BE```
```dumpdecimal 7f7e4606bf48```

## DumpDateTime

```dumpdatetime -value 08DD60F1FFC7431C```
```dumpdatetime 7f7e4606bf58```

## DumpTimeSpan

```dumptimespan -value 00000EE3A90E8B7A```
```dumptimespan 7f7e4606bf60```

## DumpDateOnly

```dumpdateonly -value 000B47EE```
```dumpdateonly 7f7e4606bf68```

## DumpTimeOnly

```dumptimeonly -value 000000B6277E9FE0```
```dumptimeonly 7f7e4606bf70```

## DumpGCHandlesExport

```dgche -d Handles -k Strong```
```dgche -d Statistics```
```dgche -d Totals```

## References

* [Single File Diagnostic Tools](https://github.com/dotnet/diagnostics/blob/main/documentation/single-file-tools.md)
* [Private Runtime Build Testing](https://github.com/dotnet/diagnostics/blob/main/documentation/privatebuildtesting.md)