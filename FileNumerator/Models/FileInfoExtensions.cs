using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNumerator.Models
{
	public static class FileInfoExtensions
	{
		/// <summary>
		/// Wrapper for Path.GetFileNameWithoutExtension(fileInfo.FullName);
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		public static string NameWithoutFileExtension(this FileInfo fileInfo)
			=> Path.GetFileNameWithoutExtension(fileInfo.FullName);
	}
}
