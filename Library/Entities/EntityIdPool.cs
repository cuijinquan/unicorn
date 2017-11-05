
namespace Unicorn.Entities {
	public static class EntityIdPool {
		static EntityIdPool() {
			Reset();
		}

		private static uint _next;

		public static EntityId Allocate() {
			checked { return new EntityId { value = _next++ }; }
		}

		public static void Reset() {
			_next = default(uint) + 1;
		}
	}
}
