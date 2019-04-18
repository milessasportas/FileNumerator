using FileNumerator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileNumerator.Views
{
	public interface IMainWindowViewmodel : INotifyPropertyChanged
	{
		/// <summary>
		/// Command wich starts the Enumeration
		/// </summary>
		ICommand RenameFiles { get; }

		/// <summary>
		/// Coomand wich SelectsTheDirectory
		/// </summary>
		ICommand SelectDirectory { get; }

		/// <summary>
		/// 
		/// </summary>
		int StartNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		int? LastNumber { get; set; }

		/// <summary>
		/// The to ignored filetyped e.g. .pdf
		/// </summary>
		IEnumerable<string> IgnorFileType { get; set; }

		/// <summary>
		/// The to removed fileendings e.g. -final
		/// </summary>
		IEnumerable<string> DeleteFileendings { get; set; }

        /// <summary>
        /// List of found files in the directory
        /// </summary>
        IEnumerable<string> FoundFiles { get; }

        /// <summary>
        /// List of files which names will change
        /// </summary>
        IEnumerable<string> FilesToActOn { get; }

        /// <summary>
        /// What the filenames will look like once renamed
        /// </summary>
        IEnumerable<string> ResultingFilenames { get; }

		/// <summary>
		/// T
		/// </summary>
		FilterType FilterType { get; set; }
	}
}
