using FileNumerator.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileNumerator.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IMainWindowViewmodel viewmodel;

        internal MainWindow() : this(new Viewmodels.MainWindowViewmodel())
        {
        }

		public MainWindow(IMainWindowViewmodel vm)
		{
			viewmodel = vm;
			DataContext = vm;
			InitializeComponent();
		}

        //todo remove
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            viewmodel.FileExtensionFilter.Add((sender as CheckBox).Content.ToString());
        }

        //todo remove
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            viewmodel.FileExtensionFilter.Remove((sender as CheckBox).Content.ToString());
        }

        private void ListView_UpdateSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            var listview = sender as ListView;
            listview.ScrollIntoView(listview.SelectedItem);

        }
    }
}
