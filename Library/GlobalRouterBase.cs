
using System;
using System.Net;
using Unicorn.Util;

namespace Unicorn {
	public abstract class GlobalRouterBase<T> where T : Router {
		protected GlobalRouterBase() { throw new NotSupportedException(); }

		private static T _router;
		/// <summary>
		/// Get the current global router instance.
		/// </summary>
		protected static T Router {
			get {
				if (_router == null)
					throw new InvalidOperationException("No global router is initialized.");
				return _router;
			}
		}

		/// <summary>
		/// Get the current network state.
		/// </summary>
		public static RouterState State {
			get { return Router.State; }
		}

		/// <summary>
		/// True if currently shutting down.
		/// </summary>
		public static bool IsShuttingDown {
			get { return Router.IsShuttingDown; }
		}

		/// <summary>
		/// True if a server or a client.
		/// </summary>
		public static bool IsOnline {
			get { return Router.IsOnline; }
		}

		/// <summary>
		/// True if a server.
		/// </summary>
		public static bool IsServer {
			get { return Router.IsServer; }
		}

		/// <summary>
		/// True if a client.
		/// </summary>
		public static bool IsClient {
			get { return Router.IsClient; }
		}

		/// <summary>
		/// Get the root set of all network connections.
		/// </summary>
		public static IReadonlyObservableSet<Connection> Connections {
			get { return Router.Connections; }
		}

		/// <summary>
		/// Start a network server.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="maxConnections"></param>
		/// <returns></returns>
		public static bool StartServer(int port, int maxConnections) {
			return Router.StartServer(port, maxConnections);
		}

		/// <summary>
		/// Start a network server.
		/// </summary>
		/// <param name="minPort"></param>
		/// <param name="maxPort"></param>
		/// <param name="maxConnections"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public static bool StartServer(int minPort, int maxPort, int maxConnections, out int port) {
			return Router.StartServer(minPort, maxPort, maxConnections, out port);
		}

		/// <summary>
		/// Connect to a server.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		public static void Connect(IPAddress address, int port) {
			Router.Connect(address, port);
		}

		/// <summary>
		/// Connect to a server.
		/// </summary>
		/// <param name="hostNameOrAddress"></param>
		/// <param name="port"></param>
		public static void Connect(string hostNameOrAddress, int port) {
			Router.Connect(hostNameOrAddress, port);
		}

		/// <summary>
		/// Send disconnect request to all connections &amp; stop after all connections have been closed.
		/// </summary>
		public static void Shutdown() {
			Router.Shutdown();
		}

		/// <summary>
		/// Immediately shutdown without notifying connections.
		/// </summary>
		public static void ShutdownImmediate() {
			Router.ShutdownImmediate();
		}

		/// <summary>
		/// Call once on application startup to initialize a global router instance.
		/// </summary>
		/// <param name="router"></param>
		protected static void Initialize(T router) {
			if (router == null)
				throw new ArgumentNullException("router");
			if (_router != null)
				_router.ShutdownImmediate();
			_router = router;
		}
	}
}
