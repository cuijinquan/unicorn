
namespace Unicorn.Util {
	public class Interval {
		public static Interval Rate(TimeSource source, float rate) { return new Interval(source, 1f / rate); }
		public static Interval Rate(float rate) { return Rate(TimeSource.Unscaled, rate); }

		public Interval(float duration) : this(TimeSource.Unscaled, duration) { }
		public Interval(TimeSource source, float duration) {
			_source = source;
			_duration = duration;
			_last = float.NegativeInfinity;
		}
		
		private readonly TimeSource _source;
		private readonly float _duration;
		private float _last;
		
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
