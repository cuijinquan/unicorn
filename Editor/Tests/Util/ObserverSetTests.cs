
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class ObserverSetTests {

		[Test]
		public void BasicUsage() {
			var observers = new ObserverSet<DummyObserver>();
			var dummy = new DummyObserver();
			var weakDummy = new DummyObserver();
			observers.Add(dummy, false);
			observers.Add(weakDummy, true);
			observers.Use(d => d.Used = true);
			Assert.True(dummy.Used);
			Assert.True(weakDummy.Used);
		}

		[Test]
		public void UsageCount() {
			var observers = new ObserverSet<DummyObserver>();
			var dummyA = new DummyObserver();
			var dummyB = new DummyObserver();
			observers.Add(dummyA, false);
			observers.Use(d => d.UsedCount++);
			Assert.AreEqual(1, dummyA.UsedCount);

			observers.Add(dummyB, false);
			observers.Use(d => d.UsedCount++);
			Assert.AreEqual(2, dummyA.UsedCount);
			Assert.AreEqual(1, dummyB.UsedCount);

			observers.Remove(dummyA);
			observers.Use(d => d.UsedCount++);
			Assert.AreEqual(2, dummyA.UsedCount);
			Assert.AreEqual(2, dummyB.UsedCount);
		}

		public class DummyObserver {
			public bool Used = false;
			public int UsedCount = 0;
		}
	}
}
