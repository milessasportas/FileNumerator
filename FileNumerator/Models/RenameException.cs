using System;
using System.Runtime.Serialization;

namespace FileNumerator.Models
{
	[Serializable]
	internal class RenameException : Exception
	{
		public RenameException()
		{
		}

		public RenameException(string message) : base(message)
		{
		}

		public RenameException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RenameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}