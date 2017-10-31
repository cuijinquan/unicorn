
using System;
using System.Collections.Generic;
using System.Net;
using Unicorn.IO;
using UnityEngine.Networking;

namespace Unicorn.PlayModeTests {
	public class TestRouter : Router {
		public const int DefaultChannel = 0;
		
		public TestRouter() : base(CreateConfig()) { }
		private static RouterConfig CreateConfig() {
			var config = new RouterConfig();
			config.AddChannel(DefaultChannel, QosType.ReliableSequenced);
			return config;
		}

		public int StartServer() {
			int port;
			if (!StartServer(4200, 4300, 16, out port))
				throw new Exception("All test ports in use.");
			return port;
		}
		
		public void Connect(int port) {
			Connect(IPAddress.Loopback, port);
		}

		private List<Guid> _messages = new List<Guid>();
		public List<Guid> Messages { get { return _messages; } }
		
		protected override void Receive(Connection sender, byte[] buffer, int length) {
			using (var reader = new MemoryReader(buffer, length)) {
				_messages.Add(reader.ReadGuid());
			}
		}
	}
}
