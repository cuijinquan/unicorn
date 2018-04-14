
using System;

namespace Unicorn.Util {
	/// <summary>
	/// This class provides simple Unicorn.Util extension methods.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Add an added event handler and remove it, when the disposable is disposed.
		/// </summary>
		/// <param name="set"></param>
		/// <param name="dispose"></param>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		public static void Added<T>(this IReadonlyObservableSet<T> set, Disposable dispose, Action<T> action) {
			dispose.Add(set.Added(action));
		}

		/// <summary>
		/// Add an removed event handler and remove it, when the disposable is disposed.
		/// </summary>
		/// <param name="set"></param>
		/// <param name="dispose"></param>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		public static void Removed<T>(this IReadonlyObservableSet<T> set, Disposable dispose, Action<T> action) {
			dispose.Add(set.Removed(action));
		}
	}
}
