using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public interface IElement
	{
		string Name { get; set; }
		int Position { get; set; }
		int Size { get; }

		void UpdateValueFromBuffer(byte[] buffer);
		void UpdateBufferFromValue(byte[] buffer);
	}

	public interface IElement<TStruct> : IElement
	{
		TStruct Value { get; set; }
	}
}
