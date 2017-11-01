
using Unicorn.IO;

namespace Unicorn.Entities.Internal {
	public interface IEntityInternal {
		void SetOwnership(bool isMine);
		void Receive(Connection sender, DataReader payload);
	}
}
