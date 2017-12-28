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
	/// Описание игры в БД
	/// </summary>
	[Table("games")]
	public class game : EntityBase
	{
		#region id_game
		private static readonly string id_gamePropertyName = GlobalDefines.GetPropertyName<game>(m => m.id_game);

		private long m_id_game = 0;

		[Key]
		public long id_game
		{
			get { return m_id_game; }
			set
			{
				if (m_id_game != value)
				{
					m_id_game = value;
					OnPropertyChanged(id_gamePropertyName);
				}
			}
		}
		#endregion

		
		#region url
		private static readonly string urlPropertyName = GlobalDefines.GetPropertyName<game>(m => m.url);

		private string m_url = "";
		[Required]
		public string url
		{
			get { return m_url; }
			set
			{
				if (m_url != value)
				{
					m_url = value;
					OnPropertyChanged(urlPropertyName);
				}
			}
		}
		#endregion

		
		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<game>(m => m.name);

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

		
		#region date
		private static readonly string datePropertyName = GlobalDefines.GetPropertyName<game>(m => m.date);

		private int m_date = 0;
		/// <summary>
		/// Дата в том виде, в котором она хранится на сервере
		/// </summary>
		[Required]
		public int date
		{
			get { return m_date; }
			set
			{
				if (m_date != value)
				{
					m_date = value;
					
					OnPropertyChanged(datePropertyName);
					OnPropertyChanged(DateHRDPropertyName);
				}
			}
		}


		private static readonly string DateHRDPropertyName = GlobalDefines.GetPropertyName<game>(m => m.DateHRD);
		/// <summary>
		/// Дата проведения игры в человекопонятном формате (human readable date)
		/// </summary>
		[NotMapped]
		public DateTime DateHRD
		{
			get
			{
				System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
				return dtDateTime.AddSeconds(date).ToLocalTime();
			}
		}
		#endregion
		

		/// <summary>
		/// Команды
		/// </summary>
		[XmlArray(ElementName="team")]
		[NotMapped]
		public List<team> teams
		{
			get
			{
				if (Context == null)
					return new List<team>();

				return (from tm in Context.teams.Local
						where tm.fk_game == id_game
						select tm).ToList();
			}
		}
	}
}
