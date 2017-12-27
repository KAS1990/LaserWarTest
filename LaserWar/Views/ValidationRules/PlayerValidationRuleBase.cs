using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using LaserWar.ViewModels;
using System.Windows;

namespace LaserWar.Views.ValidationRules
{
	public abstract class PlayerValidationRuleBase : ValidationRule
	{
		public PlayerValidationRuleWrapper Wrapper { get; set; }
		public string PropertyName { get; set; }
	}
}
