
using System;
using System.Collections.Generic;

namespace Unicorn.Util {
	/// <summary>
	/// A set of observers that can be weakly referenced.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObserverSet<T> where T : class {
		public ObserverSet() {
			_links = new HashSet<ILink>();
		}

		private readonly HashSet<ILink> _links;

		/// <summary>
		/// Add an observer.
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="weak">True, to use a weak reference.</param>
		public IDisposable Add(T observer, bool weak) {
			Remove(observer);
			var link = weak ? (ILink)new WeakLink(observer) : new Link(observer);
			_links.Add(link);
			return new Disposable(() => Remove(observer));
		}

		/// <summary>
		/// Remove an observer.
		/// </summary>
		/// <param name="observer"></param>
		public void Remove(T observer) {
			_links.RemoveWhere(link => !link.IsAlive || link.Target == observer);
		}

		/// <summary>
		/// Use all observers.
		/// </summary>
		/// <param name="action"></param>
		public void Use(Action<T> action) {
			_links.RemoveWhere(link => {
				if (link.IsAlive) {
					action(link.Target);
					return false;
				} else {
					return true;
				}
			});
		}
		


		private interface ILink {
			T Target { get; }
			bool IsAlive { get; }
		}

		private class Link : ILink {
			public Link(T target) {
				_target = target;
			}

			private readonly T _target;

			public T Target { get { return _target; } }
			public bool IsAlive { get { return true; } }
		}

		private class WeakLink : ILink {
			public WeakLink(T target) {
				_target = new WeakReference(target);
			}

			private readonly WeakReference _target;

			public T Target { get { return _target.Target as T; } }
			public bool IsAlive { get { return _target.IsAlive; } }
		}
	}
}
