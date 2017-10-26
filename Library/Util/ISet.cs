
namespace Unicorn.Util {
	/// <summary>
	/// A collection of unique items.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISet<T> : IReadonlySet<T> {
		bool Add(T item);
		bool Remove(T item);
		void Clear();
	}
}
