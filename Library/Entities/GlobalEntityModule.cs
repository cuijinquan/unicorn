
using System;
using UnityEngine;

namespace Unicorn.Entities {
	public abstract class GlobalEntityModule<T> : EntityModule where T : GlobalEntityModule<T> {
		private static T _current;
		public static T Current {
			get { return _current; }
		}

		public static T Require() {
			if (!_current)
				throw new InvalidOperationException(string.Format("Missing global entity: {0}", typeof(T).Name));
			return _current;
		}

		protected override void Awake() {
			base.Awake();
			if (_current)
				Debug.LogWarningFormat("Using multiple global entities: {0}", typeof(T).Name);
			_current = (T)this;
		}
	}
}
