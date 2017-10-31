
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unicorn.IO {
	public class DataWriter : BinaryWriter {
		public DataWriter(Stream output) : base(output, Encoding.UTF8) { }
		
		public void Write(Guid value) {
			Write(value.ToByteArray(), 0, 16);
		}

		public void Write(Vector2 value) {
			Write(value.x);
			Write(value.y);
		}

		public void Write(Vector3 value) {
			Write(value.x);
			Write(value.y);
			Write(value.z);
		}

		public void Write(Vector4 value) {
			Write(value.x);
			Write(value.y);
			Write(value.z);
			Write(value.w);
		}

		public void Write(Quaternion value) {
			Write(value.x);
			Write(value.y);
			Write(value.z);
			Write(value.w);
		}
	}
}
