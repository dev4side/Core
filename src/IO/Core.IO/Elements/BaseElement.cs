using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class BaseElement<TStruct> : IElement<TStruct>, IValidable
	{
		public string Name { get; set; }
		public int Position { get; set; }
		public virtual int Size { get; set; }

		public virtual TStruct Value { get; set; }

		protected BaseElement(string elementName, int bufferPosition, int elementSize)
		{
			Size = elementSize;	//bit size..
			Name = elementName;
			Position = bufferPosition;
		}

		public override string ToString() { return String.Format("{0}\t= {1,3}", Name, Value); }

		public virtual void UpdateValueFromBuffer(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public virtual void UpdateBufferFromValue(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsValid()
		{
			//default Element is always valid..
			return true;
		}


	}
}
