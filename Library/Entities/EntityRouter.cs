
using System;
using System.Collections.Generic;
using Unicorn.Entities.Internal;
using Unicorn.IO;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unicorn.Entities {
	public class EntityRouter : Router {
		private static EntityRouter _current;
		public static EntityRouter Current { get { return _current; } }
		
		public static EntityRouter Require() {
			if (_current == null)
				throw new InvalidOperationException("An entity router is required.");
			return _current;
		}
		
		public EntityRouter(RouterConfig config) : this(config, null) { }
		public EntityRouter(RouterConfig config, string managerSource) : base(config) {
			if (_current != null)
				Debug.LogWarning("Multiple entity routers instantiated.");

			_current = this;
			_managerSource = managerSource;
		}
		
		private readonly string _managerSource;

		public enum ClientControlCode : byte {
			CreateEntity,
			DestroyEntity,
			SetEntityOwnership
		}

		protected override void Started() {
			base.Started();
			if (IsServer && !string.IsNullOrEmpty(_managerSource)) {
				EntityIdPool.Reset();
				var manager = Entity.Create(_managerSource);
				manager.Group = Connections;
			}
		}

		protected override void Stopped() {
			base.Stopped();
			foreach(var entity in new LinkedList<Entity>(Entity.All)) {
				UnityObject.Destroy(entity.gameObject);
			}
		}

		protected override void Receive(Connection sender, byte[] buffer, int length) {
			var payload = new MemoryReader(buffer, length);
			var entityId = payload.ReadEntityId();
			if (entityId == EntityId.None) {
				if (IsServer) {
					Debug.LogWarningFormat("Invalid server message.");
				} else {
					switch((ClientControlCode)payload.ReadByte()) {
						case ClientControlCode.CreateEntity:
							Entity.Create(payload.ReadEntityId(), payload.ReadString(), payload.ReadVector3(), payload.ReadQuaternion());
							break;

						case ClientControlCode.DestroyEntity:
							Entity.Destroy(payload.ReadEntityId());
							break;
							
						case ClientControlCode.SetEntityOwnership:
							Entity.Use(payload.ReadEntityId(), entity => ((IEntityInternal)entity).SetLocalOwnership(payload.ReadBoolean()));
							break;

						default:
							Debug.LogWarning("Unknown client control code received.");
							break;
					}
				}
			} else if (!Entity.Use(entityId, entity => {
				((IEntityInternal)entity).Receive(sender, payload);
			})) {
				Debug.LogWarningFormat("Missing entity to receive message: {0}", entityId);
			}
		}
	}
}
