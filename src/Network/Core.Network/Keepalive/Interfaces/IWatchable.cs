using System;

namespace Core.Network.Keepalive
{
	public delegate void Changed<TChanging>(TChanging obj);

	public interface IWatchable<TChanging>
	{
		event Changed<TChanging> Changed;
	}
}
