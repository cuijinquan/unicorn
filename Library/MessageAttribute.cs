
using System;
using System.Reflection;

namespace Unicorn {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class MessageAttribute : Attribute {
		public MessageAttribute(int id) {
			_id = id;
		}

		public MessageAttribute(object id) {
			_id = Convert.ToInt32(id);
		}

		private readonly int _id;

		public int Id { get { return _id; } }
	}
}
