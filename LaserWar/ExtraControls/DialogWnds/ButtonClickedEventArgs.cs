using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.ExtraControls.DialogWnds
{
	public class ButtonClickedEventArgs : EventArgs
	{
		public DialogWndBase Dlg { get; private set; }
		public enButtonType ButtonType { get; private set; }

		public ButtonClickedEventArgs(DialogWndBase dlg, enButtonType buttonType)
		{
			Dlg = dlg;
			ButtonType = buttonType;
		}
	}
}
