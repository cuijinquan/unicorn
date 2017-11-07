
using System;
using Unicorn.Entities.Internal;

namespace Unicorn.Entities {
	public class ClientAttribute : EndpointAttribute {
		public ClientAttribute(object code) : base(code) { }

		public override bool IsServerEndpoint { get { return false; } }
	}
}
