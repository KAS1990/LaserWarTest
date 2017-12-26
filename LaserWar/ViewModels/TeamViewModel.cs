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
	public class TeamViewModel : Notifier
	{
		/// <summary>
		/// Моделью здесь для простоты реализации будет служить объект БД
		/// </summary>
		readonly team m_model = null;
		readonly GameViewModel m_Parent = null;


		#region id_team
		private static readonly string id_teamPropertyName = GlobalDefines.GetPropertyName<TeamViewModel>(m => m.id_team);

		public long id_team
		{
			get { return m_model.id_team; }
			set { m_model.id_team = value; }
		}
		#endregion


		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<TeamViewModel>(m => m.name);

		/// <summary>
		/// Название
		/// </summary>
		public string name
		{
			get { return m_model.name; }
			set { m_model.name = value; }
		}
		#endregion


		/// <summary>
		/// Суммарный рейтинг команды
		/// </summary>
		public int Rating
		{
			get { return m_model.players.Sum(arg => arg.rating); }
		}


		/// <summary>
		/// Средняя точность игроков команды
		/// </summary>
		public float Accuracy
		{
			get { return m_model.players.Average(arg => arg.accuracy); }
		}


		public TeamViewModel(team model, GameViewModel Parent)
		{
			m_model = model;
			m_Parent = Parent;

			// проброс изменившихся свойств модели во View
			m_model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
		}
	}
}
