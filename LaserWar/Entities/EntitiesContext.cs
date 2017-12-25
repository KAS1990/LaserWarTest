using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using LaserWar.Stuff;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data;
using System.Collections.Specialized;
using LaserWar.Global;
using System.Threading;
using System.Diagnostics;
using SQLite.CodeFirst;

namespace LaserWar.Entities
{
	public class EntitiesContext : DbContext
	{
		private class SendFuncState<TParam, TResult>
		{
			public TResult result { get; set; }
			public TParam param { get; set; }
		}

		/// <summary>
		/// Поле, позволяющее выполнять операции с EntitiesContext не только из того потока, в котором она была создана
		/// </summary>
		private SynchronizationContext m_SyncContext = SynchronizationContext.Current;


		/// <summary>
		/// Событие, которое происходит при сохранении изменений в БД
		/// </summary>
		public event EventHandler<EntitiesContextEventArgs> ChangesSavedToDB;
		private void OnChangesSavedToDB(List<EntitiesContextEventArgs.DbEntity> changes)
		{
			if (ChangesSavedToDB != null)
				ChangesSavedToDB(this, new EntitiesContextEventArgs(changes));
		}


		/// <summary>
		/// Событие, которое происходит при полном обновлении БД
		/// </summary>
		public event EventHandler DBReset;
		private void OnDBReset()
		{
			if (SynchronizationContext.Current == m_SyncContext)
			{
				if (DBReset != null)
					DBReset(this, new EventArgs());
			}
			else
				m_SyncContext.Send(arg => 
					{
						if (DBReset != null)
							DBReset(this, new EventArgs());
					}, null);
		}

		private static readonly string soundsTableName = GlobalDefines.GetPropertyName<EntitiesContext>(m => m.sounds);
		public DbSet<sound> sounds { get; set; }

		private static readonly string gamesTableName = GlobalDefines.GetPropertyName<EntitiesContext>(m => m.games);
		public DbSet<game> games { get; set; }

		private static readonly string teamsTableName = GlobalDefines.GetPropertyName<EntitiesContext>(m => m.teams);
		public DbSet<team> teams { get; set; }

		private static readonly string playersTableName = GlobalDefines.GetPropertyName<EntitiesContext>(m => m.players);
		public DbSet<player> players { get; set; }

		public EntitiesContext()
			: base("DefaultConnection")
        {
        }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<EntitiesContext>(modelBuilder);
			Database.SetInitializer(sqliteConnectionInitializer);
		}


		void CreateTable(string SQL, string TableName)
		{
			try
			{
				Database.ExecuteSqlCommand(SQL);
			}
			catch (Exception ex)
			{
				Database.ExecuteSqlCommand("DROP TABLE \"" + TableName + "\"");
				Database.ExecuteSqlCommand(SQL);
			}
		}


		public void ClearDBData()
		{
			Database.ExecuteSqlCommand("DELETE FROM sounds");
			Database.ExecuteSqlCommand("DELETE FROM players");
			Database.ExecuteSqlCommand("DELETE FROM teams");
			Database.ExecuteSqlCommand("DELETE FROM games");
			
			LoadAllDataSets();
		}


		public void DBResseted()
		{
			OnDBReset();
		}
		

		public void LoadAllDataSets()
		{
			try
			{
				sounds.Load();
			}
			catch (Exception ex)
			{	// Не удалось загрузить таблицу - удаляем её и создаём заново
				CreateTable("CREATE TABLE \"" + soundsTableName + "\" ( `id_sound` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT NOT NULL, `url` TEXT NOT NULL, `size` INTEGER NOT NULL, `file_path` TEXT )",
							soundsTableName);
				sounds.Load();
			}

			try
			{
				games.Load();
			}
			catch (Exception ex)
			{	// Не удалось загрузить таблицу - удаляем её и создаём заново
				CreateTable("CREATE TABLE \"" + gamesTableName + "\" ( `id_game` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `url` TEXT NOT NULL, `name` TEXT NOT NULL, `date` INTEGER NOT NULL )",
							gamesTableName);
				games.Load();
			}

			try
			{
				teams.Load();
			}
			catch (Exception ex)
			{	// Не удалось загрузить таблицу - удаляем её и создаём заново
				CreateTable("CREATE TABLE \"" + teamsTableName + "\" ( `id_team` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT NOT NULL, `fk_game` INTEGER NOT NULL, FOREIGN KEY(`fk_game`) REFERENCES `games`(`id_game`) )",
							teamsTableName);
				teams.Load();
			}

			try
			{
				players.Load();
			}
			catch (Exception ex)
			{	// Не удалось загрузить таблицу - удаляем её и создаём заново
				CreateTable("CREATE TABLE \"" + playersTableName + "\" ( `id_player` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT NOT NULL, `rating` INTEGER NOT NULL, `accuracy` REAL NOT NULL, `shots` INTEGER NOT NULL, `fk_team` INTEGER NOT NULL, FOREIGN KEY(`fk_team`) REFERENCES `teams`(`id_team`) )",
							playersTableName);
				players.Load();
			}
		}


		/// <summary>
		/// Перегружаем метод SaveChanges, чтобы генерировать событие внесение изменений в БД
		/// </summary>
		/// <returns></returns>
		public override int SaveChanges()
		{
			return SaveChanges(true);
		}


		/// <summary>
		/// Перегружаем метод SaveChanges, чтобы генерировать событие внесение изменений в БД
		/// </summary>
		/// <param name="RaiseEvent"> true - нужно генерировать событие </param>
		/// <returns></returns>
		public int SaveChanges(bool RaiseEvent)
		{
			SendFuncState<bool, int> state = new SendFuncState<bool, int>()
			{
				param = RaiseEvent
			};

			if (SynchronizationContext.Current == m_SyncContext)
				SaveChangesInternal(state);
			else
			{
				m_SyncContext.Send(SaveChangesInternal, state);
			}
			
			return state.result;
		}


		/// <summary>
		/// Сохранение изменений.
		/// Эту операцию нужно делать в том же потоке, в котором была создана БД!!!
		/// </summary>
		/// <param name="RaiseEvent"></param>
		/// <returns></returns>
		void SaveChangesInternal(object param)
		{
			SendFuncState<bool, int> state = param as SendFuncState<bool, int>;

			GlobalDefines.DoEvents(LaserWarApp.MainWnd);
						
			try
			{
				ChangeTracker.DetectChanges();
				if (ChangeTracker.HasChanges())
				{
					List<EntitiesContextEventArgs.DbEntity> changes = new List<EntitiesContextEventArgs.DbEntity>();
					foreach (DbEntityEntry entity in ChangeTracker.Entries())
					{
						if (entity.State != EntityState.Unchanged)
							changes.Add(new EntitiesContextEventArgs.DbEntity(entity.Entity, entity.State));
					}
					state.result = base.SaveChanges();

					if (state.param)
						OnChangesSavedToDB(changes);
				}
				else
					state.result = base.SaveChanges();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
	}
}
