using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaserWar.Entities
{
	/// <summary>
	/// Описание игрока в БД
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "player", Namespace = "")]
	[Table("players")]
	public class player : EntityBase, ICloneable
	{
		#region id_player
		private static readonly string id_playerPropertyName = GlobalDefines.GetPropertyName<player>(m => m.id_player);

		private long m_id_player = 0;
		[Key]
		[XmlIgnore]
		public long id_player
		{
			get { return m_id_player; }
			set
			{
				if (m_id_player != value)
				{
					m_id_player = value;
					OnPropertyChanged(id_playerPropertyName);
				}
			}
		}
		#endregion

		
		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<player>(m => m.name);

		private string m_name = "";
		[XmlAttribute]
		[DefaultValue("")]
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

		
		#region rating
		private static readonly string ratingPropertyName = GlobalDefines.GetPropertyName<player>(m => m.rating);

		private int m_rating = 0;
		[XmlAttribute]
		[DefaultValue(0)]
		[Required]
		public int rating
		{
			get { return m_rating; }
			set
			{
				if (m_rating != value)
				{
					m_rating = value;
					OnPropertyChanged(ratingPropertyName);
				}
			}
		}
		#endregion

		
		#region accuracy
		private static readonly string accuracyPropertyName = GlobalDefines.GetPropertyName<player>(m => m.accuracy);

		private float m_accuracy = 0;
		[XmlAttribute]
		[DefaultValue(0)]
		[Required]
		public float accuracy
		{
			get { return m_accuracy; }
			set
			{
				if (m_accuracy != value)
				{
					m_accuracy = value;
					OnPropertyChanged(accuracyPropertyName);
				}
			}
		}
		#endregion

		
		#region shots
		private static readonly string shotsPropertyName = GlobalDefines.GetPropertyName<player>(m => m.shots);

		private int m_shots = 0;
		[XmlAttribute]
		[DefaultValue(0)]
		[Required]
		public int shots
		{
			get { return m_shots; }
			set
			{
				if (m_shots != value)
				{
					m_shots = value;
					OnPropertyChanged(shotsPropertyName);
				}
			}
		}
		#endregion


		#region fk_team
		private static readonly string fk_teamPropertyName = GlobalDefines.GetPropertyName<player>(m => m.fk_team);

		private long m_fk_team = 0;
		[XmlIgnore]
		[Required]
		public long fk_team
		{
			get { return m_fk_team; }
			set
			{
				if (m_fk_team != value)
				{
					m_fk_team = value;
					OnPropertyChanged(fk_teamPropertyName);
				}
			}
		}
		#endregion


		public object Clone()
		{
			player player = new player()
			{
				id_player = this.id_player,
				fk_team = this.fk_team,
				accuracy = this.accuracy,
				name = this.name,
				rating = this.rating,
				shots = this.shots,
				Context = this.Context
			};

			return player;
		}
	}
}
