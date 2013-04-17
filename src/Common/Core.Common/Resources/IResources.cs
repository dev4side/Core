using System;

namespace Core.Common.Resources
{
    public interface IResources
    {
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
