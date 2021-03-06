﻿using System;
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
		public FileRenamer(string directory, string[] fileendingsToRemove, params string[] ignoredFiletypes) : this(directory, fileendingsToRemove, ignoredFiletypes, FilterMode.ExcludeFiltered)
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
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterMode filterType) : this(directory, fileendingsToRemove, filetypeFilter, filterType, 1)
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
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterMode filterType, int startNumber) : this(directory, fileendingsToRemove, filetypeFilter, filterType, startNumber, null)
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
		public FileRenamer(string directory, string[] fileendingsToRemove, string[] filetypeFilter, FilterMode filterType, int startNumber, int? lastNumber)
		{
			DirectoryToActOn = directory;
			FileEndingsToRemove = fileendingsToRemove;
			FileExtensionFilter = filetypeFilter;
			FilterType = filterType;
			_startNumber = startNumber;
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
					var files = Directory.GetFiles(value, "*.*", SearchOption.AllDirectories /*todo make selection*/);
					if (files.Length == 0)
						throw new FileNotFoundException($"No files found in Directory \"{value}\".");

                    /*todo: not only by date*/
                    var filesOrderdByDate = files.Select(f => new FileInfo(f)).OrderBy(fi => fi.CreationTime);

					//set the fields
					_files = filesOrderdByDate.Select(f => f.FullName).ToArray();
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

		public string[] FileExtensionFilter { get; set; }

		public FilterMode FilterType { get; set; }
		
        private int _startNumber;

        public int StartNumber
        {
            get { return _startNumber; }
            set
            {
                _startNumber = value;
                _previewRenamedFiles = null;
            }
        }


        public int? LastNumber { get; set; }

		private string[] _files;

		/// <summary>
		/// All the files found in the Directory
		/// </summary>
		public IReadOnlyList<string> Files => Array.AsReadOnly(_files);

		/// <summary>
		/// Filter on Files showing wich Files to act on
		/// </summary>
		public IReadOnlyCollection<string> FilesToActOn => Array.AsReadOnly((FilterType == FilterMode.ExcludeFiltered ? GetFittingFilesFilteredByFileTyp(_files) : GetTheFilteredFilesByFileType(_files)).ToArray());

		/// <summary>
		/// Filter on Files showing wich Files to ignore
		/// </summary>
		public IReadOnlyCollection<string> FilesToIgnore => Array.AsReadOnly((FilterType == FilterMode.ExcludeFiltered ? GetTheFilteredFilesByFileType(_files) : GetFittingFilesFilteredByFileTyp(_files)).ToArray());

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
					result[i].NewPath = Path.Combine(actOn[i].DirectoryName, $"{(i + StartNumber).ToString().PadLeft(preceedingZeros, '0')} - {Rename(actOn[i].FullName)}");
				}

				return Array.AsReadOnly(result);
			}
		} 

		#endregion [ PreviewRenamedFiles ]

		#endregion [ Properties ]

		/// <summary>
		/// Applies the sepcified filter (<see cref="FileExtensionFilter"/>) to the Enumerable input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<string> GetFittingFilesFilteredByFileTyp(IEnumerable<string> input)
			=>  returnEmptyArrayIfPassedArrayIsEmptyOrFilledWithDefaultValues(
                input.Where(f => !FileExtensionFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty());

        /// <summary>
        /// Test if the passed enumerable has any elements or only default values.<para/>
        /// Returns an empty array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        private IEnumerable<T> returnEmptyArrayIfPassedArrayIsEmptyOrFilledWithDefaultValues<T>(IEnumerable<T> enumerable)
            => emptyOrOnlyDefaultValue(enumerable)? new T[0] : enumerable;

        private bool emptyOrOnlyDefaultValue<T>(IEnumerable<T> enumerable)
        {
            if (enumerable.Count() == 0)
                return true;

            //act diffrently for diffrent types
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    return enumerable.All(s => string.IsNullOrWhiteSpace(s as string));
                
                case TypeCode.Object:
                    return enumerable.All(e => e == null || e.Equals(default(T)));

                case TypeCode.Boolean:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                default:
                    return enumerable.All(e => e.Equals(default(T)));
            }
        }

		/// <summary>
		/// Applies the sepcified filter (<see cref="FileExtensionFilter"/>) to the Enumerable input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<string> GetTheFilteredFilesByFileType(IEnumerable<string> input)
			=> returnEmptyArrayIfPassedArrayIsEmptyOrFilledWithDefaultValues(
                input.Where(f => FileExtensionFilter.Any(t => f.EndsWith(t, StringComparison.OrdinalIgnoreCase))).DefaultIfEmpty(string.Empty));

		/// <summary>
		/// Renames the files 
		/// </summary>
		/// <exception cref="RenameException"/>
		public void Rename()
		{
			int index = 0;
			try
			{
				//try to rename all the files
				while(index < PreviewRenamedFiles.Count)
					File.Move(PreviewRenamedFiles.ElementAt(index).OldPath, PreviewRenamedFiles.ElementAt(index++).NewPath);
			}
			catch (Exception ex)
			{
				//the  file at position index threw an error --> use the one before that
				int failedIndex = --index; //-- at start because index gets incremented even when exception is thrown
				List<RenamedFile> failedResettFiles = new List<RenamedFile>();

				//and reset all previous files
				while(index > 0)
				{
					try
					{
						File.Move(PreviewRenamedFiles.ElementAt(--index).NewPath, PreviewRenamedFiles.ElementAt(index).OldPath);
					}
					catch (Exception)
					{
						failedResettFiles.Add(PreviewRenamedFiles.ElementAt(index + 1));
					}
				}

				//build the exception message
				var exeptionMessageBuilder = new StringBuilder();
				if(failedResettFiles.Count > 0)
				{
					bool multiple/*Failed*/ = failedResettFiles.Count > 1;
					exeptionMessageBuilder.AppendLine($"Renaming failed for \"{PreviewRenamedFiles.ElementAt(failedIndex)}\".");
					exeptionMessageBuilder.AppendLine($"The following file{(multiple? "s": "")} couldn't be reset to {(multiple? "their" : "it's")} original name: \"{string.Join("\", \"", failedResettFiles)}\"");
					exeptionMessageBuilder.Append("Refer to inner exceotion for further details.");
				}
				else
					exeptionMessageBuilder.Append($"Renaming failed for \"{PreviewRenamedFiles.ElementAt(failedIndex)}\", no filenames where changed. Refer to inner exceotion for further details.");


				throw new RenameException(
					exeptionMessageBuilder.ToString(),
					ex
				) { FailedFile = PreviewRenamedFiles.ElementAt(failedIndex).OldPath };
			}
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
