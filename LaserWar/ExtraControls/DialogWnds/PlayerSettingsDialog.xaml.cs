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
using LaserWar.ViewModels;
using LaserWar.Stuff;

namespace LaserWar.ExtraControls.DialogWnds
{
	/// <summary>
	/// Окно редактирования сведений об игроке
	/// </summary>
	public partial class PlayerSettingsDialog : DialogWndBase
	{
		readonly PlayerViewModel m_ViewModel;

		public PlayerSettingsDialog()
		{
			InitializeComponent();
		}


		public PlayerSettingsDialog(Border host, PlayerViewModel ViewModel) :
			base(host)
		{
			DataContext = m_ViewModel = ViewModel;

			m_ViewModel.EditStateChanged += Player_EditStateChanged;

			InitializeComponent();
		}


		void Player_EditStateChanged(object sender, PlayerEditEventArgs e)
		{
			switch (e.State)
			{
				case enEditedPlayerState.Canceled:
					m_ViewModel.EditStateChanged -= Player_EditStateChanged;
					OnButtonClicked(enButtonType.Cancel);
					break;
				
				case enEditedPlayerState.Commited:
					m_ViewModel.EditStateChanged -= Player_EditStateChanged;
					OnButtonClicked(enButtonType.Save);
					break;
			}
		}
	}
}
