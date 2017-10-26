
using System;

namespace Unicorn.Util {
	/// <summary>
	/// A readonly collection of unique items that is observable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IReadonlyObservableSet<T> : IReadonlySet<T> {
		IDisposable Added(Action<T> action, bool weak);
		IDisposable Removed(Action<T> action, bool weak);
	}
}
