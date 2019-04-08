using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileNumerator.Views;
using WpfHelperLibrary;
using WpfHelperLibrary.Commands;

namespace FileNumerator.Viewmodels
{
	internal class MainWindowViewmodel : BaseNotifyPropertyChanged, IMainWindowViewmodel
	{
		public MainWindowViewmodel()
		{
			instantiateCommands();
		}


		private void instantiateCommands()
		{
			this.SelectDirectory = new OpenDirectoryCommand();
			this.RenameFiles = new GeneralCommand(o => CanExecuteRenameFilest(o), o => ExecuteRenameFiles(o));

		}


		#region  [ Properties ]

		#region [ StartNumber ]

		private int _startNumber;

		public int StartNumber
		{
			get
			{
				return _startNumber;
			}
			set
			{
				if (_startNumber != value)
				{
					_startNumber = value;
					RaisePropertyChanged(nameof(StartNumber));
				}
			}
		}

		#endregion [ StartNumber ] 

		#region [ LastNumber ]

		private int _lastNumber;

		public int LastNumber
		{
			get
			{
				return _lastNumber;
			}
			set
			{
				if (_lastNumber != value)
				{
					_lastNumber = value;
					RaisePropertyChanged(nameof(StartNumber));
				}
			}
		}

		#endregion [ StartNumber ] 

		#region [ IgnorFileType ]

		List<string> _ignorFileType;

		public List<string> IgnorFileType
		{
			get
			{
				return _ignorFileType;
			}
			set
			{
				if (_ignorFileType != value)
				{
					_ignorFileType = value;
					RaisePropertyChanged(nameof(IgnorFileType));
				}
			}
		}

		#endregion [ IgnorFileType ] 

		#region [ DeleteFileendings ]

		List<string> _deleteFileendings;

		public List<string> DeleteFileendings
		{
			get
			{
				return _deleteFileendings;
			}
			set
			{
				if (_deleteFileendings != value)
				{
					_deleteFileendings = value;
					RaisePropertyChanged(nameof(DeleteFileendings));
				}
			}
		}

		#endregion [ MyProperty ] 

		#endregion [ Properties ]

		#region [ Commands ]

		public OpenDirectoryCommand SelectDirectory { get; private set; }

		#region [ RenameFiles ]

		public GeneralCommand RenameFiles { get; private set; }

		private bool CanExecuteRenameFilest(object _)
			=> !string.IsNullOrWhiteSpace(SelectDirectory.SelectedDirectory);

		private void ExecuteRenameFiles(object _)
		{
			throw new NotImplementedException();
		}

		#endregion [ RenameFiles ]

		#endregion [ Commands ]

		#region [ Explicit IMainWindowViewmodel ]

		ICommand IMainWindowViewmodel.RenameFiles => RenameFiles;

		ICommand IMainWindowViewmodel.SelectDirectory => SelectDirectory;

		IEnumerable<string> IMainWindowViewmodel.IgnorFileType { get => IgnorFileType; set => IgnorFileType = value.ToList(); }

		IEnumerable<string> IMainWindowViewmodel.DeleteFileendings { get => DeleteFileendings; set => DeleteFileendings = value.ToList(); }

		#endregion [ Explicit IMainWindowViewmodel ]
	}
}
