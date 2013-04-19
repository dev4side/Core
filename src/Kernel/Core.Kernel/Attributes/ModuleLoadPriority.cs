using System;

namespace Core.Kernel.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ModuleLoadPriorityAttribute : Attribute
    {
        public int Priority { get; set; }
        
        public ModuleLoadPriorityAttribute(int priority)
        {
            this.Priority = priority;
        }
    }
}