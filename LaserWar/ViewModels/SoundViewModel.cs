using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using LaserWar.Models;
using LaserWar.Stuff;
using LaserWar.Entities;
using LaserWar.Commands;
using System.Windows.Input;

namespace LaserWar.ViewModels
{
	public class SoundViewModel : Notifier
	{
		readonly SoundModel m_model = null;
		readonly SoundsViewModel m_Parent = null;

		#region id_sound
		private static readonly string id_soundPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.id_sound);

		public long id_sound
		{
			get { return m_model.Sound.id_sound; }
			set { m_model.Sound.id_sound = value; }
		}
		#endregion


		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.name);

		/// <summary>
		/// Название
		/// </summary>
		public string name
		{
			get { return m_model.Sound.name; }
			set { m_model.Sound.name = value; }
		}
		#endregion


		#region url
		private static readonly string urlPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.url);

		/// <summary>
		/// URL для загрузки
		/// </summary>
		public string url
		{
			get { return m_model.Sound.url; }
			set { m_model.Sound.url = value; }
		}
		#endregion


		#region size
		private static readonly string sizePropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.size);

		/// <summary>
		/// Размер файла в байтах
		/// </summary>
		public int size
		{
			get { return m_model.Sound.size; }
			set { m_model.Sound.size = value; }
		}
		#endregion


		#region file_path
		public static readonly string file_pathPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.file_path);

		public string file_path
		{
			get { return m_model.Sound.file_path; }
			set { m_model.Sound.file_path = value; }
		}
		#endregion


		#region FileName
		private static readonly string FileNamePropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.FileName);

		public string FileName
		{
			get { return m_model.FileName; }
		}
		#endregion


		#region IsDownloaded
		public static readonly string IsDownloadedPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.IsDownloaded);

		/// <summary>
		/// Звуковой файл загружен на ПК
		/// </summary>
		public bool IsDownloaded
		{
			get { return m_model.IsDownloaded; }
		}
		#endregion


		#region InDownloading
		public static readonly string InDownloadingPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.InDownloading);

		/// <summary>
		/// В данный момент загружаем файл
		/// </summary>
		public bool InDownloading
		{
			get { return m_model.InDownloading; }
		}
		#endregion


		#region DownloadProgressPercent
		private static readonly string DownloadProgressPercentPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.DownloadProgressPercent);

		public double DownloadProgressPercent
		{
			get { return m_model.DownloadProgressPercent; }
			set { m_model.DownloadProgressPercent = value; }
		}
		#endregion


		#region PlaybackProgressPercent
		private static readonly string PlaybackProgressPercentPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.PlaybackProgressPercent);

		public double PlaybackProgressPercent
		{
			get { return m_model.PlaybackProgressPercent; }
			set { m_model.PlaybackProgressPercent = value; }
		}
		#endregion


		#region PlaybackTime
		private static readonly string PlaybackTimePropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.PlaybackTime);

		public TimeSpan PlaybackTime
		{
			get { return m_model.PlaybackTime; }
			set { m_model.PlaybackTime = value; }
		}
		#endregion


		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.IsPlaying);

		public bool IsPlaying
		{
			get { return m_model.IsPlaying; }
			set { m_model.IsPlaying = value; }
		}
		#endregion


		#region CanPlay
		private static readonly string CanPlayPropertyName = GlobalDefines.GetPropertyName<SoundViewModel>(m => m.CanPlay);

		public bool CanPlay
		{
			get { return m_model.CanPlay; }
		}
		#endregion


		#region DownloadCommand
		private readonly RelayCommand m_DownloadCommand;
		/// <summary>
		/// Команда загрузки звука на ПК
		/// </summary>
		public RelayCommand DownloadCommand
		{
			get { return m_DownloadCommand; }
		}
		#endregion


		#region PlayCommand
		private readonly RelayCommand m_PlayCommand;
		/// <summary>
		/// Проигрывание звука
		/// </summary>
		public RelayCommand PlayCommand
		{
			get { return m_PlayCommand; }
		}
		#endregion


		public SoundViewModel(SoundModel model, SoundsViewModel Parent)
		{
			m_model = model;
			m_Parent = Parent;

			// проброс изменившихся свойств модели во View
			m_model.SoundUpdated += model_SoundUpdated;

			m_DownloadCommand = new RelayCommand(arg => DownloadFile(), arg => !IsDownloaded);
			m_DownloadCommand.CanExecuteChanged += (s, e) =>
			{
				OnPropertyChanged(InDownloadingPropertyName);
			};

			m_PlayCommand = new RelayCommand(PlayingCommandExecute, arg => CanPlay);
			m_PlayCommand.CanExecuteChanged += (s, e) =>
			{
				OnPropertyChanged(IsPlayingPropertyName);
			};
		}


		void model_SoundUpdated(object sender, SoundModelEventArgs e)
		{
			foreach (string PropertyName in e.ChangedProperties)
				OnPropertyChanged(PropertyName);
		}


		protected override void OnPropertyChanged(string info)
		{
			if (info == IsDownloadedPropertyName)
				m_DownloadCommand.RaiseCanExecuteChanged();
			if (info == CanPlayPropertyName)
				m_PlayCommand.RaiseCanExecuteChanged();
			
			base.OnPropertyChanged(info);
		}


		#region Загрузка файла
		public void DownloadFile()
		{
			m_model.DownloadFile();
		}


		public void StopDownloading()
		{
			m_model.StopDownloading();
		}
		#endregion


		#region Проигрывание звука
		/// <summary>
		/// Обработчик команды m_PlayCommand
		/// </summary>
		/// <param name="parameter"></param>
		void PlayingCommandExecute(object parameter)
		{
			if (IsPlaying)
				StopPlaying();
			else
				Play();
		}


		/// <summary>
		/// Проигрывание файла
		/// </summary>
		public void Play()
		{
			m_model.Play();
		}


		/// <summary>
		/// Остановка проигрывания файла
		/// </summary>
		public void StopPlaying()
		{
			m_model.StopPlaying();
		}
		#endregion
	}
}
