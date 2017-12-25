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
	/// Описание команды в XML.
	/// Для xml не используем класс team, т.к. не хочется добавлять в него setter для списка  players
	/// </summary>
	[XmlRoot(ElementName = "team", Namespace = "")]
	public class TeamXML
	{
		[XmlAttribute]
		[DefaultValue("")]
		public string name { get; set; }

		[XmlElement("player")]
		public List<player> players { get; set; }


		public static explicit operator team(TeamXML rhs)
		{
			return new team()
			{
				name = rhs.name,
			};
		}
	}
}
