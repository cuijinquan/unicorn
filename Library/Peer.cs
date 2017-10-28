
using Unicorn.Internal;
using Unicorn.Util;

namespace Unicorn {
	public abstract class Peer : IPeerInternal {
		public Peer() {
			_disposeShutdown = new Disposable();
			_disposeStopped = new Disposable();
		}

		private Router _router;
		private readonly Disposable _disposeStopped;
		private readonly Disposable _disposeShutdown;

		/// <summary>
		/// True if currently shutting down.
		/// </summary>
		protected bool IsShuttingDown { get { return _router.IsShuttingDown; } }

		/// <summary>
		/// Get the root set of all network connections.
		/// </summary>
		protected IReadonlyObservableSet<Connection> Connections { get { return _router.Connections; } }

		/// <summary>
		/// Get a disposable that is disposed when the base of <see cref="ShuttingDown"/> is called.
		/// </summary>
		protected Disposable DisposeShutdown { get { return _disposeShutdown; } }

		/// <summary>
		/// Get a disposable that is disposed when the base of <see cref="Stopped"/> is called.
		/// </summary>
		protected Disposable DisposeStopped { get { return _disposeStopped; } }



		/// <summary>
		/// Called when the network has been started.
		/// </summary>
		protected virtual void Started() { }
		/// <summary>
		/// Called when shutting down. The <see cref="Stopped"/> function
		/// will be called as soon as all connections are closed.
		/// </summary>
		protected virtual void ShuttingDown() {
			_disposeShutdown.Dispose();
		}

		/// <summary>
		/// Called when the network has been stopped.
		/// </summary>
		protected virtual void Stopped() {
			_disposeStopped.Dispose();
		}

		/// <summary>
		/// Called to handle inbound messages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="buffer"></param>
		/// <param name="length"></param>
		protected abstract void Receive(Connection sender, byte[] buffer, int length);



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
