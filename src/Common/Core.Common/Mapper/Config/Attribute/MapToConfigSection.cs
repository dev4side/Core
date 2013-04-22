using System;

namespace Core.Common.Mapper.Config.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MapToConfigSectionAttribute : System.Attribute
	{
		public string XPathExpr { get; set; }

        public MapToConfigSectionAttribute(string xpathExpr)
		{
			XPathExpr = xpathExpr;
		}
	}
}
