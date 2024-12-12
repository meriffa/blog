# Create .NET Core Dumps On Linux

This article describes various methods to create a [core dump](https://en.wikipedia.org/wiki/Core_dump) of .NET Core application running on Linux.

## List .NET Core Processes

In order to create a core dump, you need to obtain the Process ID (\<PID\>) of the target .NET Core application first. To list all .NET Core applications use the following:

```ps -ef | grep "[d]otnet"```

## Create Dump Using `createdump`

The `createdump` tool is part of the .NET Core Runtime and does not require any additional components. To find the .NET Core Runtime installation location use the following:

```dotnet --list-runtimes```

E.g. To create a symbolic link for `createdump` in .NET Core 9.0 use the following:

```sudo ln -s /usr/share/dotnet/shared/Microsoft.NETCore.App/9.0.0/createdump /usr/bin/createdump```

* Create Mini Dump:

```createdump -f ~/coredump_Mini.%p --normal <PID>```

* Create Heap Dump:

```createdump -f ~/coredump_Heap.%p --withheap <PID>```

* Create Triage Dump:

```createdump -f ~/coredump_Triage.%p --triage <PID>```

* Create Full Dump:

```createdump -f ~/coredump_Full.%p --full <PID>```

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

* Create Heap Dump:

```dotnet-dump collect -o ~/coredump_Heap --type Heap -p <PID>```

* Create Triage Dump:

```dotnet-dump collect -o ~/coredump_Triage --type Triage -p <PID>```

* Create Full Dump:

```dotnet-dump collect -o ~/coredump_Full --type Full -p <PID>```

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

* Create Heap Dump:

```export DOTNET_DbgMiniDumpType=2```

* Create Triage Dump:

```export DOTNET_DbgMiniDumpType=3```

* Create Full Dump:

```export DOTNET_DbgMiniDumpType=4```

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

* Create 3 dumps 5 seconds apart:

```procdump -s 5 -n 3 <PID> ~```

* Create a dump when the process has CPU usage >= 90%:

```procdump -s 0 -c 90 <PID> ~```

* Create a dump when the process has memory usage >= 1024 MB:

```procdump -s 0 -m 1024 <PID> ~```

* Create a dump when the process has thread count >= 16:

```procdump -s 0 -tc 16 <PID> ~```

* Create a dump when the process has file descriptor count >= 256:

```procdump -s 0 -fc 256 <PID> ~```

* Create a dump when the process encounters an exception of type `System.*`:

```sudo procdump -s 0 -e -f System.* <PID> ~```

> [!NOTE]
> The `~` option specifies the dump output files folder. The \<PID\> value is the target process id. `ProcDump` creates Full Dump unless the `-mc` option is specified.

## Dump Types

* Mini Dump: A small dump containing module lists, thread lists, exception information and all stacks.
* Heap Dump: A large and relatively comprehensive dump containing module lists, thread lists, all stacks, exception information, handle information and all memory except for mapped images.
* Triage Dump: Same as Mini Dump, but with Personally Identifiable Information (PII) removed (e.g. paths and passwords).
* Full Dump: The largest dump containing all memory including the module images.

## References

The article script is located [here](https://github.com/meriffa/blog/blob/main/Resources/Scripts/Create%20.NET%20Core%20Dumps%20On%20Linux.sh).

* [.NET CLI Tools](https://learn.microsoft.com/dotnet/core/diagnostics/tools-overview#cli-tools)
* [createdump](https://github.com/dotnet/coreclr/blob/master/Documentation/botr/xplat-minidump-generation.md)
* [dotnet-dump](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-dump)
* [Collect Dumps On Crash](https://learn.microsoft.com/dotnet/core/diagnostics/collect-dumps-crash)
* [ProcDump](https://github.com/microsoft/ProcDump-for-Linux)

## Attributes

* Category: .NET Debugging
* Tags: .NET Core, Core Dumps, Memory Dumps, Linux