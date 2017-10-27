
namespace Unicorn.Internal {
	public interface IPeerInternal {
		void Started(Router router);
		void ShuttingDown();
		void Stopped();
		void Receive(Connection sender, byte[] buffer, int length);
	}
}
