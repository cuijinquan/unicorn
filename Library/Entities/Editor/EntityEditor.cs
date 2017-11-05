
using UnityEngine;
using UnityEditor;

namespace Unicorn.Entities {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Entity))]
	public class EntityEditor : Editor {
		public override void OnInspectorGUI() {
			if (targets.Length > 1) {
				EditorGUILayout.HelpBox(string.Format("Showing {0} entities.", targets.Length), MessageType.Info);
				foreach (var target in targets) {
					GUILayout.BeginVertical(GUI.skin.box);
					GUILayout.Label(target.name);
					GUILayout.BeginHorizontal();
					GUILayout.Space(15);
					GUILayout.BeginVertical();
					DrawEntity((Entity)target);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
			} else {
				DrawEntity((Entity)target);
			}
		}

		private void DrawEntity(Entity entity) {
			if (entity.Id == EntityId.None) {
				GUILayout.Label("Unassigned");
			} else {
				EditorGUILayout.LabelField("Id", entity.Id.ToString());
				EditorGUILayout.LabelField("Source", entity.Source);
				
				var router = EntityRouter.Current;
				if (router != null && router.IsServer) {
					EditorGUILayout.LabelField("Group size", entity.Group.Count.ToString());
					EditorGUILayout.LabelField("Owner Count", entity.Owners.Count.ToString());
				} else {
					EditorGUILayout.LabelField("Local Ownership", entity.IsMine.ToString());
				}
			}
		}
	}
}
