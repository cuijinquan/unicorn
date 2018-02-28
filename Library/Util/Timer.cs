
using System;
using Unicorn.IO;
using UnityEngine;

namespace Unicorn.Util {
	/// <summary>
	/// A timer that is designed to be synchronized across the network.
	/// </summary>
	public class Timer {
		public Timer() : this(TimerState.Paused, 0) { }
		public Timer(TimerState state, float value) : this(state, TimeSource.Unscaled, value) { }
		public Timer(TimerState state, TimeSource source, float value) {
			_state = state;
			_source = source;
			Value = value;
		}

		private TimerState _state;
		private TimeSource _source;
		private float _value;

		public override string ToString() {
			return Value.ToString();
		}

		public string ToString(int decimalDigits) {
			var f = Mathf.Pow(10, decimalDigits);
			return (Mathf.Floor(Value * f) / f).ToString();
		}

		/// <summary>
		/// Set both state and value.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="value"></param>
		public virtual void Set(TimerState state, float value) {
			_state = state;
			Value = value;
		}

		/// <summary>
		/// Pause the timer and set the value to 0.
		/// </summary>
		public virtual void Reset() {
			Set(TimerState.Paused, 0);
		}

		/// <summary>
		/// Run the timer forwards from the specified value.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Forward(float value) {
			Set(TimerState.Forward, value);
		}

		/// <summary>
		/// Run the timer backwards from the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public virtual void Backward(float value) {
			Set(TimerState.Backward, value);
		}

		/// <summary>
		/// Get or set the current timer value.
		/// </summary>
		/// <value></value>
		public virtual float Value {
			get {
				switch(_state) {
					case TimerState.Paused: return _value;
					case TimerState.Forward: return Time.time - _value;
					case TimerState.Backward: return _value - Time.time;
					default: throw new InvalidOperationException();
				}
			}

			set {
				switch(_state) {
					case TimerState.Paused: _value = value; break;
					case TimerState.Forward: _value = Time.time - value; break;
					case TimerState.Backward: _value = value + Time.time; break;
					default: throw new InvalidOperationException();
				}
			}
		}

		public static implicit operator float(Timer timer) {
			return timer.Value;
		}

		/// <summary>
		/// Get or set the timer state. This will not affect the timer value.
		/// </summary>
		/// <value></value>
		public virtual TimerState State {
			get { return _state; }
			set {
				var time = Value;
				_state = value;
				Value = time;
			}
		}

		/// <summary>
		/// Serialize the timer data.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void Write(DataWriter writer) {
			writer.Write((byte)_state);
			writer.Write((byte)_source);
			writer.Write(Value);
		}

		/// <summary>
		/// Deserialize the timer data.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void Read(DataReader reader) {
			_state = (TimerState)reader.ReadByte();
			_source = (TimeSource)reader.ReadByte();
			Value = reader.ReadSingle();
		}

		public static float GetCurrentTime(TimeSource source) {
			switch(source) {
				case TimeSource.Unscaled: return Time.unscaledTime;
				case TimeSource.Scaled: return Time.time;
				default: throw new ArgumentOutOfRangeException("source");
			}
		}
	}
}
