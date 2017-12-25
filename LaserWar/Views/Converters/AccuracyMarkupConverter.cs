using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global.Converters;
using System.Globalization;

namespace LaserWar.Views.Converters
{
	/// <summary>
	/// Конвертер из точности в долях в проценты
	/// </summary>
	public class AccuracyMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is float)
			{
				return (((float)value) * 100.0).ToString("F0") + Properties.Resources.resPercent;
			}
			else
				return "";
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				string OnlyDigits = value.ToString().Replace(Properties.Resources.resPercent, "").Replace(" ", "");
				float accuracy;
				if (float.TryParse(OnlyDigits, out accuracy))
					return accuracy / 100.0;
				else
					return -1;
			}
			else
				return -1;
		}


		public AccuracyMarkupConverter() :
			base()
		{
		}
	}
}
