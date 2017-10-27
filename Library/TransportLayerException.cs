
using System;
using UnityEngine.Networking;

namespace Unicorn {
	public class TransportLayerException : Exception {
		public TransportLayerException(byte error) : this((NetworkError)error) { }
		public TransportLayerException(NetworkError error) : base(error.ToString()) { }
	}
}
