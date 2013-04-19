using System;
using NUnit.Framework;

namespace Core.Test.CustomProperty
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class DesignRequirementAttribute : PropertyAttribute
    {
        public string DesignRequirementId { get; private set; }

        public DesignRequirementAttribute(string designRequirementId): base(designRequirementId)
        {
            DesignRequirementId = designRequirementId;
        }
    }
}