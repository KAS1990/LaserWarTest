using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Entities;
using Newtonsoft.Json;

namespace LaserWar.Stuff
{
	/// <summary>
	/// TO DO: доделать класс игры
	/// </summary>
	public class JSONGame
	{
		public string url { get; set; }
	}


	/// <summary>
	/// Загружаемый JSON объект
	/// </summary>
	public class TaskJSONObject
	{
		public string error { get; set; }
		public JSONGame[] games { get; set; }
		public sound[] sounds { get; set; }
	}
}
