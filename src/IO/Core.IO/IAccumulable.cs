namespace Core.IO
{
	public interface IAccumulable
	{
		int AccumulatorSize { get; set; }
		void AccumulateNow();
	}

	public interface IAccumulable<out TStruct> : IAccumulable
	{
		TStruct[] AccumulatedValues { get; }
	}
}
