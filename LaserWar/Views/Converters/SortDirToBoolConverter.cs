using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global.Converters;
using System.Globalization;
using System.ComponentModel;

namespace LaserWar.Views.Converters
{
	/// <summary>
	/// Конвертер направления сортировки в состояния ToggleBotton
	/// </summary>
	public class SortDirToBoolConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is ListSortDirection)
				return (ListSortDirection)value == ListSortDirection.Ascending;
			else
				return false;
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in SortDirToBoolConverter");
		}


		public SortDirToBoolConverter() :
			base()
		{
		}
	}
}
