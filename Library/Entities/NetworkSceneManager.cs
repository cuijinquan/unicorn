
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unicorn.Entities {
	public class NetworkSceneManager : EntityModule<NetworkSceneManager> {
		[Tooltip("The scene to load, when the scene manager is destroyed. (Empty string to disable)")]
		public string offlineScene = "";

		private SubSet<Connection> _clients;
		public static IReadonlyObservableSet<Connection> Clients { get { return Require()._clients; } }
		
		private enum Msg : ushort {
			LoadScene,
			SceneLoaded
		}

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
			TrackInstance();

			_clients = new SubSet<Connection>(EntityRouter.Current.Connections);

			SceneManager.sceneLoaded += SceneLoaded;
			UntilDestroy.Add(() => SceneManager.sceneLoaded -= SceneLoaded);

			if (IsServer)
				Group.Added(UntilDestroy, conn => Send(conn, MsgLoadScene(SceneManager.GetActiveScene().name)));
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (!string.IsNullOrEmpty(offlineScene))
				SceneManager.LoadSceneAsync(offlineScene);
		}

		private void SceneLoaded(Scene scene, LoadSceneMode mode) {
			if (IsServer) {
				_clients.Clear();
				Send(MsgLoadScene(scene.name));
			} else {
				Send(MsgSceneLoaded(scene.name));
			}
		}

		[Client(Msg.LoadScene)]
		private void LoadScene(Message msg) {
			SceneManager.LoadSceneAsync(msg.ReadString());
		}

		[Server(Msg.SceneLoaded)]
		private void SceneLoaded(Message msg) {
			var sceneName = msg.ReadString();
			if (sceneName == SceneManager.GetActiveScene().name) {
				_clients.Add(msg.Sender);
			} else {
				_clients.Remove(msg.Sender);
			}
		}
		
		private MessageWriter MsgLoadScene(string name) {
			return message => {
				Endpoint(message, Msg.LoadScene);
				message.Write(name);
			};
		}

		private MessageWriter MsgSceneLoaded(string name) {
			return message => {
				Endpoint(message, Msg.SceneLoaded);
				message.Write(name);
			};
		}
	}
}
