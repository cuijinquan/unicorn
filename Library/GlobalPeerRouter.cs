
namespace Unicorn {
	public class GlobalPeerRouter : GlobalRouter<PeerRouter> {
		/// <summary>
		/// Call once on application startup to initialize a global router instance.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="serverFactory"></param>
		/// <param name="clientFactory"></param>
		protected static void Initialize(RouterConfig config, PeerFactory serverFactory, PeerFactory clientFactory) {
			Initialize(new PeerRouter(config, serverFactory, clientFactory));
		}
	}
}
