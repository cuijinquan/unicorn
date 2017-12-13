
using System.Collections.Generic;
using UnityEngine;

namespace Unicorn {
	public static class ConnectionEditorGUI {
		public static void Draw(Connection conn) {
			GUILayout.Box(conn.ToString());
		}

		public static void Draw(IEnumerable<Connection> conns) {
			foreach (var conn in conns) {
				Draw(conn);
			}
		}
	}
}
