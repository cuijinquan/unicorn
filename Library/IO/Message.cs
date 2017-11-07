
namespace Unicorn.IO {
	public class Message : MemoryReader {
		public Message(Connection sender, byte[] buffer, int length) : base(buffer, length) {
			_sender = sender;
		}

		private Connection _sender;
		public Connection Sender { get { return _sender; } }
	}
}
