
using Unicorn.Entities.Internal;

namespace Unicorn.Entities {
	public class ServerAttribute : EndpointAttribute {
		public ServerAttribute(object code) : base(code) { }

		public override bool IsServerEndpoint { get { return true; } }
	}
}
