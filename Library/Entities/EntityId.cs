
using System;
using Unicorn.IO;

namespace Unicorn.Entities {
	public struct EntityId : IEquatable<EntityId>, IComparable<EntityId> {
		public uint value;

		public readonly static EntityId None = new EntityId();
		
		public override bool Equals(object obj) {
			return obj is EntityId && ((EntityId)obj).value == value;
		}

		public override int GetHashCode() {
			unchecked { return (int)value; }
		}

		public void WriteTo(DataWriter writer) {
			writer.Write(value);
		}

		public static EntityId ReadFrom(DataReader reader) {
			return new EntityId { value = reader.ReadUInt32() };
		}

		public int CompareTo(EntityId other) {
			return value.CompareTo(other.value);
		}

		public bool Equals(EntityId other) {
			return value == other.value;
		}

		public static bool operator ==(EntityId a, EntityId b) {
			return a.value == b.value;
		}

		public static bool operator !=(EntityId a, EntityId b) {
			return a.value != b.value;
		}



		private static uint _next = 1;

		public static EntityId Allocate() {
			checked { return new EntityId { value = _next++ }; }
		}

		public static void ResetPool() {
			_next = 1;
		}
	}
}
