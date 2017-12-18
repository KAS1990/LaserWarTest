using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows;

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
	}
}
