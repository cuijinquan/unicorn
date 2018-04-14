
using System;

namespace Unicorn.Util {
	/// <summary>
	/// A set that ensures that it only contains items that are in it's super set.
	/// </summary>
	public class SubSet<T> : Set<T> {
		/// <summary>
		/// Create a new sub set with the specified super set.
		/// </summary>
		/// <param name="super"></param>
		public SubSet(IReadonlyObservableSet<T> super) {
			_super = super;
			_superObserver = super.RemovedWeak(item => Remove(item));
		}

		private readonly IReadonlyObservableSet<T> _super;

		#pragma warning disable 0414
		private readonly IDisposable _superObserver;
		#pragma warning restore 0414

		public override bool Add(T item) {
			return _super.Contains(item) && base.Add(item);
		}
	}
}
