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
			_renamer.FileExtensionFilter = new string[] { ".exe", ".config", ".pdb", ".xml", ".pdf" };
			_renamer.FileEndingsToRemove = new string[] { "-part", "-final", "-edit" };
		}

		/// <summary>
		/// Returns a new default renamer instance
		/// </summary>
		/// <returns></returns>
		private static FileRenamer generateRenamer() => new FileRenamer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles"));

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
		private readonly static string[] mocDllFiles = { "three.dll", "four.dll", };
		private readonly static string[] mocFilesFunnyEndings = { "one-final.pdf", "two-part.exe", "three-edit.dll", "four.dll", "fifve.pdb" };


		#region [ Fiter Test ]

		[TestMethod]
		public void FilterMethodActOn()
		{
			var actOn = _renamer.GetFittingFilesFilteredByFileTyp(mocFiles);
			var tmp = actOn.ToArray();
			CollectionAssert.AreEquivalent(mocDllFiles, actOn.ToArray());
		}

		[TestMethod]
		public void FilterMethodFiltered()
		{
			var filtered = _renamer.GetTheFilteredFilesByFileType(mocFiles);
			CollectionAssert.AreEquivalent(mocNonDllFiles, filtered.ToArray());
		}

		[TestMethod]
		public void ExclusiveFilterTest()
		{
			var actOn = _renamer.GetFittingFilesFilteredByFileTyp(mocFiles);
			var filtered = _renamer.GetTheFilteredFilesByFileType(mocFiles);
			foreach (var s in actOn)
				CollectionAssert.DoesNotContain(filtered.ToArray(), s);
		}

		[TestMethod]
		public void NothingToFilterTest()
		{
			//get renamer ignores all but .dll
			var renamer = generateRenamer();
			var filtered = renamer.GetTheFilteredFilesByFileType(mocNonDllFiles);
			Assert.AreEqual(mocNonDllFiles.Length, filtered.Count());
		}

		[TestMethod]
		public void NothingToActOnTest()
		{
			var renamer = generateRenamer();
			renamer.FileExtensionFilter = new string[] { ".dll" };
			var filtered = renamer.GetFittingFilesFilteredByFileTyp(mocDllFiles);
			Assert.IsTrue(string.Join("", filtered.ToArray()) == string.Empty);
		}

		#endregion [ Fiter Test ]

		#region [ FilterTypeTests ]

		[TestMethod]
		public void FilterInclusive()
		{
			var renamer = generateRenamer();
			renamer.FileExtensionFilter = new[] { ".dll" };
			Assert.AreEqual(_nonDllCount, renamer.FilesToIgnore.Count);
		}

		[TestMethod]
		public void FilterExclusive()
		{
			var renamer = generateRenamer();
			renamer.FileExtensionFilter = new[] { ".dll" };
			renamer.FilterType = FilterMode.IncludeFiltered;
			Assert.AreEqual(_dllCount, renamer.FilesToIgnore.Count);
		}

		#endregion [ FilterTypeTests ]

		#region [ RenameTests ]

		[TestMethod]
		public void RemoveFileEndingsTests()
		{
			var result = Array.ConvertAll(mocFilesFunnyEndings, f => _renamer.Rename(f));
			CollectionAssert.AreEquivalent(mocFiles, result);
		}

		[TestMethod]
		public void RemoveFileEndingsCaseInsensitiveTests()
		{
			var result = Array.ConvertAll(mocFilesFunnyEndings, f => _renamer.Rename(f.ToUpper()).ToLower());
			CollectionAssert.AreEquivalent(mocFiles, result);
		}

		[ExpectedException(typeof(RenameException))]
		[TestMethod]
		public void LockAFileRenameTest()
		{
			//open the file so that only this method can use it
			using (var stream = File.Open(_renamer.PreviewRenamedFiles.ElementAt(2).OldPath, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				//try to rename all files
				_renamer.Rename();
			}
		}

		[TestMethod]
		public void RenameSucceed()
		{
			//rename the files
			_renamer.Rename();
			//analyse the renamed files
			var checkRenamer = generateRenamer();
			checkRenamer.FilterType = FilterMode.IncludeFiltered;
			checkRenamer.FileExtensionFilter = new string[] { ".dll" };

			Action<RenamedFile> reset = f => File.Move(f.NewPath, f.OldPath);
			try
			{
				var filesnmes = expectedFilenames(_renamer.PreviewRenamedFiles.ToArray());
				CollectionAssert.AreEquivalent(filesnmes, checkRenamer.FilesToActOn.ToArray());
			}
			finally
			{
				//reset the progress 
				foreach (var file in _renamer.PreviewRenamedFiles)
					reset(file);
			}
		}

		#endregion [ RenameTests ]

		private List<string> expectedFilenames(RenamedFile[] files)
		{
			var expectedResult = new List<string>(files.Length);
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo f = new FileInfo(files[i].OldPath);
				expectedResult.Add($"{f.DirectoryName}\\{i + 1} - {f.Name}");
			}

			return expectedResult;
		}

		#region [ PreviewRenameTests ]

		[TestMethod]
		public void TestGeneratedNames()
		{
			var files = _renamer.PreviewRenamedFiles.ToArray();
			var renamedNames = _renamer.PreviewRenamedFiles.Select(p => p.NewPath).ToArray();

			CollectionAssert.AreEqual(expectedFilenames(files), renamedNames);
		}

		#endregion [ PreviewRenameTests ]
	}
}
