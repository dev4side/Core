using System;

namespace Core.Network
{
	public interface ITransmissionRouter<TClient> where TClient : ITransmissionClient
	{
		void PostMessageToSend<TDto>(TClient target, TDto dataToSend, int priority, bool overwritePrecedingData)
			where TDto : class;

		void AddReceiveHandler<TDto>(Action<TClient, TDto> callback)
			where TDto : class;

		//void SetErrorHandler(Action<TransmissionClient<TSource>, Exception> callback);	//TODO
		//void AddClient	//TODO
		//void RemoveClient	//TODO
	}
}
