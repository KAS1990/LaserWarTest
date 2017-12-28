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
using System.IO;
using LaserWar.ExtraControls.DialogWnds;
using LaserWar.Views;
using LaserWar.Global;
using LaserWar.Vk;
using LaserWar.Commands;
using System.Collections.ObjectModel;
using LaserWar.Stuff;
using System.Threading.Tasks;

namespace LaserWar.VK
{
	/// <summary>
	/// Отправка данных в группу ВК
	/// </summary>
	public partial class SendToVKDialog : DialogWndBase
	{
		private const string APP_ID = "6299880";
		private const string AUTH_URL = "https://oauth.vk.com/authorize?client_id=" + APP_ID + "&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=photos+groups+wall+offline+messages+friends+docs+stats&response_type=token&v=5.52";
		private const string ACCESS_TOKEN_PARAM_NAME = "access_token=";
		public static readonly Uri AUTH_URI = new Uri(AUTH_URL);

		private string m_Token;  //Токен, использующийся при запросах
		private string m_UserId;  //ID пользователя
		private Dictionary<string, string> m_Response;  //Ответ на запросы
		

		readonly GameView m_Parent;


		public ObservableCollection<VKGroup> Groups { get; set; }

		
		#region AuthFinished
		private static readonly string AuthFinishedPropertyName = GlobalDefines.GetPropertyName<SendToVKDialog>(m => m.AuthFinished);

		private bool m_AuthFinished = false;

		public bool AuthFinished
		{
			get { return m_AuthFinished; }
			set
			{
				if (m_AuthFinished != value)
				{
					m_AuthFinished = value;
					OnPropertyChanged(AuthFinishedPropertyName);
				}
			}
		}
		#endregion

		
		#region PostText
		private static readonly string PostTextPropertyName = GlobalDefines.GetPropertyName<SendToVKDialog>(m => m.PostText);

		private string m_PostText = "";

		public string PostText
		{
			get { return m_PostText; }
			set
			{
				if (m_PostText != value)
				{
					m_PostText = value;
					OnPropertyChanged(PostTextPropertyName);
				}
			}
		}
		#endregion
		
		
		#region SelectedGroup
		private static readonly string SelectedGroupPropertyName = GlobalDefines.GetPropertyName<SendToVKDialog>(m => m.SelectedGroup);

		private VKGroup m_SelectedGroup = null;

		public VKGroup SelectedGroup
		{
			get { return m_SelectedGroup; }
			set
			{
				if (m_SelectedGroup != value)
				{
					m_SelectedGroup = value;
					OnPropertyChanged(SelectedGroupPropertyName);
					PublicateCommand.RaiseCanExecuteChanged();
				}
			}
		}
		#endregion


		bool m_PostPublished = false;
		

		private readonly RelayCommand m_CloseCommand;
		/// <summary>
		/// Закрытие окна
		/// </summary>
		public RelayCommand CloseCommand
		{
			get { return m_CloseCommand; }
		}


		private readonly RelayCommand m_PublicateCommand;
		/// <summary>
		/// Публикация в ВК
		/// </summary>
		public RelayCommand PublicateCommand
		{
			get { return m_PublicateCommand; }
		}
				

		public SendToVKDialog()
		{
			InitializeComponent();
		}


		public SendToVKDialog(Border host, GameView Parent) :
			base(host)
		{
			m_Parent = Parent;

			m_CloseCommand = new RelayCommand(arg => OnButtonClicked(enButtonType.Close));
			m_PublicateCommand = new RelayCommand(PublicateCommandHandler, arg => SelectedGroup != null);

			InitializeComponent();
		}


		/// <summary>
		/// Сохранение скрина приложения в файл
		/// </summary>
		bool CreateScreen(out string FullFilePath)
		{
			// Убираем затемнение на время создания скрина
			LaserWarApp.MainWnd.ShowShadow = false;

			PresentationSource pSource = PresentationSource.FromVisual(LaserWarApp.MainWnd);

			Matrix m = pSource.CompositionTarget.TransformToDevice;
			double dpiX = m.M11 * 96.0;
			double dpiY = m.M22 * 96.0;

			RenderTargetBitmap bmp = new RenderTargetBitmap((int)LaserWarApp.MainWnd.ActualWidth,
															(int)LaserWarApp.MainWnd.ActualHeight,
															dpiX,
															dpiY,
															PixelFormats.Pbgra32);

			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				VisualBrush visualBrush = new VisualBrush(LaserWarApp.MainWnd);
				drawingContext.DrawRectangle(visualBrush, null,
					new Rect(new System.Windows.Point(0, 0), new Size(LaserWarApp.MainWnd.Width, LaserWarApp.MainWnd.Height)));
			}

			bmp.Render(drawingVisual);

			LaserWarApp.MainWnd.ShowShadow = true;

			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
						
			FullFilePath = Directory.GetCurrentDirectory() + "\\Screen.png"; 
			try
			{
				if (File.Exists(FullFilePath))
					File.Delete(FullFilePath);
			}
			catch { }

			try
			{
				using (Stream stm = File.Create(FullFilePath))
					encoder.Save(stm);
			}
			catch
			{
				return false;
			}

			return true;
		}


		private void brsrGetTToken_Navigated(object sender, NavigationEventArgs e)
		{
			SelectedGroup = null;

			string URLFromVk = brsrGetTToken.Source.ToString();
			int TokenStrInd = URLFromVk.IndexOf(ACCESS_TOKEN_PARAM_NAME);
			if (TokenStrInd > 0 && URLFromVk.Substring(TokenStrInd + ACCESS_TOKEN_PARAM_NAME.Length) != "token")
			{	// Успешно авторизовались
				try
				{
					GetUserToken();

					if (!string.IsNullOrWhiteSpace(m_Token))
					{
						VkAPI.m_Token = m_Token;
						string[] Params = { "photo_100" };
						m_Response = VkAPI.GetInformation(m_UserId, Params);
						lblUserFirstLastName.Content = m_Response["first_name"] + " " + m_Response["last_name"];
						imgUser.Source = new BitmapImage(new Uri(m_Response["photo_100"]));

						// Получаем список групп пользователя
						Groups = new ObservableCollection<VKGroup>(VkAPI.GetGroups(m_UserId));
						if (m_Response != null)
							cmbGroups.ItemsSource = Groups;

						AuthFinished = true;
					}
					else
						AuthFinished = false;
				}
				catch (Exception ex)
				{
					AuthFinished = false;
				}
			}
			else
				AuthFinished = false;
		}


		private void SendToVKDialog_Loaded(object sender, RoutedEventArgs e)
		{
			brsrGetTToken.Navigate(AUTH_URI);
		}


		private void GetUserToken()
		{
			char[] Symbols = { '=', '&' };
			string[] URL = brsrGetTToken.Source.ToString().Split(Symbols);
			m_Token = URL[1];
			m_UserId = URL[5];
		}

		private void PublicateCommandHandler(object param)
		{
			RemoveFromHost(); // Удаляем себя с формы

			Task.Factory.StartNew(Publicate, null);
		}


		void Publicate(object state)
		{
			MessageDialog msg = null;
			
			// Создаём PDF
			PDFCreationTask Task = new PDFCreationTask()
			{
				FileName = Directory.GetCurrentDirectory() + "\\" + m_Parent.ViewModel.name + ".pdf",
				ShowMessage = false
			};
			if (!m_Parent.WriteDataToPDF(Task))
			{
				Application.Current.Dispatcher.Invoke(new Action(delegate()
				{
					msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
					{
						Title = Properties.Resources.resErrorOccured,
						Message = string.Format(Properties.Resources.resfmtPDFFileDontSaved, Task.FileName)
					};
					msg.ButtonClicked += msg_ButtonClicked;
				}));

				return;
			}
			string ScreenshotFullFilePath = "";
			bool result = false;
			Application.Current.Dispatcher.Invoke(new Action(delegate()
			{
				result = CreateScreen(out ScreenshotFullFilePath);
			}));
			if (!result)
			{
				File.Delete(Task.FileName);

				Application.Current.Dispatcher.Invoke(new Action(delegate()
				{
					msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
					{
						Title = Properties.Resources.resErrorOccured,
						Message = Properties.Resources.resCantCreateScreen
					};
					msg.ButtonClicked += msg_ButtonClicked;
				}));

				return;
			}

			photos_saveWallPhotoAns PhotoInfo = VkAPI.LoadImageToServer(SelectedGroup.id, ScreenshotFullFilePath);
			if (PhotoInfo == null)
			{
				File.Delete(Task.FileName);
				CantPublicatePostMsg();
				return;
			}

			docs_SaveAns DocInfo = VkAPI.LoadPDFToServer(Task.FileName);
			File.Delete(Task.FileName);
			if (DocInfo == null)
			{
				CantPublicatePostMsg();
				return;
			}
			
			if (VkAPI.CreatePost(SelectedGroup.id, PostText, PhotoInfo, DocInfo) < 0)
			{
				CantPublicatePostMsg();
				return;
			}

			m_PostPublished = true;
						
			Application.Current.Dispatcher.Invoke(new Action(delegate()
			{	// Это нужно делать в основном потоке
				// Всё прошло удачно
				msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
				{
					Title = Properties.Resources.resInformation,
					Message = string.Format(Properties.Resources.resfmtPostPublishedSuccessfully, SelectedGroup.name)
				};
				msg.ButtonClicked += msg_ButtonClicked;
			}));
		}


		void CantPublicatePostMsg()
		{
			Application.Current.Dispatcher.Invoke(new Action(delegate()
			{
				MessageDialog msg = new MessageDialog(LaserWarApp.MainWnd.brdShadow)
				{
					Title = Properties.Resources.resErrorOccured,
					Message = Properties.Resources.resCantPublicatePost
				};
				msg.ButtonClicked += msg_ButtonClicked;
			}));
		}


		void msg_ButtonClicked(object sender, ButtonClickedEventArgs e)
		{
			e.Dlg.RemoveFromHost(); // Удаляем сообщение с формы
			if (m_PostPublished)
				OnButtonClicked(enButtonType.Publicate);
			else
				AddToHost(); // Снова добавляем себя на форму
		}
	}
}
