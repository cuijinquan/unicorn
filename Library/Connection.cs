
using System.Collections;
using System.Collections.Generic;
using Unicorn.Internal;
using Unicorn.Util;
using UnityEngine.Networking;

namespace Unicorn {
	/// <summary>
	/// Network connection.
	/// </summary>
	public sealed class Connection : IEnumerable<Connection>, IConnectionInternal {
		/// <summary>
		/// Called by a router to create a network connection.
		/// </summary>
		/// <param name="router"></param>
		/// <param name="hostId">The transport layer host id.</param>
		/// <param name="id">The transport layer connection id.</param>
		public Connection(Router router, int hostId, int id) {
			_meta = new Meta();
			_router = router;
			_hostId = hostId;
			_id = id;
			_disconnected = false;

			ulong network;
			ushort dstNode;
			byte error;
			_remoteAddress = NetworkTransport.GetConnectionInfo(hostId, id,
				out _remotePort, out network, out dstNode, out error);
			if (error != (byte)NetworkError.Ok)
				throw new TransportLayerException(error);
		}

		private readonly Meta _meta;
		private readonly IRouterInternal _router;
		private readonly int _hostId;
		private readonly int _id;
		private readonly string _remoteAddress;
		private readonly int _remotePort;
		private bool _disconnected;

		/// <summary>
		/// Get a meta collection for this connection.
		/// </summary>
		public Meta Meta { get { return _meta; } }
		/// <summary>
		/// True if connected.
		/// </summary>
		public bool Connected { get { return !_disconnected; } }
		/// <summary>
		/// The transport layer connection id.
		/// </summary>
		public int Id { get { return _id; } }
		/// <summary>
		/// The remote address.
		/// </summary>
		public string RemoteAddress { get { return _remoteAddress; } }
		/// <summary>
		/// The remote port.
		/// </summary>
		public int RemotePort { get { return _remotePort; } }

		/// <summary>
		/// Get a short string for the connection for debugging purpose.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return string.Format("Connection({0}, {1}, {2})", _id, _remoteAddress, _remotePort);
		}

		/// <summary>
		/// Send a network message.
		/// </summary>
		/// <param name="channelKey"></param>
		/// <param name="buffer"></param>
		/// <param name="length"></param>
		public void Send(int channelKey, byte[] buffer, int length) {
			if (!_disconnected) {
				byte error;
				NetworkTransport.Send(_hostId, _id, _router.GetChannelId(channelKey), buffer, length, out error);
				if (error != (byte)NetworkError.Ok)
					throw new TransportLayerException(error);
			}
		}

		/// <summary>
		/// Send a disconnect request if not already disconnected.
		/// </summary>
		public void Disconnect() {
			if (_disconnected ? false : _disconnected = true) {
				byte error;
				NetworkTransport.Disconnect(_hostId, _id, out error);
				if (error != (byte)NetworkError.Ok)
					throw new TransportLayerException(error);
			}
		}

		void IConnectionInternal.TransportLayerDisconnected() {
			_disconnected = true;
			_router.Disconnected(this);
		}

		IEnumerator<Connection> IEnumerable<Connection>.GetEnumerator() {
			yield return this;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			yield return this;
		}
	}
}
