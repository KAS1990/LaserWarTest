using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LaserWar.ViewModels;

namespace LaserWar.Views.ValidationRules
{
	/// <summary>
	/// Обёртка для PlayerValidationRuleBase, позволяющая пользоваться Binding'ами
	/// </summary>
	public class PlayerValidationRuleWrapper : DependencyObject
	{
		public PlayerViewModel Player
		{
			get { return (PlayerViewModel)GetValue(PlayerProperty); }
			set { SetValue(PlayerProperty, value); }
		}

		public static readonly DependencyProperty PlayerProperty =
			DependencyProperty.Register("Player", typeof(PlayerViewModel), typeof(PlayerValidationRuleWrapper), new UIPropertyMetadata(null));
	}
}
