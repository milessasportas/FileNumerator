using System.IO;

namespace FileNumerator.Models
{
	public struct RenamedFile
	{
		public string OldPath { get; set; }

		public string NewPath { get; set; } 

		public string OldName => Path.GetFileName(OldPath) ?? "";

		public string NewName => Path.GetFileName(NewPath) ?? "";

		public override string ToString()
			=> $"OldPath: \"{OldPath}\" | NewPath: \"{NewPath}\"";
	}
}
