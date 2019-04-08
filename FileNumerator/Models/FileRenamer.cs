using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNumerator.Models
{
	public class FileRenamer
	{
		#region [ Constructors ]

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory) : this(directory, null)
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, params string[] fileendingsToRemove) : this(directory, fileendingsToRemove, null)
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="ignoredFiletypes">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, params string[] ignoredFiletypes) : this(directory, fileendingsToRemove, ignoredFiletypes, 1)
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="ignoredFiletypes">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <param name="startNumber">The first number that shall be in the list, defaul 1</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] ignoredFiletypes, int startNumber) : this(directory, fileendingsToRemove, ignoredFiletypes, startNumber, null)
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="ignoredFiletypes">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <param name="startNumber">The first number that shall be in the list, defaul 1</param>
		/// <param name="lastNumber">The last number that shall be set, numeration stops when thsi number is reached, deafult null (don't stop)</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] ignoredFiletypes, int? startNumber, int? lastNumber)
		{
			DirectoryToActOn = directory;
		}

		#endregion [ Constructors ]

		private string _directory;

		/// <summary>
		/// The directory toact on
		/// </summary>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public string DirectoryToActOn
		{
			get
			{
				return _directory;
			}
			set
			{
				//check the directory
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException("The directory can't be null or empty.", nameof(DirectoryToActOn));
				if(!Directory.Exists(value))
					throw new ArgumentException("The directory doesn't seem to exist.", nameof(DirectoryToActOn));

				//check if any files exists
				var files = Directory.GetFiles(value);
				if(files.Length == 0)
					throw new FileNotFoundException($"No files found in Directory {value} that fit filter criteria.");

				_files = files;

				_directory = value;
			}
		}

		#region [ Files ]

		private string[] _files;

		public IReadOnlyList<string> Files => Array.AsReadOnly(_files);

		public IReadOnlyCollection<string> FilterFiles;

		#endregion [ Files ]

	}
}
