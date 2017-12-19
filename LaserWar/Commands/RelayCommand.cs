using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LaserWar.Commands
{
	public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

		/// <summary>
		/// Поле, необходимое для реализации RaiseCanExecuteChanged
		/// </summary>
		private event EventHandler CanExecuteChangedInternal;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
			{
				CommandManager.RequerySuggested += value;
				CanExecuteChangedInternal += value;
			}
            remove
			{
				CommandManager.RequerySuggested -= value;
				CanExecuteChangedInternal -= value;
			}
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }


		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChangedInternal != null)
				CanExecuteChangedInternal(this, new EventArgs());
		}

        #endregion // ICommand Members
    }
}
