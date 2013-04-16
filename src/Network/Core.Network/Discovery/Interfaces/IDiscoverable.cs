using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.Network.Serialization;

namespace Core.Network.Discovery
{
	//public delegate void DiscoveredEventHandler<TRequestType, TResponseType>(object sender, DiscoveredEventArgs<TRequestType, TResponseType> args);

	public interface IDiscoverable<TRequestType, TResponseType>
		where TRequestType : ISerializable
		where TResponseType : ISerializable
	{
		void Publish(TRequestType key, TResponseType value);
		void Unpublish(TRequestType key);
		//event DiscoveredEventHandler<TRequestType, TResponseType> Discovered;
		event EventHandler<DiscoveredEventArgs<TRequestType, TResponseType>> Discovered;
	}

	public interface IDiscoverable<TResponseType> : IDiscoverable<Serializable<string>, TResponseType>
		where TResponseType : ISerializable
	{
		void Publish(string key, TResponseType value);
		void Unpublish(string key);
		//event DiscoveredEventHandler<TRequestType, TResponseType> Discovered;
		event EventHandler<DiscoveredEventArgs<TResponseType>> Discovered;
	}

}
