using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ElementGroup : IElement, IValidable//, IEnumerable
	{
		public string Name { get; set; }
		public int Position { get; set; }

		protected readonly List<IElement> elements;
		protected readonly List<IValidable> validables;
		protected readonly List<IAccumulable> accumulables;

		public ElementGroup(string groupName, int bufferStartPosition)
		{
			Name = groupName;
			Position = bufferStartPosition;
			elements = new List<IElement>();
			validables = new List<IValidable>();
			accumulables = new List<IAccumulable>();
		}

		public virtual int Size
		{
			get
			{
				return elements.Sum(element => element.Size);
			}
		}

		public virtual void UpdateValueFromBuffer(byte[] buffer)
		{
			elements.ForEach(e =>
			                 	{
			                 		e.UpdateValueFromBuffer(buffer);
			                 	});
			accumulables.ForEach(a => a.AccumulateNow());
		}

		public virtual void UpdateBufferFromValue(byte[] buffer)
		{
			elements.ForEach(e =>
			                 	{
			                 		e.UpdateBufferFromValue(buffer);
			                 	});
		}

		public virtual bool IsValid()
		{
			return validables.All(v => v.IsValid());
		}

		public override string ToString()
		{
			var formatter = new StringBuilder();
			formatter.AppendLine();
			formatter.AppendLine(Name + ":");
			foreach (var elem in elements)
				formatter.AppendLine(elem.ToString());
			return formatter.ToString();
		}

		public virtual List<IValidable> Validables
		{
			get { return validables; }
		}

		public virtual List<IElement> Elements
		{
			get { return elements; }
		}

		public virtual List<IAccumulable> Accumulables
		{
			get { return accumulables; }
		}
	}
}
