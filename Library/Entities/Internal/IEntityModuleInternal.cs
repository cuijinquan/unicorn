
using System;
using Unicorn.IO;

namespace Unicorn.Entities.Internal {
	public interface IEntityModuleInternal {
		void Receive(Message msg);
	}
}
