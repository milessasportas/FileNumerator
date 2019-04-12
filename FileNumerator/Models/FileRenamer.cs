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
		public FileRenamer(string directory) : this(directory, "")
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, params string[] fileendingsToRemove) : this(directory, fileendingsToRemove, "")
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
		public FileRenamer(string directory, string[] fileendingsToRemove, params string[] ignoredFiletypes) : this(directory, fileendingsToRemove, ignoredFiletypes, FilterType.ExcludeFiltered)
		{

		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="filetypeFilter">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <param name="filterType">The way to filter filetypes (only search for the <see cref="ignoredFiletypes"/> or ignore them)</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterType filterType) : this(directory, fileendingsToRemove, filetypeFilter, filterType, 1)
		{
		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="filetypeFilter">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <param name="filterType">The way to filter filetypes (only search for the <see cref="filetypeFilter"/> or ignore them)</param>
		/// <param name="startNumber">The first number that shall be in the list, defaul 1</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterType filterType, int startNumber) : this(directory, fileendingsToRemove, filetypeFilter, filterType, startNumber, null)
		{
		}

		/// <summary>
		/// Renames files in a dirctory
		/// </summary>
		/// <param name="directory">The directroy to act on</param>
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename</param>
		/// <param name="filetypeFilter">The filetypes which shall be ignored, (e.g. [ ".pdf", ".exe"] )</param>
		/// <param name="filterType">The way to filter filetypes (only search for the <see cref="filetypeFilter"/> or ignore them)</param>
		/// <param name="startNumber">The first number that shall be in the list, defaul 1</param>
		/// <param name="lastNumber">The last number that shall be set, numeration stops when thsi number is reached, deafult null (don't stop)</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterType filterType, int startNumber, int? lastNumber)
		{
			DirectoryToActOn = directory;
			FileEndingsToRemove = fileendingsToRemove;
			FiletypeFilter = filetypeFilter;
			FilterType = filterType;
			StartNumber = startNumber;
			LastNumber = lastNumber;
		}

		#endregion [ Constructors ]

		#region [ DirectoryToActOn ]

		private string _directory;

		/// <summary>
		/// The directory to act on
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
				if (!Directory.Exists(value))
					throw new ArgumentException($"The directory \"´{value}\" doesn't seem to exist.", nameof(DirectoryToActOn));

				//check if any files exists
				var files = Directory.GetFiles(value);
				if (files.Length == 0)
					throw new FileNotFoundException($"No files found in Directory \"{value}\".");

				//set the fields
				_files = files;
				_directory = value;
			}
		}

		#endregion [ DirectoryToActOn ]

		public string[] FileEndingsToRemove { get; set; }

		public string[] FiletypeFilter { get; set; }

		public FilterType FilterType { get; set; }

		public int StartNumber { get; set; }

		public int? LastNumber { get; set; }

		#region [ Files ]

		private string[] _files;

		/// <summary>
		/// All the files found in the Directory
		/// </summary>
		public IReadOnlyList<string> Files => Array.AsReadOnly(_files);

		/// <summary>
		/// Filter on Files showing wich Files to act on
		/// </summary>
		public IReadOnlyCollection<string> FilesToActOn => Array.AsReadOnly((FilterType == FilterType.ExcludeFiltered? getFilesFilteredByFileTyp(_files) : getTheFilteredFilesByType(_files)).ToArray());

		/// <summary>
		/// Filter on Files showing wich Files to ignore
		/// </summary>
		public IReadOnlyCollection<string> FilesToIgnore => Array.AsReadOnly((FilterType == FilterType.ExcludeFiltered ? getTheFilteredFilesByType(_files) : getFilesFilteredByFileTyp(_files)).ToArray());

		/// <summary>
		/// A preview of what the new filenames would look like
		/// </summary>
		public IReadOnlyCollection<RenamedFile> RenamedFiles => throw new NotImplementedException();

		/// <summary>
		/// Determ
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<string> getFilesFilteredByFileTyp(IEnumerable<string> input)
			=> input.Where(f => !FiletypeFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty(string.Empty);

		public IEnumerable<string> getTheFilteredFilesByType(IEnumerable<string> input)
			=> input.Where(f => FiletypeFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty(string.Empty);


		#endregion [ Files ]
	}
}
