using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ShortElement16 : BaseElement<short>
	{
		public BufferDirection ElementDirection = BufferDirection.Backward;

		public ShortElement16(string elementName, int bufferPosition) : base(elementName, bufferPosition, 2) { }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			Value = BufferHelper.GetShort(buffer[Position], buffer[Position + 1], ElementDirection);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			buffer[Position] = BufferHelper.GetMSB(Value, ElementDirection);
			buffer[Position + 1] = BufferHelper.GetLSB(Value, ElementDirection);
		}

	}
}
