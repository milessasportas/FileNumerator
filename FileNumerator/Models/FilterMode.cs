using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNumerator.Models
{
	public enum FilterMode
	{
		/// <summary>
		/// The in filter defined file types shall be excluded
		/// </summary>
		ExcludeFiltered,
		/// <summary>
		/// Only use the in filter definied filetypes
		/// </summary>
		IncludeFiltered
	}
}
