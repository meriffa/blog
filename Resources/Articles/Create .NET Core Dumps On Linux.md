# Create .NET Core Dumps On Linux

This article describes various methods to create a [core dump](https://en.wikipedia.org/wiki/Core_dump) of .NET Core application running on Linux.

## List .NET Core Processes

In order to create a core dump, you need to obtain the Process ID (\<PID\>) of the target .NET Core application first. To list all .NET Core applications use the following:

```ps -ef | grep "[d]otnet"```

Sample output:

```
marian      718     666  0 09:18 pts/0    00:00:00 dotnet /home/marian/ByteZoo.Blog.App/ByteZoo.Blog.App.dll Pause
```

## Create Dump Using `createdump`

The `createdump` tool is part of the .NET Core Runtime and does not require any additional components. To find the .NET Core Runtime installation location use the following:

```dotnet --list-runtimes```

Output for ASP.NET Core Runtime 9.0:

```
Microsoft.AspNetCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
Microsoft.NETCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```

E.g. To create a symbolic link for `createdump` in .NET Core 9.0 use the following:

```sudo ln -s /usr/share/dotnet/shared/Microsoft.NETCore.App/9.0.0/createdump /usr/bin/createdump```

* Create Mini Dump:

```createdump -f ~/coredump_Mini.%p --normal <PID>```

Sample output for process id 718:

```
[createdump] Gathering state for process 718 dotnet
[createdump] Writing minidump to file /home/marian/coredump_Mini.718
[createdump] Written 10039296 bytes (2451 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 130ms
```

* Create Heap Dump:

```createdump -f ~/coredump_Heap.%p --withheap <PID>```

Sample output for process id 718:

```
[createdump] Gathering state for process 718 dotnet
[createdump] Writing minidump with heap to file /home/marian/coredump_Heap.718
[createdump] Written 97521664 bytes (23809 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 180ms
```

* Create Triage Dump:

```createdump -f ~/coredump_Triage.%p --triage <PID>```

Sample output for process id 718:

```
[createdump] Gathering state for process 718 dotnet
[createdump] Writing triage minidump to file /home/marian/coredump_Triage.718
[createdump] Written 10051584 bytes (2454 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 118ms
```

* Create Full Dump:

```createdump -f ~/coredump_Full.%p --full <PID>```

Sample output for process id 718:

```
[createdump] Gathering state for process 718 dotnet
[createdump] Writing full dump to file /home/marian/coredump_Full.718
[createdump] Written 183238656 bytes (44736 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 272ms
```

> [!NOTE]
> The `-f` option specifies the dump output file. The \<PID\> value is the target process id.

## Create Dump Using `dotnet-dump`

The `dotnet-dump` tool is installed using the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) `dotnet tool install` command. This command is part of the .NET Core SDK. For details on how to install the .NET Core SDK see [Install .NET Core On Linux](https://medium.com/@meriffa/install-net-core-on-linux-aedad163b065).

To install the `dotnet-dump` tool use the following:

```dotnet tool install -g dotnet-dump```

> [!NOTE]
> There is a `dotnet-dump` direct download option.

* Create Mini Dump:

```dotnet-dump collect -o ~/coredump_Mini --type Mini -p <PID>```

Sample output for process id 718:

```
Writing dump to /home/marian/coredump_Mini
Complete
```

* Create Heap Dump:

```dotnet-dump collect -o ~/coredump_Heap --type Heap -p <PID>```

Sample output for process id 718:

```
Writing dump with heap to /home/marian/coredump_Heap
Complete
```

* Create Triage Dump:

```dotnet-dump collect -o ~/coredump_Triage --type Triage -p <PID>```

Sample output for process id 718:

```
Writing triage dump to /home/marian/coredump_Triage
Complete
```

* Create Full Dump:

```dotnet-dump collect -o ~/coredump_Full --type Full -p <PID>```

Sample output for process id 718:

```
Writing full to /home/marian/coredump_Full
Complete
```

> [!NOTE]
> The `-o` option specifies the dump output file. The \<PID\> value is the target process id.

## Create Dump On Crash

The dump creation on crash is configured using environment variables.

To enable dump generation use the following:

```export DOTNET_DbgEnableMiniDump=1```

To specify dump output location:

```export DOTNET_DbgMiniDumpName=/tmp/coredump_%t.%p```

> [!NOTE]
> `%t` is the date & time of dump and `%p` is the process id.

* Create Mini Dump:

```export DOTNET_DbgMiniDumpType=1```

Sample output for process id 1197:

```
[2024-12-12 09:21:22.6709]: ByteZoo.Blog application started.
Unhandled exception. ByteZoo.Blog.Common.Exceptions.UnhandledException: Unhandled application exception.
   at ByteZoo.Blog.App.Controllers.ExceptionController.Execute() in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/ExceptionController.cs:line 25
   at ByteZoo.Blog.App.Controllers.Controller.Execute(IServiceProvider services) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/Controller.cs:line 26
   at ByteZoo.Blog.App.Program.<>c__DisplayClass0_0.<Main>b__1(Controller i) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 33
   at CommandLine.ParserResultExtensions.WithParsed[T](ParserResult`1 result, Action`1 action)
   at ByteZoo.Blog.App.Program.Main(String[] args) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 31
[createdump] Gathering state for process 1197 dotnet
[createdump] Crashing thread 04ad signal 6 (0006)
[createdump] Writing minidump to file /tmp/coredump_1734013282.1197
[createdump] Written 11137024 bytes (2719 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 126ms
Aborted
```

* Create Heap Dump:

```export DOTNET_DbgMiniDumpType=2```

Sample output for process id 1210:

```
[2024-12-12 09:22:16.2656]: ByteZoo.Blog application started.
Unhandled exception. ByteZoo.Blog.Common.Exceptions.UnhandledException: Unhandled application exception.
   at ByteZoo.Blog.App.Controllers.ExceptionController.Execute() in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/ExceptionController.cs:line 25
   at ByteZoo.Blog.App.Controllers.Controller.Execute(IServiceProvider services) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/Controller.cs:line 26
   at ByteZoo.Blog.App.Program.<>c__DisplayClass0_0.<Main>b__1(Controller i) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 33
   at CommandLine.ParserResultExtensions.WithParsed[T](ParserResult`1 result, Action`1 action)
   at ByteZoo.Blog.App.Program.Main(String[] args) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 31
[createdump] Gathering state for process 1210 dotnet
[createdump] Crashing thread 04ba signal 6 (0006)
[createdump] Writing minidump with heap to file /tmp/coredump_1734013336.1210
[createdump] Written 99106816 bytes (24196 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 180ms
Aborted
```

* Create Triage Dump:

```export DOTNET_DbgMiniDumpType=3```

Sample output for process id 1223:

```
[2024-12-12 09:22:53.6307]: ByteZoo.Blog application started.
Unhandled exception. ByteZoo.Blog.Common.Exceptions.UnhandledException: Unhandled application exception.
   at ByteZoo.Blog.App.Controllers.ExceptionController.Execute() in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/ExceptionController.cs:line 25
   at ByteZoo.Blog.App.Controllers.Controller.Execute(IServiceProvider services) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/Controller.cs:line 26
   at ByteZoo.Blog.App.Program.<>c__DisplayClass0_0.<Main>b__1(Controller i) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 33
   at CommandLine.ParserResultExtensions.WithParsed[T](ParserResult`1 result, Action`1 action)
   at ByteZoo.Blog.App.Program.Main(String[] args) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 31
[createdump] Gathering state for process 1223 dotnet
[createdump] Crashing thread 04c7 signal 6 (0006)
[createdump] Writing triage minidump to file /tmp/coredump_1734013373.1223
[createdump] Written 12185600 bytes (2975 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 126ms
Aborted
```

* Create Full Dump:

```export DOTNET_DbgMiniDumpType=4```

Sample output for process id 1235:

```
[2024-12-12 09:23:26.1281]: ByteZoo.Blog application started.
Unhandled exception. ByteZoo.Blog.Common.Exceptions.UnhandledException: Unhandled application exception.
   at ByteZoo.Blog.App.Controllers.ExceptionController.Execute() in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/ExceptionController.cs:line 25
   at ByteZoo.Blog.App.Controllers.Controller.Execute(IServiceProvider services) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/Controller.cs:line 26
   at ByteZoo.Blog.App.Program.<>c__DisplayClass0_0.<Main>b__1(Controller i) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 33
   at CommandLine.ParserResultExtensions.WithParsed[T](ParserResult`1 result, Action`1 action)
   at ByteZoo.Blog.App.Program.Main(String[] args) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 31
[createdump] Gathering state for process 1235 dotnet
[createdump] Crashing thread 04d3 signal 6 (0006)
[createdump] Writing full dump to file /tmp/coredump_1734013406.1235
[createdump] Written 189595648 bytes (46288 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 244ms
Aborted
```

## Create Dump Using `ProcDump`

* Install software prerequisites:

```sudo apt-get install -y -qq wget apt-transport-https```

* Update the list of trusted keys and package repository:

```
wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get -qq update
```

* Install ProcDump:

```sudo apt-get install -y -qq procdump```

* Create a dump immediately:

```procdump -s 0 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
...
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:27:52 - INFO]: Starting monitor for process dotnet (1827)
[09:27:52 - INFO]: Trigger: Timer:1(s) on process ID: 1827
[09:27:52 - INFO]: Core dump 0 generated: /home/marian/dotnet_time_2024-12-12_09:27:52.1827
[09:27:52 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create 3 dumps 5 seconds apart:

```procdump -s 5 -n 3 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
...
Threshold (s):                          5
Number of Dumps:                        3
Output directory:                       /home/marian
[09:29:27 - INFO]: Starting monitor for process dotnet (1827)
[09:29:27 - INFO]: Trigger: Timer:1(s) on process ID: 1827
[09:29:27 - INFO]: Core dump 0 generated: /home/marian/dotnet_time_2024-12-12_09:29:27.1827
[09:29:32 - INFO]: Trigger: Timer:1(s) on process ID: 1827
[09:29:32 - INFO]: Core dump 1 generated: /home/marian/dotnet_time_2024-12-12_09:29:32.1827
[09:29:37 - INFO]: Trigger: Timer:1(s) on process ID: 1827
[09:29:38 - INFO]: Core dump 2 generated: /home/marian/dotnet_time_2024-12-12_09:29:37.1827
[09:29:38 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create a dump when the process has CPU usage >= 90%:

```procdump -s 0 -c 90 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
CPU Threshold:                          >= 90%
...
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:31:26 - INFO]: Starting monitor for process dotnet (1827)
[09:31:27 - INFO]: Trigger: CPU usage:90% on process ID: 1827
[09:31:27 - INFO]: Core dump 0 generated: /home/marian/dotnet_cpu_2024-12-12_09:31:27.1827
[09:31:27 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create a dump when the process has memory usage >= 1024 MB:

```procdump -s 0 -m 1024 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
CPU Threshold:                          n/a
Commit Threshold:                       >= 1024 MB
...
Polling Interval (ms):                  1000
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:34:13 - INFO]: Starting monitor for process dotnet (1827)
[09:34:14 - INFO]: Trigger: Commit usage:1073MB on process ID: 1827
[09:34:14 - INFO]: Core dump 0 generated: /home/marian/dotnet_commit_2024-12-12_09:34:14.1827
[09:34:14 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create a dump when the process has thread count >= 16:

```procdump -s 0 -tc 16 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
...
Thread Threshold:                       16
...
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:36:30 - INFO]: Starting monitor for process dotnet (1827)
[09:36:32 - INFO]: Trigger: Thread count:17 on process ID: 1827
[09:36:32 - INFO]: Core dump 0 generated: /home/marian/dotnet_thread_2024-12-12_09:36:32.1827
[09:36:32 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create a dump when the process has file descriptor count >= 256:

```procdump -s 0 -fc 256 <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
...
File Descriptor Threshold:              256
...
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:38:16 - INFO]: Starting monitor for process dotnet (1827)
[09:38:17 - INFO]: Trigger: File descriptors:257 on process ID: 1827
[09:38:17 - INFO]: Core dump 0 generated: /home/marian/dotnet_filedesc_2024-12-12_09:38:17.1827
[09:38:17 - INFO]: Stopping monitor for process dotnet (1827)
```

* Create a dump when the process encounters an exception of type `System.*`:

```sudo procdump -s 0 -e -f System.* <PID> ~```

Sample output for process id 1827:

```
...
Process:                                dotnet (1827)
...
Exception monitor:                      On
Exception filter:                       System.*
Polling Interval (ms):                  1000
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       /home/marian
[09:45:48 - INFO]: Starting monitor for process dotnet (1827)
[09:45:53 - INFO]: Core dump generated: /home/marian/dotnet_0_System.Exception_2024-12-12_09:45:53
[09:45:53 - INFO]: Stopping monitor for process dotnet (1827)
```

> [!NOTE]
> The `~` option specifies the dump output files folder. The \<PID\> value is the target process id. `ProcDump` creates Full Dump unless `-mc` option is specified.

## Dump Types

* Mini Dump: A small dump containing module lists, thread lists, exception information and all stacks.
* Heap Dump: A large and relatively comprehensive dump containing module lists, thread lists, all stacks, exception information, handle information and all memory except for mapped images.
* Triage Dump: Same as Mini Dump, but with Personally Identifiable Information (PII) removed (e.g. paths and passwords).
* Full Dump: The largest dump containing all memory including the module images.

## References

* [Article Source Code](https://github.com/meriffa/blog/tree/main/Sources)
* [Article Script](https://github.com/meriffa/blog/blob/main/Resources/Scripts/Create%20.NET%20Core%20Dumps%20On%20Linux.sh)
* [.NET CLI Tools](https://learn.microsoft.com/dotnet/core/diagnostics/tools-overview#cli-tools)
* [createdump](https://github.com/dotnet/coreclr/blob/master/Documentation/botr/xplat-minidump-generation.md)
* [dotnet-dump](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-dump)
* [Collect Dumps On Crash](https://learn.microsoft.com/dotnet/core/diagnostics/collect-dumps-crash)
* [ProcDump](https://github.com/microsoft/ProcDump-for-Linux)

## Attributes

* Category: Advanced .NET
* Tags: .NET Core, Core Dump, Memory Dump, Linux