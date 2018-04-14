
namespace Unicorn.Util {
	/// <summary>
	/// Utility for interval based updates.
	/// </summary>
	public class Interval {
		public static Interval Rate(TimeSource source, float rate) { return new Interval(source, 1f / rate); }
		public static Interval Rate(float rate) { return Rate(TimeSource.Unscaled, rate); }

		/// <summary>
		/// Create a new interval that updates after *duration* seconds (unscaled time).
		/// </summary>
		/// <param name="duration">The interval duration in seconds.</param>
		public Interval(float duration) : this(TimeSource.Unscaled, duration) { }
		/// <summary>
		/// Create a new interval that updates after *duration* seconds.
		/// </summary>
		/// <param name="source">The time source to use.</param>
		/// <param name="duration">The interval duration in seconds.</param>
		public Interval(TimeSource source, float duration) {
			_source = source;
			_duration = duration;
			_last = float.NegativeInfinity;
		}

		private readonly TimeSource _source;
		private readonly float _duration;
		private float _last;

		/// <summary>
		/// Check if the end of the interval is reached. If so, the interval will reset.
		/// </summary>
		/// <returns>
		/// True, if the end was reached.
		/// </returns>
		public bool Update() {
			var current = Timer.GetCurrentTime(_source);
			if (current > _last + _duration) {
				_last = current;
				return true;
			} else {
				return false;
			}
		}
	}
}
