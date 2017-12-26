using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Global;
using System.Windows.Controls;

namespace LaserWar.ExtraControls.DialogWnds
{
	/// <summary>
	/// Базовый класс для диалоговых окон
	/// </summary>
	public class DialogWndBase : CNotifyPropertyChangedUserCtrl
	{
		#region Host
		private Border m_Host = null;
		/// <summary>
		/// Контрол, на котором будет располагатьсь окно
		/// </summary>
		protected Border Host
		{
			get { return m_Host; }
			private set
			{
				if (m_Host != value)
				{
					m_Host = value;
				}
			}
		}
		#endregion


		/// <summary>
		/// Событие, которое происходит при нажатии на кнопку в окне
		/// </summary>
		public event EventHandler<ButtonClickedEventArgs> ButtonClicked;
		protected void OnButtonClicked(enButtonType ButtonType)
		{
			if (ButtonClicked != null)
				ButtonClicked(this, new ButtonClickedEventArgs(this, ButtonType));
		}


		public DialogWndBase()
		{
		}

	
		/// <summary>
		/// Создать окно и добавить его на хост
		/// </summary>
		/// <param name="host"></param>
		public DialogWndBase(Border host)
		{
			Host = host;
			Host.Child = this;
		}


		/// <summary>
		/// Убрать 
		/// </summary>
		public void RemoveFromHost()
		{
			Host.Child = null;
		}
	}
}
