using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Models;

namespace LaserWar.Stuff
{
	public class SoundPlayingEventArgs : EventArgs
	{
		public SoundModel Sound { get; private set; }

		public SoundPlayingEventArgs(SoundModel sound)
		{
			Sound = sound;
		}
	}
}
