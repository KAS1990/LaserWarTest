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
using LaserWar.Global;

namespace LaserWar.Stuff
{
	/// <summary>
	/// Окно сообщения.
	/// Его можно конечно же сделать не отдельным окном, а рисовать на основной форме, но для простоты и скорости я решил сделать так
	/// </summary>
	public partial class MessageWnd : CNotifyPropertyChangedWnd
	{
		#region Message
		private static readonly string MessagePropertyName = GlobalDefines.GetPropertyName<MessageWnd>(m => m.Message);

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
				
				
		public MessageWnd()
		{
			InitializeComponent();
		}
	}
}
