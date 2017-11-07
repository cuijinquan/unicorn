
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unicorn.Entities {
	[DisallowMultipleComponent]
	public class NetworkSceneManager : GlobalEntityModule<NetworkSceneManager> {
		private static SubSet<Connection> _clients;
		public static IReadonlyObservableSet<Connection> Clients { get { return _clients; } }

		[Tooltip("The scene to load after the server is started.")]
		public string onlineScene = "";
		[Tooltip("The scene to load after network is stopped.")]
		public string offlineScene = "";

		private enum ServerMessage : byte { SceneLoaded }
		private enum ClientMessage : byte { LoadScene }

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);

			if (IsServer) {
				_clients = new SubSet<Connection>(Group);

				Group.Added(UntilDestroy, conn => {
					Send(conn, payload => {
						payload.Write((byte)ClientMessage.LoadScene);
						payload.Write(SceneManager.GetActiveScene().name);
					});
				});
			}

			SceneManager.sceneLoaded += SceneLoaded;
			UntilDestroy.Add(() => SceneManager.sceneLoaded -= SceneLoaded);

			if (IsServer && !string.IsNullOrEmpty(onlineScene))
				LoadScene(onlineScene);
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (!string.IsNullOrEmpty(offlineScene))
				SceneManager.LoadSceneAsync(offlineScene);
		}

		private void SceneLoaded(Scene scene, LoadSceneMode mode) {
			if (IsServer) {
				Send(Group, payload => {
					payload.Write((byte)ClientMessage.LoadScene);
					payload.Write(scene.name);
				});
			} else {
				Send(payload => {
					payload.Write((byte)ServerMessage.SceneLoaded);
					payload.Write(scene.name);
				});
			}
		}

		protected virtual void LoadScene(string name) {
			SceneManager.LoadSceneAsync(name);
		}

		protected override void Receive(Connection sender, DataReader payload) {
			if (IsServer) {
				switch((ServerMessage)payload.ReadByte()) {
					case ServerMessage.SceneLoaded:
						var sceneName = payload.ReadString();
						if (sceneName == SceneManager.GetActiveScene().name) {
							_clients.Add(sender);
						} else {
							_clients.Remove(sender);
						}
						break;
				}
			} else {
				switch((ClientMessage)payload.ReadByte()) {
					case ClientMessage.LoadScene:
						LoadScene(payload.ReadString());
						break;
				}
			}
		}
	}
}
