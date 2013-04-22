namespace Core.Common.Resource
{
    public interface IResources
    {
        /// <summary>
        /// Retrieves the specified resource from the global resource object.
        /// </summary>
        /// <param name="classKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ClassKey property 
        /// of the requested resource object.</param>
        /// <param name="resourceKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ResourceKey 
        /// property of the requested resource object.</param>
        /// <returns>An System.Object that represents the requested application-level resource object, or null if a resource object 
        /// is not found or if a resource object is found but it does not have the requested property.</returns>
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
