
using Unicorn.Util;

namespace Unicorn.Entities {
	public interface IEntitySystem {
		/// <summary>
		/// Get a set of connections that are allowed as entity owners.
		/// </summary>
		IReadonlyObservableSet<Connection> Owners { get; }

		/// <summary>
		/// Called by an entity when a connection has been added to it's group.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="conn"></param>
		void GroupConnected(Entity entity, Connection conn);

		/// <summary>
		/// Called by an entity when a connection has been removed from it's group.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="conn"></param>
		void GroupDisconnected(Entity entity, Connection conn);

		/// <summary>
		/// Called by an entity when a connection has been added or removed from it's set of owners.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="conn"></param>
		/// <param name="isOwner"></param>
		void OwnershipChanged(Entity entity, Connection conn, bool isOwner);
	}
}
