
using System;

namespace Unicorn.Util {
	public class SubSet<T> : Set<T> {
		public SubSet(IReadonlyObservableSet<T> super) {
			_super = super;
			_superObserver = super.Removed(item => Remove(item), true);
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
