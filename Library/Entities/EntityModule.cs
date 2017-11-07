
namespace Unicorn.Entities {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Unicorn.Entities.Internal;
	using Unicorn.IO;
	using Unicorn.Util;
	using UnityEngine;
	
	[RequireComponent(typeof(Entity))]
	public abstract class EntityModule<T> : MonoBehaviour, IEntityModuleInternal where T : EntityModule<T> {
		private byte? _moduleId;
		private SortedDictionary<ushort, Action<Message>> _endpoints;

		private static SortedDictionary<ushort, MethodInfo> _serverEndpointCache;
		private static SortedDictionary<ushort, MethodInfo> _clientEndpointCache;

		

		private Entity _entity;
		public Entity Entity {
			get {
				if (!_entity)
					_entity = GetComponent<Entity>();
				return _entity;
			}
		}

		public Disposable UntilDestroy { get { return Entity.UntilDestroy; } }

		public EntityId Id {
			get {
				var id = Entity.Id;
				if (id == EntityId.None)
					throw new InvalidOperationException("Unassigned entity id.");
				return id;
			}
		}

		public string Source {
			get { return Entity.Source; }
		}

		public IReadonlyObservableSet<Connection> Group {
			get { return Entity.Group; }
			set { Entity.Group = value; }
		}

		public Set<Connection> OwnerSet {
			get { return Entity.OwnerSet; }
		}

		public IReadonlyObservableSet<Connection> Owners {
			get { return Entity.Owners; }
			set { Entity.Owners = value; }
		}

		public bool IsMine {
			get { return Entity.IsMine; }
		}



		protected bool IsServer {
			get { return EntityRouter.Require().IsServer; }
		}
		protected bool IsClient {
			get { return EntityRouter.Require().IsClient; }
		}
		protected bool IsShuttingDown {
			get { return EntityRouter.Require().IsShuttingDown; }
		}

		public void SetOwner(Connection owner) {
			Entity.SetOwner(owner);
		}



		protected void Send(MessageWriter message) {
			Send(0, message);
		}

		protected void Send(int channelKey, MessageWriter message) {
			if (IsServer) {
				Send(Group, channelKey, message);
			} else {
				Send(EntityRouter.Current.Connections, channelKey, message);
			}
		}

		protected void Send(IEnumerable<Connection> target, MessageWriter message) {
			Send(target, 0, message);
		}

		protected void Send(IEnumerable<Connection> target, int channelKey, MessageWriter message) {
			if (_moduleId == null)
				throw new InvalidOperationException("Unassigned module id.");

			target.Send(channelKey, payload => {
				payload.Write(Id);
				payload.Write((byte)_moduleId);
				message(payload);
			});
		}

		protected void Endpoint(DataWriter payload, object code) {
			payload.Write(Convert.ToUInt16(code));
		}

		protected virtual void Awake() {
			_moduleId = ((IEntityInternal)Entity).AddModule(this);
			_endpoints = new SortedDictionary<ushort, Action<Message>>();
			if (IsServer) {
				CollectEndpoints(ref _serverEndpointCache);
			} else {
				CollectEndpoints(ref _clientEndpointCache);
			}
		}

		private void CollectEndpoints(ref SortedDictionary<ushort, MethodInfo> endpointCache) {
			if (endpointCache == null) {
				endpointCache = new SortedDictionary<ushort, MethodInfo>();
				foreach (var method in GetMethods()) {
					var attrs = method.GetCustomAttributes(typeof(EndpointAttribute), true);
					if (attrs.Length > 0) {
						var attr = (EndpointAttribute)attrs[0];
						if (attr.IsServerEndpoint == IsServer) {
							endpointCache.Add(attr.Code, method);
						}
					}
				}
			}

			foreach(var pair in endpointCache) {
				_endpoints.Add(pair.Key, (Action<Message>)Delegate.CreateDelegate(typeof(Action<Message>), this, pair.Value));
			}
		}

		private IEnumerable<MethodInfo> GetMethods() {
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			for (var type = GetType(); type != null; type = type.BaseType) {
				foreach(var method in type.GetMethods(flags)) {
					yield return method;
				}
			}
		}

		

		protected virtual void OnDestroy() {
			if (_moduleId != null)
				((IEntityInternal)Entity).RemoveModule((byte)_moduleId);
		}
		
		void IEntityModuleInternal.Receive(Message msg) {
			var code = msg.ReadUInt16();
			Action<Message> endpoint;
			if (_endpoints.TryGetValue(code, out endpoint)) {
				endpoint(msg);
			} else {
				Debug.LogWarningFormat("Unknown endpoint code or wrong peer state: {0}", code);
			}
		}
	}
}

namespace Unicorn.IO {
	using Unicorn.Entities;
	
	partial class DataWriter {
		public void WriteEntityModule<T>(T value) where T : EntityModule<T> {
			Write(value ? value.Entity : null);
		}
	}

	partial class DataReader {
		public T ReadEntityModule<T>() where T : EntityModule<T> {
			var entity = ReadEntity();
			return entity ? entity.As<T>() : null;
		}
	}
}
