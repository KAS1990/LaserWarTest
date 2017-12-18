using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LaserWar.ViewModels;
using System.ComponentModel;

namespace LaserWar.Commands
{
	public class DownloadCommand : ICommand
	{
		private DataDownloaderViewModel m_ViewModel;

		public DownloadCommand(DataDownloaderViewModel viewModel)
		{
			m_ViewModel = viewModel;
			m_ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}
		

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == DataDownloaderViewModel.CanDownloadPropertyName)
				CanExecuteChanged(this, new EventArgs());
		}


		public bool CanExecute(object parameter)
		{
			return m_ViewModel.CanDownload;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			m_ViewModel.Download();
		}
	}
}
