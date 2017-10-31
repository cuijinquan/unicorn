
namespace Unicorn.Util {
	public enum TimerState : byte {
		/// <summary>
		/// The timer's value will not be modified.
		/// </summary>
		Paused,
		/// <summary>
		/// The timer's value will increase by time.
		/// </summary>
		Forward,
		/// <summary>
		/// The timer's value will decrease by time.
		/// </summary>
		Backward
	}
}
