using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LaserWar.Global;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using LaserWar.Models;
using LaserWar.ViewModels;

namespace LaserWar.Entities
{
	/// <summary>
	/// Описание звука в БД
	/// </summary>
	[Table("sounds")]
	public class sound : EntityBase
	{
		#region id_sound
		private static readonly string id_soundPropertyName = GlobalDefines.GetPropertyName<sound>(m => m.id_sound);

		private long m_id_sound = 0;

		[Key]
		[JsonIgnore]
		public long id_sound
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

		
		#region url
		private static readonly string urlPropertyName = GlobalDefines.GetPropertyName<sound>(m => m.url);

		private string m_url = "";
		/// <summary>
		/// URL для загрузки
		/// </summary>
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

		
		#region size
		private static readonly string sizePropertyName = GlobalDefines.GetPropertyName<sound>(m => m.size);

		private int m_size = 0;
		/// <summary>
		/// Размер файла в байтах
		/// </summary>
		[Required]
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
		[JsonIgnore]
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


		public override int GetHashCode()
		{
			string[] KeyFields = new string[] { name, size.ToString() };
			return KeyFields.GetHashCode();
		}


		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is sound)
				return name == (obj as sound).name && size == (obj as sound).size;

			if (obj is SoundModel)
				return name == (obj as SoundModel).Sound.name && size == (obj as SoundModel).Sound.size;

			if (obj is SoundViewModel)
				return name == (obj as SoundViewModel).name && size == (obj as SoundViewModel).size;

			return false;
		}
	}
}
