
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unicorn.Internal {
	public interface IRouterConfigInternal {
		int ReceiveBufferSize { get; }
		ConnectionConfig GetConnectionConfig();
		SortedDictionary<int, int> GetChannelMap();
	}
}
