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
		ICommand Start { get; }

		/// <summary>
		/// Coomand wich SelectsTheDirectory
		/// </summary>
		ICommand SelectDirectory { get; }

		/// <summary>
		/// 
		/// </summary>
		int StartNumber { get; }

		/// <summary>
		/// 
		/// </summary>
		int LastNumber { get; }

		/// <summary>
		/// The to ignored filetyped e.g. .pdf
		/// </summary>
		IEnumerable<string> IgnorFileType { get; }

		/// <summary>
		/// The to removed fileendings e.g. -final
		/// </summary>
		IEnumerable<string> DeleteFileendings { get; }

	}
}
