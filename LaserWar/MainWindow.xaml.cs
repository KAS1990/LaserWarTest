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
using System.ComponentModel;
using System.Media;
using System.Threading;
using System.Windows.Threading;
using LaserWar.Global.Converters;
using System.Globalization;
using LaserWar.Global;
using LaserWar.Models;
using LaserWar.Views;
using LaserWar.ViewModels;
using LaserWar.Stuff;
using LaserWar.ExtraControls.DialogWnds;
using LaserWar.ExtraControls;
using System.Windows.Controls.Primitives;


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
		GamesViewModel m_Games = null;

		
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
					ShowProgressShapeIfNeeded();
					OnPropertyChanged(ShowProgressShapePropertyName);
				}
			}
		}


		public void ShowProgressShapeIfNeeded()
		{
			if (ShowProgressShape)
				brdShadow.Child = new ProgressShape();
			else
				brdShadow.Child = null;
		}


		public void RemoveProgressShapeIfExisted()
		{
			if (brdShadow.Child is ProgressShape)
				brdShadow.Child = null;
		}
		#endregion
				
				
		
		public MainWindow()
		{
			InitializeComponent();

			LaserWarApp.MainWnd = this;
												
			try
			{
				// Этот Grid необходим для подбора размеров tbctrlPanels
				Grid grd = new Grid();
				grd.ColumnDefinitions.Add(new ColumnDefinition()
					{
						SharedSizeGroup = "PanelCol"
					});
				grd.RowDefinitions.Add(new RowDefinition()
				{
					SharedSizeGroup = "PanelRow"
				});

				m_db = new EntitiesContext();
				m_db.LoadAllDataSets();

				DataDownloaderModel modelDataDownloader = new DataDownloaderModel(m_db,
															LaserWar.Properties.Settings.Default.TaskUrl,
															LaserWar.Properties.Settings.Default.TaskLoaded ? LaserWar.Properties.Settings.Default.TaskJSONText : null);
				m_DataDownloader = new DataDownloaderViewModel(modelDataDownloader);
				m_DataDownloader.DownloadComleted += DataDownloader_DownloadComleted;
				m_DataDownloader.DownloadStarted += DataDownloader_DownloadStarted;

				grd.Children.Add(new DataDownloaderView(m_DataDownloader));
				tbctrlPanels.Items.Add(new TabItem()
					{
						Header = new Uri("Images/download.png", UriKind.Relative),
						Content = grd
					});

				grd = new Grid();
				grd.ColumnDefinitions.Add(new ColumnDefinition()
				{
					SharedSizeGroup = "PanelCol"
				});
				grd.RowDefinitions.Add(new RowDefinition()
				{
					SharedSizeGroup = "PanelRow"
				});
				SoundsModel modelSounds = new SoundsModel(m_db);
				m_Sounds = new SoundsViewModel(modelSounds);
				grd.Children.Add(new SoundsView(m_Sounds));
				tbctrlPanels.Items.Add(new TabItem()
				{
					Header = new Uri("Images/sounds.png", UriKind.Relative),
					Content = grd
				});

				grd = new Grid();
				grd.ColumnDefinitions.Add(new ColumnDefinition()
				{
					SharedSizeGroup = "PanelCol"
				});
				grd.RowDefinitions.Add(new RowDefinition()
				{
					SharedSizeGroup = "PanelRow"
				});
				m_Games = new GamesViewModel(m_db);
				grd.Children.Add(new GamesView(m_Games));
				tbctrlPanels.Items.Add(new TabItem()
				{
					Header = new Uri("Images/games.png", UriKind.Relative),
					Content = grd
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
						ev.Dlg.RemoveFromHost();
						ShowShadow = false;
					};
			}
		}


		/// <summary>
		/// Эти действия необходимы для подбора размеров tbctrlPanels
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbctrlPanels_Loaded(object sender, RoutedEventArgs e)
		{
			//NOTE: loop through tab items to force measurement and size the tab control to the largest tab
			TabControl tabControl = (TabControl)sender;

			// backup selection
			int indexItemLast = tabControl.SelectedIndex;

			int itemCount = tabControl.Items.Count;

			for (
				int indexItem = (itemCount - 1);
				indexItem >= 0;
				indexItem--)
			{
				tabControl.SetCurrentValue(Selector.SelectedIndexProperty, indexItem);
				tabControl.UpdateLayout();
			}

			// restore selection
			tabControl.SetCurrentValue(Selector.SelectedIndexProperty, indexItemLast);

			Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
			MinWidth = DesiredSize.Width;
			MinHeight = DesiredSize.Height;
		}
	}
}
