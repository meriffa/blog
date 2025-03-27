using CommandLine;
using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Method dispatch controller
/// </summary>
[Verb("Concepts-MethodDispatch", HelpText = "Method dispatch operation.")]
public class MethodDispatchController : Controller
{

    #region Interfaces & Classes
    /// <summary>
    /// Base interface
    /// </summary>
    private interface IInterfaceBase
    {

        #region Methods
        /// <summary>
        /// Interface (virtual) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        string MethodInterfaceVirtual() => "MethodInterfaceVirtual()";

        /// <summary>
        /// Interface (abstract) method
        /// </summary>
        /// <returns></returns>
        string MethodInterfaceAbstract();

        /// <summary>
        /// Interface (sealed) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        sealed string MethodInterfaceSealed() => $"{MethodInterfaceVirtual()}:Sealed";

        /// <summary>
        /// Interface (static, virtual) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        static virtual string MethodInterfaceStaticVirtual() => "MethodInterfaceStaticVirtual()";

        /// <summary>
        /// Interface (static, abstract) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        static abstract string MethodInterfaceStaticAbstract();

        /// <summary>
        /// Interface (static, sealed) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        static sealed string MethodInterfaceStaticSealed() => "MethodInterfaceStaticSealed()";
        #endregion

    }

    /// <summary>
    /// Base class
    /// </summary>
    private abstract class ClassBase
    {

        #region Fields
        private readonly string methodNonVirtual = "MethodNonVirtual()";
        #endregion

        #region Methods
        /// <summary>
        /// Static method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string MethodStatic() => "MethodStatic()";

        /// <summary>
        /// Non-Virtual method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string MethodNonVirtual() => methodNonVirtual;

        /// <summary>
        /// Virtual method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual string MethodVirtual() => "MethodVirtual()";

        /// <summary>
        /// Abstract method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public abstract string MethodAbstract();
        #endregion

    }

    /// <summary>
    /// Derived class
    /// </summary>
    private class ClassDerived : ClassBase, IInterfaceBase
    {

        #region Fields
        private readonly string methodNonVirtual = "MethodNonVirtualNew()";
        #endregion

        #region Methods
        /// <summary>
        /// Non-Virtual (replace) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public new string MethodNonVirtual() => methodNonVirtual;

        /// <summary>
        /// Virtual method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public override string MethodVirtual() => "MethodVirtualOverride()";

        /// <summary>
        /// Abstract method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public override string MethodAbstract() => "MethodAbstract()";

        /// <summary>
        /// Interface (abstract) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string MethodInterfaceAbstract() => "MethodInterfaceAbstract()";

        /// <summary>
        /// Interface (static, abstract) method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string MethodInterfaceStaticAbstract() => "MethodInterfaceStaticAbstract()";
        #endregion

    }

    /// <summary>
    /// Generic class
    /// </summary>
    private class ClassGeneric<T> : ClassDerived
    {

        #region Fields
        private readonly string methodGeneric = "MethodGeneric<{0}>()";
        #endregion

        #region Methods
        /// <summary>
        /// Generic method
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string MethodGeneric() => string.Format(methodGeneric, typeof(T).Name.ToString());
        #endregion

    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var instanceInt = new ClassGeneric<int>();
        var instanceString = new ClassGeneric<string>();
        var instanceObject = new ClassGeneric<MethodDispatchController>();
        displayService.WriteInformation(MethodStatic());
        displayService.WriteInformation(MethodNonVirtualBase(instanceObject));
        displayService.WriteInformation(MethodNonVirtualDerived(instanceObject));
        displayService.WriteInformation(MethodVirtual(instanceObject));
        displayService.WriteInformation(MethodAbstract(instanceObject));
        displayService.WriteInformation(MethodInterfaceVirtual(instanceObject));
        displayService.WriteInformation(MethodInterfaceAbstract(instanceObject));
        displayService.WriteInformation(MethodInterfaceSealed(instanceObject));
        displayService.WriteInformation(MethodInterfaceStaticVirtual<ClassGeneric<MethodDispatchController>>());
        displayService.WriteInformation(MethodInterfaceStaticAbstract<ClassGeneric<MethodDispatchController>>());
        displayService.WriteInformation(MethodInterfaceStaticSealed());
        displayService.WriteInformation(MethodGeneric(instanceObject));
        displayService.WriteInformation(MethodGeneric(instanceString));
        displayService.WriteInformation(MethodGeneric(instanceInt));
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Call static method
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodStatic() => ClassBase.MethodStatic();

    /// <summary>
    /// Call Non-Virtual (base) method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodNonVirtualBase(ClassBase instance) => instance.MethodNonVirtual();

    /// <summary>
    /// Call Non-Virtual (derived) method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodNonVirtualDerived(ClassDerived instance) => instance.MethodNonVirtual();

    /// <summary>
    /// Call virtual method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodVirtual(ClassBase instance) => instance.MethodVirtual();

    /// <summary>
    /// Call abstract method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodAbstract(ClassBase instance) => instance.MethodAbstract();

    /// <summary>
    /// Call interface (virtual) method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceVirtual(IInterfaceBase instance) => instance.MethodInterfaceVirtual();

    /// <summary>
    /// Call interface (abstract) method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceAbstract(IInterfaceBase instance) => instance.MethodInterfaceAbstract();

    /// <summary>
    /// Call interface (sealed) method
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceSealed(IInterfaceBase instance) => instance.MethodInterfaceSealed();

    /// <summary>
    /// Call interface (static, virtual) method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceStaticVirtual<T>() where T : IInterfaceBase => T.MethodInterfaceStaticVirtual();

    /// <summary>
    /// Call interface (static, abstract) method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceStaticAbstract<T>() where T : IInterfaceBase => T.MethodInterfaceStaticAbstract();

    /// <summary>
    /// Call interface (static, sealed) method
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodInterfaceStaticSealed() => IInterfaceBase.MethodInterfaceStaticSealed();

    /// <summary>
    /// Call Generic method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string MethodGeneric<T>(ClassGeneric<T> instance) => instance.MethodGeneric();
    #endregion

}