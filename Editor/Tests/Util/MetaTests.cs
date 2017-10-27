
using NUnit.Framework;
using Unicorn.Util;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unicorn.Tests.Util {
	public class MetaTests {

		[Test]
		public void Set() {
			var meta = new Meta();
			meta.Set(new DummyObject());
			Assert.True(meta.Contains<DummyObject>());
		}

		[Test]
		public void Remove() {
			var meta = new Meta();
			meta.Set(new DummyObject());
			meta.Remove<DummyObject>();
			Assert.False(meta.Contains<DummyObject>());
		}

		[Test]
		public void UseWithFunction() {
			var meta = new Meta();
			meta.Set(new DummyObject() { Value = 42 });
			Assert.True(meta.Use<DummyObject, bool>(o => o.Value > 7));
			Assert.False(meta.Use<DummyUnityObject, bool>(o => true));
		}

		[Test]
		public void UseWithFunctionDefault() {
			var meta = new Meta();
			meta.Set(new DummyObject() { Value = 42 });
			Assert.True(meta.Use<DummyObject, bool>(o => o.Value > 7, false));
			Assert.True(meta.Use<DummyUnityObject, bool>(o => false, true));
		}

		[Test]
		public void Use() {
			var meta = new Meta();
			var obj = new DummyObject();
			meta.Set(obj);
			Assert.True(meta.Use<DummyObject>(o => o.Used = true));
			Assert.True(obj.Used);
		}

		[Test]
		public void DeadObjects() {
			var meta = new Meta();
			var obj = ScriptableObject.CreateInstance<DummyUnityObject>();
			meta.Set(obj);
			Assert.True(meta.Contains<DummyUnityObject>());
			UnityObject.DestroyImmediate(obj);
			Assert.False(meta.Contains<DummyUnityObject>());
		}

		public class DummyObject {
			public bool Used;
			public int Value;
		}

		public class DummyUnityObject : ScriptableObject { }
	}
}
