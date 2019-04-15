using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		/// <param name="fileendingsToRemove">The fileending that shall be deleted from the filename (e.g. "-final")</param>
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

		#region [ Properties ]

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
				//check to see if directory is empty or null
				if (string.IsNullOrEmpty(value))
						throw new ArgumentException("The directory can't be null or empty.", nameof(DirectoryToActOn));

				//then reset everything only if directory exists
				if (_directory != value)
				{
					//check the directory
					if (!Directory.Exists(value))
						throw new ArgumentException($"The directory \"{value}\" doesn't seem to exist.", nameof(DirectoryToActOn));

					//check if any files exists
					var files = Directory.GetFiles(value);
					if (files.Length == 0)
						throw new FileNotFoundException($"No files found in Directory \"{value}\".");

					//set the fields
					_files = files;
					_directory = value;
					//reset the renamed files
					_previewRenamedFiles = null;
				}
			}
		}

		#endregion [ DirectoryToActOn ]

		/// <summary>
		/// The fileending that shall be deleted from the filename (e.g. "-final")
		/// </summary>
		public string[] FileEndingsToRemove { get; set; }

		public string[] FiletypeFilter { get; set; }

		public FilterType FilterType { get; set; }

		public int StartNumber { get; set; }

		public int? LastNumber { get; set; }

		private string[] _files;

		/// <summary>
		/// All the files found in the Directory
		/// </summary>
		public IReadOnlyList<string> Files => Array.AsReadOnly(_files);

		/// <summary>
		/// Filter on Files showing wich Files to act on
		/// </summary>
		public IReadOnlyCollection<string> FilesToActOn => Array.AsReadOnly((FilterType == FilterType.ExcludeFiltered ? GetFittingFilesFilteredByFileTyp(_files) : GetTheFilteredFilesByFileType(_files)).ToArray());

		/// <summary>
		/// Filter on Files showing wich Files to ignore
		/// </summary>
		public IReadOnlyCollection<string> FilesToIgnore => Array.AsReadOnly((FilterType == FilterType.ExcludeFiltered ? GetTheFilteredFilesByFileType(_files) : GetFittingFilesFilteredByFileTyp(_files)).ToArray());

		#region [ PreviewRenamedFiles ]

		private RenamedFile[] _previewRenamedFiles;

		/// <summary>
		/// A preview of what the new filenames would look like
		/// </summary>
		public IReadOnlyCollection<RenamedFile> PreviewRenamedFiles
		{
			get
			{
				//if already computed return the preview
				if (_previewRenamedFiles != null)	//note: gets reset to null if Directory gets set
					return _previewRenamedFiles;

				//else compute it
				//first retrieve the files we shall act on
				var actOn = Array.ConvertAll(FilesToActOn.ToArray(), f => new FileInfo(f))
								 //todo: add possibility for other search obtions in methods
								 .OrderBy(f => f.CreationTimeUtc).
								 /*Todo: test to see if toarray makes it go faster with large files*/
								 ToArray();
				//start the restult
				RenamedFile[] result = new RenamedFile[actOn.Length];

				//determin the amount of preceeding zeros
				int preceedingZeros = actOn.Length.ToString().Length;

				//start "reanming" the files
				for (int i = 0; i < actOn.Length;  i++ )
				{
					var file = actOn[i];
					result[i].OldPath = actOn[i].FullName;
					//explenation: use the index, and display the element counts number and increase it there
					result[i].NewPath = Path.Combine(actOn[i].DirectoryName, $"{(i + 1).ToString().PadLeft(preceedingZeros, '0')} - {Rename(actOn[i].FullName)}");
					//actOn[i].name
				}

				return Array.AsReadOnly(result);
			}
		} 

		#endregion [ PreviewRenamedFiles ]

		#endregion [ Properties ]

		/// <summary>
		/// Applies the sepcified filter (<see cref="FiletypeFilter"/>) to the Enumerable input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<string> GetFittingFilesFilteredByFileTyp(IEnumerable<string> input)
			=> input.Where(f => !FiletypeFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty(string.Empty);

		/// <summary>
		/// Applies the sepcified filter (<see cref="FiletypeFilter"/>) to the Enumerable input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<string> GetTheFilteredFilesByFileType(IEnumerable<string> input)
			=> input.Where(f => FiletypeFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty(string.Empty);

		/// <summary>
		/// Renames the files 
		/// </summary>
		public void Rename()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Strips the passed filename of any unwanted ending strings
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string Rename(string file)
			=> Rename(new FileInfo(file));

		/// <summary>
		/// Strips the passed filename of any unwanted ending strings
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string Rename(FileInfo file)
		{
			string filenameWithoutEnding = file.NameWithoutFileExtension();

			//maybe regex array to improve speed, could be tested 
			//e.g. Regex[] replacements = Array.ConvertAll(FileEndingsToRemove, e => new Regex(Regex.Escape(e, RegexOptions.IgnoreCase));
			foreach (var toremove in FileEndingsToRemove)
				filenameWithoutEnding = Regex.Replace(filenameWithoutEnding, $"{Regex.Escape(toremove)}$", "", RegexOptions.IgnoreCase);

			return filenameWithoutEnding + file.Extension;
		}
	}

}
