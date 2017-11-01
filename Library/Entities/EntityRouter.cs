
using System;
using Unicorn.Entities.Internal;
using Unicorn.IO;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unicorn.Entities {
	public class EntityRouter : Router {
		[ThreadStatic]
		private static EntityRouter _current;
		public static EntityRouter Current { get { return _current; } }
		
		public EntityRouter(RouterConfig config, string rootEntity) : base(config) {
			_rootEntitySource = rootEntity;
		}

		private string _rootEntitySource;
		
		protected override void Started() {
			base.Started();
			if (_current != null)
				Debug.LogError("Multiple game routers are used at the same time.");
			_current = this;
			EntityId.ResetPool();
			if (Net.IsServer && _rootEntitySource != null) {
				Entity.Create(_rootEntitySource).Group = Connections;
			}
		}
		
		protected override void Stopped() {
			base.Stopped();
			if (_current == this) {
				_current = null;

				foreach(var entity in Entity.All) {
					UnityObject.Destroy(entity.gameObject);
				}
			}
		}
		
		protected override void Receive(Connection sender, byte[] buffer, int length) {
			var payload = new MemoryReader(buffer, length);
			var entityId = EntityId.ReadFrom(payload);
			if (entityId == EntityId.None) {
				var controlCode = payload.ReadByte();
				if (IsClient) {
					switch((EntityControlCode)controlCode) {
						case EntityControlCode.CreateEntity:
							Entity.Create(EntityId.ReadFrom(payload), payload.ReadString(), payload.ReadVector3(), payload.ReadQuaternion());
							break;

						case EntityControlCode.DestroyEntity:
							Entity.Destroy(EntityId.ReadFrom(payload));
							break;

						case EntityControlCode.SetEntityOwnership:
							var isMine = payload.ReadBoolean();
							Entity.Use(EntityId.ReadFrom(payload), entity => ((IEntityInternal)entity).SetOwnership(isMine));
							break;

						default:
							Debug.LogWarning(string.Format("Unknown client control code: {0}", controlCode));
							break;
					}
				} else {
					Debug.LogWarning("Control code received on server.");
				}
			} else if (!Entity.Use(entityId, entity => {
				((IEntityInternal)entity).Receive(sender, payload);
			})) {
				Debug.LogWarningFormat("Entity not found: {0}", entityId);
			}
		}
	}
}
