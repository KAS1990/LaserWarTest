using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using LaserWar.Stuff;

namespace LaserWar.Entities
{
	public class EntitiesContext : DbContext
	{
		/// <summary>
		/// Событие, которое происходит при сохранении изменений в БД
		/// </summary>
		public event EventHandler<EntitiesContextEventArgs> ChangesSvedToDB;
		private void OnChangesSvedToDB(int EntitiesChanged)
		{
			if (ChangesSvedToDB != null)
				ChangesSvedToDB(this, new EntitiesContextEventArgs(EntitiesChanged));
		}

		public DbSet<sound> sounds { get; set; }

		public EntitiesContext()
			: base("DefaultConnection")
        {
        }
		

		public void LoadAllDataSets()
		{
			try
			{
				sounds.Load();
			}
			catch (Exception ex)
			{	// TO DO: Не удалось загрузить таблицу - удаляем её и создаём заново
			
			}
		}


		/// <summary>
		/// Перегружаем метод SaveChanges, чтобы генерировать событие внесение изменений в БД
		/// </summary>
		/// <returns></returns>
		public override int SaveChanges()
		{
			int result = base.SaveChanges();

			OnChangesSvedToDB(result);

			return result;
		}
	}
}
