using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaserWar.Global;
using LaserWar.ViewModels;

namespace LaserWar.Veiws
{
	/// <summary>
	/// Представление - загрузка
	/// </summary>
	public partial class DataDownloaderView : CNotifyPropertyChangedUserCtrl
	{
		readonly DataDownloaderViewModel m_ViewModel = null;

		public DataDownloaderView():
			base()
		{
			InitializeComponent();
		}


		public DataDownloaderView(DataDownloaderViewModel ViewModel):
			base()
		{
			DataContext = m_ViewModel = ViewModel;

			InitializeComponent();
		}
	}
}
