using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Core.Network
{
	public class NetworkClientTcp
	{
		private const int DEFAULTTIMEOUT = 5000;

		//vars..
		private int _timeout = 0;
		private IPEndPoint _rem = null;
		private IPEndPoint _loc = null;
		private bool _connected = false;

		protected ICommunicationChannel<IPEndPoint> _communication = null;
		private Object _syncLock = new object();
		private Timer _poller = null;

		public NetworkClientTcp(IPAddress address, int port)
		{
			_rem = new IPEndPoint(address, port);

			_communication = new CommunicationChannelTcp();
			_communication.Received += HandleReceivedObject;
			_communication.Connected += HandleConnectedClient;
			_communication.Error += HandleErrorOccurred;

			_timeout = DEFAULTTIMEOUT;
			Start();
		}

		~NetworkClientTcp()
		{
			if(_communication != null)
				_communication.Close();
			if(_poller != null)
				_poller.Dispose();
			_poller = null;
		}


		//
		// PUBLIC INTERFACE
		//


		public int TimeOut
		{
			get { return _timeout; }
			set { _timeout = (value > 10000) ? 10000 : value; }
		}

		public void Start()
		{
			_poller = new Timer(HandleConnectionPolling, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_timeout));
		}

		public void Stop()
		{
			if(_poller != null)
				_poller.Dispose();
			if(_communication != null)
				_communication.Close();
			_connected = false;
			_poller = null;
		}

		public bool Connected
		{
			get { return _connected; }
		}

		protected void Send(object objectToSend)
		{
			_communication.Send(objectToSend, _rem);
		}

		protected virtual void OnConnect() { }//commChan
		protected virtual void OnError() { }//sockexc
		protected virtual void OnReceive(object receivedObject) { }


		//
		// WORKING FUNCTIONS
		//


		private void HandleConnectedClient(object sender, ConnectedEventArgs<IPEndPoint> args)
		{
			lock (_syncLock)
			{
				if (!_connected)
				{
					//stop connection timer..
					_poller.Dispose();
					_poller = null;

					//_loc = _communication.Local; //todo
					_connected = true;

					OnConnect();
				}
			}
			//debug..
			Console.WriteLine("KeepAliveClient: connected to: " + args.remote.ToString());
		}

		private void HandleConnectionPolling(object state)
		{
			lock (_syncLock)
			{
				if (!_connected)
				{
					//try connection..
					_communication.Open(_rem);
				}
			}
		}

		private void HandleReceivedObject(object sender, ReceivedEventArgs<IPEndPoint> args)
		{
			//debug..
			//Console.WriteLine("KeepAliveClient: Received " + args.objectReceived.ToString());

			OnReceive(args.objectReceived);
		}

		private void HandleErrorOccurred(object sender, ErrorEventArgs args)
		{
			var socketException = args.GetException() as SocketException;
			if (socketException != null)
			{
				switch ((SocketError)socketException.ErrorCode)
				{
					case SocketError.TimedOut:
					case SocketError.TryAgain:
					case SocketError.ConnectionAborted:
					case SocketError.ConnectionRefused:
					case SocketError.ConnectionReset:
					case SocketError.HostDown:
					case SocketError.HostNotFound:
					case SocketError.HostUnreachable:
					case SocketError.NetworkDown:
					case SocketError.NetworkReset:
					case SocketError.NetworkUnreachable:
						lock (_syncLock)
						{
							if (_connected)
							{
								OnError();

								//close broken channel..
								_communication.Close();
								_connected = false;

								//start connection polling..
								_poller = new Timer(HandleConnectionPolling, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(TimeOut > 0 ? TimeOut : DEFAULTTIMEOUT));
							}
						}
						break;
				}
			}
		}

	}
}
