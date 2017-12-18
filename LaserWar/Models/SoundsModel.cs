using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Entities;
using LaserWar.Global;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using LaserWar.Stuff;

namespace LaserWar.Models
{
	public class SoundsModel
	{
		readonly EntitiesContext m_DBContext = null;
		public EntitiesContext DBContext
		{
			get { return m_DBContext; }
		}

		
		private readonly ObservableCollection<SoundModel> m_Sounds = new ObservableCollection<SoundModel>();
		/// <summary>
		/// Коллекция звуков
		/// </summary>
		public readonly ReadOnlyObservableCollection<SoundModel> Sounds;
		

		MediaPlayer m_Player = new MediaPlayer();
		/// <summary>
		/// Таймер меняющий прогресс воспроизведения записи
		/// </summary>
		DispatcherTimer m_tmrPlayingProgress = new DispatcherTimer()
		{
			Interval = new TimeSpan(0, 0, 0, 0, 500)
		};


		#region PlayingSound
		private static readonly string PlayingSoundPropertyName = GlobalDefines.GetPropertyName<SoundsModel>(m => m.PlayingSound);

		SoundModel m_PlayingSound = null;
		public SoundModel PlayingSound
		{
			private set { m_PlayingSound = value; }
			get { return m_PlayingSound; }
		}
		#endregion


		
		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundsModel>(m => m.IsPlaying);

		public bool IsPlaying
		{
			get { return PlayingSound != null; }
		}
		#endregion


		#region CanPlay
		private static readonly string CanPlayPropertyName = GlobalDefines.GetPropertyName<SoundsModel>(m => m.IsPlaying);

		public bool CanPlay
		{
			get { return Sounds.Count > 0 && Sounds.FirstOrDefault(arg => arg.IsDownloaded) != null && !IsPlaying; }
		}
		#endregion


		/// <summary>
		/// Конструктор, загружающий модели из БД или из списка
		/// </summary>
		/// <param name="dbContext"></param>
		public SoundsModel(EntitiesContext dbContext)
		{
			m_DBContext = dbContext;

			Sounds = new ReadOnlyObservableCollection<SoundModel>(m_Sounds);
			
			ReloadSounds();

			m_Player.MediaEnded += Player_MediaEnded;
			m_Player.MediaOpened += Player_MediaOpened;

			m_tmrPlayingProgress.Tick += tmrPlayingProgress_Tick;
		}


		/// <summary>
		/// Загрузить звуки из БД
		/// </summary>
		public void ReloadSounds()
		{
			m_Sounds.Clear();

			foreach (sound snd in m_DBContext.sounds)
				m_Sounds.Add(new SoundModel(snd, this));
		}


		public void ClearSounds(bool DeleteFromDB)
		{
			if (DeleteFromDB)
			{
				foreach (SoundModel snd in m_Sounds)
					m_DBContext.sounds.Remove(snd.Sound);
			}

			m_Sounds.Clear();
		}


		#region Проигрывание звука
		void Player_MediaEnded(object sender, EventArgs e)
		{
			m_tmrPlayingProgress.Stop();
			PlayingSound.PlaybackProgressPercent = 100;
			PlayingSound.PlaybackTime = m_Player.NaturalDuration.TimeSpan;

			PlayingSound.IsPlaying = false;
		}


		void Player_MediaOpened(object sender, EventArgs e)
		{
			m_tmrPlayingProgress.Start();
			m_Player.Play();

			PlayingSound.IsPlaying = true;
		}


		void tmrPlayingProgress_Tick(object sender, EventArgs e)
		{
			PlayingSound.PlaybackProgressPercent = (m_Player.Position.TotalMilliseconds * 100.0) / m_Player.NaturalDuration.TimeSpan.TotalMilliseconds;
			PlayingSound.PlaybackTime = m_Player.Position;
		}


		/// <summary>
		/// Проигрывание файла
		/// </summary>
		public void Play(int SoundId)
		{
			if (CanPlay)
			{	// Файл можно проигрывать
				PlayingSound = Sounds.FirstOrDefault(arg => arg.Sound.id_sound == SoundId);
				if (PlayingSound != null)
				{
					PlayingSound.PlaybackProgressPercent = 0;
					PlayingSound.PlaybackTime = new TimeSpan(0, 0, 0);
					m_Player.Open(new Uri(PlayingSound.Sound.file_path));
				}
			}
		}


		/// <summary>
		/// Остановка проигрывания файла
		/// </summary>
		public void StopPlaying(int SoundId)
		{
			if (IsPlaying && PlayingSound.Sound.id_sound == SoundId)
			{	// Сейчас что-то проигрываем => можно остановить
				m_tmrPlayingProgress.Stop();
				m_Player.Stop();

				PlayingSound.IsPlaying = false;
			}
		}


		public void StopPlaying()
		{
			if (IsPlaying)
			{	// Сейчас что-то проигрываем => можно остановить
				m_tmrPlayingProgress.Stop();
				m_Player.Stop();

				PlayingSound.IsPlaying = false;
			}
		}
		#endregion


		/// <summary>
		/// Прерываем все загрузки
		/// </summary>
		public void StopDownloading()
		{
			foreach (SoundModel model in Sounds)
				model.StopDownloading();
		}
	}
}
