
using System;

namespace Unicorn.Util {
	/// <summary>
	/// A disposable that does nothing.
	/// </summary>
	public class FakeDisposable : IDisposable {
		void IDisposable.Dispose() {
		}
	}
}
