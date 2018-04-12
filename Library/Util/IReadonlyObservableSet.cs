
using System;

namespace Unicorn.Util {
	/// <summary>
	/// A readonly collection of unique items that is observable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IReadonlyObservableSet<T> : IReadonlySet<T> {
		IDisposable Added(Action<T> action);
		IDisposable AddedWeak(Action<T> action);
		IDisposable Removed(Action<T> action);
		IDisposable RemovedWeak(Action<T> action);
	}
}
