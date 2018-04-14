
using System;
using System.Collections.Generic;
using System.Collections;

namespace Unicorn.Util {
	/// <summary>
	/// A set that contains a fixed single item.
	/// </summary>
	public class UnitSet<T> : IReadonlyObservableSet<T> {
		/// <summary>
		/// Create a new unit set with the specified item.
		/// </summary>
		/// <param name="item"></param>
		public UnitSet(T item) {
			_item = item;
		}

		private readonly T _item;

		public bool Contains(T item) {
			return _item.Equals(item);
		}

		public int Count { get { return 1; } }

		public IDisposable Added(Action<T> action) {
			return new FakeDisposable();
		}

		public IDisposable AddedWeak(Action<T> action) {
			return new FakeDisposable();
		}

		public IDisposable Removed(Action<T> action) {
			return new FakeDisposable();
		}

		public IDisposable RemovedWeak(Action<T> action) {
			return new FakeDisposable();
		}

		public IEnumerator<T> GetEnumerator() {
			yield return _item;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
