using System;
using NUnit.Framework;

namespace Core.Test.CustomProperty
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class IdTestAttribute : PropertyAttribute
    {
        public string IdTest { get; private set; }
        public string UrlIdtest { get; private set; }

        public IdTestAttribute(string idTest): base(idTest)
        {
            IdTest = idTest;
        }

        public IdTestAttribute(string idTest, string urlIdtest) : this(idTest)
        {
            UrlIdtest = urlIdtest;
        }
    }
}