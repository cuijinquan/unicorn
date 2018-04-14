
using System;
using System.Collections.Generic;
using System.Collections;

namespace Unicorn.Util {
	/// <summary>
	/// A set that represents the intersection of two sets.
	/// </summary>
	public class IntersectionSet<T> : IReadonlyObservableSet<T> {
		/// <summary>
		/// Create a new intersection set.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public IntersectionSet(IReadonlyObservableSet<T> a, IReadonlyObservableSet<T> b) {
			_a = a;
			_b = b;
			_added = new ObserverSet<Action<T>>();
			_removed = new ObserverSet<Action<T>>();
			_count = 0;
			foreach (var item in a) {
				if (b.Contains(item)) {
					_count++;
				}
			}

			_aAddedObserver = a.AddedWeak(AddedToA);
			_aRemovedObserver = a.RemovedWeak(RemovedFromA);
			_bAddedObserver = b.AddedWeak(AddedToB);
			_bRemovedObserver = b.RemovedWeak(RemovedFromB);
		}

		private readonly IReadonlyObservableSet<T> _a;
		private readonly IReadonlyObservableSet<T> _b;
		private readonly ObserverSet<Action<T>> _added;
		private readonly ObserverSet<Action<T>> _removed;

		// TODO: Increase when added.
		// TODO: Decrease when removed.
		private int _count;

		#pragma warning disable 0414
		private IDisposable _aAddedObserver;
		private IDisposable _aRemovedObserver;
		private IDisposable _bAddedObserver;
		private IDisposable _bRemovedObserver;
		#pragma warning restore 0414

		private void AddedToA(T item) {
			if (_b.Contains(item)) {
				_count++;
				_added.Use(a => a(item));
			}
		}

		private void RemovedFromA(T item) {
			if (_b.Contains(item)) {
				_count--;
				_removed.Use(a => a(item));
			}
		}

		private void AddedToB(T item) {
			if (_a.Contains(item)) {
				_count++;
				_added.Use(a => a(item));
			}
		}

		private void RemovedFromB(T item) {
			if (_a.Contains(item)) {
				_count--;
				_removed.Use(a => a(item));
			}
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

		public bool Contains(T item) {
			return _a.Contains(item) && _b.Contains(item);
		}

		public int Count { get { return _count; } }

		public IEnumerator<T> GetEnumerator() {
			foreach (var item in _a) {
				if (_b.Contains(item)) {
					yield return item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
