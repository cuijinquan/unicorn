
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine.SceneManagement;

namespace Unicorn.Entities {
	public class NetworkSceneManager : EntityModule<NetworkSceneManager> {
		private static SubSet<Connection> _clients;
		public static IReadonlyObservableSet<Connection> Clients { get { return _clients; } }
		
		private enum Msg : ushort {
			LoadScene,
			SceneLoaded
		}

		protected override void Awake() {
			base.Awake();
			_clients = new SubSet<Connection>(EntityRouter.Current.Connections);

			SceneManager.sceneLoaded += SceneLoaded;
			UntilDestroy.Add(() => SceneManager.sceneLoaded -= SceneLoaded);

			Group.Added(UntilDestroy, conn => {
				Send(conn, MsgLoadScene(SceneManager.GetActiveScene().name));
			});
		}

		private void SceneLoaded(Scene scene, LoadSceneMode mode) {
			if (IsServer) {
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
