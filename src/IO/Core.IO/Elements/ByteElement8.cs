using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ByteElement8 : BaseElement<byte>
	{
		public ByteElement8(string elementName, int bufferPosition) : base(elementName, bufferPosition, 1) { }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			Value = buffer[Position];

		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			buffer[Position] = Value;
		}
	}
}
