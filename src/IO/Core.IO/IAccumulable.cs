using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public interface IAccumulable
	{
		int AccumulatorSize { get; set; }
		void AccumulateNow();
	}

	public interface IAccumulable<TStruct> : IAccumulable
	{
		TStruct[] AccumulatedValues { get; }
	}

}
