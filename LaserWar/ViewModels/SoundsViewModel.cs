﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Collections.ObjectModel;
using LaserWar.Models;
using LaserWar.Stuff;
using System.Collections.Specialized;

namespace LaserWar.ViewModels
{
	public class SoundsViewModel : Notifier
	{
		readonly SoundsModel m_model = null;

		private readonly ObservableCollection<SoundViewModel> m_Sounds = new ObservableCollection<SoundViewModel>();
		/// <summary>
		/// Коллекция звуков
		/// </summary>
		public ReadOnlyObservableCollection<SoundViewModel> Sounds { get; set; }


		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundsViewModel>(m => m.IsPlaying);

		public bool IsPlaying
		{
			get { return m_model.IsPlaying; }
		}
		#endregion


		public SoundsViewModel(SoundsModel model)
		{
			m_model = model;

			// проброс изменившихся свойств модели во View
			m_model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
			m_model.PlayingStarted += m_model_PlayingStarted;
			m_model.PlayingFinished += m_model_PlayingFinished;
			((INotifyCollectionChanged)m_model.Sounds).CollectionChanged += m_model_Sounds_CollectionChanged;
										
			Sounds = new ReadOnlyObservableCollection<SoundViewModel>(m_Sounds);
			
			// Заполняем m_Sounds
			// Создаём коллекцию VM'ов на основании уже существующих моделей
			foreach (SoundModel SoundModel in m_model.Sounds)
				m_Sounds.Add(new SoundViewModel(SoundModel, this));
		}


		/// <summary>
		/// Загрузить звуки из БД
		/// </summary>
		public void ReloadSounds()
		{
			// Заставляем модель перезагрузить список звуков
			m_model.ReloadSounds();
		}


		/// <summary>
		/// В модели произошли изменения => загружаем их сюда 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void m_model_Sounds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (SoundModel snd in e.NewItems)
				{
					SoundViewModel CurVal = m_Sounds.FirstOrDefault(arg => snd.Sound.Equals(arg));
					if (CurVal != null)
						m_Sounds.Remove(CurVal);
					m_Sounds.Add(new SoundViewModel(snd, this));
				}
			}
		}



		private SoundViewModel GetSound(long SoundId)
		{
			return (from snd in Sounds
					where snd.id_sound == SoundId
					select snd).FirstOrDefault();
		}


		#region Проигрывание звука
		/// <summary>
		/// Проигрывание файла
		/// </summary>
		public void Play(SoundViewModel Sound)
		{
			m_model.Play(Sound.id_sound);
		}


		public void StopPlaying()
		{
			m_model.StopPlaying();
		}
		#endregion


		/// <summary>
		/// Прерываем все загрузки
		/// </summary>
		public void StopDownloading()
		{
			foreach (SoundViewModel sound in Sounds)
				sound.StopDownloading();
		}


		void m_model_PlayingStarted(object sender, SoundPlayingEventArgs e)
		{
			GetSound(e.Sound.Sound.id_sound).IsPlaying = true;
		}


		void m_model_PlayingFinished(object sender, SoundPlayingEventArgs e)
		{
			GetSound(e.Sound.Sound.id_sound).IsPlaying = false;
		}
	}
}
