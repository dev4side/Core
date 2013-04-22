namespace Core.Common.Resource
{
    public interface IResources
    {
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
