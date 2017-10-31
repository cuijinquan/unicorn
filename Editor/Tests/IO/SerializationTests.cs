
using NUnit.Framework;
using System;
using Unicorn.IO;
using UnityEngine;

namespace Unicorn.Tests.IO {
	public class SerializationTests {
		
		[Test]
		public void GeneralSerialization() {
			var writer = new MemoryWriter();
			writer.Write(true);
			writer.Write(123);
			writer.Write(-0.25f);
			writer.Write('c');

			var guid = Guid.NewGuid();
			writer.Write(guid);

			writer.Write("Hello");

			writer.Write(new Vector2(0.25f, -0.3f));
			writer.Write(new Vector3(0.5f, -0.6f, 0.25f));
			writer.Write(new Vector4(0.75f, -0.9f, -0.4f, 0.333f));
			writer.Write(new Quaternion(0.75f, -0.9f, -0.4f, 0.333f));

			var reader = new MemoryReader(writer.GetBuffer(), writer.Length);
			Assert.True(reader.ReadBoolean());
			Assert.AreEqual(123, reader.ReadInt32());
			Assert.AreEqual(-0.25f, reader.ReadSingle());
			Assert.AreEqual('c', reader.ReadChar());

			Assert.AreEqual(guid, reader.ReadGuid());

			Assert.AreEqual("Hello", reader.ReadString());

			Assert.AreEqual(new Vector2(0.25f, -0.3f), reader.ReadVector2());
			Assert.AreEqual(new Vector3(0.5f, -0.6f, 0.25f), reader.ReadVector3());
			Assert.AreEqual(new Vector4(0.75f, -0.9f, -0.4f, 0.333f), reader.ReadVector4());
			Assert.AreEqual(new Quaternion(0.75f, -0.9f, -0.4f, 0.333f), reader.ReadQuaternion());
		}
	}
}
