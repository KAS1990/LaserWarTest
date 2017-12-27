using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace LaserWar.Views.ValidationRules
{
	/// <summary>
	/// Проверка ввода рейтинга игрока
	/// </summary>
	public class NotNegativeValidationRule : PlayerValidationRuleBase
	{
		public override ValidationResult Validate(object value, CultureInfo ci)
		{
			int i = 0;
			if (value == null || !int.TryParse(value.ToString(), out i) || i < 0)
			{
				if (Wrapper != null && Wrapper.Player != null)
					Wrapper.Player.AddError(PropertyName);
				return new ValidationResult(false, Properties.Resources.resRatingMustBeNotNegative);
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
