
using System;
using UnityEngine;

namespace Unicorn.Internal {
	public sealed class RouterWorker : MonoBehaviour {
		private WeakReference _target = null;

		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		private void Start() {
			if (_target == null) {
				Debug.LogWarning("Invalid router worker. Use RouterWorker.Create to instantiate a worker.");
				DestroyImmediate(gameObject);
			}
		}

		private void Update() {
			if (_target.IsAlive) {
				((IRouterInternal)_target.Target).Update();
			} else {
				Destroy(gameObject);
			}
		}

		private void OnDestroy() {
			if (_target != null && _target.IsAlive) {
				((IRouterInternal)_target.Target).Shutdown();
			}
		}

		public static void Create(Router target) {
			new GameObject("RouterWorker").AddComponent<RouterWorker>()._target = new WeakReference(target);
		}
	}
}
