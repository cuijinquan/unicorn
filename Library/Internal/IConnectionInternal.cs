
namespace Unicorn.Internal {
	/// <summary>
	/// Used to access network connection internals.
	/// </summary>
	public interface IConnectionInternal {
		/// <summary>
		/// Called by the connection's router when the transport layer is disconnected.
		/// The connection should remove itself from the router immediately.
		/// </summary>
		void TransportLayerDisconnected();
	}
}
