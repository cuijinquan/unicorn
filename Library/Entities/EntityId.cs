
namespace Unicorn.Entities {
	using System;

	public struct EntityId : IComparable<EntityId>, IEquatable<EntityId> {
		public uint value;

		public override string ToString() {
			return value.ToString();
		}

		public int CompareTo(EntityId other) {
			return value.CompareTo(other.value);
		}

		public override int GetHashCode() {
			unchecked { return (int)value; }
		}

		public override bool Equals(object other) {
			return other is EntityId && ((EntityId)other).value == value;
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

		public static readonly EntityId None = new EntityId();
	}
}

namespace Unicorn.IO {
	using Unicorn.Entities;

	partial class DataWriter {
		public void Write(EntityId value) {
			Write(value.value);
		}
	}

	partial class DataReader {
		public EntityId ReadEntityId() {
			return new EntityId { value = ReadUInt32() };
		}
	}
}
