using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Core.Kernel;
using Core.Log;
using Ninject;


namespace Core.Network.Keepalive
{
	public class KeepAliveClient
	{
        [Inject]
	    public ILog<KeepAliveClient> Log { get; set; }

		//const..
		public static int DEFAULTPORT = 40060;
		public static int DEFAULTTIMEOUT = 5000;
		public static int HEARTBEATTIME = 5000;

		//vars..
		private IPEndPoint _rem = null;
		private IPEndPoint _loc = null;
		private bool _connected = false;
		private int _timeout = 0;

		//jobbyjobbers..
		private ICommunicationChannel<IPEndPoint> _communication = null;
		private InjectorInfo _injector = null;
		private Object _syncLock = new object();
		private Timer _poller = null;

        //logging
        private bool _firstHeartBeatLogged = false;

		public int TimeOut
		{
			get { return _timeout;  }
			set { _timeout = (value > 10000) ? 10000 : value; }
		}

		public InjectorInfo InjectorInfo
		{
			get { return _injector; }
		}

		public bool Connected
		{
			get { return _connected; }
		}

		//
		//	CONSTRUCTORS
		//

		public KeepAliveClient(IPAddress address, int port)
		{
            ObjectFactory.ResolveDependencies(this);

			_rem = new IPEndPoint(address, port);

			_injector = BuildInjectorInfo();

			_communication = new CommunicationChannelTcp();
			_communication.Received += HandleReceivedObject;
			_communication.Connected += HandleConnectedClient;
			_communication.Error += HandleErrorOccurred;

			_poller = new Timer(HandleConnectionPolling, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(TimeOut > 0 ? TimeOut : DEFAULTTIMEOUT));

		}

		~KeepAliveClient()
		{
			_communication.Close();
			_poller.Dispose();
		}


		//
		// PUBLIC INTERFACE
		//


		public void Stop()
		{
			_poller.Dispose();
			_communication.Close();
			_connected = false;
		}


		//
		// WORKING FUNCTIONS
		//

		private InjectorInfo BuildInjectorInfo()
		{
			var iInfo = new InjectorInfo();
			var nInfos = new List<NetworkInfo>();
			foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (nic.GetIPProperties() != null)
				{
					var n_info = new NetworkInfo();

					n_info.Ip = nic.GetIPProperties().UnicastAddresses.Count > 0
									? nic.GetIPProperties().UnicastAddresses[0].Address
									: IPAddress.None;
					n_info.Mac = nic.GetPhysicalAddress().ToString();
					n_info.Name = nic.Name;

					nInfos.Add(n_info);
				}
			}
			iInfo.Interfaces = nInfos.ToArray();

			iInfo.HostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
			iInfo.DomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;

			return iInfo;
		}

		private void HandleReceivedObject(object sender, ReceivedEventArgs<IPEndPoint> args)
		{
			//debug..
            // Log.Debug();
			//Console.WriteLine("KeepAliveClient: Received " + args.objectReceived.ToString());

			if(args.objectReceived is InjectorInfo)
			{
				var i_info = args.objectReceived as InjectorInfo;

				if ((i_info.HostName == _injector.HostName) && (i_info.HandShake == HandShakePhases.ServerReplyOnce))
				{
					_injector.Address = i_info.Address;
					_injector.Status = InjectorStates.Ready;
					_injector.HandShake = HandShakePhases.Communicating;

					//start heartbeating..
					_poller = new Timer(HandleHeartBeatPolling, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(TimeOut > 0 ? TimeOut : HEARTBEATTIME));
				}
			}
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
                                Log.Error(socketException, "an error occured during remote communication. [Network is unreachable]");
								//stop heartbeating timer..
								_poller.Dispose();

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
			else
			{
			    var candidateException = args.GetException();
                if (candidateException == null)
			        Log.Error("Unknows exception has occured. Cannot retrieve exception information.");
                else Log.Error(candidateException, "Unknows exception has occured");
			}
		}


		private void HandleConnectedClient(object sender, ConnectedEventArgs<IPEndPoint> args)
		{
			lock (_syncLock)
			{
				if (!_connected)
				{
					//stop connection timer..
					_poller.Dispose();

					//first attempt to handshake..
					_injector.HandShake = HandShakePhases.ClientTalkFirst;
					_communication.Send(_injector, _rem);

					//start handshake countdown..
					_poller = new Timer(HandleHandShakeTimeout, null, TimeSpan.FromMilliseconds(TimeOut > 0 ? TimeOut : DEFAULTTIMEOUT),TimeSpan.Zero);

					//_loc = _communication.Local; //todo
					_connected = true;
                    Log.Debug("Handshake started with: {0}", args.remote.ToString());
				}
				else
				{
                    Log.Warn("A connection has been called with an already connected client. Connected to: {0}", args.remote.ToString());
				}
			}
            
		}

		private void HandleHandShakeTimeout(object state)
		{
			lock (_syncLock)
			{
				if (_connected)
				{
					if (_injector.HandShake != HandShakePhases.Communicating)
					{
						//stop handshake timer..
						_poller.Dispose();

						//close unresponsive channel..
						_communication.Close();
						_connected = false;

						//start connection polling..
						_poller = new Timer(HandleConnectionPolling, null, TimeSpan.Zero,TimeSpan.FromMilliseconds(TimeOut > 0 ? TimeOut : DEFAULTTIMEOUT));
					}
				}
			}
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

		private void HandleHeartBeatPolling(object state)
		{
			lock (_syncLock)
			{
				if (_connected)
				{
					//send heartbeat..
					_communication.Send(_injector.HeartBeat, _rem);
                    if(!_firstHeartBeatLogged)
                    {
                        Log.Debug("Firt hartbeat sent. Others will not be logged.");
                        _firstHeartBeatLogged = true;
                    }
				    
				}
			}

		}
	}
}
