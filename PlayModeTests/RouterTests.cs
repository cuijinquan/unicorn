using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System;
using Unicorn.IO;

namespace Unicorn.PlayModeTests {
	public class RouterTests {
		public static TimeSpan ConnectTimeout = TimeSpan.FromSeconds(2);
		public static TimeSpan DisconnectTimeout = TimeSpan.FromSeconds(5);
		public static TimeSpan SendTimeout = TimeSpan.FromSeconds(1);

		[UnityTest]
		public IEnumerator SimpleConnection() {
			using (var server = new TestRouter())
			using (var client = new TestRouter()) {
				var port = server.StartServer();
				client.Connect(port);
				yield return ConnectTimeout.Wait(() => server.Connections.Count > 0
					&& client.Connections.Count > 0);
			}
		}

		[UnityTest]
		public IEnumerator DisconnectFromServer() {
			using (var server = new TestRouter())
			using (var client = new TestRouter()) {
				var port = server.StartServer();
				client.Connect(port);
				yield return ConnectTimeout.Wait(() => server.Connections.Count > 0
					&& client.Connections.Count > 0);
				foreach (var conn in server.Connections)
					conn.Disconnect();
				yield return DisconnectTimeout.Wait(() => client.Connections.Count <= 0);
			}
		}

		[UnityTest]
		public IEnumerator DisconnectFromClient() {
			using (var server = new TestRouter())
			using (var client = new TestRouter()) {
				var port = server.StartServer();
				client.Connect(port);
				yield return ConnectTimeout.Wait(() => server.Connections.Count > 0
					&& client.Connections.Count > 0);
				foreach (var conn in client.Connections)
					conn.Disconnect();
				yield return DisconnectTimeout.Wait(() => server.Connections.Count <= 0);
			}
		}

		[UnityTest]
		public IEnumerator SendFromServer() {
			using (var server = new TestRouter())
			using (var client1 = new TestRouter())
			using (var client2 = new TestRouter()) {
				var port = server.StartServer();
				client1.Connect(port);
				client2.Connect(port);
				yield return ConnectTimeout.Wait(() => server.Connections.Count > 0
					&& client1.Connections.Count > 0 && client2.Connections.Count > 0);

				var writer = new MemoryWriter();
				var message = Guid.NewGuid();
				writer.Write(message);
				foreach (var conn in server.Connections)
					conn.Send(0, writer.GetBuffer(), writer.Length);

				yield return SendTimeout.Wait(() => client1.Messages.Count > 0
					&& client2.Messages.Count > 0);
				Assert.AreEqual(message, client1.Messages[0]);
				Assert.AreEqual(message, client2.Messages[0]);
			}
		}
		
		[UnityTest]
		public IEnumerator SendFromClients() {
			using (var server = new TestRouter())
			using (var client1 = new TestRouter())
			using (var client2 = new TestRouter()) {
				var port = server.StartServer();
				client1.Connect(port);
				client2.Connect(port);
				yield return ConnectTimeout.Wait(() => server.Connections.Count > 0
					&& client1.Connections.Count > 0 && client2.Connections.Count > 0);

				var writer = new MemoryWriter();
				var message = Guid.NewGuid();
				writer.Write(message);
				foreach (var conn in client1.Connections)
					conn.Send(0, writer.GetBuffer(), writer.Length);
				foreach (var conn in client2.Connections)
					conn.Send(0, writer.GetBuffer(), writer.Length);
				
				yield return SendTimeout.Wait(() => server.Messages.Count > 0);
				Assert.AreEqual(message, server.Messages[0]);
			}
		}
	}
}
