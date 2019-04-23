using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileNumerator.Commands;
using FileNumerator.Models;
using FileNumerator.Views;
using WpfHelperLibrary;
using WpfHelperLibrary.Commands;

namespace FileNumerator.Viewmodels
{
	internal class MainWindowViewmodel : BaseNotifyPropertyChanged, IMainWindowViewmodel
	{
        private Models.FileRenamer _renamer;

		public MainWindowViewmodel()
		{
			instantiateCommands();
            _renamer = new FileRenamer(AppDomain.CurrentDomain.BaseDirectory /*TODO: add the last working dorectory to app.config*/);
		}


		private void instantiateCommands()
		{
			this.SelectDirectory = new OpenDirectroyCommandWithNotification();
            SelectDirectory.DirectorySelected += (_, e) => this.WorkingDirectory = e;
			this.RenameFiles = new GeneralCommand(o => CanExecuteRenameFilest(o), o => ExecuteRenameFiles(o));

		}


		#region  [ Properties ]

		#region [ StartNumber ]

        public int StartNumber
		{
			get
			{
				return _renamer.StartNumber;
			}
			set
			{
				if (_renamer.StartNumber != value)
				{
					_renamer.StartNumber = value;
					RaisePropertyChanged(nameof(StartNumber));
				}
			}
		}

		#endregion [ StartNumber ] 

		#region [ LastNumber ]

        public int? LastNumber
		{
			get
			{
				return _renamer.LastNumber;
			}
			set
			{
				if (_renamer.LastNumber != value)
				{
					_renamer.LastNumber = value;
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

        public string[] DeleteFileendings
		{
			get
			{
				return _renamer.FileEndingsToRemove;
			}
			set
			{
				if (_renamer.FileEndingsToRemove != value)
				{
                    _renamer.FileEndingsToRemove = value;
					RaisePropertyChanged(nameof(DeleteFileendings));
				}
			}
		}

        #endregion [ MyProperty ] 

        #region [ FilterType ]

        public FilterType FilterType
        {
            get
            {
                return _renamer == null? FilterType.ExcludeFiltered : _renamer.FilterType;
            }
            set
            {
                if (_renamer != null && _renamer.FilterType != value)
                {
                    _renamer.FilterType = value;
                    RaisePropertyChanged(nameof(FilterType));
                }
            }
        }

        #endregion [ FilterType ] 

        #region [ WorkingDirectory ]

        public string WorkingDirectory
        {
            get
            {
                return  _renamer.DirectoryToActOn;
            }
            set
            {
                if (_renamer.DirectoryToActOn != value)
                {
                    try
                    {
                        _renamer.DirectoryToActOn = value;
                        _noFiles = false;
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("No files found in directory!");
                        _noFiles = true;
                    }
                    finally
                    {
                        RaisePropertyChanged(
                            nameof(WorkingDirectory),
                            nameof(FoundFiles),
                            nameof(FilesToActOn),
                            nameof(ResultingFilenames)
                        );
                    }
                }
            }
        }

        private bool _noFiles = false;

        #endregion [ WorkingDirectory ] 

        public IEnumerable<string> FoundFiles => _noFiles ? null : _renamer.Files;
        
        public IEnumerable<string> FilesToActOn => _noFiles ? null : _renamer.FilesToActOn;

        public IEnumerable<string> ResultingFilenames => _noFiles ? null : _renamer.PreviewRenamedFiles.Select(f => f.NewPath);


        #endregion [ Properties ]

        #region [ Commands ]

        public OpenDirectroyCommandWithNotification SelectDirectory { get; private set; }

		#region [ RenameFiles ]

		public GeneralCommand RenameFiles { get; private set; }

		/// <param name="directory">Optional directory to use (used for unit testing), else uses the directory specified in <see cref="WorkingDirectory"/></param>
		private bool CanExecuteRenameFilest(object directory = null)
        {
            try
            {
                return (directory as string == null) ? 
                    !string.IsNullOrWhiteSpace(SelectDirectory.SelectedDirectory) : 
                    Directory.Exists(directory as string)
                ;
            }
            catch (DirectoryNotSelectedException)
            {
                return false;
            }

        }

        /// <param name="directory">Optional directory to use (used for unit testing), else uses the directory specified in <see cref="WorkingDirectory"/></param>
        private void ExecuteRenameFiles(object directory = null)
		{
            if (directory != null)
                _renamer.DirectoryToActOn = directory as string;
            _renamer.Rename();
		}

		#endregion [ RenameFiles ]

		#endregion [ Commands ]

		#region [ Explicit IMainWindowViewmodel ]

		ICommand IMainWindowViewmodel.RenameFiles => RenameFiles;

		ICommand IMainWindowViewmodel.SelectDirectory => SelectDirectory;

		IEnumerable<string> IMainWindowViewmodel.IgnorFileType { get => IgnorFileType; set => IgnorFileType = value.ToList(); }

		IEnumerable<string> IMainWindowViewmodel.DeleteFileendings { get => DeleteFileendings; set => DeleteFileendings = value.ToArray(); }

        #endregion [ Explicit IMainWindowViewmodel ]

        /// <summary>
        /// Raises the proeprty changed event for all passed properties
        /// </summary>
        /// <param name="properties"></param>
        public void RaisePropertyChanged(params string[] properties)
            => Array.ForEach(properties, p => base.RaisePropertyChanged(p));
    }
}
