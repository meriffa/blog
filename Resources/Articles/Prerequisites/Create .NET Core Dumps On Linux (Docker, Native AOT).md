# Create .NET Core Dumps On Linux (Docker, Native AOT)

This article builds upon [Create .NET Core Dumps On Linux](/Resources/Articles/Prerequisites/Create%20.NET%20Core%20Dumps%20On%20Linux.md) and describes additional methods to create a [core dump](https://en.wikipedia.org/wiki/Core_dump) of a .NET Core application running on Linux. The article covers [Docker](https://www.docker.com), Self-Contained, ReadyToRun and Native AOT deployments.

## Docker

### Prerequisites

Before we capture .NET Core memory dumps, we need to have Docker configured and one or more containers running. Docker installation is outside the scope of this article. There are a number of excellent online resources on how to install Docker on Linux, including [Install Docker Desktop on Linux](https://docs.docker.com/desktop/setup/install/linux/). Here we will focus on a simple test container running a .NET Core console application.

.NET Core Console Application (`ByteZoo.Blog.App.Dockerfile`):
```
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /ByteZoo.Blog.App
COPY ./ByteZoo.Blog.App.Publish .
ENTRYPOINT ["dotnet", "ByteZoo.Blog.App.dll", "Concepts-String"]
```

The container configuration is very basic. It defines a base .NET image using the [Microsoft Artifact Registry](https://mcr.microsoft.com/) images, copies the published application binaries and sets up the application startup (`dotnet`).

We can also use a Docker Compose configuration as follows:

Docker Compose Configuration (`ByteZoo.Blog.yaml`)
```
name: ByteZoo.Blog

services:

  bytezoo.blog.app:
    build:
      context: .
      dockerfile: ./ByteZoo.Blog.App.Dockerfile
```

Next we build the container image using:

```docker build -t bytezoo.blog.app-image:1.0 -f ./ByteZoo.Blog.App.Dockerfile .```

Output:
```
[+] Building 10.7s (9/9) FINISHED                                                                                                                                    docker:default
 => [internal] load build definition from ByteZoo.Blog.App.Dockerfile                                                                                                          0.1s
 => => transferring dockerfile: 303B                                                                                                                                           0.0s
 => [internal] load metadata for mcr.microsoft.com/dotnet/runtime:9.0                                                                                                          1.0s
 => [internal] load .dockerignore                                                                                                                                              0.1s
 => => transferring context: 2B                                                                                                                                                0.0s
 => [1/3] FROM mcr.microsoft.com/dotnet/runtime:9.0@sha256:9d42d7d4d892e2afc571884e9b67f3b1ebb361166ee14151fc6d1bd539cbfeb3                                                    3.2s
 ...
 => [3/3] COPY ./ByteZoo.Blog.App.Publish .                                                                                                                                    0.4s 
 => exporting to image                                                                                                                                                         0.6s 
 => => exporting layers                                                                                                                                                        0.5s 
 => => writing image sha256:eb573c920b72b5671b6ef9ff5d09ffc0e00bf84b400a5e64f3e901896e912c74                                                                                   0.0s 
 => => naming to docker.io/library/bytezoo.blog.app-image:1.0   
```

Or we can use Docker Compose build `docker compose -f ./ByteZoo.Blog.yaml build` to build the container image.

Once we have the container image built, we can start the container using `docker run` or create (`docker create`) and run the existing container (`docker start`).

### Capture Core Dumps

* Capture Core Dump From Inside A Container:

To capture a core dump from inside a running Docker container, we run a `createdump` inside the target container:

```
docker exec -it ByteZoo.Blog.App-Container sh -c "/usr/share/dotnet/shared/Microsoft.NETCore.App/9.0.4/createdump -f /tmp/CoreDump_Full.%p --full 1"
```

Output:
```
[createdump] Gathering state for process 1 dotnet
[createdump] Writing full dump to file /tmp/CoreDump_Full.1
[createdump] Written 200503296 bytes (48951 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 276ms
```

> [!NOTE]
> The `createdump` uses `<PID>` = 1, since the .NET application is set as the `ENTRYPOINT` for the container instance. The path to `createdump` will depend on the .NET Core runtime installed in the container. Depending on the security configuration of the host and container instance, you may have to add additional parameters to the `docker exec` command - e.g. `--cap-add=SYS_PTRACE`, `--privileged`, `--ulimit core=-1` and `--security-opt seccomp=unconfined`.

Once the core dump has been successfully created you can copy the file using:

```docker cp ByteZoo.Blog.App-Container:/tmp/CoreDump_Full.1 ~/```

Output:
```
Successfully copied 201MB to /home/marian
```

* Capture Core Dump From A Container Host / Sidecar:

Another method to capture core dumps from a running Docker container is to use Shared Volume and Diagnostic Port. In this case, there is no need for direct access to the target container. In the case of sidecar, there is no need for container host access as well.

First, we need to setup shared volume between the target container and the host or sidecar. We can use Docker `docker run -v` option or Docker Compose `volumes:` option as follows:

```
    volumes:
      - ~/ByteZoo.Blog.App.tmp:/tmp
```

> [!NOTE]
> The Docker Compose configuration sets up a shared folder between the container host (`~/ByteZoo.Blog.App.tmp`) and the target container (`/tmp`).

Once we have the target container running using a shared volume, we can confirm that the Diagnostic Port socket is configured and visible from the container host:

```
ls ~/ByteZoo.Blog.App.tmp/dotnet-* -al
```

Output:
```
srw------- 1 root root 0 May 11 13:57 /home/marian/ByteZoo.Blog.App.tmp/dotnet-diagnostic-1-551172-socket
```

Using the Diagnostic Port socket, we capture the core dump using `dotnet-dump` command on the container host as follows:

```
sudo ~/.dotnet/tools/dotnet-dump collect --type Full -o /tmp/CoreDump_Full.1 --diag --diagnostic-port ~/ByteZoo.Blog.App.tmp/dotnet-diagnostic-1-551172-socket,connect
```

Output:
```
Writing full to /tmp/CoreDump_Full.1
Complete
```

The core dump file `/tmp/CoreDump_Full.1` can be accessed directly from the container host / sidecar shared volume.

> [!NOTE]
> The `dotnet-dump` is one of the tools that supports Diagnostic Port (e.g. `dotnet-counters`, `dotnet-trace`, etc.). Using `dotnet-dump` on the container host / sidecar requires a .NET Core SDK installed in order to run.

### Collect Diagnostics

* Collect Diagnostics From Container Host / Sidecar:

We can re-use the Shared Volume and Diagnostic Port configuration in the previous section to collect diagnostics once the .NET Core application starts. The only difference in this case is that we will configure additional Diagnostic Port for the container application as follows:

```
    environment:
      - DOTNET_EnableDiagnostics=1
      - DOTNET_DiagnosticPorts=/tmp/ByteZoo.Blog.App.socket,nosuspend
```

This Docker Compose configuration sets up an additional Diagnostic Port in connect & no-suspend mode. This means that the application will try to connect to an additional Diagnostic Port socket that already exists and will continue if not present.

> [!NOTE]
> The `DOTNET_EnableDiagnostics=1` configuration is not necessary and for information only.

Once we have updated the container image and instance, we start the diagnostic data collection on the container host / sidecar first:

```sudo ~/.dotnet/tools/dotnet-counters collect --counters System.Runtime --diagnostic-port ~/ByteZoo.Blog.App.tmp/ByteZoo.Blog.App.socket```

Output:
```
Waiting for connection on /home/marian/ByteZoo.Blog.App.tmp/ByteZoo.Blog.App.socket
Start an application with the following environment variable: DOTNET_DiagnosticPorts=/home/marian/ByteZoo.Blog.App.tmp/ByteZoo.Blog.App.socket
```

At this point, the additional Diagnostic Port socket (`ByteZoo.Blog.App.socket`) is created in listen mode. Next we start the target container:

```docker compose -f ./ByteZoo.Blog.yaml up```

Once the .NET application starts and connects to the additional Diagnostic Port socket, the container host / sidecar data collection should start:

```
Starting a counter session. Press Q to quit.
```

## Self-Contained Application

* Capture Self-Contained Application Core Dump:

To create a self-contained application, we use the following:

```dotnet publish ByteZoo.Blog.App.csproj --output ./Publish --runtime linux-x64 --self-contained```

The `dotnet publish` command creates a self-contained .NET Core application for Linux in the `./Publish` folder.

If we want to create a ReadyToRun self-contained application we add `PublishReadyToRun=true` as follows:

```dotnet publish ByteZoo.Blog.App.csproj --output ./Publish --runtime linux-x64 --self-contained -p:PublishReadyToRun=true```

We use `createdump` to capture a core dump of the running self-contained application:

```./Publish/createdump -f CoreDump_Full.%p --full <PID>```

Output:
```
[createdump] Gathering state for process 2237 ByteZoo.Blog.Ap
[createdump] Writing full dump to file CoreDump_Full.2237
[createdump] Written 198021120 bytes (48345 pages) to core file
[createdump] Target process is alive
[createdump] Dump successfully written in 365ms
```

> [!NOTE]
> The `<PID>` is the process id of the running self-contained application (`2237` in this case).

There is one important point to note when analyzing self-contained application core dump in LLDB. The .NET host in this case is different. It is the application executable, not the `dotnet`. This means that you need to copy the application executable (`ByteZoo.Blog.App` in this case) along with the core dump (`CoreDump_Full.2237`) and load it into LLDB as follows:

```lldb -c CoreDump_Full.2237 ByteZoo.Blog.App```

* Capture Single File Self-Contained Application Core Dump:

To create a single file self-contained application, we use the following:

```dotnet publish ByteZoo.Blog.App.csproj --output ./Publish --runtime linux-x64 --self-contained -p:PublishSingleFile=true --p:DebugType=embedded -p:IncludeNativeLibrariesForSelfExtract=true```

The `dotnet publish` command creates a single file self-contained .NET Core application for Linux in the `./Publish` folder.

We can also create a single file ReadyToRun self-contained application as follows:

```dotnet publish ByteZoo.Blog.App.csproj --output ./Publish --runtime linux-x64 --self-contained -p:PublishSingleFile=true --p:DebugType=embedded -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true```

In the case of a single file self-contained application, we need to install an additional tool to capture a core dump. There are multiple options to choose from - `ProcDump`, `createdump` (.NET Core Runtime), `dotnet-dump` (.NET Core SDK) and `gcore` (GDB).

`ProcDump` is probably the most straightforward. You can see the [Create .NET Core Dumps On Linux](/Resources/Articles/Prerequisites/Create%20.NET%20Core%20Dumps%20On%20Linux.md) on how to install the tool and capture a core dump.

## Native AOT Application

* Capture Native AOT Application Core Dump:

To create a Native AOT application, we use the following:

```dotnet publish ByteZoo.Blog.App.csproj --output ./Publish --runtime linux-x64 -p:PublishAot=true```

The `dotnet publish` command creates a Native AOT .NET Core application for Linux in the `./Publish` folder, which is ahead-of-time (AOT) compiled to native code.

Native AOT applications do not contain managed (IL) code and there is no JIT compiler included. The core dump of such applications will be of limited use, since the managed code debugging extensions (e.g. SOS, DAC) will not work.

We can use `ProcDump` to capture Native AOT application core dump as follows:

```procdump -n 1 -s 0 <PID>```

Output:
```
ProcDump v3.4.1 - Sysinternals process dump utility
Copyright (C) 2025 Microsoft Corporation. All rights reserved. Licensed under the MIT license.
Mark Russinovich, Mario Hewardt, John Salem, Javid Habibi
Sysinternals - www.sysinternals.com

Monitors one or more processes and writes a core dump file when the processes exceeds the
specified criteria.

[16:53:04 - INFO]: Press Ctrl-C to end monitoring without terminating the process(es).
Process:                                ByteZoo.Blog.App (3333)
CPU Threshold:                          n/a
Commit Threshold:                       n/a
Thread Threshold:                       n/a
File Descriptor Threshold:              n/a
GC Generation:                          n/a
Resource tracking:                      n/a
Resource tracking sample rate:          n/a
Signal:                                 n/a
Exception monitor:                      n/a
Polling Interval (ms):                  1000
Threshold (s):                          0
Number of Dumps:                        1
Output directory:                       .
[16:53:04 - INFO]: Starting monitor for process ByteZoo.Blog.App (3333)
[16:53:04 - INFO]: Trigger: Timer:1(s) on process ID: 3333
[16:53:05 - INFO]: Core dump 0 generated: ./ByteZoo_Blog_App_time_250511_165304.3333
[16:53:05 - INFO]: Stopping monitor for process ByteZoo.Blog.App (3333)
```

> [!NOTE]
> `<PID>` is the target Native AOT application process id (`3333` in this case).

Next, we load the core dump into LLDB as follows:

```lldb -c ByteZoo_Blog_App_time_250511_165304.3333 ByteZoo.Blog.App```

We can confirm that the SOS extension commands are not working by using:

```dumpdomain```

Output:
```
Failed to load data access module, 0x80004002
Unknown runtime type. Command not supported.

For more information see https://go.microsoft.com/fwlink/?linkid=2135652
DumpDomain  failed
```

Although the managed debugging extensions are not working, we can still use the standard LLDB commands. We can list the application modules as follows:

```target modules list```

Output:
```
[  0] 12620094-2A91-A904-6131-BF051E35DE0D-8DDA6ECA 0x000055fc45a6e000 /home/marian/Downloads/ByteZoo.Blog.App 
      /home/marian/Downloads/ByteZoo.Blog.App.dbg
[  1] 517D108A-7660-DA56-DD6F-71835F347309-C0CC0F62 0x00007ffea6ff5000 [vdso] (0x00007ffea6ff5000)
[  2] 7084272B 0x00007ffea6ff5000 linux-vdso.so.1 (0x00007ffea6ff5000)
[  3] 6D201DF2-CB50-847F-0ED4-2DA4158C3A60-8D578F03 0x00007f857a547000 /lib/x86_64-linux-gnu/libm.so.6 
      /usr/lib/debug/.build-id/6d/201df2cb50847f0ed42da4158c3a608d578f03.debug
[  4] C047672C-AE79-6432-4658-491E7DEE2674-8AE5D2F8 0x00007f857a366000 /lib/x86_64-linux-gnu/libc.so.6 
      /usr/lib/debug/.build-id/c0/47672cae7964324658491e7dee26748ae5d2f8.debug
[  5] F4BC47DB-4679-0658-0A47-640E01E6D901-E2642A7B 0x00007f857a63a000 /lib64/ld-linux-x86-64.so.2 
      /usr/lib/debug/.build-id/f4/bc47db467906580a47640e01e6d901e2642a7b.debug
[  6] EF3832D8-F5BB-D881-DC51-F2336BA7BD76-DFC689AD 0x00007f44e4d60000 /lib/x86_64-linux-gnu/libicuuc.so.72 
[  7] B6BBCA4E-6027-1248-6A0F-A850088EFCCC-47710AD0 0x00007f44de200000 /lib/x86_64-linux-gnu/libicudata.so.72 
[  8] 0C47CEC7-5226-C773-6517-D5ACB61E373D-541A5023 0x00007f44e4a00000 /lib/x86_64-linux-gnu/libstdc++.so.6 
[  9] C3591C3E-3EE3-A4B1-1AC9-9E05A8D36479-C3DB86E9 0x00007f44e4d40000 /lib/x86_64-linux-gnu/libgcc_s.so.1 
[ 10] 1367D836-CF5A-8886-F575-1B5BA51AFBB6-16BA9598 0x00007f44e4600000 /lib/x86_64-linux-gnu/libicui18n.so.72 
```

We can search for symbols using:

```image lookup -r -n .*StringController.*```

Output:
```
7 matches found in /home/marian/Downloads/ByteZoo.Blog.App:
        Address: ByteZoo.Blog.App[0x00000000000ae720] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 15104)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat at StringController.cs:75        Address: ByteZoo.Blog.App[0x00000000000ae680] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 14944)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringStringBuilder at StringController.cs:61        Address: ByteZoo.Blog.App[0x00000000000ae4f0] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 14544)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__Execute at StringController.cs:23        Address: ByteZoo.Blog.App[0x00000000000ae760] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 15168)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringInterpolation at StringController.cs:85        Address: ByteZoo.Blog.App[0x00000000000ae620] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 14848)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringConcat at StringController.cs:49        Address: ByteZoo.Blog.App[0x00000000000ae5f0] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 14800)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayConstantString at StringController.cs:38        Address: ByteZoo.Blog.App[0x00000000000ae870] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 15440)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController___ctor
```

> [!NOTE]
> Make sure to copy all `.dbg` and `.pdb` files along with the Native AOT application executable, otherwise LLDB might not be able to resolve symbols.

We can use the symbol results output to display the native code for a specific method. We can use the first entry `ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat` for example.

First, we get the `DisplayDynamicStringFormat()` method code range using:

```image lookup -v -n ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat```

Output:
```
1 match found in /home/marian/Downloads/ByteZoo.Blog.App:
        Address: ByteZoo.Blog.App[0x00000000000ae720] (ByteZoo.Blog.App.PT_LOAD[1].__managedcode + 15104)
        Summary: ByteZoo.Blog.App`ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat at StringController.cs:75
         Module: file = "/home/marian/Downloads/ByteZoo.Blog.App", arch = "x86_64"
    CompileUnit: id = {0x00000000}, file = "/_/il.cpp", language = "c++"
       Function: id = {0x000daeaf}, name = "ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat", mangled = "ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat", range = [0x000055fc45b1c720-0x000055fc45b1c755)
       FuncType: id = {0x000daeaf}, byte-size = 0, decl = Program.cs:1, compiler_type = "void (class String &, class String &)"
         Blocks: id = {0x000daeaf}, range = [0x55fc45b1c720-0x55fc45b1c755)
      LineEntry: [0x000055fc45b1c720-0x000055fc45b1c72b): /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Controllers/Concepts/StringController.cs:75
         Symbol: id = {0x0000501f}, range = [0x000055fc45b1c720-0x000055fc45b1c755), name="ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController__DisplayDynamicStringFormat"
       Variable: id = {0x000daeca}, name = "this", type = "ByteZoo_Blog_App_ByteZoo_Blog_App_Controllers_Concepts_StringController &", location = DW_OP_reg5 RDI, decl = Program.cs:1
       Variable: id = {0x000daed9}, name = "format", type = "String &", location = DW_OP_reg4 RSI, decl = Program.cs:1
       Variable: id = {0x000daee6}, name = "arg0", type = "String &", location = DW_OP_reg1 RDX, decl = Program.cs:1
       Variable: id = {0x000daef3}, name = "text", type = "String &", location = DW_OP_reg4 RSI, decl = Program.cs:1
```

> [!NOTE]
> The `DisplayDynamicStringFormat()` method code range in this case is `0x55fc45b1c720`-`0x55fc45b1c755`. It is provided in the `Blocks:` entry.

Using the `DisplayDynamicStringFormat()` method range, we display the native code as follows:

```disassemble -s 0x55fc45b1c720 -e 0x55fc45b1c755```

Output:
```
ByteZoo.Blog.App`:
    0x55fc45b1c720 <+0>:  push   rbp
    0x55fc45b1c721 <+1>:  push   rbx
    0x55fc45b1c722 <+2>:  push   rax
    0x55fc45b1c723 <+3>:  lea    rbp, [rsp + 0x10]
    0x55fc45b1c728 <+8>:  mov    rbx, rdi
    0x55fc45b1c72b <+11>: mov    qword ptr [rbp - 0x10], rdx
    0x55fc45b1c72f <+15>: lea    rdx, [rbp - 0x10]
    0x55fc45b1c733 <+19>: mov    ecx, 0x1
    0x55fc45b1c738 <+24>: xor    edi, edi
    0x55fc45b1c73a <+26>: call   0x10dbb0                  ; String__FormatHelper at String.Manipulation.cs:539
    0x55fc45b1c73f <+31>: mov    rsi, rax
    0x55fc45b1c742 <+34>: mov    rdi, qword ptr [rbx + 0x10]
    0x55fc45b1c746 <+38>: cmp    dword ptr [rdi], edi
    0x55fc45b1c748 <+40>: call   0xae990                   ; ByteZoo_Blog_Common_ByteZoo_Blog_Common_Services_DisplayService__WriteInformation at DisplayService.cs:38
    0x55fc45b1c74d <+45>: nop    
    0x55fc45b1c74e <+46>: add    rsp, 0x8
    0x55fc45b1c752 <+50>: pop    rbx
    0x55fc45b1c753 <+51>: pop    rbp
    0x55fc45b1c754 <+52>: ret
```

## Cloud Platforms

Each Cloud Platform (AWS, Microsoft Azure, Google Cloud, etc.) provides its own set of unique troubleshooting and monitoring services, which evolve over time. Capturing core dumps on these platforms is well beyond the scope of this article. Nevertheless, there are several key features that are worth mentioning:

* Microsoft Azure: The Azure App Service allows for core dump capture using the Azure Portal and the KUDU Console. For more information see [Capture Memory Dumps On The Azure App Service Platform](https://learn.microsoft.com/troubleshoot/azure/app-service/capture-memory-dumps-app-service) and [Kudu Service Overview](https://learn.microsoft.com/azure/app-service/resources-kudu).
* AWS: ECS Exec provides direct access to ECS containers similar to `docker exec`. For more information see [Using Amazon ECS Exec To Access Your Containers on AWS Fargate and Amazon EC2](https://aws.amazon.com/blogs/containers/new-using-amazon-ecs-exec-access-your-containers-fargate-ec2/).

## References

* [Create .NET Core Dumps On Linux](/Resources/Articles/Prerequisites/Create%20.NET%20Core%20Dumps%20On%20Linux.md)
* [Article Source Code](/Sources)
* [Article Script](/Resources/Scripts/Docker.sh)
* [.NET Application Publishing Overview](https://learn.microsoft.com/dotnet/core/deploying/)
* [Collect Diagnostics In Linux Containers](https://learn.microsoft.com/dotnet/core/diagnostics/diagnostics-in-containers)
* [FAQ For Dumps](https://learn.microsoft.com/dotnet/core/diagnostics/faq-dumps)
* [Diagnostic Ports](https://learn.microsoft.com/dotnet/core/diagnostics/diagnostic-port)
* [Native AOT Deployment](https://learn.microsoft.com/dotnet/core/deploying/native-aot)

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Core Dump, Linux, Docker --->