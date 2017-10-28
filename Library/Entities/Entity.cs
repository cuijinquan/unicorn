
using System;
using System.Collections.Generic;
using Unicorn.Util;
using UnityEngine;

namespace Unicorn.Entities {
	public sealed class Entity : MonoBehaviour {
		[ThreadStatic]
		private static EntityId _nextId;
		[ThreadStatic]
		private static string _nextSource;

		private EntityId _id = EntityId.None;
		public EntityId Id { get { return _id; } }

		private string _source = null;
		public string Source { get { return _source; } }

		private SetProxy<Connection> _group;
		public IReadonlyObservableSet<Connection> Group {
			get {
				if (_group == null)
					throw new InvalidOperationException("A global entity system is required to use entity groups.");
				return _group;
			}
			set {
				if (_group == null)
					throw new InvalidOperationException("A global entity system is required to use entity groups.");
				_group.Target = value;
			}
		}

		private BacklogSubSet<Connection> _owners;
		public Set<Connection> Owners {
			get {
				if (_group == null)
					throw new InvalidOperationException("A global entity system is required to use entity owner management.");
				return _owners;
			}
		}

		private bool _isMine;
		public bool IsMine {
			get { return _isMine; }
			set { _isMine = value; }
		}

		private Disposable _disposable;



		private void Awake() {
			if (_nextId == EntityId.None || _nextSource == null) {
				throw new InvalidOperationException("Use Entity.Create to instantiate entities.");
			} else {
				_disposable = new Disposable();

				_source = _nextSource;
				_map[_id = _nextId] = this;

				var system = EntitySystem.Get();
				if (system != null) {
					_group = new SetProxy<Connection>();
					_disposable.Add(_group.Added(conn => system.GroupConnected(this, conn), false));
					_disposable.Add(_group.Removed(conn => system.GroupDisconnected(this, conn), false));

					_owners = new BacklogSubSet<Connection>(system.Owners, Connection.IsDead);
					_disposable.Add(_owners.Added(conn => system.OwnershipChanged(this, conn, true), false));
					_disposable.Add(_owners.Removed(conn => system.OwnershipChanged(this, conn, false), false));
				}
			}
		}

		private void OnDestroy() {
			if (_group != null)
				_group.Target = null;
			if (_disposable != null)
				_disposable.Dispose();
			if (_id != EntityId.None)
				_map.Remove(_id);
		}



		private static readonly SortedDictionary<EntityId, Entity> _map = new SortedDictionary<EntityId, Entity>();

		public static IEnumerable<Entity> All { get { return _map.Values; } }



		public static Entity Create(string source) {
			return Create(source, Vector3.zero, Quaternion.identity);
		}

		public static Entity Create(string source, Vector3 position, Quaternion rotation) {
			return Create(EntityId.Allocate(), source, position, rotation);
		}

		public static Entity Create(EntityId id, string source, Vector3 position, Quaternion rotation) {
			var res = Resources.Load<GameObject>(source);
			if (!res || !res.GetComponent<Entity>())
				throw new MissingReferenceException(string.Format("Missing resource or it's entity component: {0}", source));

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

		public static void DestroyAll() {
			foreach (var entity in All)
				Destroy(entity.gameObject);
		}
	}
}
