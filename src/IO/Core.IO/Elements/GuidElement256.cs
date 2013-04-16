using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class GuidElement256 : BaseElement<Guid>
	{
		//private byte[32] bytes;

		public GuidElement256(string elementName, int bufferPosition) : base(elementName, bufferPosition, 32) { Size = 32; }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			Guid tmpGuid;
			string s = BitConverter.ToString(buffer, Position, Size);
			Value = Guid.TryParseExact(s, "N", out tmpGuid) ? tmpGuid : Guid.Empty;
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			var bytes = Encoding.ASCII.GetBytes(Value.ToString("N"));
			System.Buffer.BlockCopy(bytes, 0, buffer, Position, Size);
		}

		public override string ToString() { return String.Format("{0}\t= {1}", Name, Value.ToString("B")); }
	}
}
