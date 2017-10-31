
using System;
using UnityEngine;

namespace Unicorn.PlayModeTests {
	public static class TestUtility {
		public static WaitUntil Wait(this TimeSpan timeout, Func<bool> condition) {
			var end = Time.unscaledTime + timeout.TotalSeconds;
			return new WaitUntil(() => {
				if (condition()) {
					return true;
				} else if (Time.unscaledTime > end) {
					throw new TimeoutException();
				}
				return false;
			});
		}
	}
}
