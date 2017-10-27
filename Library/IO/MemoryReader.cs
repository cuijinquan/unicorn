
using System.IO;

namespace Unicorn.IO {
	public class MemoryReader : DataReader {
		public MemoryReader(byte[] buffer, int length) : base(new MemoryStream(buffer, 0, length, false)) { }
	}
}
