
namespace Unicorn.Entities {
	using System;
	using System.Collections.Generic;
	using Unicorn.Entities.Internal;
	using Unicorn.IO;
	using Unicorn.Util;
	using UnityEngine;

	[DisallowMultipleComponent]
	public sealed class Entity : MonoBehaviour, IEntityInternal {
		[ThreadStatic]
		private static EntityId _nextId;
		[ThreadStatic]
		private static string _nextSource;

		private Disposable _untilDestroy;
		public Disposable UntilDestroy { get { return _untilDestroy; } }

		private SortedDictionary<byte, IEntityModuleInternal> _modules;
		private byte _nextModuleId;

		private EntityId _id;
		public EntityId Id { get { return _id; } }

		private string _source;
		public string Source { get { return _source; } }

		private SetProxy<Connection> _group;
		public IReadonlyObservableSet<Connection> Group {
			get {
				if (_group == null)
					throw new InvalidOperationException("Not supported on clients.");
				return _group;
			}
			set {
				if (_group == null)
					throw new InvalidOperationException("Not supported on clients.");
				_group.Target = value;
			}
		}

		private BacklogSubSet<Connection> _ownerSet;
		public Set<Connection> OwnerSet {
			get {
				if (_ownerSet == null)
					throw new InvalidOperationException("Not supported on clients.");
				if (_group.Target != _ownerSet)
					Debug.LogWarningFormat("An entity's owner set is used, but it's not used: {0}", gameObject.name);
				return _ownerSet;
			}
		}

		private SetProxy<Connection> _owners;
		public IReadonlyObservableSet<Connection> Owners {
			get {
				if (_owners == null)
					throw new InvalidOperationException("Not supported on clients.");
				return _owners;
			}
			set {
				if (_owners == null)
					throw new InvalidOperationException("Not supported on clients.");
				_owners.Target = value;
			}
		}

		private bool _isMine;
		public bool IsMine {
			get { return _isMine; }
		}



		public T As<T>() where T : Component {
			var component = GetComponent<T>();
			if (!component)
				throw new MissingComponentException(string.Format("Missing {0} component on entity: {1}", typeof(T).Name, gameObject.name));
			return component;
		}

		public void SetOwner(Connection owner) {
			OwnerSet.Clear();
			OwnerSet.Add(owner);
			Owners = OwnerSet;
		}



		private static SortedDictionary<EntityId, Entity> _map = new SortedDictionary<EntityId, Entity>();
		public static IEnumerable<Entity> All { get { return _map.Values; } }
		public static int Count { get { return _map.Count; } }

		public static bool Use(EntityId id, Action<Entity> action) {
			Entity entity;
			if (_map.TryGetValue(id, out entity)) {
				action(entity);
				return true;
			} else {
				return false;
			}
		}

		public static Entity Find(EntityId id) {
			Entity entity;
			return _map.TryGetValue(id, out entity) ? entity : null;
		}



		private void Awake() {
			if (_nextId == EntityId.None) {
				Debug.LogWarning("Entity created without Entity.Create(..)");
			} else {
				_untilDestroy = new Disposable();
				_modules = new SortedDictionary<byte, IEntityModuleInternal>();
				_id = _nextId;
				_source = _nextSource;
				_map[_id] = this;

				if (EntityRouter.Require().IsServer) {
					_group = new SetProxy<Connection>();
					_group.Added(UntilDestroy, conn => {
						conn.Send(p => {
							p.Write(EntityId.None);
							p.Write((byte)EntityRouter.ClientControlCode.CreateEntity);
							p.Write(_id);
							p.Write(_source);
							p.Write(transform.position);
							p.Write(transform.rotation);
						});
					});
					_group.Removed(UntilDestroy, conn => {
						conn.Send(p => {
							p.Write(EntityId.None);
							p.Write((byte)EntityRouter.ClientControlCode.DestroyEntity);
							p.Write(_id);
						});
					});

					_ownerSet = new BacklogSubSet<Connection>(_group, Connection.IsDead);

					_owners = new SetProxy<Connection>(_ownerSet);
					_owners.Added(UntilDestroy, conn => {
						conn.Send(p => {
							p.Write(EntityId.None);
							p.Write((byte)EntityRouter.ClientControlCode.SetEntityOwnership);
							p.Write(_id);
							p.Write(true);
						});
					});
					_owners.Removed(UntilDestroy, conn => {
						conn.Send(p => {
							p.Write(EntityId.None);
							p.Write((byte)EntityRouter.ClientControlCode.SetEntityOwnership);
							p.Write(_id);
							p.Write(false);
						});
					});
				} else {
					_isMine = false;
				}
			}
		}

		private void OnDestroy() {
			if (_group != null) {
				_group.Target = null;
			}
			if (_id != EntityId.None) {
				_map.Remove(_id);
			}
			_untilDestroy.Dispose();
		}



		public static Entity Create(string source) {
			return Create(source, Vector3.zero, Quaternion.identity);
		}

		public static Entity Create(string source, Vector3 position, Quaternion rotation) {
			return Create(EntityIdPool.Allocate(), source, position, rotation);
		}

		public static Entity Create(EntityId id, string source, Vector3 position, Quaternion rotation) {
			if (id == EntityId.None)
				throw new ArgumentOutOfRangeException("id");
			if (source == null)
				throw new ArgumentNullException("source");

			var res = Resources.Load<GameObject>(source);
			if (!res)
				throw new MissingReferenceException(string.Format("Missing resource: {0}", source));
			if (!res.GetComponent<Entity>())
				throw new MissingComponentException(string.Format("Resource is missing Entity component: {0}", source));

			try {
				_nextId = id;
				_nextSource = source;
				return Instantiate(res, position, rotation).GetComponent<Entity>();
			} finally {
				_nextId = EntityId.None;
				_nextSource = null;
			}
		}

		public static bool Destroy(EntityId id) {
			return Use(id, entity => Destroy(entity.gameObject));
		}

		void IEntityInternal.Receive(Connection sender, DataReader payload) {
			var moduleId = payload.ReadByte();
			IEntityModuleInternal module;
			if (_modules.TryGetValue(moduleId, out module)) {
				module.Receive(sender, payload);
			} else {
				Debug.LogWarningFormat("Missing entity module {0} on entity: {1}", moduleId, gameObject.name);
			}
		}

		void IEntityInternal.SetLocalOwnership(bool isMine) {
			_isMine = isMine;
		}

		byte IEntityInternal.AddModule(IEntityModuleInternal module) {
			var id = _nextModuleId;
			checked { _nextModuleId++; }
			_modules[id] = module;
			return id;
		}

		void IEntityInternal.RemoveModule(byte moduleId) {
			_modules.Remove(moduleId);
		}
	}
}

namespace Unicorn.IO {
	using System;
	using Unicorn.Entities;

	partial class DataWriter {
		public void Write(Entity entity) {
			if (entity) {
				Write(true);
				Write(entity.Id);
			} else {
				Write(false);
			}
		}
	}

	partial class DataReader {
		public Entity ReadEntity() {
			if (ReadBoolean()) {
				return Entity.Find(ReadEntityId());
			} else {
				return null;
			}
		}

		public bool ReadEntity(Action<Entity> action) {
			return ReadBoolean() && Entity.Use(ReadEntityId(), action);
		}
	}
}
