using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
//using Core.Network.Discovery;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Threading;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace Core.Network
{
	public class CommunicationChannelTcp : ICommunicationChannel<IPEndPoint>
	{
		//const..
		private readonly int MAX_FRAME_LENGTH = 4096;


		//vars..
		private IPEndPoint _rem = null;
		private IPEndPoint _loc = null;
		private int _grain = 50;
		private int _threshold = 0;
		private int _errorCount = 0;
		private bool _syncPointReached = false;
		private IAsyncResult _currentAsyncResult = null;
		private byte[] _frameBytes = null;
		private int _frameBytesCount = 0;
		private DateTime _frameLastTime;
		

		//jobbyjobbers..
		private Timer _timer = null;
		private TcpClient _client = null;
		private NetworkStream _stream = null;
		private Object _syncLock = new object();
		private readonly BackgroundWorker _mainThread = null;

		//transmission queue..
		private readonly Queue<object> _inputQueue = null;
		private readonly Queue<object> _outputQueue = null;

		public IFormatter Formatter { get; set; }

		public double ConnectionTimeOut { get; set; }	//millisec..
		public double ReceivingTimeOut { get; set; }	//millisec..

		public IPEndPoint Remote { get { return _rem; } }
		public IPEndPoint Local  { get { return _loc; } }


		public int Grain
		{
			get { return _grain; }
			set { _grain = value < 20 ? 20 : value; } //clamp freq to 50Hz..
		}

		public int Threshold
		{
			get { return _threshold; }
			set { _threshold = value < 0 ? 0 : value; } //must be 0+..
		}

		public CommunicationChannelTcp() : this(new BinaryFormatter()) { }

		public CommunicationChannelTcp(IFormatter formatter)
		{
			_mainThread = new BackgroundWorker();
			_mainThread.WorkerSupportsCancellation = true;
			_mainThread.WorkerReportsProgress = false;
			_mainThread.DoWork += new DoWorkEventHandler(DoWork);
			_mainThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

			_frameBytes = new Byte[MAX_FRAME_LENGTH];

			_inputQueue = new Queue<object>();
			_outputQueue = new Queue<object>();

			Formatter = formatter ?? new BinaryFormatter();
			ConnectionTimeOut = 0;
			ReceivingTimeOut = 500;

		}

		~CommunicationChannelTcp()
		{
			_mainThread.CancelAsync();
		}


		//
		//	INTERFACE IMPLEMENTATION
		//


		public void Open(TcpClient alreadyConnectedClient)
		{
			if (alreadyConnectedClient == null)
				throw new ArgumentNullException("alreadyConnectedClient");

			if ( ! alreadyConnectedClient.Connected)
				throw new ArgumentException("Passed TcpClient is not connected", "alreadyConnectedClient");

			_client = alreadyConnectedClient;

			_rem = _client.Client.RemoteEndPoint as IPEndPoint;
			_loc = _client.Client.LocalEndPoint  as IPEndPoint;

			ResetFrame();

			this.StartWork();

			if (Connected != null)
				Connected(this, new ConnectedEventArgs<IPEndPoint>(_rem,_loc));
		}

		public void Open(IPEndPoint connectionArgument)
		{

			if (connectionArgument == null)
				throw new ArgumentNullException("connectionArgument");

			if ((connectionArgument.Address == IPAddress.None) || (connectionArgument.Address == IPAddress.Any) || (connectionArgument.Address == IPAddress.Broadcast))
				throw new ArgumentException("Promiscuos endpoint passed", "connectionArgument");

			//ensures thread has ended before reopen
			lock (_syncLock)
			{
				while (_mainThread.CancellationPending)
					Thread.Sleep(1);	//yield..

				if (_mainThread.IsBusy)
					return;
			}

			_rem = connectionArgument;

			try
			{

				//todo: in alcuni casi particolari di continua connessione/disconnesione molto frequente.. 
				//todo: vengono ripuliti gli oggetti appena creati perchè RunWorkerCompleted viene chiamato dopo un successivo Open()..
				_timer = new Timer();
				_client = new TcpClient();
				_client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
				_client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 300);
				_client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 300);

				lock(_syncLock)
				{
					_syncPointReached = false;
					_currentAsyncResult = _client.BeginConnect(_rem.Address.ToString(), _rem.Port, HandleConnectEvent, _client);

					if (ConnectionTimeOut != 0)
					{
						_timer.Interval = ConnectionTimeOut;
						_timer.AutoReset = false;
						_timer.Elapsed += HandleTimeoutExpired;
						_timer.Start(); //race condition..
					}
				}
			}
			catch (SocketException ex)
			{
				//EventLog.WriteEntry(this.GetType().FullName, "Endpoint " + _loc.ToString() + " invalid o not available, can't listen..", EventLogEntryType.Error);
				if (_client != null) _client.Close();

				if(Error != null)
					Error(this,new ErrorEventArgs(ex));
			}
			catch (ObjectDisposedException ex)
			{
				//EventLog.WriteEntry(this.GetType().FullName, "Socket closed, can't listen..", EventLogEntryType.Error);
				if (_client != null) _client.Close();

				if (Error != null)
					Error(this, new ErrorEventArgs(ex));
			}
			catch(Exception ex)
			{
				if (Error != null)
					Error(this, new ErrorEventArgs(ex));
			}
		} 

		public void Close()
		{
			ResetFrame();
			this.StopWork();
		}

		public void Send(object objectToSend)
		{
			this.Send(objectToSend, _rem);
		}

		public void Send(object objectToSend, IPEndPoint destination)
		{
			if ((_client == null) || (_client.Client.Connected == false))
				return;
			
			lock (((ICollection)_outputQueue).SyncRoot)
			{
				_outputQueue.Enqueue(objectToSend);
			}
		}

		public event EventHandler<ConnectedEventArgs<IPEndPoint>> Connected;

		public event EventHandler<ConnectedEventArgs<IPEndPoint>> Disconnected;

		public event EventHandler<ReceivedEventArgs<IPEndPoint>> Received;

		public event EventHandler<ErrorEventArgs> Error;


		//
		//	WORKING FUNCTIONS
		//

		private void StartWork()
		{
			if (!_mainThread.IsBusy)
			{
				_mainThread.RunWorkerAsync();
				//todo: logging!!
				//EventLog.WriteEntry("DiscoveryServer", "Server listening at " + _loc.ToString(), EventLogEntryType.Information);
			}
		}

		private void StopWork()
		{
			_mainThread.CancelAsync();
			//todo spostare tutti i messaggi nelle classi finali..
			//EventLog.WriteEntry("DiscoveryServer", "Server stopped at " + _loc.ToString(), EventLogEntryType.Information);
		}

		public void ResetFrame()
		{
			//Array.Clear(_frameBytes, 0, _frameBytesCount);
			_frameBytes = new Byte[MAX_FRAME_LENGTH];
			_frameBytesCount = 0;
		}

		private void DoWork(object sender, DoWorkEventArgs e)
		{
			_stream = _client.GetStream();
			_stream.ReadTimeout = 30;
			_stream.WriteTimeout = 30;

			while (!_mainThread.CancellationPending)
			{
				if (_client.Connected)
				{
					try
					{
						lock (((ICollection)_inputQueue).SyncRoot)
						{
							var elapsedTimeSinceLastReceive = (DateTime.Now - _frameLastTime).TotalMilliseconds;
							if ((_frameBytesCount > 0) && (elapsedTimeSinceLastReceive > ReceivingTimeOut))
							{
								Console.WriteLine("RESET OLD FRAME! After {2} msec, {0} bytes: {1}", _frameBytesCount, BitConverter.ToString(_frameBytes, 0, _frameBytesCount), elapsedTimeSinceLastReceive);
								ResetFrame();
							}

							while(_client.Available > 0)
							{
								_frameLastTime = DateTime.Now;
								_frameBytesCount += _stream.Read(_frameBytes, _frameBytesCount, MAX_FRAME_LENGTH - _frameBytesCount);

								object parsedObject = TryToDeserializeStream(new MemoryStream(_frameBytes, 0, _frameBytesCount));
								
								if (parsedObject != null)
								{
									_inputQueue.Enqueue(parsedObject);
									ResetFrame();
								}
							}
						}

						lock (((ICollection)_inputQueue).SyncRoot)
						{
							while (_inputQueue.Count > 0)
							{
								//Formatter.Deserialize(_stream)
								if (Received != null)
									Received(this, new ReceivedEventArgs<IPEndPoint>(_inputQueue.Dequeue(), _rem));
							}
						}

						lock (((ICollection)_outputQueue).SyncRoot)
						{
							while (_outputQueue.Count > 0)
							{
								Formatter.Serialize(_stream, _outputQueue.Dequeue());
							}
						}
					}
					catch (IOException)  /* TODO: errorCount + rebind */
					{
						if (Error != null)
							Error(this, new ErrorEventArgs(new SocketException((int)SocketError.ConnectionReset)));
						Close();
					}
					catch (SocketException ex)
					{
						if (Error != null)
							Error(this, new ErrorEventArgs(ex));
						Close();
					}
					catch (SerializationException ex)
					{
						if (Error != null)
							Error(this, new ErrorEventArgs(ex));
						ResetFrame();
					}
					catch (DecoderFallbackException ex)
					{
						if (Error != null)
							Error(this, new ErrorEventArgs(ex));
						ResetFrame();

					}
					catch (Exception ex)
					{
						if (Error != null)
							Error(this, new ErrorEventArgs(ex));
						ResetFrame();
					}
				}

				Thread.Sleep(_grain);
			}

			if (_stream != null) _stream.Close();
			if (_client != null) _client.Close();
			if (_timer != null) _timer.Stop();

			e.Cancel = true;

			if (Disconnected != null)
				Disconnected(this, new ConnectedEventArgs<IPEndPoint>(_rem,_loc));

		}

		[DebuggerNonUserCode]	//TODO: rivedere..
		private object TryToDeserializeStream(Stream s)
		{
			try
			{
				return Formatter.Deserialize(s);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

		}

		private void HandleTimeoutExpired(object sender, ElapsedEventArgs args)
		{
			lock(_syncLock)
			{

				if (!_syncPointReached)
				{
					_syncPointReached = true;
					_client.Close();
				}
			}
			
			if (Error != null)
				Error(this, new ErrorEventArgs(new SocketException((int)SocketError.TimedOut)));
		}

		private void HandleConnectEvent(IAsyncResult ar)
		{
			try
			{
				lock(_syncLock)
				{

					if (!_syncPointReached)
					{
						_syncPointReached = true;

						_timer.Stop();

						if (ar == _currentAsyncResult)
							_client.EndConnect(ar);

						_rem = _client.Client.RemoteEndPoint as IPEndPoint;
						_loc = _client.Client.LocalEndPoint  as IPEndPoint;

						this.StartWork();

						if (Connected != null)
							Connected(this, new ConnectedEventArgs<IPEndPoint>(_rem,_loc));

					}
				}

			}
			catch (SocketException ex)
			{
				if (Error != null)
					Error(this, new ErrorEventArgs(ex));
				_client.Close();
			}

		}
	}
}
