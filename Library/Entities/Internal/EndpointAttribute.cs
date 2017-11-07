
using System;

namespace Unicorn.Entities.Internal {
	[AttributeUsage(AttributeTargets.Method)]
	public abstract class EndpointAttribute : Attribute {
		public EndpointAttribute(object code) {
			_code = Convert.ToUInt16(code);
		}
		
		private ushort _code;
		public ushort Code { get { return _code; } }

		public abstract bool IsServerEndpoint { get; }
	}
}
