using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global.Converters;
using System.Globalization;

namespace LaserWar.Veiws.Converters
{
	/// <summary>
	/// Конвертер из размера файла в байтах в строковое представление размера
	/// </summary>
	public class SizeMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				int SizeInBytes = (int)value;
				if (SizeInBytes < 0)
					return "";

				if (SizeInBytes < 1024 * 1.2)
					return SizeInBytes.ToString() + " " + Properties.Resources.resB;
				else if (SizeInBytes < 1024 * 1024 * 1.2)
					return (SizeInBytes / 1024).ToString() + " " + Properties.Resources.resKB;
				else
					return (SizeInBytes / (1024 * 1024)).ToString() + " " + Properties.Resources.resMB;
			}
			else
				return "";
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in SizeMarkupConverter");
		}


		public SizeMarkupConverter() :
			base()
		{
		}
	}
}
