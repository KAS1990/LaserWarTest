using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.Stuff
{
	public class DataDownloadComletedEventArgs : EventArgs
	{
		public Exception Error { get; private set; }
		public string SourceFileName { get; private set; }

		public DataDownloadComletedEventArgs(Exception error, string sourceFileName)
		{
			Error = error;
			SourceFileName = sourceFileName;
		}
	}
}
