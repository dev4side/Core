using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.Network.Serialization;

namespace Core.Network.Discovery
{
	public interface IDiscoverer<TRequestType, TResponseType>
		where TRequestType  : ISerializable
		where TResponseType : ISerializable
	{
		void Discover(TRequestType what);
		event EventHandler<DiscoveredEventArgs<TRequestType, TResponseType>> Discovered;
		event EventHandler<DiscoveredEventArgs<TRequestType, TResponseType>> NotFound;
	}

	public interface IDiscoverer<TResponseType> : IDiscoverer<Serializable<string>, TResponseType>
		where TResponseType : ISerializable
	{
		void Discover(string what);
		event EventHandler<DiscoveredEventArgs<TResponseType>> Discovered;
		event EventHandler<DiscoveredEventArgs<TResponseType>> NotFound;
	}

}
