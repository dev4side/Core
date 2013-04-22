using System;

namespace Core.Data.Attributes
{
    /// <summary>
    /// Indicates that the property is excluded from the cloning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CloneExcludeAttribute : Attribute { }
}
