
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Util {
	/// <summary>
	/// An observable proxy to a set.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SetProxy<T> : IReadonlyObservableSet<T> {
		public SetProxy() {
			_added = new ObserverSet<Action<T>>();
			_removed = new ObserverSet<Action<T>>();
			_target = null;
			_targetObserver = new Disposable();
		}

		public SetProxy(IReadonlyObservableSet<T> target) : this() {
			Target = target;
		}

		private readonly ObserverSet<Action<T>> _added;
		private readonly ObserverSet<Action<T>> _removed;
		private IReadonlyObservableSet<T> _target;
		private Disposable _targetObserver;



		/// <summary>
		/// Get or set the target set.
		/// </summary>
		public IReadonlyObservableSet<T> Target {
			get { return _target; }
			set {
				if (_target != value) {
					var addedItems = value == null ? new HashSet<T>() : new HashSet<T>(value);
					if (_target != null)
						addedItems.ExceptWith(_target);

					var removedItems = _target == null ? new HashSet<T>() : new HashSet<T>(_target);
					if (value != null)
						removedItems.ExceptWith(value);

					_targetObserver.Dispose();
					if ((_target = value) != null)
						_targetObserver
							.Add(_target.Added(item => _added.Use(a => a(item)), false))
							.Add(_target.Removed(item => _removed.Use(r => r(item)), false));

					foreach (var item in removedItems)
						_removed.Use(r => r(item));
					foreach (var item in addedItems)
						_added.Use(a => a(item));
				}
			}
		}



		public int Count { get { return _target == null ? 0 : _target.Count; } }

		public bool Contains(T item) {
			return _target != null && _target.Contains(item);
		}

		public IDisposable Added(Action<T> action, bool weak) {
			return _added.Add(action, weak);
		}

		public IDisposable Removed(Action<T> action, bool weak) {
			return _removed.Add(action, weak);
		}

		public IEnumerator<T> GetEnumerator() {
			return _target == null ? EmptyEnumerator() : _target.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _target == null ? EmptyEnumerator() : _target.GetEnumerator();
		}

		private static IEnumerator<T> EmptyEnumerator() { yield break; }
	}
}
