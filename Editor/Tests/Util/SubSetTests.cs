
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class SubSetTests {
		
		[Test]
		public void Add() {
			var super = new Set<int>();
			var sub = new SubSet<int>(super);

			Assert.False(sub.Add(0));
			Assert.False(sub.Contains(0));

			super.Add(0);
			Assert.False(sub.Contains(0));
			Assert.True(sub.Add(0));
			Assert.True(sub.Contains(0));
		}

		[Test]
		public void Remove() {
			var super = new Set<int>();
			var sub = new SubSet<int>(super);

			super.Add(0);
			sub.Add(0);
			Assert.True(super.Remove(0));
			Assert.False(sub.Contains(0));
			Assert.False(sub.Remove(0));

			super.Add(3);
			super.Add(5);
			sub.Add(3);
			sub.Add(5);
			super.RemoveWhere(n => n < 4);
			Assert.False(sub.Contains(3));
			Assert.True(sub.Contains(5));
		}
	}
}
