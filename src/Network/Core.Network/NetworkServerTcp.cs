using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timer = System.Threading.Timer;
using System.Net.Sockets;
using System.Net;
using Ninject;
using System.IO;
using Core.Log;
using Core.Kernel;

namespace Core.Network
{
	public class NetworkServerTcp
	{

		//vars..
		private IPEndPoint _rem = null;
		private IPEndPoint _loc = null;
		private bool _connected = false;

		//jobbyjobbers..
		protected TcpListener _listener = null;
		public IFormatter Formatter { get; set; }
		private IAsyncResult _currentAsyncResult = null;
		protected List<ICommunicationChannel<IPEndPoint>> _channels = null;

		[Inject]
		public ILog<NetworkServerTcp> Log { get; set; }

		public NetworkServerTcp(int port)
		{
			ObjectFactory.ResolveDependencies(this);
			_loc = new IPEndPoint(IPAddress.Any, port);
			_channels = new List<ICommunicationChannel<IPEndPoint>>();
			_listener = new TcpListener(_loc.Address, _loc.Port);
		}

		~NetworkServerTcp()
		{
			foreach (var channel in _channels)
				channel.Close();
			_listener.Stop();
		}

		public bool Active
		{
			get { return _connected; }
			set
			{
				if (value != _connected)
				{
					if (value)
					{
						_connected = true;
						_listener.Start();
						_currentAsyncResult = _listener.BeginAcceptTcpClient(HandleAcceptTcpCLient, _listener);
					}
					else
					{
						_connected = false;
						_listener.Stop();
					}
				}
			}
		}

		protected virtual void OnConnect(CommunicationChannelTcp source) { }//commChan
		protected virtual void OnError(CommunicationChannelTcp source, Exception exception) { }//sockexc
		protected virtual void OnReceive(CommunicationChannelTcp source, object receivedObject) { }

		private void HandleAcceptTcpCLient(IAsyncResult ar)
		{
			TcpClient tmpClient = null;
			CommunicationChannelTcp tmpChannel = null;

			try
			{
				if (_connected)
				{
					tmpClient = _listener.EndAcceptTcpClient(ar);
					tmpChannel = new CommunicationChannelTcp();

					if (this.Formatter != null)
						tmpChannel.Formatter = this.Formatter;

					tmpChannel.Connected += HandleConnectedClient;
					tmpChannel.Received += HandleReceivedObject;
					tmpChannel.Error += HandleErrorOccurred;

					tmpChannel.Open(tmpClient);

					lock (((ICollection) _channels).SyncRoot)
					{
						_channels.Add(tmpChannel);
					}
				}
			}
			catch (SocketException)
			{
				if (tmpClient != null) tmpClient.Close();
				if (tmpChannel != null) tmpChannel.Close();
			}
			catch (ObjectDisposedException)
			{
				if (tmpClient != null) tmpClient.Close();
				if (tmpChannel != null) tmpChannel.Close();
			}
			finally
			{
				if (_connected)
					_currentAsyncResult = _listener.BeginAcceptTcpClient(HandleAcceptTcpCLient, _listener);
			}
		}

		private void HandleConnectedClient(object sender, ConnectedEventArgs<IPEndPoint> args)
		{
			//TODO log..
			//Log.Debug("{1}: Network connection with {0} has been established", args.remote.ToString(), this.GetType().ToString());
			OnConnect(sender as CommunicationChannelTcp);
		}

		private void HandleReceivedObject(object sender, ReceivedEventArgs<IPEndPoint> args)
		{
			//TODO log..
			OnReceive(sender as CommunicationChannelTcp, args.objectReceived);
		}

		private void HandleErrorOccurred(object sender, ErrorEventArgs args)
		{

			//Log.Error(args.GetException());

			var socketException = args.GetException() as SocketException;
			var channel = sender as CommunicationChannelTcp;

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

						OnError(channel, socketException);

						channel.Close();
						lock (((ICollection)_channels).SyncRoot)
						{
							_channels.Remove(channel);
							Log.Debug("{1}: Network connection with {0} has broken", channel.Remote.Address.ToString());
						}
						break;
				}
			}
			else
			{
				//TODO Log..
				OnError(channel, args.GetException());
			}
		}

	}
}
