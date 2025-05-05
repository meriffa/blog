namespace Silhouette.Interfaces;

[NativeObject]
internal unsafe interface ICorProfilerInfo11 : ICorProfilerInfo10
{

    public new static readonly Guid Guid = new("06398876-8987-4154-B621-40A00D6E4D04");

    HResult GetEnvironmentVariable(char* szName, uint cchValue, out uint pcchValue, char* szValue);

    HResult SetEnvironmentVariable(char* szName, char* szValue);

}