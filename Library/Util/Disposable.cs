
using System;
using System.Collections.Generic;

namespace Unicorn.Util {
	/// <summary>
	/// Simple composite disposable.
	/// </summary>
	public sealed class Disposable : IDisposable {
		/// <summary>
		/// Create an empty disposable.
		/// </summary>
		public Disposable() {
			_children = new HashSet<Action>();
		}

		/// <summary>
		/// Create a disposable and add an action to invoke when disposed.
		/// </summary>
		/// <param name="action"></param>
		public Disposable(Action action) : this() {
			Add(action);
		}

		private readonly HashSet<Action> _children;

		/// <summary>
		/// Add an action to invoke when disposed.
		/// </summary>
		/// <param name="action"></param>
		/// <returns>This disposable.</returns>
		public Disposable Add(Action action) {
			if (action == null)
				throw new ArgumentNullException("action");
			_children.Add(action);
			return this;
		}

		/// <summary>
		/// Add a disposable to dispose when disposed.
		/// </summary>
		/// <param name="disposable"></param>
		/// <returns>This disposable.</returns>
		public Disposable Add(IDisposable disposable) {
			if (disposable != null)
				_children.Add(disposable.Dispose);
			return this;
		}

		/// <summary>
		/// </summary>
		/// <returns>This disposable.</returns>
		public Disposable Dispose() {
			_children.RemoveWhere(child => {
				child();
				return true;
			});
			return this;
		}

		void IDisposable.Dispose() {
			Dispose();
		}
	}
}
