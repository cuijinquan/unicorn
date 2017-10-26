
using System.Collections.Generic;

namespace Unicorn.Util {
	/// <summary>
	/// A readonly collection of unique items.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IReadonlySet<T> : IEnumerable<T> {
		bool Contains(T item);
		int Count { get; }
	}
}
