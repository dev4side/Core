using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Network
{
	public class ReceivedEventArgs<TArgType> : EventArgs
		where TArgType : class
	{
		public object objectReceived = null;
		public TArgType from = null;

		public ReceivedEventArgs(object o, TArgType f)
		{
			objectReceived = o;
			from = f;
		}
	}

	public class ConnectedEventArgs<TArgType> : EventArgs
		where TArgType : class
	{
		public TArgType remote = null;
		public TArgType local = null;

		public ConnectedEventArgs(TArgType r) : this(r, null)
		{
		}

		public ConnectedEventArgs(TArgType r, TArgType l)
		{
			remote = r;
			local = l;
		}
	}

	public interface ICommunicationChannel<TArgType>
		where TArgType : class
	{
		void Open(TArgType connectionArgument);
		void Close();
		void Send(object objectToSend, TArgType destination);
		void Send(object objectToSend);

		event EventHandler<ReceivedEventArgs<TArgType>> Received;
		event EventHandler<ConnectedEventArgs<TArgType>> Connected;
		event EventHandler<ErrorEventArgs> Error;
		
		IFormatter Formatter { get; set; }
		TArgType Remote { get; }
	}
}
