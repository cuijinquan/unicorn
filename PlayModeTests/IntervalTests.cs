
# if UNITY_EDITOR
using NUnit.Framework;
using System.Collections;
using Unicorn.Util;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unicorn.PlayModeTests {
	public class IntervalTests {

		[UnityTest]
		public IEnumerator General() {
			var end = new Timer(TimerState.Backward, 0.25f);
			var interval = new Interval(0.1f);
			Assert.True(interval.Update());
			var updates = 1;
			while(end > 0) {
				if (interval.Update())
					updates++;
				yield return new WaitForEndOfFrame();
			}
			Assert.AreEqual(3, updates);
		}
	}
}
#endif