using System;
using System.Web;
using Core.Common.Resources;

namespace Core.Common.Web.References
{
    public class WebResources : IResources
    {
        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return HttpContext.GetGlobalResourceObject(classKey, resourceKey);
        }
    }
}
