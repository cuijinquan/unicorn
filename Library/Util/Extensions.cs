
using System;

namespace Unicorn.Util {
	public static class Extensions {
		public static void Added<T>(this IReadonlyObservableSet<T> set, Disposable dispose, Action<T> action) {
			dispose.Add(set.Added(action));
		}

		public static void Removed<T>(this IReadonlyObservableSet<T> set, Disposable dispose, Action<T> action) {
			dispose.Add(set.Removed(action));
		}
	}
}
