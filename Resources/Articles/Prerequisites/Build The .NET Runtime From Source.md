# Build The .NET Runtime From Source

This article describes the steps to download, build and debug the [.NET Runtime](https://github.com/dotnet/runtime) locally on [Debian 12 ("Bookworm")](https://www.debian.org/) Linux.

Looking at the source code can be invaluable when trying to get more details about the inner workings of the unmanaged ([CoreCLR](https://github.com/dotnet/runtime/tree/main/src/coreclr)), managed ([CoreLib](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Private.CoreLib)) and library ([Libraries](https://github.com/dotnet/runtime/tree/main/src/libraries)) portions of the .NET Runtime.

For example, I was trying to find more information regarding the [Thread.Interrupt()](https://learn.microsoft.com/dotnet/api/system.threading.thread.interrupt) method. Although there is excellent documentation and plenty of articles online on what this method does, I could not find any information on how it does it. I assumed that the method was somehow related to the CLR thread suspension and GC safe point handling logic, but decided to look into the source code and find out.

## Prerequisites

To install the .NET Runtime build prerequisites, use the following:

```
sudo apt-get update -qq
sudo apt-get install build-essential curl git gettext locales cmake llvm clang lld lldb liblldb-dev libunwind8-dev libicu-dev liblttng-ust-dev libssl-dev libkrb5-dev pigz python-is-python3 cpio -y
sudo localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8
```

## Download The .NET Runtime Source

Download the .NET Runtime repository using:

```git clone -b main https://github.com/dotnet/runtime.git ~/CLR```

> [!NOTE]
> If you change the local path `~/CLR`, make sure to update all commands in this article accordingly. You can also specify a different branch (e.g. `release/9.0` instead of `main`).
> Make sure to have sufficient disk space, since the repository is very large (880 MB download, 1.9 GB expanded, 11.2 GB after build).

## Build The .NET Runtime From Source

Select the local .NET Runtime repository folder:

```cd ~/CLR```

To build the Runtime (CoreCLR, CoreLib) components in Debug mode, use the following:

```./build.sh -subset clr -configuration Debug```

To build the Runtime (CoreCLR, CoreLib) components in Release mode, use the following:

```./build.sh -subset clr -configuration Release```

> [!NOTE]
> The CoreCLR & CoreLib build artifacts are located in either `./artifacts/bin/coreclr/linux.x64.Debug` or `./artifacts/bin/coreclr/linux.x64.Release` depending on the build mode.

To build the Libraries component in Debug mode, use the following:

```./build.sh -subset libs -configuration Debug```

To build the Libraries component in Release mode, use the following:

```./build.sh -subset libs -configuration Release```

> [!NOTE]
> The Libraries build artifacts are located in `./artifacts/bin`.

To build the Core_Root component, use the following:

```./src/tests/build.sh -debug -p:LibrariesConfiguration=Debug -generatelayoutonly```

> [!NOTE]
> The Core_Root component is used to run external applications with the custom runtime build. The Core_Root `-debug` or `-release` parameter must match the Runtime (CoreCLR, CoreLib) components build mode. The Core_Root `LibrariesConfiguration` parameter must match the Libraries component build mode. The Core_Root build artifacts are located in either `./artifacts/tests/coreclr/linux.x64.Debug/Tests/Core_Root` or `./artifacts/tests/coreclr/linux.x64.Release/Tests/Core_Root` depending on the build mode.

## Run Application Using The Local .NET Runtime

To run an application using the local .NET Runtime build, use the following:

```
export CORE_ROOT=~/CLR/artifacts/tests/coreclr/linux.x64.Debug/Tests/Core_Root
$CORE_ROOT/corerun <Application.dll> [ApplicationArgument1] [ApplicationArgument2] ... [ApplicationArgumentN]
```

## Modify The Local .NET Runtime

In some cases, you might want to modify the local .NET Runtime build. For example, you can add `Console.WriteLine()` or `printf()` statements to provide simple logging in components that you are investigating.

Another example is to remove a `Debug.Assert()` statement, which causes an application execution to fail. Let's review the following scenario:

```$CORE_ROOT/corerun ByteZoo.Blog.App.dll Concepts-String```

Output:
```
ByteZoo.Blog application started.
Process terminated. Assertion failed.
source is not null && source.Length > 0
   at System.Linq.Enumerable.ArrayWhereSelectIterator`2..ctor(TSource[] source, Func`2 predicate, Func`2 selector) in /home/marian/CLR/src/libraries/System.Linq/src/System/Linq/Where.cs:line 263
   at System.Linq.Enumerable.OfTypeIterator`1.Select[TResult2](Func`2 selector) in /home/marian/CLR/src/libraries/System.Linq/src/System/Linq/OfType.SpeedOpt.cs:line 170
   at System.Linq.Enumerable.Select[TSource,TResult](IEnumerable`1 source, Func`2 selector) in /home/marian/CLR/src/libraries/System.Linq/src/System/Linq/Select.cs:line 15
   at CommandLine.Core.OptionMapper.MapValues(IEnumerable`1 propertyTuples, IEnumerable`1 options, Func`5 converter, StringComparer comparer)
   at CommandLine.Core.InstanceBuilder.<>c__DisplayClass1_0`1.<Build>b__5()
   at CommandLine.Core.InstanceBuilder.Build[T](Maybe`1 factory, Func`3 tokenizer, IEnumerable`1 arguments, StringComparer nameComparer, Boolean ignoreValueCase, CultureInfo parsingCulture, Boolean autoHelp, Boolean autoVersion, Boolean allowMultiInstance, IEnumerable`1 nonFatalErrors)
   at CommandLine.Core.InstanceChooser.MatchVerb(Func`3 tokenizer, IEnumerable`1 verbs, Tuple`2 defaultVerb, IEnumerable`1 arguments, StringComparer nameComparer, Boolean ignoreValueCase, CultureInfo parsingCulture, Boolean autoHelp, Boolean autoVersion, Boolean allowMultiInstance, IEnumerable`1 nonFatalErrors)
   at CommandLine.Core.InstanceChooser.<Choose>g__choose|1_1(<>c__DisplayClass1_0&)
   at CommandLine.Core.InstanceChooser.Choose(Func`3 tokenizer, IEnumerable`1 types, IEnumerable`1 arguments, StringComparer nameComparer, Boolean ignoreValueCase, CultureInfo parsingCulture, Boolean autoHelp, Boolean autoVersion, Boolean allowMultiInstance, IEnumerable`1 nonFatalErrors)
   at CommandLine.Parser.ParseArguments(IEnumerable`1 args, Type[] types)
   at ByteZoo.Blog.App.Program.Main(String[] args) in /home/marian/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.App/Program.cs:line 31
Aborted
```

In this case, we are trying to start an application (part of the [Article Source Code](/Sources)) using Core_Root. The application fails with `Assertion failed.` exception. All CLR components are built in Debug mode. The exception stack trace shows that there is an assertion that fails in `Where.cs` on line 263.

To fix this issue, we need to modify the `./src/libraries/System.Linq/src/System/Linq/Where.cs` file as follows:

Original:
```
public ArrayWhereSelectIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
{
    Debug.Assert(source is not null && source.Length > 0);
    Debug.Assert(predicate is not null);
    Debug.Assert(selector is not null);
    _source = source;
    _predicate = predicate;
    _selector = selector;
}
```

Modified:
```
public ArrayWhereSelectIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
{
    // Debug.Assert(source is not null && source.Length > 0);
    // Debug.Assert(predicate is not null);
    // Debug.Assert(selector is not null);
    _source = source;
    _predicate = predicate;
    _selector = selector;
}
```

Next, we need to rebuild to modified .NET Runtime components. In this case we have modified `System.Linq`, which is part of the Libraries component (i.e. `src/libraries`):

```./build.sh -subset libs -configuration Debug```

The last step, before we can re-run our application using the modified build, is to rebuild the Core_Root component using:

```./src/tests/build.sh -debug -p:LibrariesConfiguration=Debug -generatelayoutonly```

Once the Core_Root is updated, we can re-run the application:

```$CORE_ROOT/corerun ByteZoo.Blog.App.dll Concepts-String```

Output:
```
ByteZoo.Blog application started.
Constant string instance.
Dynamic string instance (generated using String.Concat).
Dynamic string instance (generated using StringBuilder).
Dynamic string instance (generated using String.Format).
Dynamic string instance (generated using string interpolation, Data = 36)
Press any key to continue ...
ByteZoo.Blog application completed.
```

## Debugging The Local .NET Runtime Using LLDB

Before debugging the .NET Runtime using LLDB, we need to install LLDB, .NET Core SDK and the SOS Debugging Extension. For details on how to install the .NET Core SDK, see [Install .NET Core On Linux](/Resources/Articles/Prerequisites/Install%20.NET%20Core%20On%20Linux.md). For details on how to install LLDB and the SOS Debugging Extension, see [Load .NET Core Dumps In LLDB On Linux](/Resources/Articles/Prerequisites/Load%20.NET%20Core%20Dumps%20In%20LLDB%20On%20Linux.md).

To load an application into LLDB, use the following:

```lldb -- $CORE_ROOT/corerun <Application.dll> [ApplicationArgument1] [ApplicationArgument2] ... [ApplicationArgumentN]```

To start the application, use the following:

```
process launch -s
process continue
```

You can set a breakpoint for a particular managed method using `bpmd`. E.g. to set breakpoint on `ByteZoo.Blog.App.Controllers.Concepts.StringController.Execute` method in assembly `ByteZoo.Blog.App.dll`, use the following:

```bpmd ByteZoo.Blog.App.dll ByteZoo.Blog.App.Controllers.Concepts.StringController.Execute```

> [!NOTE]
> Setting breakpoints must be done prior to running `process continue`. If you see a `warning: failed to set breakpoint site at` error, use `export DOTNET_EnableWriteXorExecute=0` before starting LLDB.

To set a breakpoint just after the CoreCLR is initialized, use the following:

```breakpoint set -n coreclr_execute_assembly```

To disable breakpoints on SIGUSR1 signals, use the following:

```process handle -s false SIGUSR1```

## Debugging The Local .NET Runtime Managed Code Using Visual Studio Code

Before debugging the .NET Runtime Managed Code using Visual Studio Code, you need to install Visual Studio Code and the [C# for Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp). For instructions on how to install Visual Studio Code on Linux see [Install Visual Studio Code On Linux](/Resources/Articles/Prerequisites/Install%20Visual%20Studio%20Code%20On%20Linux.md).

Launch Visual Studio Code in the folder containing the managed .NET Runtime component to debug. E.g. to debug the `System.Console` library use the following:

```
cd ~/CLR/src/libraries/System.Console/src
code .
```

In Visual Studio Code open the "Run And Debug" window (Ctrl+Shift+D) and select "create a lunch.json file". For the debugger, select ".NET 5+ and .NET Core". Update the launch configuration as follows:

```
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Debug Configuration",
            "type": "coreclr",
            "request": "launch",
            "program": "/home/<user>/CLR/artifacts/tests/coreclr/linux.x64.Debug/Tests/Core_Root/corerun",
            "args": [
                "Application.dll",
                "ApplicationArgument1",
                "ApplicationArgument2"
            ],
            "cwd": "/path/to/app-to-debug",
            "stopAtEntry": true,
            "console": "integratedTerminal",
            "justMyCode": false,
            "enableStepFiltering": false
        }
    ]
}
```

> [!NOTE]
> You can set `stopAtEntry` option to `false` if you do not want the debugger to break on application entry.

Once the launch configuration is updated, you can start the application using "Start Debugging (F5)". You can use all Visual Studio Code debugging tools (e.g. Variables, Watch, Call Stack, Breakpoints, Debug Console) as usual.

## Browse The Managed .NET APIs:

You can browse the managed .NET APIs using the following online tools:

* [.NET API Catalog](https://apisof.net)
* [.NET API Source Browser](https://source.dot.net)
* [.NET API Documentation Browser](https://learn.microsoft.com/dotnet/api)

## References

* [.NET Runtime](https://github.com/dotnet/runtime)
* [.NET Runtime Workflow Guide](https://github.com/dotnet/runtime/blob/main/docs/workflow/README.md)
* [.NET Runtime Debugging Guide](https://github.com/dotnet/runtime/blob/main/docs/workflow/debugging/coreclr/debugging-runtime.md)
* [Article Source Code](/Sources)
* [Article Script](/Resources/Scripts/Build%20The%20.NET%20Runtime%20From%20Source.sh)

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Source Code, Linux --->