using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Models;
using System.ComponentModel;

namespace LaserWar.Stuff
{
	public class SoundModelEventArgs : EventArgs
	{
		public HashSet<string> ChangedProperties { get; private set; } 
		
		public SoundModelEventArgs(string[] changedProperties)
		{
			ChangedProperties = new HashSet<string>(changedProperties);
		}

		public SoundModelEventArgs(string propertyName)
		{
			ChangedProperties = new HashSet<string>();
			ChangedProperties.Add(propertyName);
		}
	}
}
