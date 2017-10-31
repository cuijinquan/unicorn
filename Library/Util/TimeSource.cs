
using UnityEngine;

namespace Unicorn.Util {
	public enum TimeSource : byte {
		/// <summary>
		/// Use <see cref="Time.unscaledTime"/>.
		/// </summary>
		Unscaled,
		/// <summary>
		/// Use <see cref="Time.time"/>.
		/// </summary>
		Scaled
	}
}
