
using System;

namespace Unicorn.Game {
	public struct EntityId : IComparable<EntityId>, IEquatable<EntityId> {
		public EntityId(uint value) {
			_value = value;
		}

		private uint _value;
		public uint Value { get { return _value; } }

		public static readonly EntityId None = new EntityId(0);

		private static uint _next = 1;
		public static EntityId Allocate() {
			checked { return new EntityId(_next++); }
		}

		public static void ResetPool() {
			_next = 1;
		}



		public int CompareTo(EntityId other) {
			return _value.CompareTo(other._value);
		}

		public bool Equals(EntityId other) {
			return _value == other._value;
		}
		
		public override bool Equals(object other) {
			return other is EntityId ? _value == ((EntityId)other)._value : false;
		}

		public override int GetHashCode() {
			unchecked { return (int)_value; }
		}

		public static bool operator ==(EntityId a, EntityId b) {
			return a._value == b._value;
		}
		
		public static bool operator !=(EntityId a, EntityId b) {
			return a._value != b._value;
		}
	}
}
