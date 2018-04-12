
using NUnit.Framework;
using Unicorn.Util;

namespace Unicorn.Tests.Util {
	public class IntersectionSetTests {

		[Test]
		public void PreAdd() {
			var a = new Set<int>();
			a.Add(0);
			a.Add(1);

			var b = new Set<int>();
			b.Add(1);
			b.Add(2);

			var i = new IntersectionSet<int>(a, b);
			Assert.False(i.Contains(0));
			Assert.True(i.Contains(1));
			Assert.False(i.Contains(2));
			Assert.AreEqual(1, i.Count);
		}

		[Test]
		public void Add() {
			var a = new Set<int>();
			a.Add(0);
			a.Add(1);

			var b = new Set<int>();
			b.Add(2);

			var i = new IntersectionSet<int>(a, b);
			Assert.False(i.Contains(0));
			Assert.False(i.Contains(1));
			Assert.False(i.Contains(2));
			Assert.AreEqual(0, i.Count);

			b.Add(1);
			Assert.True(i.Contains(1));
			Assert.AreEqual(1, i.Count);
		}

		[Test]
		public void Remove() {
			var a = new Set<int>();
			a.Add(0);
			a.Add(1);

			var b = new Set<int>();
			b.Add(0);
			b.Add(1);

			var i = new IntersectionSet<int>(a, b);
			Assert.AreEqual(2, i.Count);

			a.Remove(0);
			Assert.False(i.Contains(0));
			Assert.AreEqual(1, i.Count);

			b.Remove(1);
			Assert.False(i.Contains(1));
			Assert.AreEqual(0, i.Count);
		}
	}
}
