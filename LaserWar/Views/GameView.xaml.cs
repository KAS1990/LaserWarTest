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
using System.ComponentModel;
using LaserWar.Stuff;
using System.Collections.Specialized;
using LaserWar.Global.SelectedCollection;
using LaserWar.ExtraControls.DialogWnds;

namespace LaserWar.Views
{
	/// <summary>
	/// Описание игры
	/// </summary>
	public partial class GameView : CNotifyPropertyChangedUserCtrl
	{
		#region ViewModel
		private static readonly string ViewModelPropertyName = GlobalDefines.GetPropertyName<GameView>(m => m.ViewModel);

		private GameViewModel m_ViewModel = null;

		public GameViewModel ViewModel
		{
			get { return m_ViewModel; }
			set
			{
				if (SortTasks != null)
					SortTasks.SelectedItem = null;

				if (m_ViewModel != value)
				{
					if (value != null)
					{
						DataContext = m_ViewModel = value;

						m_ViewModel.Players.CollectionChanged -= Players_CollectionChanged; // Чтобы событие не было привязано несколько раз
						m_ViewModel.Players.CollectionChanged += Players_CollectionChanged;

						foreach (PlayerViewModel Player in m_ViewModel.Players)
						{
							Player.EditStateChanged -= Player_EditStateChanged; // Чтобы событие не было привязано несколько раз
							Player.EditStateChanged += Player_EditStateChanged;
						}
					}
					OnPropertyChanged(ViewModelPropertyName);
				}
			}
		}
		#endregion


		/// <summary>
		/// Задания на сортировку
		/// </summary>
		public SelectedObservableCollection<SortTask<string>, string> SortTasks { get; set; }


		/// <summary>
		/// Источник данных для таблицы
		/// </summary>
		private CollectionViewSource vsrcPlayers
		{
			get { return Resources["vsrcPlayers"] as CollectionViewSource; }
		}
				
				
		public GameView():
			base()
		{
			InitializeComponent();
		}


		public GameView(GameViewModel viewModel) :
			base()
		{
			ViewModel = viewModel;

			CanSelectedItemManager<SortTask<string>, string> Manager = new CanSelectedItemManager<SortTask<string>, string>();
			Manager.Collection = SortTasks = new SelectedObservableCollection<SortTask<string>, string>(Manager);
			
			SortTask<string> task = new SortTask<string>(PlayerViewModel.namePropertyName, 0);
			task.Selected += task_Selected;
			SortTasks.Add(task);

			task = new SortTask<string>(PlayerViewModel.ratingPropertyName, 1);
			task.Selected += task_Selected;
			SortTasks.Add(task);

			task = new SortTask<string>(PlayerViewModel.accuracyPropertyName, 2);
			task.Selected += task_Selected;
			SortTasks.Add(task);

			task = new SortTask<string>(PlayerViewModel.shotsPropertyName, 3);
			task.Selected += task_Selected;
			SortTasks.Add(task);
			
			InitializeComponent();
		}


		/// <summary>
		/// Изменилось состояние редактирования игрока
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Player_EditStateChanged(object sender, PlayerEditEventArgs e)
		{
			if (e.Field == enEditedPlayerField.All)
			{
				LaserWarApp.MainWnd.ShowShadow = true;

				PlayerViewModel Player = (sender as PlayerViewModel);
				PlayerViewModel Copy = (sender as ICloneable).Clone() as PlayerViewModel;
				Copy.EditingField = Player.EditingField;

				PlayerSettingsDialog dlg = new PlayerSettingsDialog(LaserWarApp.MainWnd.brdShadow, Copy);
				dlg.ButtonClicked += (s, ev) =>
				{
					(s as PlayerSettingsDialog).RemoveFromHost();
					Player.EditingField = Copy.EditingField;

					if (ev.ButtonType == enButtonType.Save && SortTasks.SelectedItem != null)
					{	// Восстанавливаем сортировку
						task_Selected(SortTasks.SelectedItem, new EventArgs());
					}

					LaserWarApp.MainWnd.ShowShadow = false;
				};
			}
		}


		void task_Selected(object sender, EventArgs e)
		{
			SortTask<string> task = sender as SortTask<string>;

			if (vsrcPlayers != null && vsrcPlayers.View != null)
			{
				if (task.IsSelected)
				{
					PlayersSorter Sorter = new PlayersSorter(task.ID, task.Direction);
					(vsrcPlayers.View as ListCollectionView).CustomSort = new PlayersSorter(task.ID, task.Direction);
				}
			}
		}


		void Players_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (PlayerViewModel Player in e.NewItems)
				{
					Player.EditStateChanged -= Player_EditStateChanged; // Чтобы событие не было привязано несколько раз
					Player.EditStateChanged += Player_EditStateChanged;
				}
			}
		}


		private void btnToVk_Click(object sender, RoutedEventArgs e)
		{

		}


		private void btnToPDF_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
