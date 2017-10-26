
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class BacklogSubSetTests {

		[Test]
		public void Add() {
			var super = new Set<int>();
			super.Add(0);
			
			var sub = new BacklogSubSet<int>(super);
			Assert.True(sub.Add(0));
			Assert.True(sub.Contains(0));
			Assert.False(sub.Add(1));
			Assert.False(sub.Contains(1));
			Assert.AreEqual(1, sub.Count);

			super.Add(1);
			Assert.True(sub.Contains(1));
			Assert.AreEqual(2, sub.Count);
		}

		[Test]
		public void Remove() {
			var super = new Set<int>();
			super.Add(0);
			super.Add(1);

			var sub = new BacklogSubSet<int>(super);
			sub.Add(0);
			sub.Add(1);

			Assert.True(sub.Remove(0));
			Assert.False(sub.Contains(0));
			Assert.AreEqual(1, sub.Count);

			super.Remove(1);
			Assert.False(sub.Contains(1));
			Assert.False(sub.Remove(1));
			Assert.AreEqual(0, sub.Count);
		}
	}
}
