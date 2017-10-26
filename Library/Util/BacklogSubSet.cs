
using System;
using System.Collections.Generic;

namespace Unicorn.Util {
	public class BacklogSubSet<T> : Set<T> {
		public BacklogSubSet(IReadonlyObservableSet<T> super) : this(super, null) { }
		public BacklogSubSet(IReadonlyObservableSet<T> super, Predicate<T> isDead) {
			if (super == null)
				throw new ArgumentNullException("super");

			_backlog = new HashSet<T>();
			_super = super;
			_superObserver = new Disposable()
				.Add(super.Added(item => {
					if (_backlog.Contains(item))
						base.Add(item);
				}, true))
				.Add(super.Removed(item => {
					if (isDead != null && isDead(item)) {
						Remove(item);
					} else {
						base.Remove(item);
					}
				}, true));
		}

		private readonly HashSet<T> _backlog;
		private readonly IReadonlyObservableSet<T> _super;

#pragma warning disable 0414
		private readonly IDisposable _superObserver;
#pragma warning restore 0414

		public override bool Add(T item) {
			return _backlog.Add(item) && _super.Contains(item) && base.Add(item);
		}

		public override bool Remove(T item) {
			return _backlog.Remove(item) && base.Remove(item);
		}
	}
}
