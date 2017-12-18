﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;


namespace LaserWar.Global.Converters
{
	/// <summary>
	/// Преобразует Boolean в Visibility
	/// </summary>
	public class BoolToVisibilityConverter : IValueConverter
	{
		bool m_IsInverse = false;
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то true -> UnvisibleValue
		/// </summary>
		public bool IsInverse
		{
			get { return m_IsInverse; }
			set { m_IsInverse = value; }
		}


		Visibility m_UnvisibleValue = Visibility.Collapsed;
		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return m_UnvisibleValue; }
			set { m_UnvisibleValue = value; }
		}


		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (IsInverse)
				return (bool)value ? UnvisibleValue : Visibility.Visible;
			else
				return (bool)value ? Visibility.Visible : UnvisibleValue;
		}


		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return (Visibility)value == (IsInverse ? UnvisibleValue : Visibility.Visible);
		}
	}


	public class InvBoolToVisibilityConverter : IValueConverter
	{
		bool m_IsInverse = false;
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то true -> UnvisibleValue
		/// </summary>
		public bool IsInverse
		{
			get { return m_IsInverse; }
			set { m_IsInverse = value; }
		}


		Visibility m_UnvisibleValue = Visibility.Collapsed;
		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return m_UnvisibleValue; }
			set { m_UnvisibleValue = value; }
		}


		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (IsInverse)
				return !(bool)value ? UnvisibleValue : Visibility.Visible;
			else
				return !(bool)value ? Visibility.Visible : UnvisibleValue;
		}


		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return (Visibility)value == (!IsInverse ? UnvisibleValue : Visibility.Visible);
		}
	}

	/// <summary>
	/// Преобразует Visibility в Boolean
	/// </summary>
	public class VisibilityToBoolConverter : IValueConverter
	{
		bool m_IsInverse = false;
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то UnvisibleValue -> true 
		/// </summary>
		public bool IsInverse
		{
			get { return m_IsInverse; }
			set { m_IsInverse = value; }
		}


		Visibility m_UnvisibleValue = Visibility.Collapsed;
		/// <summary>
		/// Значение, которое используется в качестве Visible = false
		/// </summary>
		public Visibility UnvisibleValue
		{
			get { return m_UnvisibleValue; }
			set { m_UnvisibleValue = value; }
		}


		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return (Visibility)value == (IsInverse ? UnvisibleValue : Visibility.Visible);
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (IsInverse)
				return (bool)value ? UnvisibleValue : Visibility.Visible;
			else
				return (bool)value ? Visibility.Visible : UnvisibleValue;
		}
	}


	/// <summary>
	/// Преобразование int в string и обратно
	/// </summary>
	public class StrToIntConverter : IValueConverter
	{
		public int m_PrevVal = 0;

		/* int -> string */
		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is int)
				return ((int)value).ToString();
			else
				return "-1";
		}

		/* string -> int */
		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			int number;
			if (int.TryParse((string)value, out number) && !((string)value).Contains(' '))
			{
				m_PrevVal = number;
				return number;
			}
			else
				return m_PrevVal;
		}
	}


	/// <summary>
	/// true -> false, false -> true
	/// </summary>
	public class InverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value == null)
				return true;

			if (value is bool?)
				return (bool?)value == null || !((bool?)value).Value;
			else
				return !(bool)value;
		}


		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value == null)
				return true;

			if (value is bool?)
				return (bool?)value == null || !((bool?)value).Value;
			else
				return !(bool)value;
		}
	}


	/// <summary>
	/// bool? -> TrueValue/FalseValue, null -> IfNull
	/// </summary>
	public class NullBoolToBoolConverter : IValueConverter
	{
		bool m_IfTrue = true;
		/// <summary>
		/// true конвертируется в это значение
		/// </summary>
		public bool IfTrue
		{
			get { return m_IfTrue; }
			set { m_IfTrue = value; }
		}


		bool m_IfFalse = false;
		/// <summary>
		/// false конвертируется в это значение
		/// </summary>
		public bool IfFalse
		{
			get { return m_IfFalse; }
			set { m_IfFalse = value; }
		}


		bool m_IfNull = true;
		/// <summary>
		/// null конвертируется в это значение
		/// </summary>
		public bool IfNull
		{
			get { return m_IfNull; }
			set { m_IfNull = value; }
		}

		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value == null)
				return IfNull;

			if (value is bool?)
				return ((bool?)value).Value ? IfTrue : IfFalse;
			else
				return (bool)value ? IfTrue : IfFalse;
		}


		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value == null)
				return IfNull;

			if (value is bool?)
			{
				if (((bool?)value).Value == IfNull)
					return null;

				if (((bool?)value).Value == IfTrue)
					return true;

				if (((bool?)value).Value == IfFalse)
					return false;
			}
			else
			{
				if ((bool)value == IfNull)
					return null;

				if ((bool?)value == IfTrue)
					return true;

				if ((bool?)value == IfFalse)
					return false;
			}

			return null;
		}
	}


	public class EnumConverter : IValueConverter
	{
		bool m_IsEnumToInt = true;
		/// <summary>
		/// 
		/// </summary>
		public bool IsEnumToInt
		{
			get { return m_IsEnumToInt; }
			set { m_IsEnumToInt = value; }
		}

		public object Convert(object value, Type targetType, object parameter,
							  System.Globalization.CultureInfo culture)
		{
			if (IsEnumToInt)
			{
				int returnValue = 0;
				if (parameter is Type)
				{
					returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
				}
				return returnValue;
			}
			else
			{
				Enum enumValue = default(Enum);
				if (parameter is Type)
				{
					enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
				}
				return enumValue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter,
								  System.Globalization.CultureInfo culture)
		{
			if (IsEnumToInt)
			{
				Enum enumValue = default(Enum);
				if (parameter is Type)
				{
					enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
				}
				return enumValue;
			}
			else
			{
				int returnValue = 0;
				if (parameter is Type)
				{
					returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
				}
				return returnValue;
			}
		}
	}
}
