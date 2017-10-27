
using System;
using Unicorn.Util;

namespace Unicorn {
	public class GlobalRouter<T> where T : Router {
		protected GlobalRouter() { throw new NotSupportedException(); }

		private static T _router;
		/// <summary>
		/// Get the current global router instance.
		/// </summary>
		public static T Router {
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
		/// Connect to a server.
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		public static void Connect(string ipAddress, int port) {
			Router.Connect(ipAddress, port);
		}

		/// <summary>
		/// Send disconnect request to all connections &amp; stop after all connections have been closed.
		/// </summary>
		public static void Shutdown() {
			Router.Shutdown();
		}



		/// <summary>
		/// Call once on application startup to initialize a global router instance.
		/// </summary>
		/// <param name="router"></param>
		protected static void Initialize(T router) {
			if (_router != null)
				throw new InvalidOperationException("Another global router is already initialized.");
			if (router == null)
				throw new ArgumentNullException("router");
			_router = router;
		}
	}
}
