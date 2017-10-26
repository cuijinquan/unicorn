
using Unicorn.Util;
using NUnit.Framework;

namespace Unicorn.Tests.Util {
	public class DisposableTests {

		[Test]
		public void EmptyDisposable() {
			new Disposable().Dispose();
		}

		[Test]
		public void DisposalAction() {
			var disposedFromCtor = false;
			var disposed = false;
			new Disposable(() => disposedFromCtor = true)
				.Add(() => disposed = true)
				.Dispose();

			Assert.True(disposedFromCtor);
			Assert.True(disposed);
		}

		[Test]
		public void NestedDisposable() {
			var disposed = false;
			var nested = new Disposable(() => disposed = true);
			new Disposable().Add(nested).Dispose();
			Assert.True(disposed);
		}

		[Test]
		public void DisposeCount() {
			var count = 0;
			var disposable = new Disposable(() => count++);
			disposable.Dispose();
			disposable.Dispose();
			Assert.AreEqual(1, count);
		}
	}
}
