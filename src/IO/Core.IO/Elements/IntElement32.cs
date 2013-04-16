using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class IntElement32 : BaseElement<int>
	{
		public BufferDirection ElementDirection = BufferDirection.Backward;

		public IntElement32(string elementName, int bufferPosition) : base(elementName, bufferPosition, 4) { }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			Value = BufferHelper.GetInt(buffer[Position], buffer[Position + 1], buffer[Position + 2], buffer[Position + 3], ElementDirection);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			var bytes = BufferHelper.GetBytes(Value, ElementDirection);
			buffer[Position + 0] = bytes[0];
			buffer[Position + 1] = bytes[1];
			buffer[Position + 2] = bytes[2];
			buffer[Position + 3] = bytes[3];
		}

	}
}