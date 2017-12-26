using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using LaserWar.ViewModels;

namespace LaserWar.Views
{
	public class GenericPlayersSorter : IComparer<PlayerViewModel>
	{
		public ListSortDirection? Direction { get; set; }
		public string SortMember { get; set; }


		public GenericPlayersSorter(string sortMember, ListSortDirection? direction)
		{
			SortMember = sortMember;
			Direction = direction;
		}


		public int Compare(PlayerViewModel x, PlayerViewModel y)
		{
			if (x.TeamId == y.TeamId)
				return CompareBySortMember(x, y);
			return x.TeamId < y.TeamId ? -1 : 1;
		}


		protected int CompareBySortMember(PlayerViewModel xVm, PlayerViewModel yVm)
		{
			var xValue = xVm.GetType().GetProperty(SortMember).GetValue(xVm, null);
			var yValue = yVm.GetType().GetProperty(SortMember).GetValue(yVm, null);
			int result = (xValue as IComparable).CompareTo(yValue);
			return Direction == ListSortDirection.Descending ? result * -1 : result;
		}
	}


	public class PlayersSorter : GenericPlayersSorter, IComparer
	{
		public PlayersSorter(string sortMember, ListSortDirection? direction):
			base(sortMember, direction)
		{
		}

		public int Compare(object x, object y)
		{
			return base.Compare((PlayerViewModel)x, (PlayerViewModel)y);
		}
	}
}
