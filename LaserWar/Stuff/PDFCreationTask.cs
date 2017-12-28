using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.Stuff
{
	public class PDFCreationTask
	{
		/// <summary>
		/// Путь к PDF-файлу, который нужно создать
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Нужно ли отображать сообщение по результатам сохранения файла
		/// </summary>
		public bool ShowMessage { get; set; }
	}
}
