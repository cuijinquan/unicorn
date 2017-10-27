
namespace Unicorn.Internal {
	/// <summary>
	/// Used to access network router internals.
	/// </summary>
	public interface IRouterInternal {
		/// <summary>
		/// Called by a router's worker object to update.
		/// </summary>
		void Update();

		/// <summary>
		/// Called by a router's worker object to shutdown.
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Called by a connection when disconnected.
		/// </summary>
		/// <param name="conn"></param>
		void Disconnected(Connection conn);

		/// <summary>
		/// Get a transport layer channel id from a registered key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		int GetChannelId(int key);
	}
}
