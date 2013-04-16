using System;
using System.Text;

namespace Core.IO
{
	public class StringElement : BaseElement<string>
	{
		private Encoding _encoding { get; set; }
		private byte[] _null = null;

		//See "Name" column on table http://msdn.microsoft.com/en-us/library/system.text.encoding
		//Use only ASCII compatible encodings.. doesn't work for wide encodings like UTF32, ecc..
 		//most common encodings are "us-ascii", "utf-8", "utf-7", "iso-8859-1".. default is "us-ascii".
		public StringElement(string elementName, int bufferPosition, string encodingName = null) 
			: base(elementName, bufferPosition, 1)
		{
			if(encodingName == null)
				encodingName = BufferHelper.DefaultStringEncoding;
			_encoding = Encoding.GetEncoding(encodingName);
			_null = _encoding.GetBytes("\0");
		}

		public override int Size
		{
			get { return (this.Value != null ? _encoding.GetByteCount(Value) : 0) + _null.Length; }
		}

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			//int size = 0;
			//int ptr = Position;

			//while( (ptr < buffer.Length) && (buffer[ptr] != 0)) ptr++;
			//size = ptr - Position;
			int nullIndex = BufferHelper.RawFind(_null, buffer, Position, buffer.Length - Position, _null.Length);
			int size = (nullIndex != -1 ? nullIndex : buffer.Length) - Position;

			//TODO: check null termination..
			Value = _encoding.GetString(buffer, Position, size);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			var bytes = this.Value != null ? _encoding.GetBytes(Value) : new Byte[0];
			System.Buffer.BlockCopy(bytes, 0, buffer, Position, bytes.Length);
			System.Buffer.BlockCopy(_null, 0, buffer, Position + bytes.Length, _null.Length);
		}

		public override bool IsValid()
		{
			return this.Value != null;
		}

		public override string ToString() { return String.Format("{0}\t= {1}", Name, Value); }

	}
}