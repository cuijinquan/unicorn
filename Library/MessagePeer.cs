
using System;
using System.Collections.Generic;
using Unicorn.IO;
using UnityEngine;

namespace Unicorn {
	/// <summary>
	/// A <see cref="Peer"/> that is able to send or receive messages with ids.
	/// </summary>
	public abstract class MessagePeer : Peer {
		public MessagePeer() {
			_endpoints = new MessageEndpointMap();
		}

		private readonly MessageEndpointMap _endpoints;

		protected MessageEndpointMap Endpoints { get { return _endpoints; } }

		/// <summary>
		/// Send a message using the first defined channel.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="id"></param>
		/// <param name="serializePayload"></param>
		public static void Send(IEnumerable<Connection> target, object id, Action<DataWriter> serializePayload) {
			Send(target, 0, id, serializePayload);
		}

		/// <summary>
		/// Send a message.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="channelKey"></param>
		/// <param name="id"></param>
		/// <param name="serializePayload"></param>
		public static void Send(IEnumerable<Connection> target, object channelKey, object id, Action<DataWriter> serializePayload) {
			var writer = new MemoryWriter();

#if UNICORN_32BIT_MESSAGE_IDS
			writer.Write(Convert.ToInt32(id));
#else
			writer.Write(Convert.ToInt16(id));
#endif
			serializePayload(writer);
			var buffer = writer.GetBuffer();
			var length = writer.Length;
			foreach (var conn in target)
				conn.Send(Convert.ToInt32(channelKey), buffer, length);
		}

		protected override void Receive(Connection sender, byte[] buffer, int length) {
			var payload = new MemoryReader(buffer, length);

#if UNICORN_32BIT_MESSAGE_IDS
			var id = payload.ReadInt32();
#else
			var id = payload.ReadInt16();
#endif

			MessageEndpoint endpoint;
			if (_endpoints.TryGet(id, out endpoint)) {
				endpoint(sender, payload);
			} else {
				Debug.LogErrorFormat("Unknown message endpoint id: {0}", id);
			}
		}
	}
}
