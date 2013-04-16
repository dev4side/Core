using System;
using System.Collections.Generic;
using System.Linq;
using Core.Kernel;
using Core.Log;
using System.Collections;
using System.Threading;
using System.Net;
using Ninject;

namespace Core.Network
{
	//TODO: levare IControllableService e tutte le dipendenze da Athena.. mettere in Core.Network
	public class NetworkServerTcpRouter<TClient> : NetworkServerTcp, ITransmissionRouter<TClient> where TClient : ITransmissionClient
	{
		[Inject] public ILog<NetworkServerTcpRouter<TClient>> Log { get; set; }
		[Inject] public virtual ITransmissionClientFactory<TClient> ClientFactory { get; set; }

		public List<TClient> Clients { get; protected set; }

		internal List<TransmissionEnvelope> _transmissionQueue = new List<TransmissionEnvelope>();
		internal Dictionary<Type, Action<TClient, object>> _receiveCallbacks = new Dictionary<Type, Action<TClient, object>>();
		internal Action<TClient, Exception> _errorCallback;

		private int _transmissionTimerGrain = 50;
		private Timer _transmissionTimer = null;

		public NetworkServerTcpRouter(int serverPort)
			: base(serverPort)
		{
			ObjectFactory.ResolveDependencies(this);
			Clients = new List<TClient>();
			_transmissionTimer = new Timer(HandleTransmissionTimer);
		}

		public void Start()
		{
			StartTransmissionTimer();
			this.Active = true;
		}

		public void Stop()
		{
			StopTransmissionTimer();
			this.Active = false;
		}

		#region ITransmissionRouter members

		public void AddReceiveHandler<TDto>(Action<TClient, TDto> callback)
			where TDto : class
		{
			//class mechanism guarantee valid casting..
			_receiveCallbacks[typeof (TDto)] = (client, data) => callback(client, (TDto) data);
		}

		public void PostMessageToSend<TDto>(TClient target, TDto dataToSend, int priority = TransmissionPriority.NORMAL, bool overwritePrecedingData = false)
			where TDto : class
		{
			//prerequisites..
			if (target == null) throw new ArgumentNullException("target");
			if (dataToSend == null) throw new ArgumentNullException("dataToSend");

			var envelope = new TransmissionEnvelope(target,dataToSend,priority);

			//check if preceding messages like this must be forgotten..
			if (overwritePrecedingData)
				lock (((ICollection)_transmissionQueue).SyncRoot)
					_transmissionQueue.RemoveAll(e => (e.Payload.GetType() == envelope.Payload.GetType()) && e.Target.Equals(envelope.Target));

			//check if message must be sent immediately..	
			if (envelope.Priority == TransmissionPriority.IMMEDIATE)
			{
				var channel = envelope.Target.Channel;
				if (channel != null)	//if channel is null, probably client is not connected yet. so we leave the message in the queue..
				{
					channel.Send(envelope.Payload);
					return;
				}
			}

			//update values..
			envelope.Submission = DateTime.Now;
			//envelope.Expiration = DateTime.Now + TimeSpan.FromMinutes(30);

			//enqueue message for later (ASAP) transmission..
			lock (((ICollection)_transmissionQueue).SyncRoot)
				_transmissionQueue.Add(envelope);
		}

		#endregion

		#region Transmission timer handling

		private void HandleTransmissionTimer(object data)
		{
			var sentMessages = new List<TransmissionEnvelope>();
			var thrownExceptions = new List<Exception>();

			lock (((ICollection)_transmissionQueue).SyncRoot)
			{
				foreach (var priorityGroup in _transmissionQueue.GroupBy(e => e.Priority).OrderByDescending(g => g.Key))
				{
					foreach (var envelope in priorityGroup.OrderBy(e => e.Submission))
					{
						try
						{
							if (envelope.Target.CanReceive)
							{
								SendData(envelope.Target, envelope.Payload);
								sentMessages.Add(envelope);
							}
						}
						catch (Exception ex)
						{
							thrownExceptions.Add(ex);
						}
					}
				}

				foreach (var envelope in sentMessages)
					_transmissionQueue.Remove(envelope);
			}

			foreach (var ex in thrownExceptions)
				Log.Error(ex, "TODO");//TODO: logica per distinzione delle eccezioni..
		}

		private void StartTransmissionTimer()
		{
			_transmissionTimer.Change(_transmissionTimerGrain, _transmissionTimerGrain);
		}

		private void StopTransmissionTimer()
		{
			_transmissionTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		#endregion

		#region NetworkServerTcp Event Handlers

		protected override void OnConnect(CommunicationChannelTcp channel)
		{
			Log.Debug("Connection established with {0} at port {1}",
				((channel.Remote != null) && (channel.Remote.Address != null)) ? channel.Remote.Address.ToString() : "[missing remote ip address]",
				channel.Local.Port.ToString());

			//create new client..
			Clients.Add(ClientFactory.GetClient(channel));

		}

		protected override void OnError(CommunicationChannelTcp source, Exception exception)
		{
			Log.Error(exception, "Error in communication with {0} at port {1}",
				((source.Remote != null) && (source.Remote.Address != null)) ? source.Remote.Address.ToString() : "[missing remote ip address]",
				source.Local.Port.ToString());

			//inject the Exception in the receiving pipe to be handled, shall be handled by a dedicated AddErrorHandler<Exception>..
			Log.Error(exception, "An error occurred in transmission pipeline (receive,deserialize,handle,serialize,send).");
		}

		protected override void OnReceive(CommunicationChannelTcp channel, object receivedObject)
		{
			try
			{
				if (receivedObject == null)
					return;

				//search it in cache..
				TClient client = GetClientByChannel(channel);
				if (client != null)
				{
					if (_receiveCallbacks.ContainsKey(receivedObject.GetType()))
						_receiveCallbacks[receivedObject.GetType()](client, receivedObject);
					else
						Log.Error("Received unknown DTO type: {0}. Do not have a function to handle it, will be discarded",
						          receivedObject.GetType().Name);
				}

			}
			catch (Exception ex)
			{
				Log.Error(ex, "An error occurred handling received object: {0}.", receivedObject != null ? receivedObject.ToString() : "null");
			}

		}

		#endregion

		#region Working functions

		protected TClient GetClientByChannel(ICommunicationChannel<IPEndPoint> channel)
		{
			if (channel == null) throw new ArgumentNullException("channel");

			lock (((ICollection)Clients).SyncRoot)
				return Clients.FirstOrDefault(c => c.Channel == channel);
		}

		protected void SendData(ITransmissionClient toClient, object dataToSend)
		{
			var channel = toClient.Channel;
			if (channel != null)
				channel.Send(dataToSend);
		}

		#endregion

	}
}
