
using System;
using UnityEngine;

namespace Unicorn.Entities {
	public class GlobalEntityBehaviour<T> : EntityBehaviour where T : GlobalEntityBehaviour<T> {
		private static T _current;
		public static T Current { get { return _current ? _current : null; } }
		
		protected override void Awake() {
			base.Awake();
			if (_current != null) {
				Debug.LogWarningFormat("Duplicate global entity instance of: {0}", typeof(T).Name);
			}
			_current = (T)this;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			_current = null;
		}
	}
}
