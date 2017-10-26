
namespace Unicorn.Util {
	/// <summary>
	/// A collection of unique items that is observable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IObservableSet<T> : ISet<T>, IReadonlyObservableSet<T> { }
}
