using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using LaserWar.ViewModels;

namespace LaserWar.Views.ValidationRules
{
	/// <summary>
	/// Проверка ввода имени игрока
	/// </summary>
	public class PlayerNameValidationRule : PlayerValidationRuleBase
	{
		public override ValidationResult Validate(object value, CultureInfo ci)
		{
			if (string.IsNullOrWhiteSpace(value.ToString()))
			{
				if (Wrapper != null && Wrapper.Player != null)
					Wrapper.Player.AddError(PropertyName);
				return new ValidationResult(false, Properties.Resources.resNameCouldNotBeEmpty);
			}
			else
			{
				if (Wrapper != null && Wrapper.Player != null)
					Wrapper.Player.AddError(PropertyName);
				return new ValidationResult(true, null);
			}
		}
	}
}
