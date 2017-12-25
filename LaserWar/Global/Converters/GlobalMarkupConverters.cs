﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace LaserWar.Global.Converters
{
	/// <summary>
	/// Базовый класс для всех конверторов, который позволяет не создавать ресурсов, а использовать их как расширение разметки XAML
	/// </summary>
	public abstract class MarkupConverterBase : MarkupExtension, IValueConverter
	{
		/// <summary>
		/// Override if needed.
		/// </summary>
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (_convToDelegate != null)
				return _convToDelegate.Convert(value, targetType, parameter, culture);
			else
				throw new NotImplementedException("MarkupConverterBase.Convert");
		}

		/// <summary>
		/// Override if needed.
		/// </summary>
		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (_convToDelegate != null)
				return _convToDelegate.ConvertBack(value, targetType, parameter, culture);
			else
				throw new NotImplementedException("MarkupConverterBase.ConvertBack");
		}

		#region MarkupExtension members

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		/// <summary>
		/// Чтобы не было дублирования кода конвертора
		/// и можно было использовать конвертор как в виде расширения разметки и ресурса,
		/// данный класс будет передавать все данные на обработку обычному конвертору, который и будет содержаться в этом поле
		/// </summary>
		protected IValueConverter _convToDelegate = null;

		public MarkupConverterBase()
		{
		}

		#endregion
	}


	/// <summary>
	/// Базовый класс для всех конверторов, который позволяет не создавать ресурсов, а использовать их как расширение разметки XAML
	/// </summary>
	public abstract class MarkupMultiConverterBase : MarkupExtension, IMultiValueConverter
	{
		public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (_convToDelegate != null)
				return _convToDelegate.Convert(values, targetType, parameter, culture);
			else
				throw new NotImplementedException("MarkupMultiConverterBase.Convert");
		}

		public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			if (_convToDelegate != null)
				return _convToDelegate.ConvertBack(value, targetTypes, parameter, culture);
			else
				throw new NotImplementedException("MarkupMultiConverterBase.ConvertBack");
		}


		#region MarkupExtension members

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		/// <summary>
		/// Чтобы не было дублирования кода конвертора
		/// и можно было использовать конвертор как в виде расширения разметки и ресурса,
		/// данный класс будет передавать все данные на обработку обычному конвертору, который и будет содержаться в этом поле
		/// </summary>
		protected IMultiValueConverter _convToDelegate = null;

		public MarkupMultiConverterBase()
		{
		}

		#endregion
	}


	/// <summary>
	/// Преобразует Boolean в Visibility
	/// </summary>
	public class BoolToVisibilityMarkupConverter : MarkupConverterBase
	{
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то true -> UnvisibleValue
		/// </summary>
		public bool IsInverse
		{
			get { return (_convToDelegate as BoolToVisibilityConverter).IsInverse; }
			set { (_convToDelegate as BoolToVisibilityConverter).IsInverse = value; }
		}


		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return (_convToDelegate as BoolToVisibilityConverter).UnvisibleValue; }
			set { (_convToDelegate as BoolToVisibilityConverter).UnvisibleValue = value; }
		}


		public BoolToVisibilityMarkupConverter()
		{
			_convToDelegate = new BoolToVisibilityConverter();
		}
	}


	public class InvBoolToVisibilityMarkupConverter : MarkupConverterBase
	{
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то true -> UnvisibleValue
		/// </summary>
		public bool IsInverse
		{
			get { return (_convToDelegate as InvBoolToVisibilityConverter).IsInverse; }
			set { (_convToDelegate as InvBoolToVisibilityConverter).IsInverse = value; }
		}


		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return (_convToDelegate as InvBoolToVisibilityConverter).UnvisibleValue; }
			set { (_convToDelegate as InvBoolToVisibilityConverter).UnvisibleValue = value; }
		}


		public InvBoolToVisibilityMarkupConverter()
		{
			_convToDelegate = new InvBoolToVisibilityConverter();
		}
	}

	/// <summary>
	/// Преобразует Visibility в Boolean
	/// </summary>
	public class VisibilityToBoolMarkupConverter : MarkupConverterBase
	{
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то UnvisibleValue -> true 
		/// </summary>
		public bool IsInverse
		{
			get { return (_convToDelegate as VisibilityToBoolConverter).IsInverse; }
			set { (_convToDelegate as VisibilityToBoolConverter).IsInverse = value; }
		}


		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return (_convToDelegate as VisibilityToBoolConverter).UnvisibleValue; }
			set { (_convToDelegate as VisibilityToBoolConverter).UnvisibleValue = value; }
		}


		public VisibilityToBoolMarkupConverter()
		{
			_convToDelegate = new VisibilityToBoolConverter();
		}
	}


	/// <summary>
	/// Преобразование int в string и обратно
	/// </summary>
	public class StrToIntMarkupConverter : MarkupConverterBase
	{
		public StrToIntMarkupConverter()
		{
			_convToDelegate = new StrToIntConverter();
		}
	}


	/// <summary>
	/// true -> false, false -> true
	/// </summary>
	public class InverseBoolMarkupConverter : MarkupConverterBase
	{
		public InverseBoolMarkupConverter()
		{
			_convToDelegate = new InverseBoolConverter();
		}
	}


	/// <summary>
	/// bool? -> TrueValue/FalseValue, null -> IfNull
	/// </summary>
	public class NullBoolToBoolMarkupConverter : MarkupConverterBase
	{
		#region Свойство IfTrue
		/// <summary>
		/// true конвертируется в это значение
		/// </summary>
		public bool IfTrue
		{
			get { return (_convToDelegate as NullBoolToBoolConverter).IfTrue; }
			set { (_convToDelegate as NullBoolToBoolConverter).IfTrue = value; }
		}
		#endregion

		#region Свойство IfFalse
		/// <summary>
		/// false конвертируется в это значение
		/// </summary>
		public bool IfFalse
		{
			get { return (_convToDelegate as NullBoolToBoolConverter).IfFalse; }
			set { (_convToDelegate as NullBoolToBoolConverter).IfFalse = value; }
		}
		#endregion

		#region Свойство IfNull
		/// <summary>
		/// null конвертируется в это значение
		/// </summary>
		public bool IfNull
		{
			get { return (_convToDelegate as NullBoolToBoolConverter).IfNull; }
			set { (_convToDelegate as NullBoolToBoolConverter).IfNull = value; }
		}
		#endregion


		public NullBoolToBoolMarkupConverter()
		{
			_convToDelegate = new NullBoolToBoolConverter();
		}
	}


	public class EnumMarkupConverter : MarkupConverterBase
	{
		public bool IsEnumToInt
		{
			get { return (_convToDelegate as EnumConverter).IsEnumToInt; }
			set { (_convToDelegate as EnumConverter).IsEnumToInt = value; }
		}


		public EnumMarkupConverter()
		{
			_convToDelegate = new EnumConverter();
		}
	}


	/// <summary>
	/// Color -> SolidColorBrush и обратно
	/// </summary>
	public class ColorToSolidBrushMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			try
			{
				if (value != null && value is Color)
					return new SolidColorBrush((Color)value);
				else
					return Brushes.Transparent;
			}
			catch (Exception ex)
			{
				ex.ToString();
				return null;
			}
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			try
			{
				if (value != null && value is SolidColorBrush)
					return (value as SolidColorBrush).Color;
				else
					return Colors.Transparent;
			}
			catch (Exception ex)
			{
				ex.ToString();
				return null;
			}
		}


		public ColorToSolidBrushMarkupConverter() :
			base()
		{
		}
	}


	/// <summary>
	/// Проверяет правильность заполнения полей типа int
	/// </summary>
	public class IntParseMarkupConverter : MarkupConverterBase
	{
		/// <summary>
		/// int -> string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value != null && value is int)
				return value.ToString();
			else
				return "";
		}


		/// <summary>
		/// string -> int
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			int i = 0;
			if (value != null && int.TryParse(value.ToString(), out i))
				return i;
			else
				return -1;
		}


		public IntParseMarkupConverter() :
			base()
		{
		}
	}


	/// <summary>
	/// Проверяет правильность заполнения полей типа float
	/// </summary>
	public class FloatParseMarkupConverter : MarkupConverterBase
	{
		/// <summary>
		/// float -> string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value != null && value is float)
			{	// В parameter находится формат вывода числа
				try
				{
					return parameter != null ? ((float)value).ToString(parameter.ToString()) : value.ToString();
				}
				catch
				{
					return "";
				}
			}
			else
				return "";
		}


		/// <summary>
		/// string -> float
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			float i = 0;
			if (value != null && float.TryParse(value.ToString(), out i))
				return i;
			else
				return -1;
		}


		public FloatParseMarkupConverter() :
			base()
		{
		}
	}
}
