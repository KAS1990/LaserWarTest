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
using System.Collections.Specialized;
using System.Data.Entity.Infrastructure;

namespace LaserWar.Models
{
	public class SoundsModel : Notifier
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


		#region События
		public event EventHandler<SoundPlayingEventArgs> PlayingStarted;
		private void OnPlayingStarted(SoundModel Sound)
		{
			if (PlayingStarted != null)
				PlayingStarted(this, new SoundPlayingEventArgs(Sound));
		}


		public event EventHandler<SoundPlayingEventArgs> PlayingFinished;
		private void OnPlayingFinished(SoundModel Sound)
		{
			if (PlayingFinished != null)
				PlayingFinished(this, new SoundPlayingEventArgs(Sound));
		}
		#endregion


		#region PlayingSound
		private static readonly string PlayingSoundPropertyName = GlobalDefines.GetPropertyName<SoundsModel>(m => m.PlayingSound);

		SoundModel m_PlayingSound = null;
		public SoundModel PlayingSound
		{
			get { return m_PlayingSound; }
			private set
			{
				if (m_PlayingSound != value)
				{
					m_PlayingSound = value;
					OnPropertyChanged(PlayingSoundPropertyName);
					OnPropertyChanged(IsPlayingPropertyName);
				}
			}
		}
		#endregion

				
		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundsModel>(m => m.IsPlaying);

		public bool IsPlaying
		{
			get { return PlayingSound != null; }
		}
		#endregion


		/// <summary>
		/// Конструктор, загружающий модели из БД или из списка
		/// </summary>
		/// <param name="dbContext"></param>
		public SoundsModel(EntitiesContext dbContext)
		{
			m_DBContext = dbContext;
			m_DBContext.ChangesSavedToDB += m_DBContext_ChangesSavedToDB;

			Sounds = new ReadOnlyObservableCollection<SoundModel>(m_Sounds);
			
			ReloadSounds();

			m_Player.MediaEnded += Player_MediaEnded;
			m_Player.MediaOpened += Player_MediaOpened;

			m_tmrPlayingProgress.Tick += tmrPlayingProgress_Tick;
		}


		/// <summary>
		/// Изменения были сохранены в БД
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void m_DBContext_ChangesSavedToDB(object sender, EntitiesContextEventArgs e)
		{
			foreach (EntitiesContextEventArgs.DbEntity entity in e.Changes)
			{
				if (entity.Value is sound)
				{	// Нас интересуют только звуки
					SoundModel SoundToChange = m_Sounds.FirstOrDefault(arg => arg.Sound.Equals(entity.Value));
					switch (entity.State)
					{
						case System.Data.Entity.EntityState.Modified:
							if (SoundToChange != null)
								SoundToChange.UpdateFromDB();
							break;

						case System.Data.Entity.EntityState.Deleted:
							if (SoundToChange != null)
								m_Sounds.Remove(SoundToChange);
							break;

						case System.Data.Entity.EntityState.Added:
							if (SoundToChange != null)
								m_Sounds.Remove(SoundToChange);
							m_Sounds.Add(new SoundModel((entity.Value as sound), this));
							break;
					}
				}
			}
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
			if (IsPlaying)
			{
				PlayingSound.PlaybackProgressPercent = 100;
				PlayingSound.PlaybackTime = m_Player.NaturalDuration.TimeSpan;
				PlayingSound.IsPlaying = false;

				StopPlayingInternal();
			}
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
		/// Проигрывание файла. Если при этом воспроизводится другой файл, то его воспроизведение прерывается
		/// </summary>
		public void Play(long SoundId)
		{
			SoundModel SoundToPlay = Sounds.FirstOrDefault(arg => arg.Sound.id_sound == SoundId);
			if (SoundToPlay != null)
			{
				if (IsPlaying)
				{	// Сейчас проигрываем другой файл => останавливаем его
					StopPlaying();
				}
				PlayingSound = SoundToPlay;
										
				PlayingSound.PlaybackProgressPercent = 0;
				PlayingSound.PlaybackTime = new TimeSpan(0, 0, 0);
				PlayingSound.IsPlaying = true;
					
				m_Player.Open(new Uri(PlayingSound.Sound.file_path));

				OnPlayingStarted(PlayingSound);
			}
		}
		
		
		/// <summary>
		/// Остановка проигрывания файла
		/// </summary>
		public void StopPlaying()
		{
			if (IsPlaying)
			{	// Сейчас что-то проигрываем => можно остановить
				StopPlayingInternal();
			}
		}

		void StopPlayingInternal()
		{
			m_tmrPlayingProgress.Stop();
			m_Player.Stop();
			m_Player.Close();

			PlayingSound.IsPlaying = false;
						
			OnPlayingFinished(PlayingSound);

			PlayingSound = null;
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
