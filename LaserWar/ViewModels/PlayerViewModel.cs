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
using System.Collections.Specialized;
using System.ComponentModel;

namespace LaserWar.ViewModels
{
	public class PlayerViewModel : Notifier, ICloneable
	{
		/// <summary>
		/// Моделью здесь для простоты реализации будет служить объект БД
		/// </summary>
		player m_modelPlayer = null;
		team m_modelTeam = null;
		readonly GameViewModel m_Parent = null;
									

		#region id_player
		private static readonly string id_playerPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.id_player);

		public long id_player
		{
			get { return m_modelPlayer.id_player; }
			set { m_modelPlayer.id_player = value; }
		}
		#endregion


		#region name
		public static readonly string namePropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.name);

		/// <summary>
		/// Название игрока
		/// </summary>
		public string name
		{
			get { return m_modelPlayer.name; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					AddError(namePropertyName);
				else
				{
					RemoveError(namePropertyName);
					m_modelPlayer.name = value;
				}
			}
		}
		#endregion


		#region rating
		public static readonly string ratingPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.rating);

		/// <summary>
		/// Рейтинг игрока
		/// </summary>
		public int rating
		{
			get { return m_modelPlayer.rating; }
			set
			{
				if (value < 0)
					AddError(ratingPropertyName);
				else
				{
					RemoveError(ratingPropertyName);
					m_modelPlayer.rating = value;
				}
			}
		}
		#endregion


		#region accuracy
		public static readonly string accuracyPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.accuracy);

		/// <summary>
		/// Точность в долях
		/// </summary>
		public float accuracy
		{
			get { return m_modelPlayer.accuracy; }
			set
			{
				if (value < 0 || value > 1)
					AddError(accuracyPropertyName);
				else
				{
					RemoveError(accuracyPropertyName);
					m_modelPlayer.accuracy = value;
				}
			}
		}
		#endregion


		#region shots
		public static readonly string shotsPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.shots);

		public int shots
		{
			get { return m_modelPlayer.shots; }
			set
			{
				if (value < 0)
				{
					AddError(shotsPropertyName);
					throw new ArgumentException(Properties.Resources.resShotsMustBeNotNegative);
				}
				else
				{
					RemoveError(shotsPropertyName);
					m_modelPlayer.shots = value;
				}
			}
		}
		#endregion


		#region TeamId
		private static readonly string TeamIdNamePropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.TeamId);

		/// <summary>
		/// Идентификатор команды, в которой состоит игрок
		/// </summary>
		public long TeamId
		{
			get { return m_modelTeam.id_team; }
			set { m_modelTeam.id_team = value; }
		}
		#endregion


		#region TeamName
		private static readonly string TeamNamePropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.TeamName);

		/// <summary>
		/// Название команды, в которой состоит игрок
		/// </summary>
		public string TeamName
		{
			get { return m_modelTeam.name; }
			set { m_modelTeam.name = value; }
		}
		#endregion


		#region EditedField
		private static readonly string EditingFieldPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.EditingField);

		private enEditedPlayerField m_EditingField = enEditedPlayerField.None;
		/// <summary>
		/// Поле, которое сейчас редактируется
		/// </summary>
		public enEditedPlayerField EditingField
		{
			get { return m_EditingField; }
			set
			{
				if (m_EditingField != value)
				{
					m_EditingField = value;
					OnPropertyChanged(EditingFieldPropertyName);
					OnPropertyChanged(InEditingPropertyName);
				}
			}
		}
		#endregion

				
		#region InEditing
		private static readonly string InEditingPropertyName = GlobalDefines.GetPropertyName<PlayerViewModel>(m => m.InEditing);

		public bool InEditing
		{
			get { return EditingField != enEditedPlayerField.None; }
		}
		#endregion


		#region m_FieldsWithErrors
		private HashSet<string> m_FieldsWithErrors = new HashSet<string>();
		public void AddError(string FieldName)
		{
			if (m_FieldsWithErrors.Add(FieldName))
			{	// Такого элемента ещё не было => нужно изменить состояние команды
				m_CommitChangesCommand.RaiseCanExecuteChanged();
			}
		}
		public void RemoveError(string FieldName)
		{
			if (m_FieldsWithErrors.Remove(FieldName))
			{	// Такой элемент был => нужно изменить состояние команды
				m_CommitChangesCommand.RaiseCanExecuteChanged();
			}
		}
		public void RemoveErrorsExcept(List<string> ExceptList)
		{
			foreach (string FieldName in m_FieldsWithErrors.Where(arg => !ExceptList.Contains(arg)).ToList())
				m_FieldsWithErrors.Remove(FieldName);
			m_CommitChangesCommand.RaiseCanExecuteChanged();
		}
		#endregion


		#region EditCommands
		private readonly RelayCommand m_EditCommand;
		/// <summary>
		/// Переход в режим редактирования игрока
		/// </summary>
		public RelayCommand EditCommand
		{
			get { return m_EditCommand; }
		}


		private readonly RelayCommand m_CommitChangesCommand;
		/// <summary>
		/// Сохранени введённых изменений в БД
		/// </summary>
		public RelayCommand CommitChangesCommand
		{
			get { return m_CommitChangesCommand; }
		}


		private readonly RelayCommand m_RestoreCommand;
		/// <summary>
		/// Выход из режима редактирования без сохранения изменений в БД
		/// </summary>
		public RelayCommand RestoreCommand
		{
			get { return m_RestoreCommand; }
		}
		#endregion


		#region EditStateChanged
		public event EventHandler<PlayerEditEventArgs> EditStateChanged;

		protected virtual void OnEditStateChanged(enEditedPlayerField Field, enEditedPlayerState State)
		{
			if (EditStateChanged != null)
				EditStateChanged(this, new PlayerEditEventArgs(Field, State));
		}
		#endregion


		public PlayerViewModel(player modelPlayer, team modelTeam, GameViewModel Parent)
		{
			m_modelPlayer = modelPlayer;
			m_modelTeam = modelTeam;
			m_Parent = Parent;

			// проброс изменившихся свойств модели во View
			m_modelPlayer.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

			m_EditCommand = new RelayCommand(EditCommandExecute, arg => !InEditing);
			m_CommitChangesCommand = new RelayCommand(CommitChangesCommandExecute, arg => InEditing && m_FieldsWithErrors.Count == 0);
			m_RestoreCommand = new RelayCommand(RestoreCommandExecute, arg => InEditing);
		}

			
		public void RefreshAllProperties()
		{
			OnPropertyChanged(id_playerPropertyName);
			OnPropertyChanged(namePropertyName);
			OnPropertyChanged(ratingPropertyName);
			OnPropertyChanged(accuracyPropertyName);
			OnPropertyChanged(shotsPropertyName);
			OnPropertyChanged(TeamIdNamePropertyName);
			OnPropertyChanged(TeamNamePropertyName);
			OnPropertyChanged(EditingFieldPropertyName);
			OnPropertyChanged(InEditingPropertyName);
		}
		
		
		protected override void OnPropertyChanged(string info)
		{
			if (info == EditingFieldPropertyName)
			{
				m_EditCommand.RaiseCanExecuteChanged();
				m_CommitChangesCommand.RaiseCanExecuteChanged();
				m_RestoreCommand.RaiseCanExecuteChanged();
			}

			base.OnPropertyChanged(info);
		}


		#region Обработчики комманд
		void EditCommandExecute(object arg)
		{
			m_FieldsWithErrors.Clear(); // Сброс ошибок перед началом редактирования
			EditingField = (enEditedPlayerField)arg;
			OnEditStateChanged(EditingField, enEditedPlayerState.Started);
		}


		void CommitChangesCommandExecute(object arg)
		{
			EditingField = enEditedPlayerField.None;
			if (m_modelPlayer.Context.Entry(m_modelPlayer).State == System.Data.Entity.EntityState.Detached)
			{	// Работаем с копией => загружаем оригинал и сохраняем его в БД
				player PlayerInDB = m_modelPlayer.Context.players.First(x => x.id_player == m_modelPlayer.id_player) as player;
				PlayerInDB.name = m_modelPlayer.name;
				PlayerInDB.accuracy = m_modelPlayer.accuracy;
				PlayerInDB.fk_team = m_modelPlayer.fk_team;
				PlayerInDB.shots = m_modelPlayer.shots;
				PlayerInDB.rating = m_modelPlayer.rating;
				PlayerInDB.Context = m_modelPlayer.Context;
				m_modelPlayer = PlayerInDB;
			}
			m_modelPlayer.ToDB();

			if (m_modelTeam.Context.Entry(m_modelTeam).State == System.Data.Entity.EntityState.Detached)
			{	// Работаем с копией => загружаем оригинал и сохраняем его в БД
				team TeamInDB = m_modelTeam.Context.teams.First(x => x.id_team == m_modelTeam.id_team) as team;
				TeamInDB.name = TeamName;
				TeamInDB.fk_game = m_modelTeam.fk_game;
				TeamInDB.Context = m_modelTeam.Context;
				m_modelTeam = TeamInDB;
			}
			
			m_FieldsWithErrors.Clear();
			
			OnEditStateChanged(EditingField, enEditedPlayerState.Commited);
		}


		void RestoreCommandExecute(object arg)
		{
			EditingField = enEditedPlayerField.None;
			if (m_modelPlayer.Context.Entry(m_modelPlayer).State == System.Data.Entity.EntityState.Detached)
			{	// Работаем с копией => загружаем оригинал и сохраняем его в БД
				m_modelPlayer = m_modelPlayer.Context.players.First(x => x.id_player == m_modelPlayer.id_player) as player;
				m_modelPlayer.Context.Entry(m_modelPlayer).State = System.Data.Entity.EntityState.Unchanged;
			}
			
			if (m_modelTeam.Context.Entry(m_modelTeam).State == System.Data.Entity.EntityState.Detached)
			{	// Работаем с копией => загружаем оригинал и сохраняем его в БД
				m_modelTeam = m_modelTeam.Context.teams.First(x => x.id_team == m_modelTeam.id_team) as team;
				m_modelPlayer.Context.Entry(m_modelTeam).State = System.Data.Entity.EntityState.Unchanged;
			}

			m_FieldsWithErrors.Clear();
			
			OnEditStateChanged(EditingField, enEditedPlayerState.Canceled);
		}
		#endregion


		/// <summary>
		/// Клонирование объекта для редактирования
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			player player = m_modelPlayer.Clone() as player;

			team team = new team()
			{
				id_team = this.TeamId,
				name = this.TeamName,
				fk_game = m_modelTeam.fk_game,
				Context = m_modelTeam.Context
			};

			return new PlayerViewModel(player, team, m_Parent);
		}
	}
}
