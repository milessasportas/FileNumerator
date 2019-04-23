using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            FileExtensionFilter.CollectionChanged += (s, e) =>
            {
                _renamer.FileExtensionFilter = FileExtensionFilter.ToArray();
                this.RaisePropertyChanged(nameof(FilesToActOn), nameof(ResultingFilenames));
            };
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
					RaisePropertyChanged(
                        nameof(StartNumber),
                        nameof(ResultingFilenames)
                    );
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

        /// <summary>
        /// List of FileExtensions which will be filtered on
        /// </summary>
        public ObservableCollection<string> FileExtensionFilter { get; } = new ObservableCollection<string>();


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

        public FilterMode FilterType
        {
            get
            {
                return _renamer == null? FilterMode.ExcludeFiltered : _renamer.FilterType;
            }
            set
            {
                if (_renamer != null && _renamer.FilterType != value)
                {
                    _renamer.FilterType = value;
                    RaisePropertyChanged(
                        nameof(FilterType),
                        nameof(FilesToActOn),
                        nameof(ResultingFilenames)
                    );
                    
                }
            }
        }

        #endregion [ FilterType ] 

        public IEnumerable<FilterMode> PossibleFilters => Array.ConvertAll(Enum.GetNames(typeof(FilterMode)), f => (FilterMode) Enum.Parse(typeof(FilterMode), f));


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
                            nameof(FoundFileextensions),
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

        public IEnumerable<string> FoundFileextensions => _noFiles ? null : _renamer.Files.Select(f => Path.GetExtension(f)).Distinct().OrderBy(f => f[1]);

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
                    !string.IsNullOrWhiteSpace(_renamer.DirectoryToActOn) && _renamer.FilesToActOn.Count != 0: 
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
