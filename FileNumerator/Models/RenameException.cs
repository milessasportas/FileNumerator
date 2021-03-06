﻿using System;
using System.Runtime.Serialization;

namespace FileNumerator.Models
{
	[Serializable]
	internal class RenameException : Exception
	{
		/// <summary>
		/// File which couldn't be renamed
		/// </summary>
		public string FailedFile { get; set; }

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