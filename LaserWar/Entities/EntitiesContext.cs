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
		public event EventHandler<EntitiesContextEventArgs> ChangesSavedToDB;
		private void OnChangesSavedToDB(int EntitiesChanged)
		{
			if (ChangesSavedToDB != null)
				ChangesSavedToDB(this, new EntitiesContextEventArgs(EntitiesChanged));
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

			OnChangesSavedToDB(result);

			return result;
		}


		/// <summary>
		/// Перегружаем метод SaveChanges, чтобы генерировать событие внесение изменений в БД
		/// </summary>
		/// <param name="RaiseEvent"> true - нужно генерировать событие </param>
		/// <returns></returns>
		public int SaveChanges(bool RaiseEvent)
		{
			int result = base.SaveChanges();

			if (RaiseEvent)
				OnChangesSavedToDB(result);

			return result;
		}
	}
}
