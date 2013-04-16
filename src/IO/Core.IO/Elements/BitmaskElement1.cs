using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class BitmaskElement1 : BaseElement<bool>
	{
		protected byte ValueMask = 0x01;

		public BitmaskElement1(string elementName, int bufferPosition, int bit = 0)
			: base(elementName, bufferPosition, 1)
		{
			if (bit < 0 || bit > 7)
				throw new ArgumentException("Bit out of range", "bit");

			ValueMask = (byte)(1 << bit);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			if (Value)
				buffer[Position] |= ValueMask;
			else
				buffer[Position] &= (byte)(~ValueMask);
		}

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			Value = (buffer[Position] & ValueMask) != 0;
		}

	}
}
