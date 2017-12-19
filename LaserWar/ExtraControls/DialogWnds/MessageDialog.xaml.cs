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

namespace LaserWar.ExtraControls.DialogWnds
{
	/// <summary>
	/// Окно сообщения
	/// </summary>
	public partial class MessageDialog : DialogWndBase
	{
		#region Title
		private static readonly string TitlePropertyName = GlobalDefines.GetPropertyName<MessageDialog>(m => m.Title);

		private string m_Title = "";
		/// <summary>
		/// Заголовок сообщения
		/// </summary>
		public string Title
		{
			get { return m_Title; }
			set
			{
				if (m_Title != value)
				{
					m_Title = value;
					OnPropertyChanged(TitlePropertyName);
				}
			}
		}
		#endregion
		
		
		#region Message
		private static readonly string MessagePropertyName = GlobalDefines.GetPropertyName<MessageDialog>(m => m.Message);

		private string m_Message = "";
		/// <summary>
		/// Текст сообщения
		/// </summary>
		public string Message
		{
			get { return m_Message; }
			set
			{
				if (m_Message != value)
				{
					m_Message = value;
					OnPropertyChanged(MessagePropertyName);
				}
			}
		}
		#endregion


		public MessageDialog()
		{
			InitializeComponent();
		}


		public MessageDialog(Border host):
			base(host)
		{
			InitializeComponent();
		}


		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			OnButtonClicked(enButtonType.Close);
		}
	}
}
