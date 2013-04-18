using System;

namespace Core.Common.Mappers
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MapToConfigSection : Attribute
	{
		public string XPathExpr { get; set; }

		public MapToConfigSection(string xpathExpr)
		{
			XPathExpr = xpathExpr;
		}
	}
}
