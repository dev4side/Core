using System;

namespace Core.IO.Threading
{
	public interface IActivityMonitorable
	{
		Action Activity { get; set; }
	}
}