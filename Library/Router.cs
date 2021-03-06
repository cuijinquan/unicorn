﻿
using System;
using System.Collections.Generic;
using Unicorn.Internal;
using Unicorn.Util;
using UnityEngine.Networking;
using UnityEngine;
using System.Net;
using Unicorn.IO;

namespace Unicorn {
	public abstract class Router : IRouterInternal, IDisposable {
		public Router(RouterConfig config) {
			if (config == null)
				throw new ArgumentNullException("config");

			NetworkTransport.Init();
			var conf = (IRouterConfigInternal)config;

			_connectionConfig = conf.GetConnectionConfig();
			_channelMap = conf.GetChannelMap();
			_state = RouterState.None;
			_shuttingDown = false;
			_hostId = -1;
			_clientId = -1;
			_connectionMap = new SortedDictionary<int, Connection>();
			_connections = new Set<Connection>();
			_connections.Added(conn => _connectionMap[conn.Id] = conn);
			_connections.Removed(conn => _connectionMap.Remove(conn.Id));
			_receiveBuffer = new byte[conf.ReceiveBufferSize];

			RouterWorker.Create(this);
		}

		private readonly ConnectionConfig _connectionConfig;
		private readonly SortedDictionary<int, int> _channelMap;

		private RouterState _state;
		private bool _shuttingDown;
		private int _hostId;
		private int _clientId;
		private SortedDictionary<int, Connection> _connectionMap;
		private Set<Connection> _connections;
		private byte[] _receiveBuffer;

		/// <summary>
		/// Get the current network state.
		/// </summary>
		public RouterState State { get { return _state; } }
		/// <summary>
		/// True if currently shutting down.
		/// </summary>
		public bool IsShuttingDown { get { return _shuttingDown; } }
		/// <summary>
		/// True if a server or a client.
		/// </summary>
		public bool IsOnline { get { return _state != RouterState.None; } }
		/// <summary>
		/// True if a server.
		/// </summary>
		public bool IsServer { get { return _state == RouterState.Server; } }
		/// <summary>
		/// True if a client.
		/// </summary>
		public bool IsClient { get { return _state == RouterState.Client; } }
		/// <summary>
		/// Get the root set of all network connections.
		/// </summary>
		public IReadonlyObservableSet<Connection> Connections { get { return _connections; } }



		/// <summary>
		/// Called when the network has been started.
		/// </summary>
		protected virtual void Started() { }
		/// <summary>
		/// Called when shutting down. The <see cref="Stopped"/> function
		/// will be called as soon as all connections are closed.
		/// </summary>
		protected virtual void ShuttingDown() { }
		/// <summary>
		/// Called when the network has been stopped.
		/// </summary>
		protected virtual void Stopped() { }
		/// <summary>
		/// Called to handle inbound messages.
		/// </summary>
		/// <param name="msg"></param>
		protected abstract void Receive(Message msg);



		/// <summary>
		/// Start a network server.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="maxConnections"></param>
		/// <returns></returns>
		public bool StartServer(int port, int maxConnections) {
			if (_state != RouterState.None || _shuttingDown)
				throw new InvalidOperationException();
			if ((_hostId = NetworkTransport.AddHost(new HostTopology(_connectionConfig, maxConnections), port)) >= 0) {
				_state = RouterState.Server;
				Started();
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Start a network server.
		/// </summary>
		/// <param name="minPort"></param>
		/// <param name="maxPort"></param>
		/// <param name="maxConnections"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool StartServer(int minPort, int maxPort, int maxConnections, out int port) {
			for (port = minPort; port < maxPort; port++)
				if (StartServer(port, maxConnections))
					return true;
			return false;
		}

		/// <summary>
		/// Connect to a server.
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		public void Connect(IPAddress address, int port) {
			if (_state != RouterState.None || _shuttingDown)
				throw new InvalidOperationException();
			_hostId = NetworkTransport.AddHost(new HostTopology(_connectionConfig, 1));

			byte error;
			_clientId = NetworkTransport.Connect(_hostId, address.ToString(), port, 0, out error);
			if (error == (byte)NetworkError.Ok) {
				_state = RouterState.Client;
				Started();
			} else {
				NetworkTransport.RemoveHost(_hostId);
			}
		}

		/// <summary>
		/// Connect to a server.
		/// </summary>
		/// <param name="hostNameOrAddress"></param>
		/// <param name="port"></param>
		public void Connect(string hostNameOrAddress, int port) {
			Connect(Dns.GetHostEntry(hostNameOrAddress).AddressList[0], port);
		}

		/// <summary>
		/// Send disconnect request to all connections &amp; stop after all connections have been closed.
		/// </summary>
		public void Shutdown() {
			if (_state != RouterState.None && !_shuttingDown) {
				_shuttingDown = true;
				ShuttingDown();
				foreach (var conn in _connections)
					conn.Disconnect();

				TryFinalizeShutdown();
			}
		}

		/// <summary>
		/// Immediately shutdown without notifying connections.
		/// </summary>
		public void ShutdownImmediate() {
			if (_state != RouterState.None) {
				try {
					_shuttingDown = true;
					ShuttingDown();

					var connections = new LinkedList<Connection>(_connections);
					foreach (var conn in connections)
						((IConnectionInternal)conn).TransportLayerDisconnected();

					NetworkTransport.RemoveHost(_hostId);
					_hostId = -1;
					_clientId = -1;
					Stopped();
				} finally {
					_shuttingDown = false;
					_state = RouterState.None;
				}
			}
		}

		private void TryFinalizeShutdown() {
			if ((_state == RouterState.Client || _shuttingDown) && _connections.Count == 0) {
				try {
					NetworkTransport.RemoveHost(_hostId);
					_hostId = -1;
					_clientId = -1;
					Stopped();
				} finally {
					_shuttingDown = false;
					_state = RouterState.None;
				}
			}
		}



		void IRouterInternal.Update() {
			if (_state != RouterState.None) {
				NetworkEventType eventType;
				Connection conn;
				int connId;
				int channelId;
				var buffer = _receiveBuffer;
				int length;
				byte error;

				do {
					eventType = NetworkTransport.ReceiveFromHost(_hostId, out connId,
						out channelId, buffer, buffer.Length, out length, out error);

					switch (eventType) {
						case NetworkEventType.ConnectEvent:
							if (!_shuttingDown && (_state == RouterState.Server || connId == _clientId)) {
								conn = new Connection(this, _hostId, connId);
								_connections.Add(conn);
							} else {
								NetworkTransport.Disconnect(_hostId, connId, out error);
							}
							break;

						case NetworkEventType.DisconnectEvent:
							if (_connectionMap.TryGetValue(connId, out conn)) {
								((IConnectionInternal)conn).TransportLayerDisconnected();
								TryFinalizeShutdown();
							}
							break;

						case NetworkEventType.DataEvent:
							if (error == (byte)NetworkError.MessageToLong) {
								// TODO: Use buffer pool / output warning if messages frequently are too long.
								buffer = new byte[length];

							} else if (_connectionMap.TryGetValue(connId, out conn)) {
								Receive(new Message(conn, buffer, length));

							} else {
								Debug.LogWarningFormat("Unknown connection: {0}", connId);
							}
							break;
					}
				} while (eventType != NetworkEventType.Nothing && _hostId >= 0);
			}
		}

		void IRouterInternal.Disconnected(Connection conn) {
			_connections.Remove(conn);
		}

		int IRouterInternal.GetChannelId(int key) {
			return _channelMap[key];
		}

		void IDisposable.Dispose() {
			ShutdownImmediate();
		}
	}
}
