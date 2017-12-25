using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace LaserWar.Stuff
{
	public class EntitiesContextEventArgs : EventArgs
	{
		/// <summary>
		/// Сущность БД.
		/// Введена для копирования DbEntityEntry
		/// </summary>
		public class DbEntity
		{
			public object Value { get; private set; }
			public EntityState State { get; private set; }

			public DbEntity(object value, EntityState state)
			{
				Value = value;
				State = state;
			}
		}


		public List<DbEntity> Changes { get; private set; }

		public EntitiesContextEventArgs(List<DbEntity> changes)
		{
			Changes = changes;
		}
	}
}
