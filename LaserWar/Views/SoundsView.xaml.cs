using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaserWar.Global;
using LaserWar.ViewModels;

namespace LaserWar.Views
{
	/// <summary>
	/// Представление - звуки
	/// </summary>
	public partial class SoundsView : CNotifyPropertyChangedUserCtrl
	{
		readonly SoundsViewModel m_ViewModel = null;
		
		public SoundsView():
			base()
		{
			InitializeComponent();
		}


		public SoundsView(SoundsViewModel ViewModel) :
			base()
		{
			DataContext = m_ViewModel = ViewModel;

			InitializeComponent();
			
			IsVisibleChanged += SoundsView_IsVisibleChanged;
		}


		void SoundsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{	// Представление скрылось => нужно останавливаем воспроизведение звуков и загрузку
				m_ViewModel.StopDownloading();
				m_ViewModel.StopPlaying();
			}
		}
	}
}
