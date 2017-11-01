
using System;
using System.Collections.Generic;
using Unicorn.Entities.Internal;
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine;

namespace Unicorn.Entities {
	[DisallowMultipleComponent]
	public sealed class Entity : MonoBehaviour, IEntityInternal {
		[ThreadStatic]
		private static EntityId _nextId = EntityId.None;
		[ThreadStatic]
		private static string _nextSource = null;



		private Disposable _untilDestroy;

		private EntityId _id = EntityId.None;
		public EntityId Id { get { return _id; } }

		private string _source = null;
		public string Source { get { return _source; } }

		private bool _isMine = false;
		public bool IsMine { get { return _isMine; } }

		private SetProxy<Connection> _group;
		public IReadonlyObservableSet<Connection> Group {
			get {
				if (_group == null)
					throw new InvalidOperationException();
				return _group;
			}
			set {
				if (_group == null)
					throw new InvalidOperationException();
				_group.Target = value;
			}
		}

		private BacklogSubSet<Connection> _ownerSet;
		public Set<Connection> OwnerSet { get { return _ownerSet; } }
		
		private SetProxy<Connection> _owners;
		public IReadonlyObservableSet<Connection> Owners {
			get {
				if (_owners == null)
					throw new InvalidOperationException();
				return _owners;
			}
			set {
				if (_owners == null)
					throw new InvalidOperationException();
				_owners.Target = value;
			}
		}



		[ThreadStatic]
		private static SortedDictionary<EntityId, Entity> _map = new SortedDictionary<EntityId, Entity>();

		public static int Count { get { return _map.Count; } }
		public static IEnumerable<Entity> All { get { return _map.Values; } }
		


		private void Awake() {
			if (_nextId == EntityId.None) {
				Debug.LogError("Use Entity.Create for creating entities.");
			} else {
				_untilDestroy = new Disposable();
				_id = _nextId;
				_source = _nextSource;

				var router = EntityRouter.Current;
				if (router == null) {
					Debug.LogError("Missing active GameRouter instance.");
				} else if (router.IsServer) {
					_group = new SetProxy<Connection>();
					_group.Added(_untilDestroy, conn => conn.Send(payload => {
						EntityId.None.WriteTo(payload);
						payload.Write((byte)EntityControlCode.CreateEntity);
						_id.WriteTo(payload);
						payload.Write(_source);
						payload.Write(transform.position);
						payload.Write(transform.rotation);
					}));
					_group.Removed(_untilDestroy, conn => conn.Send(payload => {
						EntityId.None.WriteTo(payload);
						payload.Write((byte)EntityControlCode.DestroyEntity);
						_id.WriteTo(payload);
					}));

					_ownerSet = new BacklogSubSet<Connection>(_group, Connection.IsDead);
					_owners = new SetProxy<Connection>(_ownerSet);
					_owners.Added(_untilDestroy, conn => conn.Send(payload => {
						EntityId.None.WriteTo(payload);
						payload.Write((byte)EntityControlCode.SetEntityOwnership);
						_id.WriteTo(payload);
						payload.Write(true);
					}));
					_owners.Removed(_untilDestroy, conn => conn.Send(payload => {
						EntityId.None.WriteTo(payload);
						payload.Write((byte)EntityControlCode.SetEntityOwnership);
						_id.WriteTo(payload);
						payload.Write(false);
					}));
				}

				_map[_id] = this;
			}
		}

		private void OnDestroy() {
			if (_group != null)
				_group.Target = null;
			if (_untilDestroy != null)
				_untilDestroy.Dispose();
			if (_id != EntityId.None)
				_map.Remove(_id);
		}



		public static Entity Create(string source) {
			return Entity.Create(source, Vector3.zero, Quaternion.identity);
		}

		public static Entity Create(string source, Vector3 position, Quaternion rotation) {
			return Create(EntityId.Allocate(), source, position, rotation);
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
				throw new MissingComponentException(string.Format("Resource needs to be an entity: {0}", source));

			try {
				_nextId = id;
				_nextSource = source;
				return Instantiate(res, position, rotation).GetComponent<Entity>();
			} finally {
				_nextId = EntityId.None;
				_nextSource = null;
			}
		}

		public static bool Use(EntityId id, Action<Entity> action) {
			Entity entity;
			if (_map.TryGetValue(id, out entity)) {
				action(entity);
				return true;
			} else {
				return false;
			}
		}

		public static bool Destroy(EntityId id) {
			return Use(id, entity => Destroy(entity.gameObject));
		}



		void IEntityInternal.SetOwnership(bool isMine) {
			_isMine = isMine;
		}

		void IEntityInternal.Receive(Connection sender, DataReader payload) {
		}
	}
}
