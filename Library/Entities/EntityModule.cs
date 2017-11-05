
using System;
using System.Collections.Generic;
using Unicorn.Entities.Internal;
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine;

namespace Unicorn.Entities {
	public abstract class EntityModule<T> : EntityModule where T : EntityModule<T> {
		private static SortedDictionary<EntityId, T> _map = new SortedDictionary<EntityId, T>();
		public static IEnumerable<T> All { get { return _map.Values; } }
		public static int Count { get { return _map.Count; } }

		public static bool Use(EntityId id, Action<T> action) {
			T module;
			if (_map.TryGetValue(id, out module)) {
				action(module);
				return true;
			} else {
				return false;
			}
		}

		public static T Find(EntityId id) {
			T module;
			return _map.TryGetValue(id, out module) ? module : null;
		}

		protected override void Awake() {
			base.Awake();
			_map[Id] = (T)this;
		}

		protected override void OnDestroy() {
			_map.Remove(Id);
			base.OnDestroy();
		}
	}

	[RequireComponent(typeof(Entity))]
	public abstract class EntityModule : MonoBehaviour, IEntityModuleInternal {
		private byte? _moduleId;

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



		public void Send(Action<DataWriter> message) {
			Send(0, message);
		}

		public void Send(int channelKey, Action<DataWriter> message) {
			Send(EntityRouter.Require().Connections, message);
		}

		public void Send(IEnumerable<Connection> target, Action<DataWriter> message) {
			Send(target, 0, message);
		}

		public void Send(IEnumerable<Connection> target, int channelKey, Action<DataWriter> message) {
			if (_moduleId == null)
				throw new InvalidOperationException("Unassigned module id.");

			target.Send(channelKey, payload => {
				payload.Write(Id);
				payload.Write((byte)_moduleId);
				message(payload);
			});
		}

		protected virtual void Awake() {
			_moduleId = ((IEntityInternal)Entity).AddModule(this);
		}

		protected virtual void OnDestroy() {
			if (_moduleId != null)
				((IEntityInternal)Entity).RemoveModule((byte)_moduleId);
		}

		protected abstract void Receive(Connection sender, DataReader payload);

		void IEntityModuleInternal.Receive(Connection sender, DataReader payload) {
			Receive(sender, payload);
		}
	}
}
