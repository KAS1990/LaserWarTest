using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaserWar.Entities
{
	/// <summary>
	/// Описание команды в БД
	/// </summary>
	[Table("teams")]
	public class team : EntityBase
	{
		#region id_team
		private static readonly string id_teamPropertyName = GlobalDefines.GetPropertyName<team>(m => m.id_team);

		private long m_id_team = 0;
		[Key]
		public long id_team
		{
			get { return m_id_team; }
			set
			{
				if (m_id_team != value)
				{
					m_id_team = value;
					OnPropertyChanged(id_teamPropertyName);
				}
			}
		}
		#endregion

		
		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<team>(m => m.name);

		private string m_name = "";
		[Required]
		public string name
		{
			get { return m_name; }
			set
			{
				if (m_name != value)
				{
					m_name = value;
					OnPropertyChanged(namePropertyName);
				}
			}
		}
		#endregion


		#region fk_game
		private static readonly string fk_gamePropertyName = GlobalDefines.GetPropertyName<team>(m => m.fk_game);

		private long m_fk_game = 0;
		public long fk_game
		{
			get { return m_fk_game; }
			set
			{
				if (m_fk_game != value)
				{
					m_fk_game = value;
					OnPropertyChanged(fk_gamePropertyName);
				}
			} 
		}
		#endregion


		/// <summary>
		/// Игроки команды
		/// </summary>
		[NotMapped]
		public List<player> players
		{
			get
			{
				if (Context == null)
					return new List<player>();

				return (from plyr in Context.players.Local
						where plyr.fk_team == id_team
						select plyr).ToList();
			}
		}


		public object Clone()
		{
			team team = new team()
			{
				id_team = this.id_team,
				name = this.name,
				fk_game = this.fk_game,
				Context = this.Context
			};

			return team;
		}
	}
}
