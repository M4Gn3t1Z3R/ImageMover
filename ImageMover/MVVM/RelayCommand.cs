/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System;
using System.Windows.Input;

namespace ImageMover.MVVM
{
    class RelayCommand<T> : ICommand
    {
        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;
        bool _isExecuting = false;
        public bool IsExecuting { get { return _isExecuting; } }

        public RelayCommand(Action<T> execute) : this(execute, null)
        {

        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _isExecuting = true;
            try
            {
                _execute((T)parameter);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }
}
