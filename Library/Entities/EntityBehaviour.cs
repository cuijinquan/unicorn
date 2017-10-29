
using System;
using System.Collections.Generic;
using Unicorn.Util;
using UnityEngine;

namespace Unicorn.Entities {
	[RequireComponent(typeof(Entity))]
	public abstract class EntityBehaviour : MonoBehaviour {
		private Entity _entity;
		public Entity Entity {
			get {
				if (_entity == null)
					_entity = GetComponent<Entity>();
				return _entity;
			}
		}

		public EntityId Id { get { return Entity.Id; } }
		public string Source { get { return Entity.Source; } }
		public IReadonlyObservableSet<Connection> Group {
			get { return Entity.Group; }
			set { Entity.Group = value; }
		}
		public Set<Connection> Owners { get { return Entity.Owners; } }
		public bool IsMine {
			get { return Entity.IsMine; }
			set { Entity.IsMine = value; }
		}

		protected virtual void Awake() {
		}

		protected virtual void OnDestroy() {
		}
	}

	public abstract class EntityBehaviour<T> : EntityBehaviour where T : EntityBehaviour<T> {
		private static SortedDictionary<EntityId, T> _map = new SortedDictionary<EntityId, T>();

		public static SortedDictionary<EntityId, T>.ValueCollection All { get { return _map.Values; } }
		


		public static bool Use(EntityId id, Action<T> action) {
			T instance;
			if (_map.TryGetValue(id, out instance)) {
				action(instance);
				return true;
			} else {
				return false;
			}
		}



		protected override void Awake() {
			base.Awake();
			if (Id == EntityId.None) {
				Debug.LogWarning("Unassigned entity id. Make sure the Entity component is above all EntityBehaviours.");
			} else {
				_map[Id] = (T)this;
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (Id != EntityId.None) {
				_map.Remove(Id);
			}
		}
	}
}
