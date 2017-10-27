
using Unicorn.Internal;
using Unicorn.Util;

namespace Unicorn {
	public class Peer : IPeerInternal {
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
		/// <param name="sender"></param>
		/// <param name="buffer"></param>
		/// <param name="length"></param>
		protected virtual void Receive(Connection sender, byte[] buffer, int length) { }

		private Router _router;



		/// <summary>
		/// True if currently shutting down.
		/// </summary>
		protected bool IsShuttingDown { get { return _router.IsShuttingDown; } }

		/// <summary>
		/// Get the root set of all network connections.
		/// </summary>
		protected IReadonlyObservableSet<Connection> Connections { get { return _router.Connections; } }



		void IPeerInternal.Started(Router router) {
			_router = router;
			Started();
		}

		void IPeerInternal.ShuttingDown() {
			ShuttingDown();
		}

		void IPeerInternal.Stopped() {
			Stopped();
		}

		void IPeerInternal.Receive(Connection sender, byte[] buffer, int length) {
			Receive(sender, buffer, length);
		}
	}
}
