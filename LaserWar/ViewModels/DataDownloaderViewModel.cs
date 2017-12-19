using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using LaserWar.Models;
using System.Windows.Input;
using LaserWar.Commands;
using LaserWar.Stuff;


namespace LaserWar.ViewModels
{
	/// <summary>
	/// 
	/// </summary>
	public class DataDownloaderViewModel: Notifier
	{
		readonly DataDownloaderModel m_model = null;

		#region События
		public event EventHandler DownloadStarted;
		private void OnDownloadStarted(EventArgs e)
		{
			if (DownloadStarted != null)
				DownloadStarted(this, e);

			OnPropertyChanged(IsDataDownloadedPropertyName);
			OnPropertyChanged(InDataDownloadingPropertyName);
			OnPropertyChanged(CanDownloadPropertyName);
		}


		public event EventHandler<DataDownloadComletedEventArgs> DownloadComleted;
		private void OnDownloadComleted(DataDownloadComletedEventArgs e)
		{
			if (DownloadComleted != null)
				DownloadComleted(this, e);

			OnPropertyChanged(IsDataDownloadedPropertyName);
			OnPropertyChanged(InDataDownloadingPropertyName);
			OnPropertyChanged(CanDownloadPropertyName);
		}
		#endregion


		#region TaskUrl
		public static readonly string TaskUrlPropertyName = GlobalDefines.GetPropertyName<DataDownloaderViewModel>(m => m.TaskUrl);

		/// <summary>
		/// Адрес файла
		/// </summary>
		public string TaskUrl
		{
			get { return m_model.TaskUrl; }
			set { m_model.TaskUrl = value; }
		}
		#endregion


		#region JSONText
		public static readonly string JSONTextPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.JSONText);

		/// <summary>
		/// Загруженный объект 
		/// </summary>
		public string JSONText
		{
			get { return m_model.JSONText; }
			set { m_model.JSONText = value; }
		}
		#endregion


		#region IsDataDownloaded
		private static readonly string IsDataDownloadedPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.IsDataDownloaded);

		public bool IsDataDownloaded
		{
			get { return m_model.IsDataDownloaded; }
		}
		#endregion


		#region InDataDownloading
		private static readonly string InDataDownloadingPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.IsDataDownloaded);

		public bool InDataDownloading
		{
			get { return !m_DownloadCommand.CanExecute(null); }
		}
		#endregion


		#region CanDownload
		public static readonly string CanDownloadPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.CanDownload);

		public bool CanDownload
		{
			get { return m_model.CanDownload; }
		}
		#endregion


		#region DownloadCommand
		private readonly RelayCommand m_DownloadCommand;
		/// <summary>
		/// Команда загрузки задания на ПК
		/// </summary>
		public RelayCommand DownloadCommand
		{
			get { return m_DownloadCommand; }
		}
		#endregion


		public DataDownloaderViewModel(DataDownloaderModel model)
		{
			m_model = model;
			
			// проброс изменившихся свойств модели во View
			m_model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

			m_model.DownloadStarted += (s, e) => { OnDownloadStarted(e); };
			m_model.DownloadComleted += (s, e) => { OnDownloadComleted(e); };

			m_DownloadCommand = new RelayCommand(arg => m_model.Download(), arg => m_model.CanDownload);
			m_DownloadCommand.CanExecuteChanged += (s, e) =>
			{
				OnPropertyChanged(InDataDownloadingPropertyName);
			};
		}


		public void Download()
		{
			m_model.Download();
		}


		protected override void OnPropertyChanged(string info)
		{
			if (info == CanDownloadPropertyName)
				m_DownloadCommand.RaiseCanExecuteChanged();
			base.OnPropertyChanged(info);
		}
	}
}
