﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using LaserWar.Entities;
using LaserWar.Global;
using LaserWar.Stuff;

namespace LaserWar.Models
{
	/// <summary>
	/// Модель, описывающая 1 звук
	/// </summary>
	public class SoundModel
	{
		readonly SoundsModel m_Parent = null;
		public event EventHandler<SoundModelEventArgs> SoundUpdated = delegate { };
		

		WebClient m_FileDowloader = new WebClient();
				

		#region Sound
		private static readonly string SoundPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.Sound);

		readonly int m_SoundId = 0;
		sound m_Sound = null;
		public sound Sound
		{
			private set
			{
				m_Sound = value;
			}
			get { return m_Sound; }
		}
		#endregion


		#region FileName
		private static readonly string FileNamePropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.FileName);

		public string FileName
		{
			get
			{
				return Path.GetFileName(Sound.url);
			}
		}
		#endregion


		#region IsDownloaded
		private static readonly string IsDownloadedPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.IsDownloaded);

		/// <summary>
		/// Звуковой файл загружен на ПК
		/// </summary>
		public bool IsDownloaded
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Sound.file_path);
			}
		}
		#endregion


		#region DownloadProgressPercent
		private static readonly string DownloadProgressPercentPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.DownloadProgressPercent);

		private double m_DownloadProgressPercent = 0;

		public double DownloadProgressPercent
		{
			get { return m_DownloadProgressPercent; }
			set
			{
				if (m_DownloadProgressPercent != value)
				{
					m_DownloadProgressPercent = value;
					OnSoundUpdated(DownloadProgressPercentPropertyName);
				}
			}
		}
		#endregion


		#region PlaybackProgressPercent
		private static readonly string PlaybackProgressPercentPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.PlaybackProgressPercent);

		private double m_PlaybackProgressPercent = 0;

		public double PlaybackProgressPercent
		{
			get { return m_PlaybackProgressPercent; }
			set
			{
				if (m_PlaybackProgressPercent != value)
				{
					m_PlaybackProgressPercent = value;
					OnSoundUpdated(PlaybackProgressPercentPropertyName);

				}
			}
		}
		#endregion


		#region PlaybackTime
		private static readonly string PlaybackTimePropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.PlaybackTime);

		private TimeSpan m_PlaybackTime = new TimeSpan(0, 0, 0);

		public TimeSpan PlaybackTime
		{
			get { return m_PlaybackTime; }
			set
			{
				if (m_PlaybackTime != value)
				{
					m_PlaybackTime = value;
					OnSoundUpdated(PlaybackTimePropertyName);
				}
			}
		}
		#endregion


		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.IsPlaying);

		private bool m_IsPlaying = false;

		public bool IsPlaying
		{
			get { return m_IsPlaying; }
			set
			{
				if (m_IsPlaying != value)
				{
					m_IsPlaying = value;
					OnSoundUpdated(IsPlayingPropertyName);
				}
			}
		}
		#endregion


		#region CanPlay
		private static readonly string CanPlayPropertyName = GlobalDefines.GetPropertyName<SoundModel>(m => m.CanPlay);

		public bool CanPlay
		{
			get { return IsDownloaded; }
		}
		#endregion


		public SoundModel(int SoundId, SoundsModel Parent)
		{
			m_SoundId = SoundId;
			Sound = GetSound();
			m_Parent = Parent;

			m_FileDowloader.DownloadFileCompleted += FileDowloader_DownloadAsyncCompleted;
			m_FileDowloader.DownloadProgressChanged += FileDowloader_DownloadProgressChanged;
		}


		public SoundModel(sound sound, SoundsModel Parent)
		{
			m_SoundId = sound.id_sound;
			Sound = sound;
			m_Parent = Parent;

			m_FileDowloader.DownloadFileCompleted += FileDowloader_DownloadAsyncCompleted;
			m_FileDowloader.DownloadProgressChanged += FileDowloader_DownloadProgressChanged;
		}


		public void UpdateSoundInDB(sound NewValue)
		{
			sound CurValue = Sound;
			try
			{
				CurValue = NewValue;
				m_Parent.DBContext.Entry(CurValue).State = System.Data.Entity.EntityState.Modified;
				m_Parent.DBContext.SaveChanges();
			}
			catch (Exception ex)
			{
				ex.ToString();
				return;
			}

			Sound = CurValue;

			OnSoundUpdated(new string[] { SoundPropertyName, FileNamePropertyName, IsDownloadedPropertyName, CanPlayPropertyName });
		}


		private sound GetSound()
		{
			return (from snd in m_Parent.DBContext.sounds
					where snd.id_sound == m_SoundId
					select snd).FirstOrDefault();
		}


		#region Загрузка файла
		public void DownloadFile()
		{
			if (!IsDownloaded)
			{
				string DestFilePath = Directory.GetCurrentDirectory() + "\\" + FileName;

				if (File.Exists(DestFilePath))
					File.Delete(DestFilePath);

				m_FileDowloader.QueryString.Add(sound.file_pathPropertyName, DestFilePath);
				m_FileDowloader.DownloadFileAsync(new Uri(Sound.url), DestFilePath);
			}
		}


		public void StopDownloading()
		{
			m_FileDowloader.CancelAsync();
		}


		void FileDowloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			DownloadProgressPercent = e.ProgressPercentage;
		}


		void FileDowloader_DownloadAsyncCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				Sound.file_path = null;
				m_FileDowloader.QueryString.Clear();
				DownloadProgressPercent = 0;
			}
			else
			{
				Sound.file_path = m_FileDowloader.QueryString[sound.file_pathPropertyName];
				m_FileDowloader.QueryString.Clear();
				DownloadProgressPercent = 100;

				UpdateSoundInDB(Sound);
			}
		}
		#endregion


		#region Проигрывание звука
		/// <summary>
		/// Проигрывание файла
		/// </summary>
		public void Play()
		{
			if (CanPlay)
			{	// Файл можно проигрывать => сообщаем об этом родителю
				m_Parent.Play(m_SoundId);
			}
		}


		/// <summary>
		/// Остановка проигрывания файла
		/// </summary>
		public void StopPlaying()
		{
			if (IsPlaying)
			{	// Сейчас что-то проигрываем => можно остановить
				m_Parent.StopPlaying(m_SoundId);
			}
		}
		#endregion



		public void OnSoundUpdated(string[] changedProperties)
		{
			if (SoundUpdated != null)
				SoundUpdated(this, new SoundModelEventArgs(changedProperties));
		}


		public void OnSoundUpdated(string propertyName)
		{
			if (SoundUpdated != null)
				SoundUpdated(this, new SoundModelEventArgs(propertyName));
		}
	}
}
