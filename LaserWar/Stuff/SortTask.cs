using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.ComponentModel;
using LaserWar.Commands;
using LaserWar.Global.SelectedCollection;

namespace LaserWar.Stuff
{
	public class SortTask<TId> : CanSelectedItem<TId>, ICloneable
	{
		#region Direction
		private static readonly string DirectionPropertyName = GlobalDefines.GetPropertyName<SortTask<TId>>(m => m.Direction);

		private ListSortDirection m_Direction = ListSortDirection.Ascending;

		public ListSortDirection Direction
		{
			get { return m_Direction; }
			set
			{
				if (m_Direction != value)
				{
					m_Direction = value;
					OnPropertyChanged(DirectionPropertyName);
				}
			}
		}
		#endregion


		public SortTask(TId id, int index)
			: base(id, index)
		{
			
		}


		public override void OnSelected()
		{
			Direction = ListSortDirection.Ascending;
			base.OnSelected();
		}


		protected override void OnReselected()
		{
			Direction = Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
			base.OnSelected();
		}


		object ICloneable.Clone()
		{
			SortTask<TId> result = new SortTask<TId>(this.ID, this.Index)
			{
				IsSelected = this.IsSelected,
				Direction = this.Direction
			};

			return result;
		}
	}
}
