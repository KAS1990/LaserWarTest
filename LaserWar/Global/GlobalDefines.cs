using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace LaserWar.Global
{
	public static class GlobalDefines
	{
		public const string RUSSIAN_CULTURE_NAME = "ru-RU";

		/// <summary>
		/// Аргумент для функции FrameworkElement.Measure()
		/// </summary>
		public static readonly Size STD_SIZE_FOR_MEASURE = new Size(double.PositiveInfinity, double.PositiveInfinity);


		[DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
		/// <summary>
		/// Отключаем COOKIE
		/// </summary>
		public static unsafe void SuppressWininetBehavior()
		{
			const int INTERNET_SUPPRESS_COOKIE_PERSIST = 3;
			const int INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81;

			/* SOURCE: http://msdn.microsoft.com/en-us/library/windows/desktop/aa385328%28v=vs.85%29.aspx
				* INTERNET_OPTION_SUPPRESS_BEHAVIOR (81):
				*      A general purpose option that is used to suppress behaviors on a process-wide basis. 
				*      The lpBuffer parameter of the function must be a pointer to a DWORD containing the specific behavior to suppress. 
				*      This option cannot be queried with InternetQueryOption. 
				*      
				* INTERNET_SUPPRESS_COOKIE_PERSIST (3):
				*      Suppresses the persistence of cookies, even if the server has specified them as persistent.
				*      Version:  Requires Internet Explorer 8.0 or later.
				*/

			int option = INTERNET_SUPPRESS_COOKIE_PERSIST;
			int* optionPtr = &option;

			bool success = InternetSetOption(0, INTERNET_OPTION_SUPPRESS_BEHAVIOR, new IntPtr(optionPtr), sizeof(int));
			if (!success)
			{
				//Something went wrong
			}
		}


		public static string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> property)
		{
			UnaryExpression convertExpression = property.Body as UnaryExpression;
			if (convertExpression != null)
				return ((MemberExpression)convertExpression.Operand).Member.Name;

			return ((MemberExpression)property.Body).Member.Name;
		}


		/// <summary>
		/// Функция масштабирует <paramref name="tbctrl"/> таким  образом, чтобы он имел
		/// высоту и ширину, равные высоте и ширине одной из вкладок.
		/// Для ширины и высоты могут быть заданы разные вкладки.
		/// </summary>
		/// <param name="tbctrl">
		/// Набор вкладок. Вкладки должны содержать в свойстве Content элемент типа CSettingsTabBase.
		/// </param>
		/// <param name="WidthPatternTab">
		/// Вкладка, по которой устанавливается ширина всех остальных. Если null, то выбирается ширины максимально широкой вкладки
		/// </param>
		/// <param name="HeightPatternTab">
		/// Вкладка, по которой устанавливается высота всех остальных. Если null, то выбирается высота максимально высокой вкладки
		/// </param>
		public static void AutoscaleTabs(TabControl tbctrl, FrameworkElement WidthPatternTab, FrameworkElement HeightPatternTab)
		{
			double MaxWidth = 0, MaxHeight = 0;

			if (WidthPatternTab == null && HeightPatternTab == null)
			{
				for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
				{
					if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
					{
						((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
						if (MaxWidth < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width)
							MaxWidth = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width;
						if (MaxHeight < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height)
							MaxHeight = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height;
					}
				}
			}
			else
				if (WidthPatternTab == null)
				{	/* HeightPatternTab != null => определяем ширину как максимум из всех, а высоту - высоту вкладки HeightPatternTab */
					HeightPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
					MaxHeight = HeightPatternTab.DesiredSize.Height;
					for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
					{
						if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
						{
							((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
							if (MaxWidth < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width)
								MaxWidth = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width;
						}
					}
				}
				else
					if (HeightPatternTab == null)
					{	/* WidthPatternTab != null => определяем высоту как максимум из всех, а ширину - ширину вкладки HeightPatternTab */
						WidthPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
						MaxWidth = WidthPatternTab.DesiredSize.Height;
						for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
						{
							if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
							{
								((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
								if (MaxHeight < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height)
									MaxHeight = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height;
							}
						}
					}
					else
					{	/* (WidthPatternTab != null && HeightPatternTab != null) */
						HeightPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
						MaxHeight = HeightPatternTab.DesiredSize.Height;
						if (HeightPatternTab != WidthPatternTab)
							WidthPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
						MaxWidth = WidthPatternTab.DesiredSize.Width;
					}

			if (!((MaxWidth < double.MinValue) || (MaxHeight < double.MinValue)))// если измерения были произведены
				for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
				{
					if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
					{
						((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Width = MaxWidth;
						((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Height = MaxHeight;
					}
				}
		}
	}
}
