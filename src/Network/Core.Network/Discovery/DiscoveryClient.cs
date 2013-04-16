using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using Core.Network.Serialization;

namespace Core.Network.Discovery
{
	public class DiscoveryClient<TResponseType> : DiscoveryClient<Serializable<string>, TResponseType>, IDiscoverer<TResponseType>
		where TResponseType : class, ISerializable
	{
		public DiscoveryClient()
			: base()
		{
			base.Discovered += wrapDiscoveredHandler;
			base.NotFound += wrapNotFoundHandler;
		}

		public DiscoveryClient(int port)
			: base(port)
		{
			base.Discovered += wrapDiscoveredHandler;
			base.NotFound += wrapNotFoundHandler;
		}

		public void Discover(string request)
		{
			if (string.IsNullOrEmpty(request))
				throw new ArgumentException("request", "Trying to search anonymous object!");
			base.Discover(request.GetSerializable<string>());
		}

		public new event EventHandler<DiscoveredEventArgs<TResponseType>> Discovered;
		public new event EventHandler<DiscoveredEventArgs<TResponseType>> NotFound;

		private void wrapDiscoveredHandler(object sender, DiscoveredEventArgs<Serializable<string>, TResponseType> args)
		{
			if (Discovered != null)
			{
				Discovered(sender, new DiscoveredEventArgs<TResponseType>(args.request.Value, args.response, args.client, args.server));
			}
		}

		private void wrapNotFoundHandler(object sender, DiscoveredEventArgs<Serializable<string>, TResponseType> args)
		{
			if (NotFound != null)
			{
				NotFound(sender, new DiscoveredEventArgs<TResponseType>(args.request.Value, args.response, args.client, args.server));
			}
		}

	}


	public class DiscoveryClient<TRequestType, TResponseType> : IDiscoverer<TRequestType, TResponseType> 
		where TRequestType  : class, ISerializable 
		where TResponseType : class, ISerializable
	{
		//const..
		public static int DEFAULTPORT = 40040;
		public static int DEFAULTTIMEOUT = 5000;

		//vars..
		private EndPoint _loc = null;
		private EndPoint _rem = null;

		//data..
		private List<TRequestType> _pendingRequests = null;

		//jobbyjobbers..
		private Timer _poller = null;
		private ICommunicationChannel<EndPoint> _communication = null;

		//
		//	CONSTRUCTORS
		//

		public DiscoveryClient()
			: this(DEFAULTPORT)
		{ }

		public DiscoveryClient(int port)
		{
			_loc = new IPEndPoint(IPAddress.Any, 0);
			_rem = new IPEndPoint(IPAddress.Broadcast, port);
			_pendingRequests = new List<TRequestType>();
			_communication   = new CommunicationChannelUdp();
			_communication.Received += new EventHandler<ReceivedEventArgs<EndPoint>>(HandleReceivedObject);

			_poller = new Timer {AutoReset = false, Interval = DEFAULTTIMEOUT};
			_poller.Elapsed += new ElapsedEventHandler(HandleTimeoutExpired);
		}

		//
		//	INTERFACE IMPLEMENTATION
		//

		public void Discover(TRequestType request)
		{
			if (request == null) throw new ArgumentException("request", "Trying to search anonymous object!");

			lock (((ICollection)_pendingRequests).SyncRoot)
			{
				if (_pendingRequests.Count == 0)
				{
					_communication.Open(_loc);
					_pendingRequests.Add(request);
					_communication.Send(request,_rem);
					_poller.Start();
				}

			}
		}

		public event EventHandler<DiscoveredEventArgs<TRequestType, TResponseType>> Discovered;
		public event EventHandler<DiscoveredEventArgs<TRequestType, TResponseType>> NotFound;

		// 
		//	WORKING FUNCTIONS
		//
		//TODO: TESTARE DEADLOCK IN POLLING MANUALE: delegate NotFount() { Discover(ancora) }
		private void HandleTimeoutExpired(object sender, ElapsedEventArgs args)
		{
			TRequestType request = null;

			lock (((ICollection)_pendingRequests).SyncRoot)
			{
				if (_pendingRequests.Count > 0)
				{
					request = SerializedObjectCloner.Clone<TRequestType>(_pendingRequests[0]);
					_pendingRequests.Remove(_pendingRequests[0]);
					_communication.Close();
					_poller.Stop();
				}
			}
			//deadlock safe..
			if( (NotFound != null) && (request != null) )
				this.NotFound(this, new DiscoveredEventArgs<TRequestType, TResponseType>(request, null, _loc, null));
		}

		void HandleReceivedObject(object sender, ReceivedEventArgs<EndPoint> args)
		{
			TRequestType  request  = null;
			TResponseType response = null;

			lock (((ICollection)_pendingRequests).SyncRoot)
			{
				if(args.objectReceived is TResponseType)
				{
					if (_pendingRequests.Count > 0)
					{
						response = args.objectReceived as TResponseType;
						request = SerializedObjectCloner.Clone<TRequestType>(_pendingRequests[0]);
						_pendingRequests.Remove(_pendingRequests[0]);
						_communication.Close();
						_poller.Stop();
					}
				}
			}
			//deadlock safe..
			if ((Discovered != null) && (response != null))
				this.Discovered(this, new DiscoveredEventArgs<TRequestType, TResponseType>(request, response, _loc, args.from));
		}

		public double Timeout
		{
			get	{ return  _poller.Interval;	 }
			set { _poller.Interval = value;  }
		}


	}
}
