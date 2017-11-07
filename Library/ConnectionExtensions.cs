
using System;
using System.Collections.Generic;
using Unicorn.IO;

namespace Unicorn {
	public static class ConnectionExtensions {
		public static void Send(this IEnumerable<Connection> target, MessageWriter message) {
			Send(target, 0, message);
		}

		public static void Send(this IEnumerable<Connection> target, int channelKey, MessageWriter message) {
			var payload = new MemoryWriter();
			message(payload);
			var buffer = payload.GetBuffer();
			var length = payload.Length;
			foreach (var conn in target)
				conn.Send(channelKey, buffer, length);
		}
	}
}
