
using System;
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class SetProxyTests {

		[Test]
		public void BasicUsage() {
			var a = new Set<int>();
			a.Add(0);
			a.Add(1);

			var b = new Set<int>();
			b.Add(1);
			b.Add(2);
			b.Add(3);

			var proxy = new SetProxy<int>();
			Assert.AreEqual(0, proxy.Count);

			proxy.Target = a;
			Assert.AreEqual(2, proxy.Count);
			Assert.True(proxy.Contains(0));
			Assert.True(proxy.Contains(1));

			proxy.Target = b;
			Assert.AreEqual(3, proxy.Count);
			Assert.False(proxy.Contains(0));
			Assert.True(proxy.Contains(1));
			Assert.True(proxy.Contains(2));

			proxy.Target = null;
			Assert.AreEqual(0, proxy.Count);
			Assert.False(proxy.Contains(0));
			Assert.False(proxy.Contains(1));
			Assert.False(proxy.Contains(2));
		}

		[Test]
		public void ObserveAdded() {
			var a = new Set<int>();
			a.Add(1);
			a.Add(2);

			var b = new Set<int>();
			b.Add(4);
			b.Add(8);

			var proxy = new SetProxy<int>();
			var sum = 0;
			var observers = new Disposable()
				.Add(proxy.Added(item => sum += item, false))
				.Add(proxy.Removed(item => sum -= item, false));

			proxy.Target = a;
			Assert.AreEqual(3, sum);
			a.Remove(1);
			Assert.AreEqual(2, sum);

			proxy.Target = b;
			Assert.AreEqual(12, sum);
			b.Add(1);
			Assert.AreEqual(13, sum);

			proxy.Target = null;
			Assert.AreEqual(0, sum);

			b.Remove(8);
			proxy.Target = b;
			Assert.AreEqual(5, sum);
			observers.Dispose();
			b.Add(7);
			proxy.Target = null;
			Assert.AreEqual(5, sum);
		}
	}
}
