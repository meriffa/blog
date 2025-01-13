namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee schema attribute
/// </summary>
/// <param name="version"></param>
[AttributeUsage(AttributeTargets.Class)]
public class EmployeeSchemaAttribute(string version) : Attribute
{

    #region Public Members
    /// <summary>
    /// Employee schema version
    /// </summary>
    public readonly string Version = version;
    #endregion

}