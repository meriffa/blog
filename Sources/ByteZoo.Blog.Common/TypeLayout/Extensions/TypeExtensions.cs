using ByteZoo.Blog.Common.TypeLayout.Models;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.Common.TypeLayout.Extensions;

/// <summary>
/// Type extension methods
/// </summary>
public static class TypeExtensions
{

    #region Public Methods
    /// <summary>
    /// Returns type instance size and overhead (Value Type Overhead = 0, Reference Type Overhead = 2 * IntPtr.Size)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static (int size, int overhead) GetSize(this Type type)
    {
        if (type.IsValueType)
            return (size: GetSizeValueType(type), overhead: 0);
        else
            return (size: GetSizeReferenceType(type), overhead: 2 * IntPtr.Size);
    }

    /// <summary>
    /// Return field size (Reference Type Size = IntPtr.Size)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetFieldSize(this Type type) => type.IsValueType ? GetSizeValueType(type) : IntPtr.Size;

    /// <summary>
    /// Return field & offset information list
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static (FieldInfo field, int offset)[] GetFieldOffsets(this Type type)
    {
        // Use custom function, since System.Type.GetFields() does not return private fields from base types.
        var fields = type.GetInstanceFields();
        // var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        Func<object?, long[]> inspectInstance = GetFieldOffsetInspectionFunction(fields);
        var (instance, success) = type.TryCreateInstanceSafe();
        if (!success)
            return [];
        var addresses = inspectInstance(instance);
        if (addresses.Length <= 1)
            return [];
        var baseline = GetBaselineAddress(type, addresses[0]);
        // Convert field addresses to offsets using the first field as a baseline.
        return [.. fields.Select((field, index) => (Field: field, Offset: (int)(addresses[index + 1] - baseline))).OrderBy(i => i.Offset)];
    }

    /// <summary>
    /// Returns all instance fields including the fields in all base types
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static FieldInfo[] GetInstanceFields(this Type type) => [.. GetCurrentAndAllBaseTypes(type).SelectMany(GetDeclaredFields).Where(i => !i.IsStatic)];

    /// <summary>
    /// Create type instance
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <remarks>
    /// Types not supported by this function:
    /// - Open generic types (List<T>)
    /// - Abstract types
    /// </remarks>
    public static (object? result, bool success) TryCreateInstanceSafe(this Type type)
    {
        if (!CanCreateInstance(type))
            return (result: null, success: false);
        // Value types are handled separately
        if (type.IsValueType)
            return IsNullable(type) ? Success(GetUninitializedObject(type)) : Success(Activator.CreateInstance(type));
        // String is handled separately as well due to security restrictions
        if (type == typeof(string))
            return Success(string.Empty);
        // It is actually possible to get null for some security related types
        return Success(GetUninitializedObject(type));
    }

    /// <summary>
    /// Check if type instance can be created
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanCreateInstance(this Type type)
    {
        // Abstract types, open generics and COM types are not supported
        if (type.IsAbstract || IsOpenGenericType(type) || type.IsCOMObject)
            return false;
        // Special types
        if (type == typeof(RuntimeArgumentHandle) || type == typeof(TypedReference) || type.Name == "Void" || type == typeof(IsVolatile) || type == typeof(RuntimeFieldHandle) || type == typeof(RuntimeMethodHandle) || type == typeof(RuntimeTypeHandle))
            return false;
        if (type.BaseType == typeof(ContextBoundObject))
            return false;
        return true;
    }

    /// <summary>
    /// Check if type is unsafe
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsUnsafeValueType(this Type type) => type.GetCustomAttribute<UnsafeValueTypeAttribute>() != null;
    #endregion

    #region Private Methods
    /// <summary>
    /// Check if value type is nullable
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

    /// <summary>
    /// Return value type size
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int GetSizeValueType(Type type) => GetFieldOffsets(typeof(PlaceholderStruct<>).MakeGenericType(type))[1].offset;

    /// <summary>
    /// Return reference type size excluding overhead
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int GetSizeReferenceType(Type type)
    {
        var fields = GetFieldOffsets(type);
        if (fields.Length == 0)
            // The size of empty class is IntPtr.Size
            return IntPtr.Size;
        // Reference Type Size = MaxFieldOffset + MaxFieldSize -> Round To Closest Pointer Size Boundary
        var (field, offset) = fields.GetMaximum(i => i.offset);
        var sizeCandidate = offset + GetFieldSize(field.FieldType);
        var roundTo = IntPtr.Size - 1;
        return (sizeCandidate + roundTo) & (~roundTo);
    }

    /// <summary>
    /// Return current and all base types
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static IEnumerable<Type> GetCurrentAndAllBaseTypes(Type? type)
    {
        while (type != null)
        {
            yield return type;
            type = type.BaseType;
        }
    }

    /// <summary>
    /// Return declared fields
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static IEnumerable<FieldInfo> GetDeclaredFields(Type type)
    {
        if (type is TypeInfo typeInfo)
            return typeInfo.DeclaredFields;
        return type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    /// <summary>
    /// Creates a new instance of the specified object type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static object? GetUninitializedObject(Type type)
    {
        try
        {
            return RuntimeHelpers.GetUninitializedObject(type);
        }
        catch (TypeInitializationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Return object instance result
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    private static (object? result, bool success) Success(object? instance) => (instance, instance != null);

    /// <summary>
    /// Check if type is open generic type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsOpenGenericType(Type type) => type.IsGenericTypeDefinition && !type.IsConstructedGenericType;

    /// <summary>
    /// Return baseline address
    /// </summary>
    /// <param name="type"></param>
    /// <param name="referenceAddress"></param>
    /// <returns></returns>
    private static long GetBaselineAddress(Type type, long referenceAddress) => type.IsValueType ? referenceAddress : referenceAddress + IntPtr.Size;

    /// <summary>
    /// Return field offset inspection function
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private static Func<object?, long[]> GetFieldOffsetInspectionFunction(FieldInfo[] fields)
    {
        var method = new DynamicMethod(name: "GetFieldOffsets", returnType: typeof(long[]), parameterTypes: [typeof(object)], m: typeof(TypeExtensions).Module, skipVisibility: true);
        var il = method.GetILGenerator();
        // Declare local variable of type long[]
        il.DeclareLocal(typeof(long[]));
        // Push field array size onto evaluation stack
        il.Emit(OpCodes.Ldc_I4, fields.Length + 1);
        // Push new long[] array reference onto evaluation stack
        il.Emit(OpCodes.Newarr, typeof(long));
        // Pop long[] array reference and store in local
        il.Emit(OpCodes.Stloc_0);
        // Load local with the array reference
        il.Emit(OpCodes.Ldloc_0);
        // Load an index of the array where we are going to store the element
        il.Emit(OpCodes.Ldc_I4, 0);
        // Load object instance onto evaluation stack
        il.Emit(OpCodes.Ldarg_0);
        // Convert reference to long
        il.Emit(OpCodes.Conv_I8);
        // Store the reference in the array
        il.Emit(OpCodes.Stelem_I8);
        for (int i = 0; i < fields.Length; i++)
        {
            // Load the local with an array
            il.Emit(OpCodes.Ldloc_0);
            // Load an index of the array where we are going to store the element
            il.Emit(OpCodes.Ldc_I4, i + 1);
            // Load object instance onto evaluation stack
            il.Emit(OpCodes.Ldarg_0);
            // Get the address for a given field
            il.Emit(OpCodes.Ldflda, fields[i]);
            // Convert field offset to long
            il.Emit(OpCodes.Conv_I8);
            // Store the offset in the array
            il.Emit(OpCodes.Stelem_I8);
        }
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ret);
        return (Func<object?, long[]>)method.CreateDelegate(typeof(Func<object, long[]>));
    }
    #endregion

}