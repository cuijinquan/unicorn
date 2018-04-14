
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unicorn.Util {
	/// <summary>
	/// Utility for storing (exact)type dependent meta information.
	/// This collection can contain on object of a specific type.
	/// In addition, UnityEngine.Object instances that have been destroyed are removed automatically.
	/// </summary>
	public class Meta {
		public Meta() {
			_map = new SortedDictionary<string, object>();
		}

		private readonly SortedDictionary<string, object> _map;



		/// <summary>
		/// Set or add an object of the specified type.
		/// </summary>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		public void Set<T>(T value) where T : class {
			_map[typeof(T).FullName] = value;
		}

		/// <summary>
		/// Remove an object of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Remove<T>() where T : class {
			_map.Remove(typeof(T).FullName);
		}

		/// <summary>
		/// Remove an object of the specified type that matches the condition.
		/// </summary>
		/// <param name="match"></param>
		/// <typeparam name="T"></typeparam>
		public void Remove<T>(Predicate<T> match) where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value) && IsDead(value) || match((T)value)) {
				_map.Remove(typeof(T).FullName);
			}
		}

		/// <summary>
		/// Check if this collection contains an object of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public bool Contains<T>() where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value)) {
				if (IsDead(value)) {
					_map.Remove(typeof(T).FullName);
				} else {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if this collection contains an object of the specified type that matches the condition.
		/// </summary>
		/// <param name="match"></param>
		/// <typeparam name="T"></typeparam>
		public bool Contains<T>(Predicate<T> match) where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value)) {
				if (IsDead(value)) {
					_map.Remove(typeof(T).FullName);
				} else {
					return match((T)value);
				}
			}
			return false;
		}

		/// <summary>
		/// Call a function for an object of the specified type.
		/// </summary>
		/// <param name="func"></param>
		/// <param name="defaultValue"></param>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <typeparam name="R">The return type of the function.</typeparam>
		/// <returns>The return value of the function or the default value.</returns>
		public R Use<T, R>(Func<T, R> func, R defaultValue) where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value)) {
				if (IsDead(value)) {
					_map.Remove(typeof(T).FullName);
				} else {
					return func((T)value);
				}
			}
			return defaultValue;
		}

		/// <summary>
		/// Call a function for an object of the specified type.
		/// </summary>
		/// <param name="func"></param>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <typeparam name="R">The return type of the function.</typeparam>
		/// <returns>The return value of the function or the default value.</returns>
		public R Use<T, R>(Func<T, R> func) where T : class {
			return Use(func, default(R));
		}

		/// <summary>
		/// Call a method for an object of the specified type.
		/// </summary>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>true, if the method was called.</returns>
		public bool Use<T>(Action<T> action) where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value)) {
				if (IsDead(value)) {
					_map.Remove(typeof(T).FullName);
				} else {
					action((T)value);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Get an object of the specified type.
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The object or the default value if not found.</returns>
		public T Get<T>(T defaultValue) where T : class {
			object value;
			if (_map.TryGetValue(typeof(T).FullName, out value)) {
				if (IsDead(value)) {
					_map.Remove(typeof (T).FullName);
				} else {
					return (T)value;
				}
			}
			return null;
		}

		/// <summary>
		/// Get an object of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>The object or null if not found.</returns>
		public T Get<T>() where T : class {
			return Get<T>(null);
		}



		private static bool IsDead(object value) {
			return (value is UnityObject) && !(UnityObject)value;
		}
	}
}
