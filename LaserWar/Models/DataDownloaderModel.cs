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
using System.Xml.Serialization;
using System.Xml;
using LaserWar.Stuff.XMLDataClasses;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;



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
			get
			{
				try
				{
					return new Uri(m_TaskUrl);
				}
				catch
				{
					return null;
				}
			}
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

			// Это событие по любому должно вызваться, чтобы вызывающий объект смог понять, что отправка данных началась,
			// но, возможно, быстро закончилась
			OnDownloadStarted();
						
			if (TaskUri == null)
				OnDownloadComleted(new DataDownloadComletedEventArgs(new ArgumentException("Invalid URL"), TaskUrl));
			else
			{
				m_wc = new WebClient();
				m_wc.DownloadStringCompleted += wc_DownloadStringCompleted;

				m_wc.DownloadStringAsync(TaskUri);
			}
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
			{	// JSON объект успешно загружен => выполняем дальнейшие работы в другом потоке, чтобы приложение не зависало
				Task.Factory.StartNew(HandleJSONObject, e.Result);
			}
			else
				OnDownloadComletedInternal(new DataDownloadComletedEventArgs(err, TaskUrl));
		}


		/// <summary>
		/// Обработка считанного JSON объекта
		/// </summary>
		/// <returns>
		/// Исключение, если оно произошло
		/// </returns>
		void HandleJSONObject(object JSONObjectInString)
		{
			Exception err = null;

			try
			{
				TaskJSONObject LoadedObject = JsonConvert.DeserializeObject<TaskJSONObject>(JSONObjectInString as string);

				// Пишем объект в БД, предварительно очистив её
				m_DBContext.ClearDBData();
					
				// Загружаем звуки в БД
				foreach (sound jsonSound in LoadedObject.sounds)
				{
					jsonSound.Context = m_DBContext;
					m_DBContext.sounds.Add(jsonSound);
				}
				m_DBContext.SaveChanges();
				m_DBContext.sounds.Load();
								
				// Загружаем игры
				foreach (JSONGame GameInJSON in LoadedObject.games)
				{
					using (XmlReader reader = XmlReader.Create(GameInJSON.url))
					{
						try
						{
							XmlSerializer ser = new XmlSerializer(typeof(GameXML));

							// Заносим игру в БД
							GameXML GameInXML = ser.Deserialize(reader) as GameXML;
							game GameInDB = (game)GameInXML;
							GameInDB.Context = m_DBContext;
							GameInDB.url = GameInJSON.url;
							m_DBContext.games.Add(GameInDB);
							
							m_DBContext.SaveChanges(); // Чтобы получить GameInDB.id_game

							// Заносим команду в БД
							foreach (TeamXML TeamInXML in GameInXML.teams)
							{
								team TeamInDB = (team)TeamInXML;
								TeamInDB.Context = m_DBContext;
								TeamInDB.fk_game = GameInDB.id_game;
								m_DBContext.teams.Add(TeamInDB);

								m_DBContext.SaveChanges(); // Чтобы получить TeamInDB.id_team

								// Заносим игроков в БД
								foreach (player PlayerInDB in TeamInXML.players)
								{
									PlayerInDB.Context = m_DBContext;
									PlayerInDB.fk_team = TeamInDB.id_team;
									m_DBContext.players.Add(PlayerInDB);
								}
							}
						}
						catch (Exception ex)
						{
							err = ex;
						}
					}
				}
				m_DBContext.SaveChanges();

				m_DBContext.players.Load();
				m_DBContext.teams.Load();
				m_DBContext.games.Load();

				m_DBContext.DBResseted();

				// Красиво форматируем текст
				JSONText = JsonConvert.SerializeObject(LoadedObject, Newtonsoft.Json.Formatting.Indented);
			}
			catch (Exception ex)
			{
				err = ex;
			}

			Application.Current.Dispatcher.Invoke(new Action(delegate()
			{
				OnDownloadComletedInternal(new DataDownloadComletedEventArgs(err, TaskUrl));
			}));
		}
		

		private void OnDownloadComletedInternal(DataDownloadComletedEventArgs e)
		{
			m_wc.Dispose();
			m_wc = null;
			OnPropertyChanged(CanDownloadPropertyName);

			OnDownloadComleted(new DataDownloadComletedEventArgs(e.Error, TaskUrl));
		}
	}
}
