using System;

namespace Core.Network
{
	public class TransmissionPriority
	{
		public const int LOW = -1;
		public const int NORMAL = 0;
		public const int HIGH = 1;
		public const int IMMEDIATE = 2;
	}

	public class TransmissionEnvelope
	{
		public ITransmissionClient Target;
		public object Payload;
		public DateTime Submission;
		public DateTime Expiration;	//???
		public int Priority;

		public TransmissionEnvelope()
		{
			Submission = DateTime.Now;
		}

		public TransmissionEnvelope(ITransmissionClient source, object data, int priority = TransmissionPriority.NORMAL)
			: this()
		{
			Target = source;
			Payload = data;
			Priority = priority;
		}
	}

}
