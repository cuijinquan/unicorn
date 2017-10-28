
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unicorn {
	public class MessageEndpointMap {
		public MessageEndpointMap() {
			_endpoints = new SortedDictionary<int, MessageEndpoint>();
		}

		private readonly SortedDictionary<int, MessageEndpoint> _endpoints;

		/// <summary>
		/// Try getting an endpoint.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		public bool TryGet(int id, out MessageEndpoint endpoint) {
			return _endpoints.TryGetValue(id, out endpoint);
		}

		/// <summary>
		/// Add a message endpoint with a unique id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="endpoint"></param>
		public void Add(int id, MessageEndpoint endpoint) {
			if (endpoint == null)
				throw new ArgumentNullException("endpoint");
			if (_endpoints.ContainsKey(id))
				throw new MessageEndpointOccupiedException(id);
			_endpoints[id] = endpoint;
		}

		/// <summary>
		/// Add a message endpoint with a unique id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="target"></param>
		/// <param name="method"></param>
		public void Add(int id, object target, MethodInfo method) {
			if (method == null)
				throw new ArgumentNullException("method");
			Add(id, (MessageEndpoint)Delegate.CreateDelegate(typeof(MessageEndpoint), target, method));
		}

		/// <summary>
		/// Add message endpoints with a <see cref="MessageAttribute"/>.
		/// </summary>
		/// <param name="target"></param>
		public void AddFrom(object target) {
			if (target == null)
				throw new ArgumentNullException("target");

			for (var type = target.GetType(); type != null; type = type.BaseType) {
				AddPossible(target, type.GetMethods(BindingFlags.Instance
					| BindingFlags.DeclaredOnly
					| BindingFlags.Public
					| BindingFlags.NonPublic));
			}
		}

		/// <summary>
		/// Add static message endpoints with a <see cref="MessageAttribute"/>
		/// </summary>
		/// <param name="type"></param>
		public void AddStatic(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			for(; type != null; type = type.BaseType) {
				AddPossible(null, type.GetMethods(BindingFlags.Static
					| BindingFlags.DeclaredOnly
					| BindingFlags.Public
					| BindingFlags.NonPublic));
			}
		}

		/// <summary>
		/// Add static message endpoints with a <see cref="MessageAttribute"/>
		/// </summary>
		/// <param name="type"></param>
		public void AddStatic<T>() {
			AddStatic(typeof(T));
		}

		private void AddPossible(object target, IEnumerable<MethodInfo> methods) {
			foreach (var method in methods) {
				var attributes = method.GetCustomAttributes(typeof(MessageAttribute), true);
				if (attributes.Length > 0) {
					Add(((MessageAttribute)attributes[0]).Id, target, method);
				}
			}
		}
	}
}
