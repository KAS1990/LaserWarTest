using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.ViewModels;

namespace LaserWar.Stuff
{
	/// <summary>
	/// Поле игрока, которое сейчас редактируется
	/// </summary>
	public enum enEditedPlayerField
	{
		/// <summary>
		/// Ничего не редактируется
		/// </summary>
		None,

		Name,

		Rating,

		Accuracy,

		Shots,

		/// <summary>
		/// Находимся в окне редактирования
		/// </summary>
		All
	}


	/// <summary>
	/// Состояние редактирования игрока
	/// </summary>
	public enum enEditedPlayerState
	{
		/// <summary>
		/// Начали редактирование
		/// </summary>
		Started,

		/// <summary>
		/// Отменили внесённые изменения
		/// </summary>
		Canceled,

		/// <summary>
		/// Подтвердили внесённые изменения 
		/// </summary>
		Commited
	}


	public class PlayerEditEventArgs : EventArgs
	{
		public enEditedPlayerField Field { get; private set; }
		public enEditedPlayerState State { get; private set; }

		public PlayerEditEventArgs(enEditedPlayerField field, enEditedPlayerState state)
		{
			Field = field;
			State = state;
		}
	}
}
