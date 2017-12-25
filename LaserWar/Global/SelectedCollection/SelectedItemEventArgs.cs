using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.Global.SelectedCollection
{
	public class SelectedItemEventArgs<T, TId> : EventArgs where T : CanSelectedItem<TId>
	{
		public T PrevSelectedItem { get; private set; }
		public T CurSelectedItem { get; private set; }

		public SelectedItemEventArgs(T prevSelectedItem, T curSelectedItem)
		{
			PrevSelectedItem = prevSelectedItem;
			CurSelectedItem = curSelectedItem;
		}
	}
}
