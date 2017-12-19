using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.ExtraControls.DialogWnds
{
	public class ButtonClickedEventArgs : EventArgs
	{
		public enButtonType ButtonType { get; private set; }

		public ButtonClickedEventArgs(enButtonType buttonType)
		{
			ButtonType = buttonType;
		}
	}
}
