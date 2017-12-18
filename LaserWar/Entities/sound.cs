using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LaserWar.Global;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaserWar.Entities
{
	public class sound : Notifier
	{
		#region id_sound
		private static readonly string id_soundPropertyName = GlobalDefines.GetPropertyName<sound>(m => m.id_sound);

		private int m_id_sound = 0;

		[Key]
		public int id_sound
		{
			get { return m_id_sound; }
			set
			{
				if (m_id_sound != value)
				{
					m_id_sound = value;
					OnPropertyChanged(id_soundPropertyName);
				}
			}
		}
		#endregion
				
		
		#region name
		private static readonly string namePropertyName = GlobalDefines.GetPropertyName<sound>(m => m.name);

		private string m_name = "";
		/// <summary>
		/// Название
		/// </summary>
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

		
		#region url
		private static readonly string urlPropertyName = GlobalDefines.GetPropertyName<sound>(m => m.url);

		private string m_url = "";
		/// <summary>
		/// URL для загрузки
		/// </summary>
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

		
		#region size
		private static readonly string sizePropertyName = GlobalDefines.GetPropertyName<sound>(m => m.size);

		private int m_size = 0;
		/// <summary>
		/// Размер файла в байтах
		/// </summary>
		public int size
		{
			get { return m_size; }
			set
			{
				if (m_size != value)
				{
					m_size = value;
					OnPropertyChanged(sizePropertyName);
				}
			}
		}
		#endregion

		
		#region file_path
		public static readonly string file_pathPropertyName = GlobalDefines.GetPropertyName<sound>(m => m.file_path);

		private string m_file_path = null;

		public string file_path
		{
			get { return m_file_path; }
			set
			{
				if (m_file_path != value)
				{
					if (!string.IsNullOrWhiteSpace(value) && !File.Exists(value))
						m_file_path = null;
					else
						m_file_path = value;
					OnPropertyChanged(file_pathPropertyName);
				}
			}
		}
		#endregion
	}
}
