using FileNumerator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNumeratorTests.Models
{
	[TestClass]
	public class FileRenamerTests
	{
		private readonly static FileRenamer _renamer;
		private readonly static int _filecount;
		private readonly static int _dllCount;
		private readonly static int _nonDllCount;

		static FileRenamerTests()
		{
			_renamer = generateRenamer();
			_filecount = 18;
			_dllCount = 9;
			_nonDllCount = _filecount - _dllCount;
			_renamer.IgnoredFiletypes = new string[] { ".exe", ".config", ".pdb", ".xml", ".pdf" };
		}

		/// <summary>
		/// Returns a new default renamer instance
		/// </summary>
		/// <returns></returns>
		private static FileRenamer generateRenamer() => new FileRenamer(AppDomain.CurrentDomain.BaseDirectory);

		#region [ Constructor Tests ]

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CtorTestNullDirectory()
		{
			var namer = new FileRenamer(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CtorTestEmptyStringDirectory()
		{
			var namer = new FileRenamer("");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void CtorTestEmptyDirectory()
		{
			var namer = new FileRenamer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "empty"));
		}

		[TestMethod]
		public void CtorTestPropperDirectory()
		{
			var namer = generateRenamer();
			Assert.IsNotNull(namer);
		}

		#endregion [ Constructor Tests ]

		#region [ Files Tests ]
		
		[TestMethod]
		public void FilesCountTest()
		{
			//manually counted
			Assert.AreEqual(_filecount, _renamer.Files.Count);
		}

		[TestMethod]
		public void FilesFilterCountTest()
		{
			Assert.AreEqual(_renamer.FilesToActOn.Count, _dllCount);
		}

		[TestMethod]
		public void FilteredFilesCounterTest()
		{
			Assert.AreEqual(_renamer.FilesToIgnore.Count, _nonDllCount);
		}

		#endregion [ Files Tests ]

		private readonly static string[] mocFiles = { "one.pdf", "two.exe", "three.dll", "four.dll", "fifve.pdb" };
		private readonly static string[] mocNonDllFiles = { "one.pdf", "two.exe", "fifve.pdb" };
		private readonly static string[] mocDllFiles =    { "three.dll", "four.dll", };

		[TestMethod]
		public void FilterMethodActOn()
		{
			var actOn = _renamer.getFilesFilteredByFileTyp(mocFiles);
			var tmp = actOn.ToArray();
			CollectionAssert.AreEquivalent(mocDllFiles, actOn.ToArray());
		}

		[TestMethod]
		public void FilterMethodFiltered()
		{
			var filtered = _renamer.getTheFilteredFilesByType(mocFiles);
			CollectionAssert.AreEquivalent(mocNonDllFiles, filtered.ToArray());
		}

		[TestMethod]
		public void ExclusiveFilterTest()
		{
			var actOn = _renamer.getFilesFilteredByFileTyp(mocFiles);
			var filtered = _renamer.getTheFilteredFilesByType(mocFiles);
			foreach (var s in actOn)
				CollectionAssert.DoesNotContain(filtered.ToArray(), s);
		}

		[TestMethod]
		public void NothingToFilterTest()
		{
			//get renamer ignores all but .dll
			var renamer = generateRenamer();
			var filtered = renamer.getTheFilteredFilesByType(mocNonDllFiles);
			Assert.AreEqual(mocNonDllFiles.Length, filtered.Count());
		}

		[TestMethod]
		public void NothingToActOnTest()
		{
			var renamer = generateRenamer();
			renamer.IgnoredFiletypes = new string[] {".dll" };
			var filtered = renamer.getFilesFilteredByFileTyp(mocDllFiles);
			Assert.IsTrue(string.Join("", filtered.ToArray()) == string.Empty); 
		}

	}
}
