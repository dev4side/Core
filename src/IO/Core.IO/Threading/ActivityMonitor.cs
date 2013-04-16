using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Collections;

namespace Core.IO.Threading
{
	public class ActivityMonitor
	{
		#region Inner item class
		protected class ActivityMonitorableItem
		{
			public IActivityMonitorable Monitored;
			public DateTime LastActivity;
			public TimeSpan InactivityTimeout;
			public bool HasBeenSignaled;
			
			public ActivityMonitorableItem(IActivityMonitorable m, TimeSpan t)
			{
				Monitored = m;
				InactivityTimeout = t;
				LastActivity = DateTime.Now;
				Monitored.Activity = ActivityAction;
			}

			public void ActivityAction()
			{
				HasBeenSignaled = false;
				LastActivity = DateTime.Now;
			}

			public bool IsInactive()
			{
				return (InactivityTimeout > TimeSpan.Zero) && ((DateTime.Now - LastActivity) > InactivityTimeout);
			}
		}
		#endregion

		protected readonly List<ActivityMonitorableItem> _beingMonitored = new List<ActivityMonitorableItem>();

		protected Thread _spinner;
		protected Thread Spinner
		{
			get
			{
				if(_spinner == null)
					_spinner = new Thread(DoWork);
				return _spinner;
			}
		}

		public event Action<IActivityMonitorable> Inactive;

		public void AddMonitorable(IActivityMonitorable monitorable, TimeSpan timeout)
		{
			if (monitorable != null)
			{
				lock (((ICollection)_beingMonitored).SyncRoot)
				{
					var itemAlreadyMonitored = GetItemFromMonitorable(monitorable);
					if (itemAlreadyMonitored != null)
						_beingMonitored.Remove(itemAlreadyMonitored);

					_beingMonitored.Add(new ActivityMonitorableItem(monitorable, timeout));
					if (!Spinner.IsAlive)
						Spinner.Start();
				}
			}
		}

		public void RemoveMonitorable(IActivityMonitorable monitorable)
		{
			if (monitorable != null)
			{
				lock (((ICollection) _beingMonitored).SyncRoot)
				{
					var itemToRemove = GetItemFromMonitorable(monitorable);
					if (itemToRemove != null)
					{
						monitorable.Activity = () => { }; //avoid nullpointer expceptions..
						_beingMonitored.Remove(itemToRemove);
						if (_beingMonitored.Count == 0)
							Spinner.Abort();
					}
				}
			}
		}

		//set to TimeSpan.Zero to disable monitoring..
		public void ChangeMonitorableTimeout(IActivityMonitorable monitorable, TimeSpan timeout)
		{
			lock (((ICollection)_beingMonitored).SyncRoot)
			{
				var itemToChange = GetItemFromMonitorable(monitorable);
				if (itemToChange != null)
				{
					itemToChange.InactivityTimeout = timeout;
				}
			}
		}

		private ActivityMonitorableItem GetItemFromMonitorable(IActivityMonitorable monitorable)
		{
			return _beingMonitored.FirstOrDefault(i => i.Monitored.Equals(monitorable));
		}

		private void DoWork()
		{
			while ((_spinner.ThreadState != ThreadState.StopRequested) && (_spinner.ThreadState != ThreadState.AbortRequested))
			{
				lock (((ICollection)_beingMonitored).SyncRoot)
				{
					foreach (var item in _beingMonitored)
					{
						if ((!item.HasBeenSignaled) && item.IsInactive())
						{
							if (Inactive != null)
								Inactive(item.Monitored);
							item.HasBeenSignaled = true;
						}
					}
				}
				Thread.Sleep(50);
			}
		}
	}
}