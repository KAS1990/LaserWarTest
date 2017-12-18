using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using LaserWar.Global;

namespace LaserWar
{
	/// <summary>
	/// Interaction logic for LaserWarApp.xaml
	/// </summary>
	public partial class LaserWarApp : Application
	{
		public static MainWindow MainWnd = null;

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += DumpMaker.CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.FirstChanceException += (source, ev) =>
			{
				ev.ToString();
			};
		}
	}
}
