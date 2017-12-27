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
using Microsoft.Win32;
using LaserWar.Views.Converters;
using System.Globalization;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.awt.geom;
using System.Threading.Tasks;
using LaserWar.Views.ValidationRules;

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


		private bool m_IsManualEditCommit;


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

			m_IsManualEditCommit = false;

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
					ev.Dlg.RemoveFromHost();
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


		public void OnViewClosed()
		{
			/*Key key = Key.Escape;                    // Key to send
			IInputElement target = Keyboard.FocusedElement;    // Target element
			RoutedEvent routedEvent = Keyboard.KeyDownEvent; // Event to send

			target.RaiseEvent(
				  new KeyEventArgs(
					Keyboard.PrimaryDevice,
					PresentationSource.FromVisual(target as Visual),
					0,
					key) { RoutedEvent = routedEvent }
				);*/
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


		#region Работа с PDF
		private void btnToPDF_Click(object sender, RoutedEventArgs e)
		{
			LaserWarApp.MainWnd.ShowShadow = LaserWarApp.MainWnd.ShowProgressShape = true;

			SaveFileDialog dlg = new SaveFileDialog()
			{
				AddExtension = true,
				DefaultExt = ".pdf",
				Filter = Properties.Resources.resSavePDFFilter,
				OverwritePrompt = true,
				ValidateNames = true
			};

			bool? res = dlg.ShowDialog();
			if (res.HasValue && res.Value)
			{	// Создаём PDF в другом потоке
				Task.Factory.StartNew(WriteDataToPDF, dlg.FileName);
			}
			else
				LaserWarApp.MainWnd.ShowShadow = LaserWarApp.MainWnd.ShowProgressShape = false;
		}


		private void WriteDataToPDF(object FileName)
		{
			try
			{
				using (FileStream fs = new FileStream(FileName.ToString(), FileMode.Create, FileAccess.Write, FileShare.None))
				{
					Document Doc = new Document(PageSize.A4, 30, 30, 30, 30);
					PdfWriter writer = PdfWriter.GetInstance(Doc, fs);
					Doc.Open();
					PdfContentByte canvas = writer.DirectContent;
					Doc.AddCreator(Environment.UserName);

					// Название игры
					BaseFont baseFont = BaseFont.CreateFont("Arial Cyr.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
					iTextSharp.text.Paragraph para = new iTextSharp.text.Paragraph(new Phrase(m_ViewModel.name + "  " + m_ViewModel.date.ToString(),
																					new Font(baseFont, 30, Font.BOLD)))
					{
						Alignment = Element.ALIGN_LEFT,
					};
					Doc.Add(para);

					// Таблица с игроками
					Font HeaderFont = new Font(baseFont, 10, Font.BOLD);
					PdfPTable Table = new PdfPTable(new float[] { 1.2f, 1, 1, 1 })
					{
						SpacingBefore = 10,
						SpacingAfter = 20,
						PaddingTop = 10,
						HeaderRows = 1,
						HorizontalAlignment = Element.ALIGN_JUSTIFIED,
						WidthPercentage = 100,
					};
					Table.DefaultCell.UseVariableBorders = true;
					Table.DefaultCell.BorderWidthBottom = 2;
					Table.DefaultCell.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(255, 240, 240, 240));
					Table.DefaultCell.BorderWidthLeft = 0;
					Table.DefaultCell.BorderWidthRight = 0;
					Table.DefaultCell.BorderWidthTop = 0;
					Table.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
					Table.DefaultCell.PaddingTop = 10;
					Table.DefaultCell.PaddingBottom = 5;

					// Заголовок таблицы
					PdfPCell Cell = new PdfPCell(Table.DefaultCell)
					{
						Phrase = new Phrase(Properties.Resources.resPlayerName, HeaderFont),
						Border = PdfPCell.NO_BORDER,
						PaddingTop = 20
					};
					Table.AddCell(Cell);
					Cell = new PdfPCell(Table.DefaultCell)
					{
						Phrase = new Phrase(Properties.Resources.resPlayerName, HeaderFont),
						Border = PdfPCell.NO_BORDER,
						PaddingTop = 20
					};
					Table.AddCell(Cell);
					Cell = new PdfPCell(Table.DefaultCell)
					{
						Phrase = new Phrase(Properties.Resources.resRating, HeaderFont),
						Border = PdfPCell.NO_BORDER,
						PaddingTop = 20
					};
					Table.AddCell(Cell);
					Cell = new PdfPCell(Table.DefaultCell)
					{
						Phrase = new Phrase(Properties.Resources.resShots, HeaderFont),
						Border = PdfPCell.NO_BORDER,
						PaddingTop = 20
					};
					Table.AddCell(Cell);

					// Формируем список участников
					IEnumerable<PlayerViewModel> PlayersForPDF = m_ViewModel.Players;
					if (SortTasks.SelectedItem != null)
					{	// Оставляем ту сортировку, которая сейчас выбрана на форме
						PlayersSorter Sorter = new PlayersSorter(SortTasks.SelectedItem.ID, SortTasks.SelectedItem.Direction);
						PlayersForPDF = PlayersForPDF.OrderBy(arg => arg, Sorter);
					}

					// Заполняем таблицу
					Font TeamNameFont = new Font(baseFont, 16, Font.BOLD);
					Font StandartFont = new Font(baseFont, 10);
					long TeamId = -1;
					AccuracyMarkupConverter convAccuracy = new AccuracyMarkupConverter();
					foreach (PlayerViewModel player in PlayersForPDF)
					{
						if (player.TeamId != TeamId)
						{	// Добавляем строку с командой
							Cell = new PdfPCell(Table.DefaultCell)
							{
								Phrase = new Phrase(player.TeamName, TeamNameFont),
								Colspan = 4,
								Border = PdfPCell.NO_BORDER,
								PaddingTop = 15,
							};
							Table.AddCell(Cell);
							TeamId = player.TeamId;
						}

						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(player.name, StandartFont),
						};
						Table.AddCell(Cell);
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(player.rating.ToString(), StandartFont),
						};
						Table.AddCell(Cell);
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(convAccuracy.Convert(player.accuracy, typeof(string), null, CultureInfo.CurrentCulture) as string, StandartFont),
						};
						Table.AddCell(Cell);
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(player.shots.ToString(), StandartFont),
						};
						Table.AddCell(Cell);
					}
					Doc.Add(Table);

					// Статистика по командам
					Table = new PdfPTable(new float[] { 1, 1 })
					{
						SpacingBefore = 10,
						PaddingTop = 10,
						HorizontalAlignment = Element.ALIGN_JUSTIFIED,
						WidthPercentage = 100,
					};
					Table.DefaultCell.Border = PdfPCell.NO_BORDER;

					int MaxRating = m_ViewModel.Teams.Max(arg => arg.Rating);

					// Формируем список команд
					IEnumerable<TeamViewModel> TeamsForPDF = m_ViewModel.Teams.OrderBy(arg => arg.id_team);
					BaseColor BarColor = new BaseColor(System.Drawing.Color.FromArgb(255, 0, 90, 255));
					int TeamNumber = -1;
					foreach (TeamViewModel team in TeamsForPDF)
					{
						TeamNumber++;

						PdfPTable TableTeam = new PdfPTable(new float[] { 1, 1 })
						{
							WidthPercentage = 95,
							HorizontalAlignment = TeamNumber % 2 == 0 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT,
						};

						// Название команды
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(team.name, TeamNameFont),
							HorizontalAlignment = Element.ALIGN_LEFT,
							PaddingTop = 25,
							PaddingBottom = 0,
							Colspan = 2,
						};
						TableTeam.AddCell(Cell);

						// Рейтинг
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(Properties.Resources.resRating, HeaderFont),
							HorizontalAlignment = Element.ALIGN_LEFT,
							PaddingTop = 20,
							PaddingBottom = 5,
						};
						TableTeam.AddCell(Cell);
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(team.Rating.ToString("F0"), HeaderFont),
							HorizontalAlignment = Element.ALIGN_RIGHT,
							PaddingTop = 20,
							PaddingBottom = 3,
						};
						TableTeam.AddCell(Cell);

						// Синий прямоугольник
						float BarWidth = ((float)team.Rating / MaxRating) * (Doc.PageSize.Width - Doc.RightMargin - Doc.LeftMargin) * 0.5f * (TableTeam.WidthPercentage - 1) / 100.0f;
						PdfTemplate template = canvas.CreateTemplate(BarWidth, 10);
						template.SetColorFill(BarColor);
						template.Rectangle(0, 0, BarWidth, 10);
						template.Fill();
						writer.ReleaseTemplate(template);
						Cell = new PdfPCell(iTextSharp.text.Image.GetInstance(template))
						{
							PaddingTop = 0,
							PaddingBottom = 0,
							BorderWidth = 2,
							BorderColor = BarColor,
							FixedHeight = 10,
							Colspan = 2,
						};
						TableTeam.AddCell(Cell);

						// Точность
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(Properties.Resources.resAccuracy, HeaderFont),
							HorizontalAlignment = Element.ALIGN_LEFT,
							PaddingTop = 20,
							PaddingBottom = 5,
						};
						TableTeam.AddCell(Cell);
						Cell = new PdfPCell(Table.DefaultCell)
						{
							Phrase = new Phrase(convAccuracy.Convert(team.Accuracy, typeof(string), null, CultureInfo.CurrentCulture) as string, HeaderFont),
							HorizontalAlignment = Element.ALIGN_RIGHT,
							PaddingTop = 20,
							PaddingBottom = 3,
						};
						TableTeam.AddCell(Cell);

						// Синий прямоугольник
						BarWidth = team.Accuracy * (Doc.PageSize.Width - Doc.RightMargin - Doc.LeftMargin) * 0.5f * (TableTeam.WidthPercentage - 1) / 100.0f;
						template = canvas.CreateTemplate(BarWidth, 10);
						template.SetColorFill(BarColor);
						template.Rectangle(0, 0, BarWidth, 10);
						template.Fill();
						writer.ReleaseTemplate(template);
						Cell = new PdfPCell(iTextSharp.text.Image.GetInstance(template))
						{
							PaddingTop = 0,
							PaddingBottom = 0,
							BorderWidth = 2,
							BorderColor = BarColor,
							FixedHeight = 10,
							Colspan = 2,
						};
						TableTeam.AddCell(Cell);

						Cell = new PdfPCell(Table.DefaultCell);
						Cell.AddElement(TableTeam);
						Table.AddCell(Cell);
					}
					Table.CompleteRow(); // Без этого при нечётном количестве строк не будет выведена последняя строка
					Doc.Add(Table);

					Doc.Close();
					writer.Close();

					Application.Current.Dispatcher.Invoke(new Action(delegate()
					{	// Это нужно делать в основном потоке
						MessageDialog msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
						{
							Title = Properties.Resources.resInformation,
							Message = string.Format(Properties.Resources.resfmtPDFFileSavedSuccessfully, FileName)
						};
						msg.ButtonClicked += msg_ButtonClicked;
					}));
				}
			}
			catch (Exception ex)
			{
				Application.Current.Dispatcher.Invoke(new Action(delegate()
				{	// Это нужно делать в основном потоке
					MessageDialog msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
					{
						Title = Properties.Resources.resErrorOccured,
						Message = string.Format(Properties.Resources.resfmtPDFFileDontSaved, FileName)
					};
					msg.ButtonClicked += msg_ButtonClicked;
				}));
			}
		}
		#endregion


		void msg_ButtonClicked(object sender, ButtonClickedEventArgs e)
		{
			e.Dlg.RemoveFromHost();
			LaserWarApp.MainWnd.ShowShadow = LaserWarApp.MainWnd.ShowProgressShape = false;
		}
		

		private void dgrdPlayers_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			// Переводим объект, привязанный к строке в режим редактирования
			PlayerViewModel SelctedPlayer = (e.Row.Item as PlayerViewModel);
			if (SelctedPlayer.EditCommand.CanExecute(null))
				SelctedPlayer.EditCommand.Execute(enEditedPlayerField.Field);
		}


		private void dgrdPlayers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			try
			{
				if (!m_IsManualEditCommit)
				{
					m_IsManualEditCommit = true;
					// Вручную вызываем обновление ViewModel
					PlayerViewModel SelectedPlayer = (e.Row.Item as PlayerViewModel);

					// Удаляем из SelectedPlayer те ошибки, которых уже нет
					List<string> ErrorsToLeave = new List<string>();
					foreach (ValidationError error in Validation.GetErrors(e.Row))
					{
						BindingExpression be = error.BindingInError as BindingExpression;
						if (be != null)
						{
							PlayerViewModel Player = be.DataItem as PlayerViewModel;
							if (Player != null && Player.id_player == SelectedPlayer.id_player)
								ErrorsToLeave.Add((error.RuleInError as PlayerValidationRuleBase).PropertyName);
						}
					}
					SelectedPlayer.RemoveErrorsExcept(ErrorsToLeave);

					if (e.EditAction == DataGridEditAction.Commit)
						dgrdPlayers.CommitEdit(DataGridEditingUnit.Row, true);
					else
						dgrdPlayers.CancelEdit(DataGridEditingUnit.Row);

					m_IsManualEditCommit = false;
				}
			}
			catch (Exception ex)
			{

			}
		}


		private void dgrdPlayers_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			// Вызываем команду для объекта, который привязан к строке
			PlayerViewModel SelectedPlayer = (e.Row.Item as PlayerViewModel);
			if (e.EditAction == DataGridEditAction.Commit)
			{
				if (SelectedPlayer.CommitChangesCommand.CanExecute(null))
					SelectedPlayer.CommitChangesCommand.Execute(enEditedPlayerField.Field);
			}
			else
			{
				if (SelectedPlayer.RestoreCommand.CanExecute(null))
					SelectedPlayer.RestoreCommand.Execute(enEditedPlayerField.Field);
			}
		}


		void Row_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (((e.Source as DataGridRow).Item as PlayerViewModel).EditCommand.CanExecute(null))
			{	// Вызываем команду EditCommand отсюда, а не из XAML, т.к. в этом случае оно будет обрабатываться в большем количестве случаев
				((e.Source as DataGridRow).Item as PlayerViewModel).EditCommand.Execute(enEditedPlayerField.All);
				e.Handled = true;
			}
		}


		private void Cell_ValidationError(object sender, ValidationErrorEventArgs e)
		{
			BindingExpression be = e.Error.BindingInError as BindingExpression;
			if (be != null)
			{
				PlayerViewModel Player = be.DataItem as PlayerViewModel;
				if (Player != null)
					Player.AddError((e.Error.RuleInError as PlayerValidationRuleBase).PropertyName);
			}
		}



	}
}
