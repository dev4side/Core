using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class BitmaskElement8 : BaseElement<bool[]>
	{
		protected byte ValueMask = 0xFF;

		public BitmaskElement8(string elementName, int bufferPosition, byte valueMask = 0xFF)
			: base(elementName, bufferPosition, 1)
		{
			ValueMask = valueMask;
		}

		public void SetBitAt(int bit, bool val)
		{
			if (bit < 0 || bit > 7)
				throw new ArgumentException("Bit out of range", "bit");

			if ((ValueMask & (byte)(1 << bit)) != 0)
			{
				Value[bit] = val;
				/*
				if (val)
					Value |= (byte)(1 << bit);
				else
					Value &= (byte)~(1 << bit);
				 */
			}
		}

		public bool GetBitAt(int bit)
		{
			if (bit < 0 || bit > 7)
				throw new ArgumentException("Bit out of range", "bit");

			return Value[bit];
			//return (Value & (byte)(1 << bit)) != 0;
		}

		public override string ToString()
		{
			return String.Format("{0}\t= |{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|",
				Name,
				GetBitAt(7) ? 'Y' : 'N',
				GetBitAt(6) ? 'Y' : 'N',
				GetBitAt(5) ? 'Y' : 'N',
				GetBitAt(4) ? 'Y' : 'N',
				GetBitAt(3) ? 'Y' : 'N',
				GetBitAt(2) ? 'Y' : 'N',
				GetBitAt(1) ? 'Y' : 'N',
				GetBitAt(0) ? 'Y' : 'N');
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			byte b = 0x00;
			for (int i = 0; i < 8; i++)
				if(Value[i])
					b |= (byte)(1 << i);
			buffer[Position] = b;
		}

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			byte b = buffer[Position];
			for (int i = 0; i < 8; i++)
				if ((ValueMask & (byte)(1 << i)) != 0)
					Value[i] = (b & (byte)(1 << i)) != 0;
		}

	}
}
