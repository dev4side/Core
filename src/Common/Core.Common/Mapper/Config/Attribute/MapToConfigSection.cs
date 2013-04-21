using System;

namespace Core.Common.Mappers
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MapToConfigSectionAttribute : Attribute
	{
		public string XPathExpr { get; set; }

        public MapToConfigSectionAttribute(string xpathExpr)
		{
			XPathExpr = xpathExpr;
		}
	}
}
