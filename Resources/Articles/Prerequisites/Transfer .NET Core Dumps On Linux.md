# Transfer .NET Core Dumps On Linux

This article describes the steps required to transfer a .NET core dump between different Linux hosts. Using different hosts is a common scenario when capturing a core dump on one Linux host (e.g. Production, Staging) and analyzing it on a different one (e.g. Development). This article assumes that the reader is already familiar with capturing and loading core dumps on Linux into LLDB. For details see [Create .NET Core Dumps On Linux](/Resources/Articles/Prerequisites/Create%20.NET%20Core%20Dumps%20On%20Linux.md) and [Load .NET Core Dumps In LLDB On Linux](/Resources/Articles/Prerequisites/Load%20.NET%20Core%20Dumps%20In%20LLDB%20On%20Linux.md).

## Prerequisites

To illustrate the core dump transfer, we will use the following scenario:

* Target Host: Arch Linux (kernel: 6.13.3, architecture: x86_64), .NET Core Runtime (version: 8.0.15, folder: /opt/dotnet/shared/Microsoft.NETCore.App)
* Analysis Host: Debian 12 (kernel: 6.1.0-27, architecture: x86_64), .NET Core Runtime (version: 9.0.4 folder: /usr/share/dotnet/shared/Microsoft.NETCore.App), .NET Core SDK (version: 9.0.203, folder: /usr/share/dotnet/sdk), LLDB (version: 14.0.6), dotnet-debugger-extensions (version: 9.0.607601), dotnet-symbol (version: 9.0.621003)

In this case, the Target Host is the Linux machine where the application runs and a core dump is created. The Analysis Host is where the core dump will be transferred and analyzed. The scenario assumes different Linux distributions and .NET Runtime versions. The Analysis Host has additional software installed (.NET Core SDK, LLDB, SOS) required for debugging and analysis.

> [!NOTE]
> Both Target Host & Analysis Host must have the same platform architecture (e.g. `x86_64` in this case). Cross-platform analysis requires an emulator (e.g. QEMU) and is outside the scope of this article.

Let's assume we have a core dump `CoreDump_Full.2263` captured using `createdump` on the Target Host and the file has been copied to the Analysis Host.

## ELF Core Dump

On Linux, Executable and Linkable Format ([ELF](https://en.wikipedia.org/wiki/Executable_and_Linkable_Format)) is a standard file format for executable files, object code, shared libraries and core dumps.

In the context of a .NET Core dump, we are interested in specific parts of the ELF Core Dump.

First, we can inspect the ELF Core Dump header to confirm that we have indeed a valid core dump and the expected platform architecture:

```file CoreDump_Full.2263```

Output:
```
CoreDump_Full.2263: ELF 64-bit LSB core file, x86-64, version 1 (GNU/Linux), SVR4-style, from 'dotnet', real uid: 1000, effective uid: 1000, real gid: 1000, effective gid: 1000, execfn: '/usr/bin/dotnet', platform: 'x86_64'
```

Or the following:

```readelf --file-header CoreDump_Full.2263```

Output:
```
ELF Header:
  Magic:   7f 45 4c 46 02 01 01 03 00 00 00 00 00 00 00 00 
  Class:                             ELF64
  Data:                              2's complement, little endian
  Version:                           1 (current)
  OS/ABI:                            UNIX - GNU
  ABI Version:                       0
  Type:                              CORE (Core file)
  Machine:                           Advanced Micro Devices X86-64
  Version:                           0x1
  Entry point address:               0x0
  Start of program headers:          64 (bytes into file)
  Start of section headers:          0 (bytes into file)
  Flags:                             0x0
  Size of this header:               64 (bytes)
  Size of program headers:           56 (bytes)
  Number of program headers:         347
  Size of section headers:           0 (bytes)
  Number of section headers:         0
  Section header string table index: 0
```

> [!NOTE]
> The output shows that the file is a valid ELF Core Dump and has the expected `x86_64` platform architecture. The `readelf` command is part of the `binutils` package on most Linux distributions.

Next, we can review the program headers (segments) stored in the ELF Core Dump file and make sure there is a `NOTE` segment:

```readelf --program-headers CoreDump_Full.2263```

Output:
```
lf file type is CORE (Core file)
Entry point 0x0
There are 347 program headers, starting at offset 64

Program Headers:
  Type           Offset             VirtAddr           PhysAddr
                 FileSiz            MemSiz              Flags  Align
  NOTE           0x0000000000004c28 0x0000000000000000 0x0000000000000000
                 0x0000000000008520 0x0000000000000000         0x0
  LOAD           0x000000000000e000 0x00005c29c84f5000 0x0000000000000000
                 0x0000000000006000 0x0000000000006000  R      0x1000
  LOAD           0x0000000000014000 0x00005c29c84fb000 0x0000000000000000
                 0x000000000000b000 0x000000000000b000  R E    0x1000
  LOAD           0x000000000001f000 0x00005c29c8506000 0x0000000000000000
                 0x0000000000002000 0x0000000000002000  R      0x1000
...
```

> [!NOTE]
> The output shows that there is a `NOTE` segment at offset `0x0000000000004c28` and size `0x0000000000008520`. You can also use `objdump --section-headers CoreDump_Full.2263` to get the same information.

The `NOTE` segment contains the shared library references required to load the core dump into a debugger for analysis. We can view the `NOTE` segment contents using:

```readelf --notes CoreDump_Full.2263```

Output:
```
Displaying notes found at file offset 0x00004c28 with length 0x00008520:
  Owner                Data size 	Description
  CORE                 0x00000088	NT_PRPSINFO (prpsinfo structure)
  CORE                 0x00000170	NT_AUXV (auxiliary vector)
  CORE                 0x000063b4	NT_FILE (mapped files)
    Page size: 4096
                 Start                 End         Page Offset
    0x00005c29c84f5000  0x00005c29c84fb000  0x0000000000000000
        /opt/dotnet/dotnet
    0x00005c29c84fb000  0x00005c29c8506000  0x0000000000000005
        /opt/dotnet/dotnet
    0x00005c29c8506000  0x00005c29c8508000  0x000000000000000f
        /opt/dotnet/dotnet
    0x00005c29c8508000  0x00005c29c8509000  0x0000000000000010
        /opt/dotnet/dotnet
    0x0000755286a00000  0x0000755286f81000  0x0000000000000000
        /home/marian/App/ClrDebug.dll
    0x0000755294061000  0x0000755294200000  0x0000000000000000
        /opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Net.Http.dll
    0x0000755294200000  0x0000755294505000  0x0000000000000000
        /home/marian/App/Microsoft.Diagnostics.Tracing.TraceEvent.dll
    0x0000755294e00000  0x0000755294ed8000  0x0000000000000000
        /usr/lib/libicui18n.so.76.1
...
    0x0000759338122000  0x0000759338158000  0x0000000000000000
        /opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Net.Primitives.dll
    0x0000759338158000  0x00007593381ff000  0x0000000000000000
        /home/marian/App/Microsoft.Diagnostics.Runtime.dll
    0x0000759338e00000  0x0000759338e96000  0x0000000000000000
        /opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/libclrjit.so
    0x0000759338e96000  0x0000759339161000  0x0000000000000095
        /opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/libclrjit.so
...
    0x0000759340c30000  0x0000759340c31000  0x0000000000000000
        /memfd:doublemapper (deleted)
...
  CORE                 0x00000150	NT_PRSTATUS (prstatus structure)
  CORE                 0x00000200	NT_FPREGSET (floating point registers)
```

> [!NOTE]
> The output shows the memory pages and the referenced shared libraries. References marked with `(deleted)` represent unloaded pages. If you get an error `Cannot decode 64-bit note in 32-bit build`, make sure you use a `readelf` version 2.44 or later. You can use `objdump --section=.note.linuxcore.file --full-contents CoreDump_Full.2263` to view the raw `NOTE` segment contents.

## Core Dump Transfer

To successfully load a .NET Core dump created on the Target Host into a debugger (LLDB, dotnet-dump), we need to make sure we have the following components available on the Analysis Host:

* Executable: This is the .NET Core host application (`dotnet` in this case).
* Data Access Component (DAC) Components: The DAC allows the debugger to read managed structures inside the core dump. It is a subset of the .NET Runtime and is implemented using `libcoreclr.so` and `libmscordaccore.so` on Linux.
* Shared Library References: These are all managed (`.dll`) and native (Shared Object `.so`) libraries referenced by the .NET Core application at runtime.

> [!NOTE]
> The component file versions above must match between the Target Host and Analysis Host.

The most straightforward method to obtain the missing dependencies on the Analysis Host is to use the `dotnet-symbol` as follows:

```~/.dotnet/tools/dotnet-symbol -o ~/Symbols --host-only --debugging --modules --symbols CoreDump_Full.2263```

Output:
```
Downloading from https://msdl.microsoft.com/download/symbols/
Writing files to /home/marian/Symbols
Writing: /home/marian/Symbols/dotnet
Writing: /home/marian/Symbols/dotnet.dbg
Writing: /home/marian/Symbols/System.Net.Http.dll
Writing: /home/marian/Symbols/Microsoft.Diagnostics.Tracing.TraceEvent.dll
Writing: /home/marian/Symbols/libclrjit.so
Writing: /home/marian/Symbols/libclrjit.so.dbg
...
```

> [!NOTE]
> The `--host-only` parameter specifies `dotnet` host download. The `--debugging` parameter specifies DAC Components download. The `--modules --symbols` parameters specify Shared Library References and their corresponding debugging symbols download.

If there are no errors (`ERROR: Not Found:` messages) from the `dotnet-symbol` command output, the core dump transfer is complete. You can load it into LLDB and start your analysis:

```lldb -c CoreDump_Full.2263 ~/Symbols/dotnet```

In most cases, there will be references that the `dotnet-symbol` will not be able to resolve. E.g. custom managed libraries, shared object libraries from non-supported Linux distributions, private .NET Core builds, etc. In these cases, you need to copy the missing references from the Target Host manually. There are a several options to identify the missing reference files.

**Identify Missing References Using `dotnet-symbol`**

The `dotnet-symbol` command displays error messages for all missing references.

Output:
```
...
ERROR: Not Found: ByteZoo.Blog.Common.dll - 'https://msdl.microsoft.com/download/symbols/bytezoo.blog.common.dll/F68FBB621c000/bytezoo.blog.common.dll'
ERROR: Not Found: ByteZoo.Blog.App.dll - 'https://msdl.microsoft.com/download/symbols/bytezoo.blog.app.dll/D95285262e000/bytezoo.blog.app.dll'
...
ERROR: Not Found: libc.so.6 - 'https://msdl.microsoft.com/download/symbols/libc.so.6/elf-buildid-0b707b217b15b106c25fe51df3724b25848310c0/libc.so.6'
ERROR: Not Found: libc.so.6.dbg - 'https://msdl.microsoft.com/download/symbols/_.debug/elf-buildid-sym-0b707b217b15b106c25fe51df3724b25848310c0/_.debug'
ERROR: Not Found: libgcc_s.so.1 - 'https://msdl.microsoft.com/download/symbols/libgcc_s.so.1/elf-buildid-d3703b1a870228dc233ef552442ece411d83ce50/libgcc_s.so.1'
ERROR: Not Found: libgcc_s.so.1.dbg - 'https://msdl.microsoft.com/download/symbols/_.debug/elf-buildid-sym-d3703b1a870228dc233ef552442ece411d83ce50/_.debug'
...
```

> [!NOTE]
> The output shows that there are managed (`ByteZoo.Blog.Common.dll`, `ByteZoo.Blog.App.dll`) and native (`libc.so.6`, `libgcc_s.so.1`) missing references.

Using these error messages, we can look for these files on the Target Host using `find` or other methods. The drawback of this approach is that we do not have the exact location of the missing reference, only the library name and ID. If there are multiple instances of a particular file, we might pick the wrong one.

**Identify Missing Shared Object Library References Using LLDB**

We can use LLDB to identify missing shared object library references. We start by turning on the module logging in LLDB as follows:

```lldb -c CoreDump_Full.2263 ~/Symbols/dotnet -O "log enable -v -f ./Modules.log lldb module" --batch```

Output:
```
Loading extension /home/marian/.dotnet/sos/extensions/Microsoft.Diagnostics.DataContractReader.dll
Loading extension /home/marian/.dotnet/sos/extensions/Microsoft.Diagnostics.DataContractReader.Extension.dll
Loading extension /home/marian/.dotnet/sos/extensions/Microsoft.Diagnostics.DebuggerCommands.dll
Current symbol store settings:
-> Directory: /home/marian/Symbols
(lldb) log enable -v -f ./Modules.log lldb module
(lldb) target create "/home/marian/Symbols/dotnet" --core "CoreDump_Full.2263"
Core file '/home/marian/Downloads/CoreDump_Full.2263' (x86_64) was loaded.
```

> [!NOTE]
> The `log enable` command enables module logging to a `Modules.log` text file.

Once we have the logging file created, we can look for missing module messages as follows:

```grep "(unknown) '.\+'" Modules.log```

Output:
```
0x8b32210 Module::Module((unknown) '[vdso]')
0x8a603f0 Module::Module((unknown) 'linux-vdso.so.1')
0x8a12cf0 Module::Module((unknown) '/usr/lib/libpthread.so.0')
0x7dc2b50 Module::Module((unknown) '/usr/lib/libdl.so.2')
0x7dc56b0 Module::Module((unknown) '/usr/lib/libstdc++.so.6')
0x7dc8f80 Module::Module((unknown) '/usr/lib/libm.so.6')
0x89e7030 Module::Module((unknown) '/usr/lib/libgcc_s.so.1')
0x89ea740 Module::Module((unknown) '/usr/lib/libc.so.6')
0x9011e60 Module::Module((unknown) '/usr/lib/librt.so.1')
0x989ddc0 Module::Module((unknown) '/usr/lib/libicuuc.so.76')
0x989e300 Module::Module((unknown) '/usr/lib/libicudata.so.76')
0x98c6d10 Module::Module((unknown) '/usr/lib/libicui18n.so.76')
```

> [!NOTE]
> We can also use `ldd /usr/bin/dotnet` on the Target Host to find all `.so` references for the .NET host.

**Identify Managed Assembly References Using LLDB**

We can use the following statement to extract a list of all managed assembly references from the core dump:

```lldb -c CoreDump_Full.2263 ~/Symbols/dotnet -o "dumpdomain" --batch | sed -n "s/^Assembly:[^\[]*\[\(.*\)\]$/\1/p"```

Output:
```
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Private.CoreLib.dll
/home/marian/App/ByteZoo.Blog.App.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Runtime.dll
/home/marian/App/ByteZoo.Blog.Common.dll
/home/marian/App/Microsoft.Extensions.Hosting.Abstractions.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.ComponentModel.dll
/home/marian/App/Microsoft.Extensions.DependencyInjection.Abstractions.dll
/home/marian/App/CommandLine.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/netstandard.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Collections.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Linq.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Threading.Thread.dll
/home/marian/App/Microsoft.Extensions.Hosting.dll
/home/marian/App/Microsoft.Extensions.Configuration.Abstractions.dll
/home/marian/App/Microsoft.Extensions.DependencyInjection.dll
/home/marian/App/System.Diagnostics.DiagnosticSource.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Threading.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Diagnostics.Tracing.dll
/home/marian/App/Microsoft.Extensions.Configuration.dll
/home/marian/App/Microsoft.Extensions.Configuration.EnvironmentVariables.dll
/home/marian/App/Microsoft.Extensions.Configuration.CommandLine.dll
/home/marian/App/Microsoft.Extensions.Primitives.dll
/home/marian/App/Microsoft.Extensions.FileProviders.Abstractions.dll
/home/marian/App/Microsoft.Extensions.FileProviders.Physical.dll
/home/marian/App/Microsoft.Extensions.Configuration.FileExtensions.dll
/home/marian/App/Microsoft.Extensions.Configuration.Json.dll
/home/marian/App/Microsoft.Extensions.Configuration.UserSecrets.dll
/home/marian/App/Microsoft.Extensions.Configuration.Binder.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Memory.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.IO.FileSystem.Watcher.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.ComponentModel.Primitives.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Collections.Concurrent.dll
/home/marian/App/Microsoft.Extensions.FileSystemGlobbing.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Runtime.InteropServices.dll
/home/marian/App/System.Text.Json.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Text.Encoding.Extensions.dll
/home/marian/App/System.IO.Pipelines.dll
/home/marian/App/Microsoft.Extensions.Options.dll
/home/marian/App/Microsoft.Extensions.Logging.dll
/home/marian/App/Microsoft.Extensions.Diagnostics.dll
/home/marian/App/Microsoft.Extensions.Logging.Abstractions.dll
/home/marian/App/Microsoft.Extensions.Diagnostics.Abstractions.dll
/home/marian/App/Microsoft.Extensions.Logging.Configuration.dll
/home/marian/App/Microsoft.Extensions.Logging.Debug.dll
/home/marian/App/Microsoft.Extensions.Logging.EventSource.dll
/home/marian/App/Microsoft.Extensions.Logging.EventLog.dll
/home/marian/App/Microsoft.Extensions.Logging.Console.dll
/home/marian/App/Microsoft.Extensions.Options.ConfigurationExtensions.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Console.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/Microsoft.Win32.Primitives.dll
/home/marian/App/Microsoft.Diagnostics.Tracing.TraceEvent.dll
/home/marian/App/Microsoft.Diagnostics.NETCore.Client.dll
/home/marian/App/Microsoft.Diagnostics.Runtime.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Net.Http.dll
/opt/dotnet/shared/Microsoft.NETCore.App/8.0.15/System.Net.Primitives.dll
/home/marian/App/ClrDebug.dll
```

## References

* [Create .NET Core Dumps On Linux](/Resources/Articles/Prerequisites/Create%20.NET%20Core%20Dumps%20On%20Linux.md)
* [Load .NET Core Dumps In LLDB On Linux](/Resources/Articles/Prerequisites/Load%20.NET%20Core%20Dumps%20In%20LLDB%20On%20Linux.md)
* [Data Access Component (DAC)](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/dac-notes.md)
* [Executable and Linkable Format (ELF)](https://en.wikipedia.org/wiki/Executable_and_Linkable_Format)

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Core Dump, Linux --->