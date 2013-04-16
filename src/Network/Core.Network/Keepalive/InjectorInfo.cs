using System;
using System.Net;

namespace Core.Network.Keepalive
{
	public enum InjectorStates
	{
		Unreachable,
		Inactive,
		Ready,
		Armed,
		Injecting,
		Stopping
	}


	public enum HandShakePhases
	{
		Unknown,
		ClientTalkFirst,
		ServerReplyOnce,
		Communicating
	}


	[Serializable]
	public class NetworkInfo
	{
		public string Name;
		public string Mac;
		public IPAddress Ip;
	}


	[Serializable]
	public class HeartBeat
	{
		public string Name { get; internal set; }
		public DateTime Time { get; internal set; }
		public InjectorStates Status { get; internal set; }
	}


	[Serializable]
	public class InjectorInfo : IWatchable<InjectorInfo>
	{
		//fields..
		private InjectorStates _status = InjectorStates.Unreachable;
		private InjectorStates _lastStatus = InjectorStates.Unreachable;

		//vars..
		public string HostName { get; set; }
		public string DomainName { get; set; }
		public HandShakePhases HandShake { get; set; }
		public NetworkInfo[] Interfaces { get; set; }
		public DateTime LastSeenAlive { get; set; }
		public IPAddress Address { get; set; }

		[field:NonSerialized]
		public event Changed<InjectorInfo> Changed;

		public InjectorStates Status
		{
			get { return _status; }
			set
			{
				if (value != _status)
				{
					_lastStatus = _status;
					_status = value;
					if (Changed != null)
						Changed(this);
				}
			}
		}

		public InjectorStates LastStatus
		{
			get { return _lastStatus; }
		}

		public HeartBeat HeartBeat
		{
			get
			{
				return new HeartBeat() { Name = HostName, Time = DateTime.Now, Status = _status};
			}
			set
			{
				Status = value.Status;
				LastSeenAlive = DateTime.Now;
			}
		}

		public override string ToString()
		{
			string result = "";
			if (!String.IsNullOrEmpty(HostName))
				result += HostName;
			if (!String.IsNullOrEmpty(DomainName))
				result += "." + DomainName;
			if (Address != null)
				result += " (" + Address.ToString() + ")";

			//result += "Status:" + Status.ToString();
			return result;
		}

	}
}
