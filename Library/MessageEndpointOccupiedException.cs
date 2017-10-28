
using System;

namespace Unicorn {
	public class MessageEndpointOccupiedException : Exception {
		public MessageEndpointOccupiedException(int id) : base(id.ToString()) { }
	}
}
