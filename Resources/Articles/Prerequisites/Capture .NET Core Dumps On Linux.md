# Capture .NET Core Dumps On Linux

... Build on [Create .NET Core Dumps On Linux.md] & [Load .NET Core Dumps In LLDB On Linux.md] ...

## Capture Core Dumps

* Docker (--cap-add=SYS_PTRACE or --privileged)
* Azure: Functions, Containers
* AWS: Lambda, Containers
* NativeAOT (self hosted: lldb -c CoreDump_Full.%p ByteZoo.Blog.App)
* systemd-coredump

## References

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Core Dump, Linux --->

---

* Update README.md