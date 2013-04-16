using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class ProfileAccumulator<TStruct> : IAccumulable<TStruct>
	{
		protected BaseElement<TStruct> WatchedElement { get; set; }
		protected Queue<TStruct> AccumulationQueue { get; set; }
		private int _accumulatorSize;
		protected const int DEFAULT_MAX_ACCUMULATIONS = 300;

		public ProfileAccumulator(BaseElement<TStruct> element) : this(element, DEFAULT_MAX_ACCUMULATIONS) { }
		public ProfileAccumulator(BaseElement<TStruct> element, int size)
		{
			AccumulationQueue = new Queue<TStruct>();
			WatchedElement = element;
			AccumulatorSize = size;
		}

		public virtual void AccumulateNow()
		{
			if (_accumulatorSize > 0)
				AccumulationQueue.Enqueue(WatchedElement.Value);
			while (AccumulationQueue.Count > _accumulatorSize)
				AccumulationQueue.Dequeue();
		}

		public virtual TStruct[] AccumulatedValues
		{
			get
			{
				return AccumulationQueue.ToArray();
			}
		}

		public virtual int AccumulatorSize
		{
			get
			{
				return _accumulatorSize;
			}
			set
			{
				_accumulatorSize = value;
				while (AccumulationQueue.Count > _accumulatorSize)
					AccumulationQueue.Dequeue();
			}
		}
	}
}
