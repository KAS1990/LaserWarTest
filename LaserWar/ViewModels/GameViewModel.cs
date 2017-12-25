using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Collections.ObjectModel;
using LaserWar.Models;
using LaserWar.Stuff;
using System.Collections.Specialized;
using LaserWar.Entities;
using LaserWar.Commands;

namespace LaserWar.ViewModels
{
	public class GameViewModel : Notifier
	{
		/// <summary>
		/// Моделью здесь для простоты реализации будет служить объект БД
		/// </summary>
		readonly game m_model = null;
		readonly GamesViewModel m_Parent = null;
		

		#region id_game
		private static readonly string id_gamePropertyName = GlobalDefines.GetPropertyName<GameViewModel>(m => m.id_game);

		public long id_game
		{
			get { return m_model.id_game; }
			set { m_model.id_game = value; }
		}
		#endregion


		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<GameViewModel>(m => m.name);

		/// <summary>
		/// Название
		/// </summary>
		public string name
		{
			get { return m_model.name; }
			set { m_model.name = value; }
		}
		#endregion


		#region date
		private static readonly string datePropertyName = GlobalDefines.GetPropertyName<GameViewModel>(m => m.date);

		/// <summary>
		/// Дата проведения соревнований
		/// </summary>
		public int date
		{
			get { return m_model.date; }
			set { m_model.date = value; }
		}
		#endregion


		#region url
		private static readonly string urlPropertyName = GlobalDefines.GetPropertyName<GameViewModel>(m => m.url);

		/// <summary>
		/// Путь к файлу с командой на сервере
		/// </summary>
		public string url
		{
			get { return m_model.url; }
			set { m_model.url = value; }
		}
		#endregion


		#region PlayersCount
		private static readonly string PlayersCountPropertyName = GlobalDefines.GetPropertyName<GameViewModel>(m => m.PlayersCount);

		/// <summary>
		/// Количество игроков
		/// </summary>
		public int PlayersCount
		{
			get { return Players.Count; }
		}
		#endregion


		#region GameSelectedCommand
		private readonly RelayCommand m_GameSelectedCommand;
		/// <summary>
		/// Команда выделена
		/// </summary>
		public RelayCommand GameSelectedCommand
		{
			get { return m_GameSelectedCommand; }
		}
		#endregion


		#region GameDeselectedCommand
		private readonly RelayCommand m_GameDeselectedCommand;
		/// <summary>
		/// Теперь не выделена
		/// </summary>
		public RelayCommand GameDeselectedCommand
		{
			get { return m_GameDeselectedCommand; }
		}
		#endregion


		private readonly ObservableCollection<TeamViewModel> m_Teams = new ObservableCollection<TeamViewModel>();
		/// <summary>
		/// Коллекция команд
		/// </summary>
		public ReadOnlyObservableCollection<TeamViewModel> Teams { get; set; }


		private readonly ObservableCollection<PlayerViewModel> m_Players = new ObservableCollection<PlayerViewModel>();
		/// <summary>
		/// Коллекция игроков
		/// </summary>
		public ObservableCollection<PlayerViewModel> Players
		{
			get { return m_Players; }
		}


		public GameViewModel(game model, GamesViewModel Parent)
		{
			m_model = model;
			m_Parent = Parent;
			
			// проброс изменившихся свойств модели во View
			m_model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

			Teams = new ReadOnlyObservableCollection<TeamViewModel>(m_Teams);
			m_Teams.CollectionChanged += (s, e) =>
				{
					OnPropertyChanged(PlayersCountPropertyName);
				};

			m_GameSelectedCommand = new RelayCommand(arg => m_Parent.SelectedGame = this);
			m_GameDeselectedCommand = new RelayCommand(arg => m_Parent.SelectedGame = null);

			// Заполняем коллекции
			ReloadCollections();
		}


		/// <summary>
		/// Загрузить коллекции из БД
		/// </summary>
		public void ReloadCollections()
		{
			m_Teams.Clear();
			m_Players.Clear();

			// Создаём коллекцию VM'ов на основании уже существующих моделей
			foreach (team TeamModel in m_model.teams)
			{
				TeamModel.Context = m_model.Context;
				m_Teams.Add(new TeamViewModel(TeamModel, this));

				foreach (player PlayerModel in TeamModel.players)
				{
					PlayerModel.Context = m_model.Context;
					m_Players.Add(new PlayerViewModel(PlayerModel, TeamModel, this));
				}
			}
		}
	}
}
