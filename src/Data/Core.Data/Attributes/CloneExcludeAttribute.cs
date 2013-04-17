using System;

namespace Core.Data.Attributes
{
    /// <summary>
    /// indica che questa proprietà viene esclusa dalla clonazione  CloneAsNew()
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CloneExcludeAttribute : Attribute { }
}
