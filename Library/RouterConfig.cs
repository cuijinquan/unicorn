
using System;
using System.Collections.Generic;
using Unicorn.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace Unicorn {
	/// <summary>
	/// Router configuration object.
	/// </summary>
	public class RouterConfig : IRouterConfigInternal {
		public RouterConfig() {
			_channelMap = new SortedDictionary<int, int>();
			_connectionConfig = new ConnectionConfig();
		}
		
		private SortedDictionary<int, int> _channelMap;
		private readonly ConnectionConfig _connectionConfig;

		/// <summary>
		/// Add a channel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="qos"></param>
		public RouterConfig AddChannel(object key, QosType qos) {
			var keyValue = Convert.ToInt32(key);
			if (keyValue == 0 && qos != QosType.ReliableSequenced)
				Debug.LogWarning("RouterConfig: QosType for the default channel (0) should be QosType.ReliableSequenced");

			_channelMap.Add(keyValue, _connectionConfig.AddChannel(qos));
			return this;
		}

		int IRouterConfigInternal.ReceiveBufferSize { get { return 8192; } }

		SortedDictionary<int, int> IRouterConfigInternal.GetChannelMap() {
			if (!_channelMap.ContainsKey(0))
				Debug.LogWarning("RouterConfig: Default channel (0) is not assigned.");
			return _channelMap;
		}

		ConnectionConfig IRouterConfigInternal.GetConnectionConfig() {
			return _connectionConfig;
		}
	}
}
