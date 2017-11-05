
using Unicorn.IO;

namespace Unicorn.Entities.Internal {
	public interface IEntityInternal {
		void Receive(Connection sender, DataReader payload);
		void SetLocalOwnership(bool isMine);
		byte AddModule(IEntityModuleInternal module);
		void RemoveModule(byte moduleId);
	}
}
