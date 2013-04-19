using System;
using NUnit.Framework;

namespace Core.Test.CustomProperty
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class TestCategoryAttribute : PropertyAttribute
    {
        public string Category { get; private set; }

        public TestCategoryAttribute(string category) : base(category)
        {
            Category = category;
        }
    }
}