using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using LaserWar.Stuff;
using LaserWar.Entities;
using System.Data.Entity;

namespace LaserWar.Models
{
	/// <summary>
	/// Модель для загрузок
	/// </summary>
	public class DataDownloaderModel : Notifier
	{
		WebClient m_wc = null;
		readonly EntitiesContext m_DBContext = null;

		#region События
		public event EventHandler DownloadStarted;
		private void OnDownloadStarted()
		{
			OnPropertyChanged(CanDownloadPropertyName);
			if (DownloadStarted != null)
				DownloadStarted(this, new EventArgs());
		}


		public event EventHandler<DataDownloadComletedEventArgs> DownloadComleted;
		private void OnDownloadComleted(DataDownloadComletedEventArgs e)
		{
			OnPropertyChanged(CanDownloadPropertyName);
			if (DownloadComleted != null)
				DownloadComleted(this, e);
		}
		#endregion

		
		#region TaskUrl
		public static readonly string TaskUrlPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.TaskUrl);

		private string m_TaskUrl = "";

		public string TaskUrl
		{
			get { return m_TaskUrl; }
			set
			{
				if (m_TaskUrl != value)
				{
					m_TaskUrl = value;
					OnPropertyChanged(TaskUrlPropertyName);
				}
			}
		}


		public Uri TaskUri
		{
			get { return new Uri(m_TaskUrl); }
		}
		#endregion

		
		#region JSONText
		public static readonly string JSONTextPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.JSONText);

		private string m_JSONText = "";
		/// <summary>
		/// Загруженный объект 
		/// </summary>
		public string JSONText
		{
			get { return m_JSONText; }
			set
			{
				if (m_JSONText != value)
				{
					m_JSONText = value;
					OnPropertyChanged(JSONTextPropertyName);
					OnPropertyChanged(IsDataDownloadedPropertyName);
				}
			}
		}
		#endregion
		
		
		#region IsDataDownloaded
		private static readonly string IsDataDownloadedPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.IsDataDownloaded);

		public bool IsDataDownloaded
		{
			get { return !string.IsNullOrWhiteSpace(JSONText); }
		}
		#endregion


		#region CanDownload
		private static readonly string CanDownloadPropertyName = GlobalDefines.GetPropertyName<DataDownloaderModel>(m => m.CanDownload);

		public bool CanDownload
		{
			get { return m_wc == null; }
		}
		#endregion



		/// <summary>
		/// 
		/// </summary>
		/// <param name="Url">
		/// Url объекта, который был ранее загружен
		/// null - ничего не загружено
		/// </param>
		/// <param name="JsonText">
		/// Текст объекта, который был ранее загружен
		/// null - ничего не загружено
		/// </param>
		public DataDownloaderModel(EntitiesContext DBContext, string Url = null, string JsonText = null)
		{
			m_DBContext = DBContext;
			TaskUrl = Url;
			JSONText = JsonText;
		}


		public void Download()
		{
			if (string.IsNullOrWhiteSpace(TaskUrl))
				return;

			JSONText = "";
			
			m_wc = new WebClient();
			m_wc.DownloadStringCompleted += wc_DownloadStringCompleted;

			m_wc.DownloadStringAsync(TaskUri);

			OnDownloadStarted();
		}

		/// <summary>
		/// Объект загружен с сервера 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			Exception err = e.Error;
			if (err == null)
			{	// JSON объект успешно загружен
				try
				{
					TaskJSONObject LoadedObject = JsonConvert.DeserializeObject<TaskJSONObject>(e.Result);
										
					// Пишем объект в БД, предварительно очистив её
					for (int i = 0; i < m_DBContext.sounds.Local.Count; i++)
						m_DBContext.Entry(m_DBContext.sounds.Local[i]).State = EntityState.Deleted;
					m_DBContext.SaveChanges();

					foreach (sound jsonSound in LoadedObject.sounds)
						m_DBContext.sounds.Add(jsonSound);
					m_DBContext.SaveChanges();

					// Красиво форматируем текст
					JSONText = JsonConvert.SerializeObject(LoadedObject, Formatting.Indented);
				}
				catch (Exception ex)
				{
					err = ex;
					ex.ToString();
				}
			}

			m_wc.Dispose();
			m_wc = null;
			OnPropertyChanged(CanDownloadPropertyName);

			OnDownloadComleted(new DataDownloadComletedEventArgs(err, TaskUrl));
		}
	}
}
