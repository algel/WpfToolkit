using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Algel.WpfTools.Events;

namespace Algel.WpfTools.Windows.Input
{
    public class ViewModelCommand : IViewModelCommand
    {
        #region Fields

        private readonly Action<object> _executeMethod;
        private readonly Func<object, Task> _executeMethodAsync;
        private readonly Func<object, bool> _canExecuteMethod;
        private readonly Action<Task, Exception> _exceptionHandlerMethod;

        #endregion

        #region .ctor

        /// <inheritdoc />
        internal ViewModelCommand(Action executeMethod)
        {
            _executeMethod = p => executeMethod();
        }

        /// <inheritdoc />
        internal ViewModelCommand(Action<object> executeMethod)
        {
            _executeMethod = executeMethod;
        }

        internal ViewModelCommand(Func<Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            _executeMethodAsync = p => executeMethod();
            _exceptionHandlerMethod = exceptionHandlerMethod;
            IsAsync = true;
        }

        internal ViewModelCommand(Func<object, Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            _executeMethodAsync = executeMethod;
            _exceptionHandlerMethod = exceptionHandlerMethod;
            IsAsync = true;
        }

        /// <inheritdoc />
        internal ViewModelCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _executeMethod = p => executeMethod();
            _canExecuteMethod = e => canExecuteMethod();
        }

        /// <inheritdoc />
        internal ViewModelCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <inheritdoc />
        internal ViewModelCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            _executeMethodAsync = p => executeMethod();
            _exceptionHandlerMethod = exceptionHandlerMethod;
            _canExecuteMethod = e => canExecuteMethod();
            IsAsync = true;
        }

        /// <inheritdoc />
        internal ViewModelCommand(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            _executeMethodAsync = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _exceptionHandlerMethod = exceptionHandlerMethod;
            IsAsync = true;
        }


        #endregion

        /// <summary>ICommand.CanExecuteChanged implementation</summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }



        /// <inheritdoc />
        public event EventHandler<DataEventArgs> CommandExecuted;

        /// <inheritdoc />
        public event EventHandler<CancelDataEventArgs> CommandExecuting;

        /// <inheritdoc />
        public void Execute()
        {
            Execute(null);
        }

        public async void Execute(object arg)
        {
            if (IsExecuting) return;
            if (!IsAsync)
            {
                IsExecuting = true;
                try
                {
                    if (RaiseCommandExecuting(arg))
                    {
                        _executeMethod.Invoke(arg);
                    }
                }
                finally
                {
                    IsExecuting = false;
                    RaiseCommandExecuted(arg);
                    RaiseCanExecuteChanged();
                }
            }
            else
            {
                Task task = null;
                try
                {
                    task = ExecuteAsync(arg);
                    await task.ConfigureAwait(true);
                }
                catch (Exception e)
                {
                    _exceptionHandlerMethod(task, e);
                }
            }
        }

        public async Task ExecuteAsync()
        {
            await ExecuteAsync(null);
        }

        public async Task ExecuteAsync(object arg)
        {
            if (IsExecuting) return;

            if (!IsAsync)
            {
                Execute(arg);
                return;
            }

            IsExecuting = true;
            Task task = null;
            try
            {
                if (RaiseCommandExecuting(arg))
                {
                    task = _executeMethodAsync(arg);
                    await task;
                }
            }
            catch (Exception e)
            {
                _exceptionHandlerMethod.Invoke(task, e);
            }
            finally
            {
                IsExecuting = false;
                RaiseCommandExecuted(arg);
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(object arg)
        {
            if (IsExecuting)
                return false;

            if (_canExecuteMethod != null)
                return _canExecuteMethod.Invoke(arg);
            return true;
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        private bool RaiseCommandExecuting(object arg)
        {
            var cancelArgs = new CancelDataEventArgs(arg);
            CommandExecuting?.Invoke(this, cancelArgs);
            return !cancelArgs.Cancel;
        }

        private void RaiseCommandExecuted(object arg)
        {
            CommandExecuted?.Invoke(this, new DataEventArgs(arg));
        }

        /// <summary>Raises the CanExecuteChaged event</summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region Properties

        public bool IsExecuting { get; set; }

        public bool IsAsync { get; }

        #endregion
    }
}
