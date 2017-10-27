
using System.IO;

namespace Unicorn.IO {
	public class MemoryWriter : DataWriter {
		public MemoryWriter() : this(new MemoryStream()) { }
		private MemoryWriter(MemoryStream memory) : base(memory) {
			_memory = memory;
		}

		private readonly MemoryStream _memory;
		
		public int Length { get { checked { return (int)_memory.Length; } } }

		public byte[] GetBuffer() {
			return _memory.GetBuffer();
		}
	}
}
