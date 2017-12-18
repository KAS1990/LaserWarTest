﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace LaserWar.Global
{
	/// <summary>
	/// Базовый класс для всех окон, у которых необходимо реализовать интерфейс INotifyPropertyChanged
	/// </summary>
	public class CNotifyPropertyChangedWnd : Window, INotifyPropertyChanged
	{
		public CNotifyPropertyChangedWnd()
		{
			DataContext = this;
		}


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		public virtual void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}
