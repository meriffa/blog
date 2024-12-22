# References

## SOS Commands

- analyzeoom, AnalyzeOOM                                  Displays the info of the last OOM that occurred on an allocation request to the GC heap.
- assemblies, clrmodules                                  Lists the managed assemblies in the process.
- clrstack <arguments>                                    Provides a stack trace of managed code only.
- clrthreads <arguments>                                  Lists the managed threads running.
- crashinfo                                               Displays the crash details that created the dump.
- dbgout <arguments>                                      Enables/disables (-off) internal SOS logging.
- dumpalc <arguments>                                     Displays details about a collectible AssemblyLoadContext into which the specified object is loaded.
- dumparray <arguments>                                   Displays details about a managed array.
- dumpassembly <arguments>                                Displays details about an assembly.
- dumpasync, DumpAsync                                    Displays information about async "stacks" on the garbage-collected heap.
- dumpclass <arguments>                                   Displays information about a EE class structure at the specified address.
- dumpconcurrentdictionary, dcd <address>                 Displays concurrent dictionary content.
- dumpconcurrentqueue, dcq <address>                      Displays concurrent queue content.
- dumpdelegate <arguments>                                Displays information about a delegate.
- dumpdomain <arguments>                                  Displays the Microsoft intermediate language (MSIL) that's associated with a managed method.
- dumpexceptions                                          Displays a list of all managed exceptions.
- dumpgcdata <arguments>                                  Displays information about the GC data.
- dumpgen, dg <generation>                                Displays heap content for the specified generation.
- dumpheap, DumpHeap <memoryrange>                        Displays a list of all managed objects.
- dumphttp, DumpHttp                                      Displays information about HTTP requests.
- dumpil <arguments>                                      Displays the Microsoft intermediate language (MSIL) that is associated with a managed method.
- dumplocks, DumpLocks                                    Displays information about System.Threading.Lock objects, such as those being held by threads (default), or those being waited upon by threads.
- dumplog <arguments>                                     Writes the contents of an in-memory stress log to the specified file.
- dumpmd <arguments>                                      Displays information about a MethodDesc structure at the specified address.
- dumpmodule <arguments>                                  Displays information about a EE module structure at the specified address.
- dumpmt <arguments>                                      Displays information about a method table at the specified address.
- dumpobj, do <arguments>                                 Displays info about an object at the specified address.
- dumpobjgcrefs <object>                                  A helper command to implement !dumpobj -refs
- dumprequests, DumpRequests                              Displays all currently active incoming HTTP requests.
- dumpruntimetypes, DumpRuntimeTypes                      Finds all System.RuntimeType objects in the GC heap and prints the type name and MethodTable they refer too.
- dumpsig <arguments>                                     Dumps the signature of a method or field specified by <sigaddr> <moduleaddr>.
- dumpsigelem <arguments>                                 Dumps a single element of a signature object.
- dumpstackobjects, dso, DumpStackObjects <stackbounds>   Displays all managed objects found within the bounds of the current stack.
- dumpvc <arguments>                                      Displays info about the fields of a value class.
- eeheap, EEHeap <memoryrange>                            Displays information about native memory that CLR has allocated.
- eeversion <arguments>                                   Displays information about the runtime version.
- ehinfo <arguments>                                      Displays the exception handling blocks in a JIT-ed method.
- ephrefs                                                 Finds older generation objects which reference objects in the ephemeral segment.
- ephtoloh                                                Finds ephemeral objects which reference the large object heap.
- finalizequeue, fq, FinalizeQueue                        Displays all objects registered for finalization.
- findappdomain <arguments>                               Attempts to resolve the AppDomain of a GC object.
- gchandles <arguments>                                   Provides statistics about GCHandles in the process.
- gcheapstat, GCHeapStat                                  Displays various GC heap stats.
- gcinfo <arguments>                                      Displays JIT GC encoding for a method.
- gcroot, GCRoot <target>                                 Displays info about references (or roots) to an object at the specified address.
- gcwhere, GCWhere <address>                              Displays the location in the GC heap of the specified address.
- help, soshelp <command>                                 Displays help for a command.
- histclear <arguments>                                   Releases any resources used by the family of Hist commands.
- histinit <arguments>                                    Initializes the SOS structures from the stress log saved in the debuggee.
- histobj <arguments>                                     Examines all stress log relocation records and displays the chain of garbage collection relocations that may have led to  the address passed in as an argument.
- histobjfind <arguments>                                 Displays all the log entries that reference an object at the specified address.
- histroot <arguments>                                    Displays information related to both promotions and relocations of the specified root.
- histstats <arguments>                                   Displays stress log stats.
- ip2md <arguments>                                       Displays the MethodDesc structure at the specified address in code that has been JIT-compiled.
- listnearobj, lno, ListNearObj <address>                 Displays the object preceding and succeeding the specified address.
- loadsymbols <url>                                       Loads symbols for all modules.
- logclose <path>                                         Disables console file logging.
- logging <path>                                          Enables/disables internal diagnostic logging.
- logopen <path>                                          Enables console file logging.
- modules, lm                                             Displays the native modules in the process.
- name2ee <arguments>                                     Displays the MethodTable structure and EEClass structure for the specified type or method in the specified module.
- notreachableinrange <start> <end>                       A helper command for !finalizerqueue
- objsize, ObjSize <objectaddress>                        Lists the sizes of the all the objects found on managed threads.
- parallelstacks, pstacks                                 Displays the merged threads stack similarly to the Visual Studio 'Parallel Stacks' panel.
- pathto, PathTo <source> <target>                        Displays the GC path from <root> to <target>.
- printexception, pe <arguments>                          Displays and formats fields of any object derived from the Exception class at the specified address.
- registers, r                                            Displays the thread's registers.
- runtimes <id>                                           Lists the runtimes in the target or changes the default runtime.
- setclrpath <path>                                       Sets the path to load coreclr DAC/DBI files.
- setsymbolserver, SetSymbolServer <url>                  Enables and sets symbol server support for symbols and module download.
- sizestats                                               Size statistics for the GC heap.
- sosflush                                                Resets the internal cached state.
- sosstatus                                               Displays internal status.
- syncblk <arguments>                                     Displays the SyncBlock holder info.
- taskstate, tks <address>                                Displays a Task state in a human readable format.
- threadpool, ThreadPool                                  Displays info about the runtime thread pool.
- threadpoolqueue, tpq                                    Displays queued ThreadPool work items.
- threads, setthread <thread>                             Lists the threads in the target or sets the current thread.
- threadstate <arguments>                                 Pretty prints the meaning of a threads state.
- timerinfo, ti                                           Displays information about running timers.
- traverseheap, TraverseHeap <filename>                   Writes out heap information to a file in a format understood by the CLR Profiler.
- verifyheap, VerifyHeap <memoryrange>                    Searches the managed heap for memory corruption.
- verifyobj, VerifyObj <objectaddress>                    Checks the given object for signs of corruption.

## LLDB Commands

- bpmd                                                    Creates a breakpoint at the specified managed method in the specified module.
- breakpoint delete, br del                               Delete the specified breakpoint(s). If no breakpoints are specified, delete them all.
- breakpoint list, br l                                   List some or all breakpoints at configurable levels of detail.
- breakpoint set, br s                                    Sets a breakpoint or set of breakpoints in the executable.
- clru                                                    Displays an annotated disassembly of a managed method.
- disassemble, di                                         Disassemble specified instructions in the current target.
- expression,expr                                         Evaluate an expression on the current thread. Displays any returned value with LLDB's default formatting.
- process continue, continue                              Continue execution of all threads in the current process.
- run                                                     Launch the executable in the debugger.
- target modules list, image list                         List current executable and dependent shared library images.
- target modules lookup, image lookup                     Look up information within executable and dependent shared library images.
- thread backtrace, bt                                    Show thread call stacks.
- thread info                                             Show an extended summary of one or more threads.
- thread list                                             Show a summary of each thread in the current target process.
- thread select, t                                        Change the currently selected thread.
- thread step-in, s                                       Source level single step, stepping into calls.
- thread step-inst-over, ni                               Instruction level single step, stepping over calls.
- thread step-inst, si                                    Instruction level single step, stepping into calls.
- thread step-out, finish                                 Finish executing the current stack frame and stop after returning.
- thread step-over, n                                     Source level single step, stepping over calls.
- watch list                                              List all watchpoints at configurable levels of detail.
- watch set expression                                    Set a watchpoint on an address by supplying an expression. 

## LLDB Configuration

- Setup Intel Disassembly Format                          `echo "settings set target.x86-disassembly-flavor intel" >> ~/.lldbinit`

## System.String

!wfrom -type System.String where $substr($string(), 0, 6) == "User 0" select $a("Address\t", $addr()), $a("Value\t", $string()), $a("Length\t", _stringLength)

## System.Collections.Generic.Dictionary<TKey, TValue>

.foreach ($instance {!wheap -type System.Collections.Generic.Dictionary* -short}) {.echo "${$instance}-${$instance}"}
.foreach ($instance {!windex -short -type System.Collections.Generic.Dictionary*}) {!wselect * from $instance}
!wfrom -type System.Collections.Generic.Dictionary* where ($contains($typename(), "<System.")) && (_count > 0) select $typename(), _count

## System.Net.Http.HttpClient

- Active Requests
  !wfrom -type *.HttpConnection where (_inUse == 1) select $addr(), _currentRequest._method._method, _currentRequest._requestUri._string
  !wfrom -type *.HttpClient where (_disposed == 0) select $addr()
  .foreach ($instance {!DumpHeap -short -type HttpClient+<<SendAsync>}) {!wfrom -obj $instance where (m_result == 0) select StateMachine.__4__this, StateMachine.request._requestUri._string}
  .foreach ($instance {!DumpHeap -short -type <SendAsyncCore>}) {!wfrom -obj $instance where (m_result == 0) select StateMachine.__4__this, StateMachine.request._requestUri._string}
- Completed Requests
  !wfrom -type *.HttpResponseMessage select $addr(), _statusCode, _requestMessage._method._method, _requestMessage._requestUri._string, $rawfield(_content._bufferedContent._buffer)
  !wfrom -type *.HttpConnection where (_inUse == 0) select $addr()
  !wfrom -type *.HttpClient where (_disposed == 1) select $addr()
  .foreach ($instance {!DumpHeap -short -type HttpClient+<<SendAsync>}) {!wfrom -obj $instance where (m_result != 0) select m_result}
  .foreach ($instance {!DumpHeap -short -type <SendAsyncCore>}) {!wfrom -obj $instance where (m_result != 0) select m_result}
- All Requests
  !wfrom -type *.HttpConnection select $addr(), _inUse
  !wfrom -type *.HttpClient select $addr(), _disposed
  !wfrom -type *.HttpRequestMessage select $addr(), _method._method, _requestUri._string

## Kestrel

- Active Requests
  !wfrom -type *.Http.Http1Connection* where (_requestProcessingStatus != 0) select $addr(), _methodText, _parsedRawTarget, _requestId, _endpoint._DisplayName_k__BackingField, _responseBytesWritten, _requestProcessingStatus
  !wfrom -type *.Http.DefaultHttpContext select $addr(), _request._features._Collection_k__BackingField._methodText, _request._features._Collection_k__BackingField._parsedRawTarget, _request._features._Collection_k__BackingField._requestId, _request._features._Collection_k__BackingField._endpoint._DisplayName_k__BackingField, _request._features._Collection_k__BackingField._responseBytesWritten, _request._features._Collection_k__BackingField._requestProcessingStatus, _response._features._Collection_k__BackingField._requestId, _response._features._Collection_k__BackingField._responseBytesWritten, _response._features._Collection_k__BackingField._requestProcessingStatus
  !wfrom -type *.Http.DefaultHttpRequest select $addr(), _features._Collection_k__BackingField._methodText, _features._Collection_k__BackingField._parsedRawTarget, _features._Collection_k__BackingField._requestId, _features._Collection_k__BackingField._endpoint._DisplayName_k__BackingField, _features._Collection_k__BackingField._responseBytesWritten, _features._Collection_k__BackingField._requestProcessingStatus
  !wfrom -type *.Http.DefaultHttpResponse select $addr(), _features._Collection_k__BackingField._methodText, _features._Collection_k__BackingField._parsedRawTarget, _features._Collection_k__BackingField._requestId, _features._Collection_k__BackingField._endpoint._DisplayName_k__BackingField, _features._Collection_k__BackingField._responseBytesWritten, _features._Collection_k__BackingField._requestProcessingStatus
- Pending Requests
  !wfrom -type *.Http.Http1Connection* where (_requestProcessingStatus == 0) select $addr(), _parsedRawTarget
- List Middleware
  !wfrom -type *Middleware select $addr(), $typename()