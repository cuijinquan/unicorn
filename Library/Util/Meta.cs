
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unicorn.Util {
	public class Meta {
		public Meta() {
			_map = new SortedDictionary<string, object>();
		}

		private readonly SortedDictionary<string, object> _map;



		public void Set<T>(T value) where T : class {
			_map[typeof(T).FullName] = value;
		}

		public void Remove<T>() {
			_map.Remove(typeof(T).FullName);
		}

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

		public R Use<T, R>(Func<T, R> func) where T : class {
			return Use(func, default(R));
		}

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



		private static bool IsDead(object value) {
			return (value is UnityObject) && !(UnityObject)value;
		}
	}
}
