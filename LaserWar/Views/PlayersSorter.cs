using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using LaserWar.ViewModels;

namespace LaserWar.Views
{
	public class PlayersSorter : IComparer
	{
		public ListSortDirection? Direction { get; set; }
		public string SortMember { get; set; }

		public PlayersSorter(string sortMember, ListSortDirection? direction)
		{
			SortMember = sortMember;
			Direction = direction;
		}

		public int Compare(object x, object y)
		{
			var xVm = (PlayerViewModel)x;
			var yVm = (PlayerViewModel)y;

			if (xVm.TeamId == yVm.TeamId)
				return CompareBySortMember(xVm, yVm);
			return xVm.TeamId < yVm.TeamId ? -1 : 1;
		}

		
		private int CompareBySortMember(PlayerViewModel xVm, PlayerViewModel yVm)
		{
			var xValue = xVm.GetType().GetProperty(SortMember).GetValue(xVm, null);
			var yValue = yVm.GetType().GetProperty(SortMember).GetValue(yVm, null);
			int result = (xValue as IComparable).CompareTo(yValue);
			return Direction == ListSortDirection.Descending ? result * -1 : result;
		}
	}
}
