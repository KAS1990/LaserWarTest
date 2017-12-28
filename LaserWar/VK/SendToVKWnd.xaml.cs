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
using System.Windows.Shapes;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace LaserWar.Vk
{
	/// <summary>
	/// Interaction logic for SendToVKWnd.xaml
	/// </summary>
	public partial class SendToVKWnd : Window
	{
		private const string AUTH_URL = "https://oauth.vk.com/authorize?client_id=6299880&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token&v=5.52";
		private const string ACCESS_TOKEN_PARAM_NAME = "access_token=";

		private string _Token;  //Токен, использующийся при запросах
		private string _UserId;  //ID пользователя
		private Dictionary<string, string> _Response;  //Ответ на запросы
		public static readonly Uri AUTH_URI = new Uri(AUTH_URL);
		
		public SendToVKWnd()
		{
			InitializeComponent();
		}

		private void brsrGetTToken_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			string URLFromVk = brsrGetTToken.Source.ToString();
			int TokenStrInd = URLFromVk.IndexOf(ACCESS_TOKEN_PARAM_NAME);
			if (TokenStrInd > 0 && URLFromVk.Substring(TokenStrInd + ACCESS_TOKEN_PARAM_NAME.Length) != "token")
			{	// Успешно авторизовались
				try
				{
					GetUserToken();

					if (!string.IsNullOrWhiteSpace(_Token))
					{
						VkAPI._Token = _Token;
						string[] Params = { "city", "country", "photo_max" };
						_Response = VkAPI.GetInformation(_UserId, Params);
						if (_Response != null)
						{
							lblUser_ID.Content = _UserId;
							lblUser_Photo.Content = _Response["photo_max"];
							lblUser_Name.Content = _Response["first_name"];
							lblUser_Surname.Content = _Response["last_name"];
							lblUser_Country.Content = _Response["country"];
							lblUser_City.Content = _Response["city"];
						}

						// Получаем список групп пользователя
						List<VKGroup> Groups = VkAPI.GetGroups(_UserId);
						if (_Response != null)
						{
							cmbGroups.ItemsSource = Groups;
						}
					}
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
			}
		}

		private void SendToVKWnd_Loaded(object sender, RoutedEventArgs e)
		{
			brsrGetTToken.Navigate(AUTH_URI);
		}


		private void GetUserToken()
		{
			char[] Symbols = { '=', '&' };
			string[] URL = brsrGetTToken.Source.ToString().Split(Symbols);
			_Token = URL[1];
			_UserId = URL[5];
		}
	}
}
