using System;
using System.Net;

namespace Core.Network.Discovery
{
	public class DiscoveredEventArgs<TResponseType> : EventArgs
	{
		public string request;
		public TResponseType response;
		public EndPoint client;
		public EndPoint server;

		public DiscoveredEventArgs(string q, TResponseType r, EndPoint c, EndPoint s)
		{
			this.request = q;
			this.response = r;
			this.client = c;
			this.server = s;
		}
	}

	public class DiscoveredEventArgs<TRequestType, TResponseType> : EventArgs
	{
		public TRequestType request;
		public TResponseType response;
		public EndPoint client;
		public EndPoint server;

		public DiscoveredEventArgs(TRequestType q, TResponseType r, EndPoint c, EndPoint s)
		{
			this.request = q;
			this.response = r;
			this.client = c;
			this.server = s;
		}
	}
}