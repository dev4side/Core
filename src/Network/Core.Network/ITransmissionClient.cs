using System;
using System.Net;

namespace Core.Network
{
	public interface ITransmissionClient : IEquatable<ITransmissionClient>
	{
		ICommunicationChannel<IPEndPoint> Channel { get; }
		bool CanReceive { get; }
		bool CanSend { get; }
	}
}