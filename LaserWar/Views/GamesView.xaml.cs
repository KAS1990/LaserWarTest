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
using System.Collections.Specialized;
using System.ComponentModel;

namespace LaserWar.Views
{
	/// <summary>
	/// Список игр
	/// </summary>
	public partial class GamesView : CNotifyPropertyChangedUserCtrl
	{
		readonly GamesViewModel m_ViewModel = null;
		GameView m_GameView = null;
					

		public GamesView():
			base()
		{
			InitializeComponent();
		}


		public GamesView(GamesViewModel ViewModel) :
			base()
		{
			DataContext = m_ViewModel = ViewModel;
									
			InitializeComponent();
						
			IsVisibleChanged += GamesView_IsVisibleChanged;

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
			
			m_GameView = new GameView(null)
			{
				Visibility = Visibility.Hidden
			};
			
			grdMain.Children.Add(m_GameView);
		}


		void GamesView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{	// Представление скрылось => отображаем игры
				m_ViewModel.SelectedGame = null;
			}
		}


		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (m_GameView == null)
				return;
					
			if (e.PropertyName == GamesViewModel.SelectedGamePropertyName)
			{
				if (m_ViewModel.SelectedGame == null)
				{
					m_GameView.OnViewClosed();
					m_GameView.Visibility = Visibility.Hidden;
					dpGames.Visibility = Visibility.Visible;
				}
				else
				{
					m_GameView.Visibility = Visibility.Visible;
					dpGames.Visibility = Visibility.Hidden;
					m_GameView.ViewModel = m_ViewModel.SelectedGame;
				}
			}
		}
	}
}
