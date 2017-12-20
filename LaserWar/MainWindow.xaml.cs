using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaserWar.Entities;
using sharpPDF;
using System.ComponentModel;
using System.Media;
using System.Threading;
using System.Windows.Threading;
using LaserWar.Global.Converters;
using System.Globalization;
using LaserWar.Global;
using LaserWar.Models;
using LaserWar.Veiws;
using LaserWar.ViewModels;
using LaserWar.Stuff;
using LaserWar.ExtraControls.DialogWnds;
using LaserWar.ExtraControls;


namespace LaserWar
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : CNotifyPropertyChangedWnd
	{
		EntitiesContext m_db = null;
		DataDownloaderViewModel m_DataDownloader = null;
		SoundsViewModel m_Sounds = null;

		
		#region ShowShadow
		private static readonly string ShowShadowPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.ShowShadow);

		private bool m_ShowShadow = false;

		public bool ShowShadow
		{
			get { return m_ShowShadow; }
			set
			{
				if (m_ShowShadow != value)
				{
					m_ShowShadow = value;
					if (!m_ShowShadow)
					{
						ShowProgressShape = false;
						brdShadow.Child = null; // Если убрали затемнение, то никакие эдементы на нём нам больше не нужны
					}
					OnPropertyChanged(ShowShadowPropertyName);
				}
			}
		}
		#endregion

		
		#region ShowProgressShape
		private static readonly string ShowProgressShapePropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.ShowProgressShape);

		private bool m_ShowProgressShape = false;

		public bool ShowProgressShape
		{
			get { return m_ShowProgressShape; }
			set
			{
				if (m_ShowProgressShape != value)
				{
					m_ShowProgressShape = value;
					if (m_ShowProgressShape)
						brdShadow.Child = new ProgressShape();
					else
						brdShadow.Child = null;
					OnPropertyChanged(ShowProgressShapePropertyName);
				}
			}
		}
		#endregion
				
				
		
		public MainWindow()
		{
			InitializeComponent();
			
			GlobalDefines.SuppressWininetBehavior();
						
			try
			{
				m_db = new EntitiesContext();
				m_db.LoadAllDataSets();

				DataDownloaderModel modelDataDownloader = new DataDownloaderModel(m_db,
															LaserWar.Properties.Settings.Default.TaskUrl,
															LaserWar.Properties.Settings.Default.TaskLoaded ? LaserWar.Properties.Settings.Default.TaskJSONText : null);
				m_DataDownloader = new DataDownloaderViewModel(modelDataDownloader);
				m_DataDownloader.DownloadComleted += DataDownloader_DownloadComleted;
				m_DataDownloader.DownloadStarted += DataDownloader_DownloadStarted;
								
				tbctrlPanels.Items.Add(new TabItem()
					{
						Header = new Uri("Images/download.png", UriKind.Relative),
						Content = new DataDownloaderView(m_DataDownloader)
					});

				SoundsModel modelSounds = new SoundsModel(m_db);
				m_Sounds = new SoundsViewModel(modelSounds);
				tbctrlPanels.Items.Add(new TabItem()
				{
					Header = new Uri("Images/sounds.png", UriKind.Relative),
					Content = new SoundsView(m_Sounds)
				});
				tbctrlPanels.Items.Add(new TabItem()
				{
					Header = new Uri("Images/games.png", UriKind.Relative),
				});
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
		
		
		void DataDownloader_DownloadStarted(object sender, EventArgs e)
		{
			ShowShadow = ShowProgressShape = true;
		}


		/// <summary>
		/// JSON-объект загружен => сохраняем информацию в файл настроек
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DataDownloader_DownloadComleted(object sender, DataDownloadComletedEventArgs e)
		{
			if (e.Error == null)
			{
				LaserWar.Properties.Settings.Default.TaskUrl = m_DataDownloader.TaskUrl;
				LaserWar.Properties.Settings.Default.TaskLoaded = m_DataDownloader.IsDataDownloaded;
				LaserWar.Properties.Settings.Default.TaskJSONText = m_DataDownloader.JSONText;

				LaserWar.Properties.Settings.Default.Save();

				ShowShadow = false;
			}
			else
			{	// Не удалось загрузить файл => показываем окно сообщения
				MessageDialog dlg = new MessageDialog(brdShadow)
				{
					Title = Properties.Resources.resErrorOccured,
					Message = string.Format(Properties.Resources.resfmtCantDownloadFile, e.SourceFileName)
				};
				dlg.ButtonClicked += (s, ev) => 
					{
						(s as MessageDialog).RemoveFromHost();
						ShowShadow = false;
					};
			}
		}


		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
			MinWidth = DesiredSize.Width;
			MinHeight = DesiredSize.Height;

			GlobalDefines.AutoscaleTabs(tbctrlPanels, null, null);
		}
	}
}
