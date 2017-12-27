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
	/// Проверка ввода рейтинга игрока
	/// </summary>
	public class PlayerAccuracyValidationRule : PlayerValidationRuleBase
	{
		public override ValidationResult Validate(object value, CultureInfo ci)
		{
			if (value == null || !(value is float) || (float)value < 0.0 || (float)value > 1.0)
			{
				if (Wrapper != null && Wrapper.Player != null)
					Wrapper.Player.AddError(PropertyName);
				return new ValidationResult(false, Properties.Resources.resAccuracyMustBeFrom0To1);
			}
			else
			{
				if (Wrapper != null && Wrapper.Player != null)
					Wrapper.Player.RemoveError(PropertyName);
				return new ValidationResult(true, null);
			}
		}
	}
}
