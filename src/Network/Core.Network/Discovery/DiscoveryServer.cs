using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using Core.Network.Serialization;


namespace Core.Network.Discovery
{
	public class DiscoveryServer<TResponseType> : DiscoveryServer<Serializable<string>, TResponseType>, IDiscoverable<TResponseType>
		where TResponseType : class, ISerializable
	{
		public DiscoveryServer()
			: base()
		{
			base.Discovered += wrapDiscoveredHandler;
		}

		public DiscoveryServer(int port)
			: base(port)
		{
			base.Discovered += wrapDiscoveredHandler;
		}

		public void Publish(string key, TResponseType val)
		{
			if(string.IsNullOrEmpty(key))
				throw new ArgumentException("key", "Trying to publish anonymous object!");
			base.Publish(key.GetSerializable<string>(), val);
		}

		public void Unpublish(string key)
		{
			base.Unpublish(key.GetSerializable<string>());
		}

		public new event EventHandler<DiscoveredEventArgs<TResponseType>> Discovered;

		private void wrapDiscoveredHandler(object sender, DiscoveredEventArgs<Serializable<string>,TResponseType> args)
		{
			if (Discovered != null)
			{
				Discovered(this, new DiscoveredEventArgs<TResponseType>(args.request.Value, args.response, args.client, args.server));
			}
		}

	}


	public class DiscoveryServer<TRequestType, TResponseType> : IDiscoverable<TRequestType, TResponseType>
		where TRequestType  : class, ISerializable
		where TResponseType : class, ISerializable
	{
		//const..
		public static int DEFAULTPORT = 40040;
		public static int DEFAULTTIMEOUT = 2000;

		//vars..
		private EndPoint _loc = null;

		//data..
		private readonly Dictionary<TRequestType, TResponseType> _publishedObjects = null;

		//jobbyjobbers..
		private ICommunicationChannel<EndPoint> _communication = null;

		//internal class 

		//
		//   CONSTRUCTORS
		//

		public DiscoveryServer()
			: this(DEFAULTPORT)
		{ }

		public DiscoveryServer(int port)
		{
			_loc = new IPEndPoint(IPAddress.Any, port);
			_communication = new CommunicationChannelUdp();
			_publishedObjects = new Dictionary<TRequestType, TResponseType>();

			_communication.Received += HandleReceivedObject;
		}

		//
		//   SINGLETON
		//

		//TODO: move to singleton

		//private static DiscoveryServer<request_type,response_type> me = null;

		//public static DiscoveryServer<request_type, response_type> GetSingleton()
		//{
		//    return me = (me ?? new DiscoveryServer<request_type, response_type>());
		//}

		//
		//    INTERFACE IMPLEMENTATION
		// 

		public void Publish(TRequestType key, TResponseType val)
		{
			//arg check..
			if (key == null) throw new ArgumentException("key", "Trying to publish anonymous object!");
			if (val == null) throw new ArgumentException("val", "Trying to publish a null object!");

			lock (((ICollection)_publishedObjects).SyncRoot)
			{
				if (_publishedObjects.Count == 0)
					_communication.Open(_loc);
				_publishedObjects.Add(key, val);
			}
		}

		public void Unpublish(TRequestType key)
		{
			lock (((ICollection)_publishedObjects).SyncRoot)
			{
				_publishedObjects.Remove(key);
				if (_publishedObjects.Count == 0)
					_communication.Close();
			}
		}

		public event EventHandler<DiscoveredEventArgs<TRequestType,TResponseType>> Discovered;


		//
		//   WORKING FUNCTIONS
		//

		private void HandleReceivedObject(object sender, ReceivedEventArgs<EndPoint> args)
		{
			TRequestType  request  = null;
			TResponseType response = null;

			if (args.objectReceived is TRequestType)
			{
				request = args.objectReceived as TRequestType;
				lock (((ICollection)_publishedObjects).SyncRoot)
				{
					//if (_publishedObjects.ContainsKey(request))
					try
					{
						response = _publishedObjects[request];
						_communication.Send(response, args.from);
					}
					catch(KeyNotFoundException)
					{
						; //TODO: signal notfound
					}
				}
				//deadlock safe..
				if( (Discovered != null) && (request != null) && (response != null) )
					this.Discovered(this, new DiscoveredEventArgs<TRequestType, TResponseType>(request, response, args.from, _loc));
				//EventLog.WriteEntry("Empower.CMS.DiscoveryServer", "Discover request for \"" + name + "\" from " + ((IPEndPoint)rem) + "published to " + uri, EventLogEntryType.Information);
			}
			else
			{
				; //TODO: signal bad object request
			}
		}

	}
}
