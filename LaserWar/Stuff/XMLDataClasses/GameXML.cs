using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LaserWar.Entities;
using System.ComponentModel;

namespace LaserWar.Stuff.XMLDataClasses
{
	/// <summary>
	/// Описание игры в XML
	/// Для xml не используем класс game, т.к. не хочется добавлять в него setter для списка  teams
	/// </summary>
	[XmlRoot(ElementName = "game", Namespace = "")]
	public class GameXML
	{
		[XmlAttribute]
		[DefaultValue("")]
		public string name { get; set; }
		
		[XmlAttribute]
		[DefaultValue(0)]
		public int date { get; set; }

		[XmlElement("team")]
		public List<TeamXML> teams { get; set; }


		public static explicit operator game(GameXML rhs)
		{
			return new game()
			{
				name = rhs.name,
				date = rhs.date
			};
		}
	}
}
