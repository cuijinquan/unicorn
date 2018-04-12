
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class SetTests {
		
		[Test]
		public void BasicUsage() {
			var set = new Set<int>();
			Assert.True(set.Add(0));
			Assert.True(set.Contains(0));
			Assert.True(set.Add(1));
			Assert.False(set.Add(1));
			Assert.True(set.Contains(1));
			Assert.AreEqual(2, set.Count);

			Assert.True(set.Remove(0));
			Assert.False(set.Contains(0));
			Assert.True(set.Remove(1));
			Assert.False(set.Remove(1));
			Assert.False(set.Contains(1));
			Assert.AreEqual(0, set.Count);

			set.Add(2);
			set.Add(3);
			Assert.AreEqual(2, set.Count);
			set.Clear();
			Assert.AreEqual(0, set.Count);

			set.Add(5);
			set.Add(3);
			Assert.AreEqual(1, set.RemoveWhere(n => n < 4));
			Assert.AreEqual(1, set.Count);
			Assert.True(set.Contains(5));
		}

		[Test]
		public void ObserveAdded() {
			var added = 0;
			var set = new Set<int>();
			set.Add(0);
			var observer = set.Added(item => added++);
			set.Add(1);
			Assert.AreEqual(1, added);
			observer.Dispose();
			set.Add(2);
			Assert.AreEqual(1, added);
		}

		[Test]
		public void ObserveRemoved() {
			var removed = 0;
			var set = new Set<int>();
			set.Add(0);
			set.Add(1);
			var observer = set.Removed(item => removed++);
			set.Remove(0);
			Assert.AreEqual(1, removed);

			observer.Dispose();
			set.Remove(1);
			Assert.AreEqual(1, removed);

			removed = 0;
			set.Add(5);
			set.Add(3);
			observer = set.Removed(item => removed++);
			Assert.AreEqual(1, set.RemoveWhere(n => n < 4));
			Assert.AreEqual(1, removed);
		}

		[Test]
		public void ObserveClear() {
			var removed = 0;
			var set = new Set<int>();
			set.Add(0);
			set.Add(1);
			var observer = set.Removed(item => removed++);
			set.Clear();
			Assert.AreEqual(2, removed);

			observer.Dispose();
			set.Add(0);
			set.Clear();
			Assert.AreEqual(2, removed);
		}
	}
}
