
using System;
using Unicorn.Internal;

namespace Unicorn {
	public class PeerRouter : Router {
		public PeerRouter(RouterConfig config, PeerFactory serverFactory, PeerFactory clientFactory) : base(config) {
			if (serverFactory == null)
				throw new ArgumentNullException("serverFactory");
			if (clientFactory == null)
				throw new ArgumentNullException("clientFactory");

			_serverFactory = serverFactory;
			_clientFactory = clientFactory;
			_peer = null;
		}

		private readonly PeerFactory _serverFactory;
		private readonly PeerFactory _clientFactory;
		private IPeerInternal _peer;

		protected override void Started() {
			base.Started();
			_peer = IsServer ? _serverFactory() : _clientFactory();
			_peer.Started(this);
		}

		protected override void ShuttingDown() {
			base.ShuttingDown();
			_peer.ShuttingDown();
		}

		protected override void Stopped() {
			base.Stopped();
			_peer.Stopped();
			_peer = null;
		}

		protected override void Receive(Connection sender, byte[] buffer, int length) {
			_peer.Receive(sender, buffer, length);
		}
	}
}
