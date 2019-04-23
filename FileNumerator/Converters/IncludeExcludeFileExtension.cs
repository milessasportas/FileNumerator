using FileNumerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileNumerator.Converters
{
	class IncludeExcludeFileExtension : IValueConverter
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            if (! (value is FilterMode))
                throw new NotSupportedException($"{nameof(value)} has to be of type {nameof(FilterMode)}");
            switch ((FilterMode)value)
            {
                case FilterMode.ExcludeFiltered:
                    return "Exclude Fileextension";
                case FilterMode.IncludeFiltered:
                    return "Include Fileextension";
                default:
                    throw new NotImplementedException($"Conversion for {value.ToString()} hasn't been added yet.");
            }
        }

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("Only one way conversion supported");
		}
	}
}
