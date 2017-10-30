
using System;
using Unicorn.Util;

namespace Unicorn.Game {
	public static class EntitySystem {
		[ThreadStatic]
		private static IEntitySystem _instance = null;

		/// <summary>
		/// Get the global entity system.
		/// </summary>
		/// <returns></returns>
		public static IEntitySystem Get() {
			return _instance;
		}

		/// <summary>
		/// Set the global entity system.
		/// </summary>
		/// <param name="instance"></param>
		public static void Set(IEntitySystem instance) {
			if (instance == null)
				throw new ArgumentNullException("instance");
			_instance = instance;
		}
	}
}
