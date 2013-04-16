using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ElementTypeAttribute : Attribute
	{
		public Type ElementType { get; set; }

		public int? BufferPosition { get; set; }

		public string ElementName { get; set; }

		public string ArrayLengthProperty { get; set; }

		public bool IsArray { get; set; }
	}
}
