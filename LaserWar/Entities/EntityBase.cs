using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LaserWar.Entities
{
	public class EntityBase : Notifier
	{
		/// <summary>
		/// Это поле необходимо для реализации запросов в классах-сущностях
		/// </summary>
		[NotMapped]
		[JsonIgnore]
		[XmlIgnore]
		public EntitiesContext Context { get; set; }


		/// <summary>
		/// Переписать данные из объекта в БД
		/// </summary>
		public void ToDB()
		{
			try
			{
				Context.Entry(this).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
			}
			catch (Exception ex)
			{
				ex.ToString();
				return;
			}
		}
	}
}
