# Load .NET Core Dumps In LLDB On Linux

This article describes the steps to load a [core dump](https://en.wikipedia.org/wiki/Core_dump) from .NET Core application running on Linux in [LLDB](https://lldb.llvm.org/). The debugger and the [SOS Debugging Extension](https://learn.microsoft.com/dotnet/core/diagnostics/sos-debugging-extension) can be used for .NET Core application troubleshooting and analysis. The extension provides additional debugger commands, which allows you to view information about managed code, managed heaps and runtime internal data types.

## Initial Configuration

In order to load .NET Core dumps in LLDB, you have to configure your environment first.

* Install LLDB:

```sudo apt-get install -y -qq lldb```

If you encounter `ModuleNotFoundError: No module named 'lldb.embedded_interpreter'` error while running `lldb -P`, use the following:

```sudo mkdir -p /usr/lib/local/lib/python3.11 && sudo ln -s /usr/lib/llvm-14/lib/python3.11/dist-packages /usr/lib/local/lib/python3.11/dist-packages```

> [!NOTE]
> You may have to adjust the link source and target based on the output of the `lldb -P` command in your environment.

* Install SOS Debugging Extension:

Method 1 (Using `dotnet-debugger-extensions`, Recommended):

```
dotnet tool install --global dotnet-debugger-extensions
~/.dotnet/tools/dotnet-debugger-extensions install
```

Method 2 (Using `dotnet-sos`):

```
dotnet tool install --global dotnet-sos
~/.dotnet/tools/dotnet-sos install
```

* Install Symbol Downloader (`dotnet-symbol`):

```dotnet tool install --global dotnet-symbol```

## Download Dump Symbols

There are several types of symbol information available for download - symbol files (.pdb, .dbg, .dwarf), module files (.dll, .so, .dylib) and special debugging modules (DAC, DBI, SOS).

* Download All Symbols:

```~/.dotnet/tools/dotnet-symbol -o ~/Symbols <CoreDumpFile>```

> [!NOTE]
> The `-o` option specifies the symbol output folder `~/Symbols`.

* Minimum Symbol Download:

```dotnet-symbol --host-only --debugging <CoreDumpFile>```

> [!NOTE]
> If the `-o` option is not specified, the symbol output folder is the same as the `<CoreDumpFile>` location.

* Clear Download Symbols Cache

```rm -rf ~/.dotnet/symbolcache```

## Load Dump In LLDB

To load a .NET Core dump in LLDB use the following:

```lldb --core <CoreDumpFile> dotnet```

## Load Dump Symbols In LLDB

* Load Symbols From Folder:

```
setsymbolserver -directory /home/<user>/Symbols
loadsymbols
```

* Download Symbols Online:

```
setsymbolserver -ms
loadsymbols
```

* Change LLDB Symbol Settings:

To persists the symbol source settings between LLDB sessions edit `.lldbinit` using:

```nano ~/.lldbinit```

and update existing or add new `setsymbolserver` source.

## Load Dump From Another Machine

You can load a core dump from another machine for analysis. In this case, both machines (core dump source, core dump analysis) must have the same architecture and Linux distributions. If the distributions differ you need to get the following files from the source machine:

* dotnet (application host)
* libcoreclr.so
* libmscordaccore.so

The source files can be placed either in the same folder as the core dump file or in a different folder. When using a different folder use the `setclrpath <path>` command.

> [!NOTE]
> Cross platform architecture (e.g. source: ARM64, analysis: AMD64) dump analysis is not supported.

## References

* [Article Script](https://github.com/meriffa/blog/blob/main/Resources/Scripts/Load%20.NET%20Core%20Dumps%20In%20LLDB%20On%20Linux.sh)
* [dotnet-debugger-extensions](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-debugger-extensions)
* [dotnet-sos](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-sos)
* [dotnet-symbol](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-symbol)

## Attributes

* Category: Advanced .NET
* Tags: .NET Core, Core Dump, Memory Dump, Linux