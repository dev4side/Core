using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timer = System.Threading.Timer;
using System.Net.Sockets;
using System.Net;
using Ninject;
using System.IO;
using Core.Log;
using Core.Kernel;

namespace Core.Network.Keepalive
{
	public class InjectorConnectedEventArgs : EventArgs
	{
		public InjectorInfo info;

		public InjectorConnectedEventArgs(InjectorInfo i)
		{
			info = i;
		}
	}

	public class KeepAliveServer
	{
		//const..
		public static int DEFAULTPORT = 40060;
		public static int DEFAULTTIMEOUT = 2500;
		public static int HEARTBEATTIME = 5000;

		//vars..
		protected IPEndPoint _rem = null;
		protected IPEndPoint _loc = null;
		protected bool _connected = false;

		//jobbyjobbers..
		protected TcpListener _server = null;
		protected IAsyncResult _currentAsyncResult = null;
		protected List<ICommunicationChannel<IPEndPoint>> _clients = null;
		protected List<InjectorInfo> _injectors = null;
		protected Timer _poller = null;

		//events..
		public EventHandler<InjectorConnectedEventArgs> InjectorConnected;
		public EventHandler<InjectorConnectedEventArgs> InjectorDisconnected;


        [Inject]
        public ILog<KeepAliveServer> Log { get; set; }

		public KeepAliveServer(int port)
		{
            ObjectFactory.ResolveDependencies(this);

			_loc = new IPEndPoint(IPAddress.Any, port);
			_clients = new List<ICommunicationChannel<IPEndPoint>>();
			_injectors = new List<InjectorInfo>();

			_server = new TcpListener(_loc.Address,_loc.Port);
			_poller = new Timer(HandlePeriodicInjectorsCheck);
		}

		~KeepAliveServer()
		{
			_poller.Dispose();
			foreach (var client in _clients)
				client.Close();
			foreach (var injector in _injectors)
				injector.Status = InjectorStates.Unreachable;
			_server.Stop();
		}

		//
		//	PUBLIC INTERFACE
		//

		public bool Active
		{
			get { return _connected; }
			set
			{
				if (value != _connected)
				{
					if (value)
					{
						_server.Start();
						_currentAsyncResult = _server.BeginAcceptTcpClient(HandleAcceptTcpCLient, _server);
						_poller.Change(0, DEFAULTTIMEOUT);
						_connected = true;
					}
					else
					{
						_server.Stop();
						_connected = false;
					}
				}
			}
		}

		public void Start()
		{
			this.Active = true;
		}

		public void Stop()
		{
			this.Active = false;
			foreach (var client in _clients)
				client.Close();
		}

		public void AddInjector(InjectorInfo injector)
		{
			if (String.IsNullOrEmpty(injector.HostName))
				return;

			if (this.GetInjectorByHostname(injector.HostName) == null)
			{
				lock (((ICollection)_injectors).SyncRoot)
				{
					injector.Status = InjectorStates.Unreachable;

					//set event handler and add to watch list..
					injector.Changed += HandleInjectorStatusChanged;
					_injectors.Add(injector);
				}
			}
		}

		public void RemoveInjector(InjectorInfo injector)
		{
			if (String.IsNullOrEmpty(injector.HostName))
				return;

			if (this.GetInjectorByHostname(injector.HostName) != null)
			{
				lock (((ICollection)_injectors).SyncRoot)
				{
					//remove event handler and remove from watch list..
					injector.Changed -= HandleInjectorStatusChanged;
					_injectors.Remove(injector);
				}
			}
		}

		public InjectorInfo GetInjectorByAddress(IPAddress address)
		{
			lock (((ICollection)_injectors).SyncRoot)
			{
				return (from i in _injectors where i.Address.ToString() == address.ToString() select i).FirstOrDefault();
			}
		}

		public InjectorInfo GetInjectorByHostname(string hostname)
		{
			lock (((ICollection)_injectors).SyncRoot)
			{
				return (from i in _injectors where i.HostName == hostname select i).FirstOrDefault();
			}
		}

		public InjectorInfo[] GetAllInjectorInfo()
		{
			lock (((ICollection)_injectors).SyncRoot)
			{
				return _injectors.ToArray();
			}
		}

		//
		//	WORKING FUNCTIONS
		//


		private void HandleAcceptTcpCLient(IAsyncResult ar)
		{
			TcpClient tmpClient = null;
			CommunicationChannelTcp tmpChannel = null;
			try
			{
				tmpClient = _server.EndAcceptTcpClient(ar);
				tmpChannel = new CommunicationChannelTcp();
				tmpChannel.Connected += HandleConnectedClient;
				tmpChannel.Received += HandleReceivedObject;
				tmpChannel.Error += HandleErrorOccurred;
				tmpChannel.Open(tmpClient);

				lock (((ICollection) _clients).SyncRoot)
				{
					_clients.Add(tmpChannel);
				}

				
			}
			catch(SocketException)
			{
				if(tmpClient != null) tmpClient.Close();
				if(tmpChannel != null) tmpChannel.Close();
			}
			catch(ObjectDisposedException)
			{
				if (tmpClient != null) tmpClient.Close();
				if (tmpChannel != null) tmpChannel.Close();					
			}
			finally
			{
				if(Active)
					_currentAsyncResult = _server.BeginAcceptTcpClient(HandleAcceptTcpCLient, _server);
			}
		}

		private void HandleConnectedClient(object sender, ConnectedEventArgs<IPEndPoint> args)
		{
			//debug..
			Log.Debug("{1}: Network connection with {0} has been established", args.remote.ToString(), this.GetType().ToString());
			//Console.WriteLine("KeepAliveServer: connection from: " + args.remote.ToString());
		}

		private void HandleReceivedObject(object sender, ReceivedEventArgs<IPEndPoint> args)
		{

			var client = sender as CommunicationChannelTcp;

			if (args.objectReceived is InjectorInfo)
			{
				var i_info = args.objectReceived as InjectorInfo;

				//debug..
				Log.Debug("{2}: Received {0} from {1}. Handshaking.", args.objectReceived.GetType().Name, args.from.Address.ToString(), this.GetType().ToString());
				//Console.WriteLine("KeepAliveServer: Received " + args.objectReceived.ToString());

				//this is a first connection attempt..
				if (i_info.HandShake == HandShakePhases.ClientTalkFirst)
				{
					lock (((ICollection)_injectors).SyncRoot)
					{
						//injector already exists in the watch list?.. //HOSTNAME IS THE PRIMARY KEY..
						var injector = _injectors.Where(i => i.HostName == i_info.HostName).FirstOrDefault();
						if (injector == null)
						{
							i_info.Changed += HandleInjectorStatusChanged;
							_injectors.Add(i_info);
							injector = i_info;
						}
						else
						{
							//update internal network info..
							injector.DomainName = i_info.DomainName;
							injector.Interfaces = i_info.Interfaces;
						}
						//update handshaking phase..
						injector.HandShake = HandShakePhases.ServerReplyOnce;
						injector.LastSeenAlive = DateTime.Now;
						//update external network info..
						injector.Address = args.from.Address;
						//send back updated info..
						client.Send(injector);
					}
				}


			}
				//this is a presence signaling..
			else if (args.objectReceived is HeartBeat)
			{
				var hb = args.objectReceived as HeartBeat;

				lock (((ICollection)_injectors).SyncRoot)
				{
					var injector = _injectors.Where(i => i.HostName == hb.Name).FirstOrDefault();
					if (injector != null)
					{
						//update heartbeat..
						injector.HeartBeat = hb;
						//update handshaking phase if needed..
						if (injector.HandShake == HandShakePhases.ServerReplyOnce)
							injector.HandShake = HandShakePhases.Communicating;
					}
				}


			}
		}

		private void HandleInjectorStatusChanged(InjectorInfo iInfo)
		{
			//EventLog.WriteEntry("KeepAliveServer", "StatusChanged: " + iInfo.LastStatus.ToString() + " - " + iInfo.Status.ToString());
			Log.Debug("{3}: Remote status changed ({2}) {0} -> {1}", iInfo.LastStatus.ToString(), iInfo.Status.ToString(), iInfo.ToString(), this.GetType().ToString());

			if(iInfo != null)
			{
				if(iInfo.Status == InjectorStates.Ready)
				{
					if(iInfo.LastStatus == InjectorStates.Unreachable)
					{
						//APPENA COLLEGATO..
						Log.Info("{1}: Remote connected ({0})", iInfo.ToString(), this.GetType().ToString());
						if(InjectorConnected != null)
							InjectorConnected(this,new InjectorConnectedEventArgs(iInfo));
					}
					//else
					//if(i_info.LastStatus == InjectorStates.Injecting){}
					//else
					//if(i_info.LastStatus == InjectorStates.Armed){}
					//else
					//if(i_info.LastStatus == InjectorStates.Inactive){}
				}
				else if (iInfo.Status == InjectorStates.Unreachable)
				{
					Log.Info("{1}: Remote disconnected ({0})", iInfo.ToString(), this.GetType().ToString());
					if(InjectorDisconnected != null)
						InjectorDisconnected(this,new InjectorConnectedEventArgs(iInfo));

					if (iInfo.LastStatus == InjectorStates.Ready)
					{
					}
					//todo..
				}

			}
		}

		private void HandlePeriodicInjectorsCheck(object data)
		{
			lock (((ICollection)_injectors).SyncRoot)
			{
				var t = DateTime.Now;
				_injectors.ForEach(i_info =>
				                   	{
				                   		if((t - i_info.LastSeenAlive) > TimeSpan.FromMilliseconds(11000))
				                   		{
				                   			i_info.Status = InjectorStates.Unreachable;
				                   		}
				                   	});
			}

		}

		private void HandleErrorOccurred(object sender, ErrorEventArgs args)
		{
      
			Log.Error(args.GetException());
           
			var socketException = args.GetException() as SocketException;
			var client = sender as CommunicationChannelTcp;

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
						client.Close();
						lock(((ICollection)_clients).SyncRoot)
						{
							_clients.Remove(client);
							Log.Debug("{1}: Network connection with {0} has broken", client.Remote.Address.ToString(), this.GetType().ToString());
						}
						break;
				}
			}
		}

	}
}
