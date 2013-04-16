using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace Core.Network
{
	public class CommunicationChannelUdp : ICommunicationChannel<EndPoint>
	{
		//const..
		private const int BUFSIZE = 8192;	//8Kb
		private readonly byte[] _buf;
		private int _grain = 50;

		//vars..
		private EndPoint _loc = null;
		private EndPoint _rem = null;

		//jobbyjobbers..
		private readonly BackgroundWorker _mainThread = null;
		private readonly MemoryStream _stream = null;
		private Socket _mainSocket = null;

		//transmission queue..
		private readonly Queue<KeyValuePair<object, EndPoint>> _inputQueue  = null;
		private readonly Queue<KeyValuePair<object, EndPoint>> _outputQueue = null;

		public IFormatter Formatter { get; set; }

		public EndPoint Remote { get { return _rem; } }


		public CommunicationChannelUdp()
		{
			_mainThread = new BackgroundWorker();
			_mainThread.WorkerSupportsCancellation = true;
			_mainThread.WorkerReportsProgress = false;
			_mainThread.DoWork += new DoWorkEventHandler(DoWork);
			_mainThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

			_buf = new byte[BUFSIZE];
			_stream = new MemoryStream(_buf, 0, BUFSIZE, true, true);

			_inputQueue = new Queue<KeyValuePair<object, EndPoint>>();
			_outputQueue = new Queue<KeyValuePair<object, EndPoint>>();

			Formatter = new BinaryFormatter();

		}

		~CommunicationChannelUdp()
		{
			_mainThread.CancelAsync();
		}

		//
		//	INTERFACE IMPLEMENTATION
		//


		public void Open(EndPoint connectionArgument)
		{
			try
			{
				_loc = connectionArgument;
				_rem = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

				_mainSocket = new Socket(_loc.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				_mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
				_mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
				_mainSocket.Blocking = false;
				_mainSocket.Bind(_loc);

				this.StartWork();
			}
			catch (SocketException)
			{
				EventLog.WriteEntry(this.GetType().FullName, "Endpoint " + _loc.ToString() + " invalid o not available, can't listen..", EventLogEntryType.Error);
			}
			catch (ObjectDisposedException)
			{
				EventLog.WriteEntry(this.GetType().FullName, "Socket closed, can't listen..", EventLogEntryType.Error);
			}
		}

		public void Close()
		{
			this.StopWork();
		}


		public void Send(object objectToSend)
		{
			this.Send(objectToSend, _rem);
		}

		public void Send(object objectToSend, EndPoint destination)
		{
			lock (((ICollection)_outputQueue).SyncRoot)
			{
				_outputQueue.Enqueue(new KeyValuePair<object, EndPoint>(objectToSend, destination));
			}
		}

		public event EventHandler<ReceivedEventArgs<EndPoint>> Received;
		public event EventHandler<ConnectedEventArgs<EndPoint>> Connected;
		public event EventHandler<ErrorEventArgs> Error;


		//
		//	WORKING FUNCTIONS
		//

		private void StartWork()
		{
			if (_mainSocket.IsBound && (!_mainThread.IsBusy))
			{
				_mainThread.RunWorkerAsync();
				EventLog.WriteEntry("DiscoveryServer", "Server listening at " + _loc.ToString(), EventLogEntryType.Information);
			}
		}

		private void StopWork()
		{
			_mainThread.CancelAsync();
			//todo spostare tutti i messaggi nelle classi finali..
			EventLog.WriteEntry("DiscoveryServer", "Server stopped at " + _loc.ToString(), EventLogEntryType.Information);
		}

		private void DoWork(object sender, DoWorkEventArgs e)
		{

			while (!_mainThread.CancellationPending)
			{

				int readBytes = 0;

				while (_mainSocket.Available > 0)
				{
					try
					{
						readBytes = _mainSocket.ReceiveFrom(_buf, ref _rem);
						if (readBytes <= BUFSIZE)
						{
							lock (((ICollection)_inputQueue).SyncRoot)
							{
								_stream.Position = 0;
								_inputQueue.Enqueue(new KeyValuePair<object, EndPoint>(Formatter.Deserialize(_stream), _rem));
							}
						}
						else ;
						//TODO: ricezione di oggetti su piu datagrammi..
					}
					catch (SocketException) { /* TODO: errorCount + rebind */ }
					catch (ObjectDisposedException) { break; }
				}

				lock (((ICollection)_inputQueue).SyncRoot)
				{
					while (_inputQueue.Count > 0)
						if (Received != null)
						{
							KeyValuePair<object, EndPoint> kv = _inputQueue.Dequeue();
							Received(this,new ReceivedEventArgs<EndPoint>(kv.Key,kv.Value));
						}
				}

				lock (((ICollection)_outputQueue).SyncRoot)
				{
					while (_outputQueue.Count > 0)
					{

						KeyValuePair<object, EndPoint> kv = _outputQueue.Dequeue();
						_stream.Position = 0;
						Formatter.Serialize(_stream, kv.Key);
						//TODO: rivedere MemoryStream, da fare resizeable.. + GetBuffer
						_mainSocket.SendTo(_buf, (int)_stream.Position + 1, SocketFlags.None, kv.Value);
					}

				}

				Thread.Sleep(_grain);

			}

			e.Cancel = true;
		}

		private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//release..
			//mainSocket.Disconnect(true);
			_mainSocket.Close();
			_mainSocket = null;
		}

		public int Grain
		{
			get { return _grain; }
			set { _grain = value < 20 ? 20 : value; } //clamp freq to 50Hz..
		}

	}

}
