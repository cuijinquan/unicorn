
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Util {
	/// <summary>
	/// A collection of unique items that is observable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Set<T> : IObservableSet<T> {
		public Set() {
			_items = new HashSet<T>();
			_added = new ObserverSet<Action<T>>();
			_removed = new ObserverSet<Action<T>>();
		}
		
		private readonly HashSet<T> _items;
		private readonly ObserverSet<Action<T>> _added;
		private readonly ObserverSet<Action<T>> _removed;
		
		public int Count { get { return _items.Count; } }

		public virtual bool Contains(T item) {
			return _items.Contains(item);
		}

		public virtual bool Add(T item) {
			if (_items.Add(item)) {
				_added.Use(a => a(item));
				return true;
			} else {
				return false;
			}
		}

		public virtual bool Remove(T item) {
			if (_items.Remove(item)) {
				_removed.Use(r => r(item));
				return true;
			} else {
				return false;
			}
		}

		public virtual int RemoveWhere(Predicate<T> match) {
			return _items.RemoveWhere(item => {
				if (match(item)) {
					_removed.Use(r => r(item));
					return true;
				}
				return false;
			});
		}
		
		public virtual void Clear() {
			_items.RemoveWhere(item => {
				_removed.Use(r => r(item));
				return true;
			});
		}

		public IDisposable Added(Action<T> action) {
			return _added.Add(action);
		}

		public IDisposable AddedWeak(Action<T> action) {
			return _added.AddWeak(action);
		}

		public IDisposable Removed(Action<T> action) {
			return _removed.Add(action);
		}

		public IDisposable RemovedWeak(Action<T> action) {
			return _removed.AddWeak(action);
		}

		public IEnumerator<T> GetEnumerator() {
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _items.GetEnumerator();
		}
	}
}
