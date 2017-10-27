
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unicorn.IO {
	public class DataReader : BinaryReader {
		public DataReader(Stream input) : base(input, Encoding.UTF8) { }
		
		public Guid ReadGuid() {
			var buffer = new byte[16];
			if (Read(buffer, 0, 16) < 16)
				throw new EndOfStreamException();
			return new Guid(buffer);
		}

		public Vector2 ReadVector2() {
			return new Vector2(ReadSingle(), ReadSingle());
		}
		
		public Vector3 ReadVector3() {
			return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
		}

		public Vector4 ReadVector4() {
			return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}

		public Quaternion ReadQuaternion() {
			return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}
	}
}
