
#if UNITY_EDITOR
using NUnit.Framework;
using System.Collections;
using Unicorn.Util;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unicorn.PlayModeTests {
	public class TimerTests {

		[UnityTest]
		public IEnumerator Forward() {
			var timer = new Timer(TimerState.Forward, 0);
			yield return new WaitForSeconds(0.2f);
			Assert.True(timer.Value > 0.1f && timer.Value < 0.3f);
		}

		[UnityTest]
		public IEnumerator Backward() {
			var timer = new Timer(TimerState.Backward, 0);
			yield return new WaitForSeconds(0.2f);
			Assert.True(timer.Value < -0.1f && timer.Value > -0.3f);
		}

		[UnityTest]
		public IEnumerator Paused() {
			var timer = new Timer(TimerState.Paused, 0);
			yield return new WaitForSeconds(0.1f);
			Assert.True(timer.Value == 0);
		}

		[UnityTest]
		public IEnumerator ChangingState() {
			var timer = new Timer(TimerState.Forward, 0f);
			yield return new WaitForSeconds(0.1f);
			var valueBeforeChange = timer.Value;
			timer.State = TimerState.Backward;
			Assert.AreEqual(valueBeforeChange, timer.Value);
			yield return new WaitForSeconds(0.2f);
			Assert.True(timer.Value < 0f && timer.Value > -0.2f);
		}
	}
}
#endif