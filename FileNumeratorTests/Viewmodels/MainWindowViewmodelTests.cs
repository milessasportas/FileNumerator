using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileNumerator.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileNumerator.Viewmodels.Tests
{
	[TestClass()]
	public class MainWindowViewmodelTests
	{
		private static MainWindowViewmodel viewmodel;
		private static Views.IMainWindowViewmodel interfaceViewmodel;
				
		static MainWindowViewmodelTests()
		{
			//generate the viewmodel b4 all test run
			viewmodel = new MainWindowViewmodel();
			interfaceViewmodel = viewmodel;
		}

		[TestMethod]
		[Priority(1)]
		[Ignore]
		public void ShowSelectDirectoryTest()
		{
			interfaceViewmodel.SelectDirectory.Execute(null);
			Assert.IsNotNull(viewmodel.SelectDirectory);
		}

	}
}