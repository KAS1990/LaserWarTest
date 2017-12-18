using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.Stuff
{
	public class EntitiesContextEventArgs : EventArgs
	{
		public int EntitiesChanged { get; private set; }

		public EntitiesContextEventArgs(int entitiesChanged)
		{
			EntitiesChanged = entitiesChanged;
		}
	}
}
