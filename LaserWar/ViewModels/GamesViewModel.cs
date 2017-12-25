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

namespace LaserWar.ViewModels
{
	public class GamesViewModel : Notifier
	{
		readonly EntitiesContext m_Context = null;

		private readonly ObservableCollection<GameViewModel> m_Games = new ObservableCollection<GameViewModel>();
		/// <summary>
		/// Коллекция звуков
		/// </summary>
		public ObservableCollection<GameViewModel> Games
		{
			get { return m_Games; }
		}
		
		
		#region SelectedGame
		public static readonly string SelectedGamePropertyName = GlobalDefines.GetPropertyName<GamesViewModel>(m => m.SelectedGame);

		private GameViewModel m_SelectedGame = null;

		public GameViewModel SelectedGame
		{
			get { return m_SelectedGame; }
			set
			{
				if (m_SelectedGame != value)
				{
					m_SelectedGame = value;
					OnPropertyChanged(SelectedGamePropertyName);
				}
			}
		}
		#endregion
				


		public GamesViewModel(EntitiesContext Context)
		{
			m_Context = Context;
			m_Context.DBReset += Context_DBReset;
			
			// Заполняем m_Games
			ReloadGames();
		}


		/// <summary>
		/// Загрузить игры из БД
		/// </summary>
		public void ReloadGames()
		{
			m_Games.Clear();

			foreach (game Game in m_Context.games)
			{
				Game.Context = m_Context;
				m_Games.Add(new GameViewModel(Game, this));
			}
		}


		/// <summary>
		/// База была полностью обновлена
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Context_DBReset(object sender, EventArgs e)
		{
			ReloadGames();
		}


		/// <summary>
		/// Изменения были сохранены в БД
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Context_ChangesSavedToDB(object sender, EntitiesContextEventArgs e)
		{
			foreach (EntitiesContextEventArgs.DbEntity entity in e.Changes)
			{
				if (entity.Value is game)
				{	// Нас интересуют только звуки
					GameViewModel GameToChange = m_Games.FirstOrDefault(arg => arg.id_game == (entity.Value as game).id_game);
					switch (entity.State)
					{
						case System.Data.Entity.EntityState.Deleted:
							if (GameToChange != null)
								m_Games.Remove(GameToChange);
							break;

						case System.Data.Entity.EntityState.Added:
							if (GameToChange != null)
								m_Games.Remove(GameToChange);
							(entity.Value as game).Context = m_Context;
							m_Games.Add(new GameViewModel(entity.Value as game, this));
							break;
					}
				}
			}
		}
	}
}
