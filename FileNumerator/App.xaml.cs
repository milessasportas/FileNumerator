using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileNumerator
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var window = new Views.MainWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                if(System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                throw;
            }

        }
    }
}
