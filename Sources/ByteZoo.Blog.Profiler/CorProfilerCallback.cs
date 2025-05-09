using ByteZoo.Blog.Profiler.Models;
using ByteZoo.Blog.Profiler.Services;
using Silhouette;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.Profiler;

/// <summary>
/// CorProfilerCallback
/// </summary>
internal partial class CorProfilerCallback : CorProfilerCallback11Base
{

    #region Constants
    private const string CONFIG_MONITOR_THREADS = "BYTEZOO_BLOG_PROFILER_MONITOR_THREADS";
    private const string CONFIG_MONITOR_MODULES = "BYTEZOO_BLOG_PROFILER_MONITOR_MODULES";
    private const string CONFIG_MONITOR_ALLOCATED_TYPE_NAME = "CONFIG_MONITOR_ALLOCATED_TYPE_NAME";
    private const string CONFIG_ENABLE_STACK_SNAPSHOT = "BYTEZOO_BLOG_PROFILER_ENABLE_STACK_SNAPSHOT";
    #endregion

    #region Private Members
    private string? allocatedTypeName;
    private bool stackWalkEnabled;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="iCorProfilerInfoVersion"></param>
    /// <returns></returns>
    protected override HResult Initialize(int iCorProfilerInfoVersion)
    {
        if (iCorProfilerInfoVersion < 11)
        {
            DisplayService.WriteError($"Profiler requires ICorProfilerInfo11 (Current = ICorProfilerInfo{iCorProfilerInfoVersion}).");
            return HResult.E_FAIL;
        }
        var result = ICorProfilerInfo11.SetEventMask(GetEventMask());
        if (result)
            DisplayService.WriteInformation($"Profiler initialized (API Version = {iCorProfilerInfoVersion}).");
        return result;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Thread created event
    /// </summary>
    /// <param name="threadId"></param>
    /// <returns></returns>
    protected override HResult ThreadCreated(ThreadId threadId)
    {
        DisplayService.WriteInformation($"Thread created (OSID = {ICorProfilerInfo.GetThreadInfo(threadId).Result:X4}).");
        return HResult.S_OK;
    }

    /// <summary>
    /// Thread assigned to OS thread event
    /// </summary>
    /// <param name="managedThreadId"></param>
    /// <param name="osThreadId"></param>
    /// <returns></returns>
    protected override HResult ThreadAssignedToOSThread(ThreadId managedThreadId, int osThreadId)
    {
        DisplayService.WriteInformation($"Thread assigned to OS thread (OSID = {osThreadId:X4}).");
        return HResult.S_OK;
    }

    /// <summary>
    /// Thread destroyed event
    /// </summary>
    /// <param name="threadId"></param>
    /// <returns></returns>
    protected override HResult ThreadDestroyed(ThreadId threadId)
    {
        DisplayService.WriteInformation($"Thread destroyed (OSID = {ICorProfilerInfo.GetThreadInfo(threadId).Result:X4}).");
        return HResult.S_OK;
    }

    /// <summary>
    /// Module load started event
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    protected override HResult ModuleLoadStarted(ModuleId moduleId)
    {
        DisplayService.WriteInformation($"Module load started (Module ID = {moduleId.Value:X8}).");
        return HResult.S_OK;
    }

    /// <summary>
    /// Module load finished event
    /// </summary>
    /// <param name="moduleId"></param>
    /// <param name="hrStatus"></param>
    /// <returns></returns>
    protected override HResult ModuleLoadFinished(ModuleId moduleId, HResult hrStatus)
    {
        var moduleInfo = ICorProfilerInfo.GetModuleInfo(moduleId).ThrowIfFailed();
        DisplayService.WriteInformation($"Module load finished (Module ID = {moduleId.Value:X8}, Name = {moduleInfo.ModuleName}, Status = {hrStatus}, Base = {FormatAddress(moduleInfo.BaseLoadAddress)}).");
        return HResult.S_OK;
    }

    /// <summary>
    /// Object allocated event
    /// </summary>
    /// <param name="objectId"></param>
    /// <param name="classId"></param>
    /// <returns></returns>
    protected override HResult ObjectAllocated(ObjectId objectId, ClassId classId)
    {
        var typeName = GetTypeName(classId);
        if (typeName.Equals(allocatedTypeName, StringComparison.OrdinalIgnoreCase))
        {
            var mdToken = GetMetaDataToken(classId);
            DisplayService.WriteInformation($"Object allocated (Type = {typeName}, MethodTable = {FormatMethodTable(classId.Value)}, mdToken = {FormatMdToken(mdToken)}, Address = {FormatAddress(objectId.Value)}).");
            if (stackWalkEnabled)
                PerformStackWalk();
        }
        return HResult.S_OK;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return event mask
    /// </summary>
    /// <returns></returns>
    private COR_PRF_MONITOR GetEventMask()
    {
        var result = COR_PRF_MONITOR.COR_PRF_MONITOR_NONE;
        if (IsSwitchEnabled(CONFIG_MONITOR_THREADS))
            result |= COR_PRF_MONITOR.COR_PRF_MONITOR_THREADS;
        if (IsSwitchEnabled(CONFIG_MONITOR_MODULES))
            result |= COR_PRF_MONITOR.COR_PRF_MONITOR_MODULE_LOADS;
        if ((allocatedTypeName = GetSwitchValue(CONFIG_MONITOR_ALLOCATED_TYPE_NAME)) != null)
            result |= COR_PRF_MONITOR.COR_PRF_MONITOR_OBJECT_ALLOCATED | COR_PRF_MONITOR.COR_PRF_ENABLE_OBJECT_ALLOCATED;
        if (stackWalkEnabled = IsSwitchEnabled(CONFIG_ENABLE_STACK_SNAPSHOT))
            result |= COR_PRF_MONITOR.COR_PRF_ENABLE_STACK_SNAPSHOT;
        return result;
    }

    /// <summary>
    /// Check if switch is enabled
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool IsSwitchEnabled(string name) => (bool.TryParse(Environment.GetEnvironmentVariable(name), out var valueBool) && valueBool) || (int.TryParse(Environment.GetEnvironmentVariable(name), out var valueInt) && valueInt != 0);

    /// <summary>
    /// Return switch value
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string? GetSwitchValue(string name) => Environment.GetEnvironmentVariable(name);

    /// <summary>
    /// Format MethodTable
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string FormatMethodTable(nint value) => $"{value:X16}";

    /// <summary>
    /// Format mdToken
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string FormatMdToken(int? value) => value != null ? $"{value:X8}" : "N/A";

    /// <summary>
    /// Format address
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string FormatAddress(nint value) => $"0x{value:X16}";

    /// <summary>
    /// Format thread id
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string FormatThreadId(nuint value) => $"0x{value:X4}";

    /// <summary>
    /// Return mdToken
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    private int? GetMetaDataToken(ClassId classId)
    {
        try
        {
            return ICorProfilerInfo.GetClassIdInfo(classId).ThrowIfFailed().TypeDef.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Return type name
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    private string GetTypeName(ClassId classId)
    {
        try
        {
            var classIdInfo = ICorProfilerInfo.GetClassIdInfo(classId).ThrowIfFailed();
            using var moduleMetadata = ICorProfilerInfo.GetModuleMetaData(classIdInfo.ModuleId, CorOpenFlags.ofRead, KnownGuids.IMetaDataImport2).ThrowIfFailed().Wrap();
            var typeDefProps = moduleMetadata.Value.GetTypeDefProps(classIdInfo.TypeDef).ThrowIfFailed();
            return typeDefProps.TypeName;
        }
        catch
        {
            return "[N/A]";
        }
    }

    /// <summary>
    /// Perform stack walk
    /// </summary>
    private unsafe void PerformStackWalk()
    {
        StackWalkContext context = default;
        context.Count = 0;
        ThreadId threadId = default;
        if (ICorProfilerInfo2.DoStackSnapshot(threadId, &PerformStackWalkCallback, COR_PRF_SNAPSHOT_INFO.COR_PRF_SNAPSHOT_DEFAULT, &context, null, 0))
        {
            DisplayService.WriteInformation($"Thread Stack Frames (Current Thread ID = {FormatThreadId(GetCurrentThreadId())}):");
            for (int i = 0; i < context.Count; i++)
                DisplayService.WriteInformation($"- Thread Stack Frame[{i}] (Method = {GetMethodName(context.Frames[i])}, IP = {FormatMethodTable(context.Frames[i])}).");
        }
        else
            DisplayService.WriteError($"Stack walk failed.");
    }

    /// <summary>
    /// Stack snapshot callback
    /// </summary>
    /// <param name="funcId"></param>
    /// <param name="ip"></param>
    /// <param name="frameInfo"></param>
    /// <param name="contextSize"></param>
    /// <param name="context"></param>
    /// <param name="clientData"></param>
    /// <returns></returns>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe HResult PerformStackWalkCallback(FunctionId funcId, nint ip, COR_PRF_FRAME_INFO frameInfo, uint contextSize, byte* context, void* clientData)
    {
        ref StackWalkContext stackWalkContext = ref *(StackWalkContext*)clientData;
        if (stackWalkContext.Count >= StackWalkContext.MaxFrames)
            return HResult.E_ABORT;
        stackWalkContext.Frames[stackWalkContext.Count++] = ip;
        return HResult.S_OK;
    }

    /// <summary>
    /// Return current thread OSID
    /// </summary>
    /// <returns></returns>
    private nuint GetCurrentThreadId() => ICorProfilerInfo.GetThreadInfo(ICorProfilerInfo.GetCurrentThreadId().ThrowIfFailed()).ThrowIfFailed();

    /// <summary>
    /// Return method name
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private string GetMethodName(nint ip)
    {
        try
        {
            var functionId = ICorProfilerInfo.GetFunctionFromIP(ip).ThrowIfFailed();
            var functionInfo = ICorProfilerInfo.GetFunctionInfo(functionId).ThrowIfFailed();
            using var metaDataImport = ICorProfilerInfo.GetModuleMetaData(functionInfo.ModuleId, CorOpenFlags.ofRead, KnownGuids.IMetaDataImport).ThrowIfFailed().Wrap();
            var methodProperties = metaDataImport.Value.GetMethodProps(new MdMethodDef(functionInfo.Token)).ThrowIfFailed();
            var typeDefProps = metaDataImport.Value.GetTypeDefProps(methodProperties.Class).ThrowIfFailed();
            return $"{typeDefProps.TypeName}.{methodProperties.Name}";
        }
        catch
        {
            return "[N/A]";
        }
    }
    #endregion

}