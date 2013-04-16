using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ElementSequence : ElementGroup
	{
		public ElementSequence(string groupName, int bufferStartPosition) 
			: base(groupName, bufferStartPosition) { }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			int ptr = Position;
			foreach(var element in elements)
			{
				element.Position = ptr;
				element.UpdateValueFromBuffer(buffer);
				ptr += element.Size;
			}
			accumulables.ForEach(a => a.AccumulateNow());
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			int ptr = Position;
			foreach (var element in elements)
			{
				element.Position = ptr;
				element.UpdateBufferFromValue(buffer);
				ptr += element.Size;
			}
		}

	}
}
