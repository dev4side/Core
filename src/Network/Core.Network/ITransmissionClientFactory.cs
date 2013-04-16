using System;

namespace Core.Network
{
	public interface ITransmissionClientFactory<out TClient> where TClient : ITransmissionClient
	{
		TClient GetClient(CommunicationChannelTcp channel);
	}
}
