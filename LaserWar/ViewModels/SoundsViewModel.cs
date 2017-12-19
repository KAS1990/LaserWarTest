using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Collections.ObjectModel;
using LaserWar.Models;

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


		#region PlayingSound
		private static readonly string PlayingSoundPropertyName = GlobalDefines.GetPropertyName<SoundsViewModel>(m => m.PlayingSound);

		public SoundModel PlayingSound
		{
			get { return m_model.PlayingSound; }
		}
		#endregion


		#region IsPlaying
		private static readonly string IsPlayingPropertyName = GlobalDefines.GetPropertyName<SoundsViewModel>(m => m.IsPlaying);

		public bool IsPlaying
		{
			get { return m_model.IsPlaying; }
		}
		#endregion


		#region CanPlay
		private static readonly string CanPlayPropertyName = GlobalDefines.GetPropertyName<SoundsViewModel>(m => m.IsPlaying);

		public bool CanPlay
		{
			get { return m_model.CanPlay; }
		}
		#endregion


		public SoundsViewModel(SoundsModel model)
		{
			m_model = model;

			// проброс изменившихся свойств модели во View
			m_model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

			ReloadSounds();
			
			Sounds = new ReadOnlyObservableCollection<SoundViewModel>(m_Sounds);
		}


		/// <summary>
		/// Загрузить звуки из БД
		/// </summary>
		public void ReloadSounds()
		{
			m_Sounds.Clear();

			// Создаём коллекцию VM'ов на основании уже существующих моделей
			foreach (SoundModel SoundModel in m_model.Sounds)
				m_Sounds.Add(new SoundViewModel(SoundModel, this));

			m_model.ReloadSounds();
		}


		#region Проигрывание звука
		/// <summary>
		/// Проигрывание файла
		/// </summary>
		public void Play(SoundViewModel Sound)
		{
			m_model.Play(Sound.id_sound);
		}


		/// <summary>
		/// Остановка проигрывания файла
		/// </summary>
		public void StopPlaying(SoundViewModel Sound)
		{
			m_model.StopPlaying(Sound.id_sound);
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
	}
}
