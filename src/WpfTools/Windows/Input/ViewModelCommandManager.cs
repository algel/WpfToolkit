﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Algel.WpfTools.Events;

namespace Algel.WpfTools.Windows.Input
{
    /// <summary>
    /// Command container and command factory
    /// </summary>
    public class ViewModelCommandManager
    {
        private readonly Dictionary<string, CommandEntry> _commandsCollection = new Dictionary<string, CommandEntry>();

        /// <summary>
        /// Получить экземпляр команды по ее имени
        /// </summary>
        /// <param name="key">Имя команды, использовавшееся при ее регистрации</param>
        /// <returns></returns>
        public IViewModelCommand this[string key]
        {
            get
            {
                if (_commandsCollection.TryGetValue(key, out var entry))
                    return entry.Command;
                return null;
            }
            private set
            {
                var entry = new CommandEntry(key, value);
                _commandsCollection[key] = entry;

                entry.Executing += OnEntryCommand_Executing;
                entry.Executed += OnEntryCommand_Executed;
            }
        }

        #region Methods

        #region CreateCommand

        public IViewModelCommand CreateCommand(string name)
        {
            return this[name] = new ViewModelCommand(() => { });
        }

        public IViewModelCommand CreateCommand(string name, Action executeMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod);
        }

        public IViewModelCommand CreateCommand(string name, Action<object> executeMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod);
        }

        public IViewModelCommand CreateCommand(string name, Action executeMethod, Func<bool> canExecuteMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, canExecuteMethod);
        }

        public IViewModelCommand CreateCommand(string name, Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, canExecuteMethod);
        }

        public IViewModelCommand CreateCommand(string name, Func<Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, exceptionHandlerMethod);
        }

        public IViewModelCommand CreateCommand(string name, Func<object, Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, exceptionHandlerMethod);
        }

        public IViewModelCommand CreateCommand(string name, Func<Task> executeMethod, Func<bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, canExecuteMethod, exceptionHandlerMethod);
        }

        public IViewModelCommand CreateCommand(string name, Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod)
        {
            return this[name] = new ViewModelCommand(executeMethod, canExecuteMethod, exceptionHandlerMethod);
        }

        #endregion

        private void OnEntryCommand_Executed(object sender, DataEventArgs e)
        {
            var handler = CommandExecuted;
            if (handler != null)
            {
                var entry = (CommandEntry)sender;
                handler.Invoke(this, new CommandExecuteEventArgs(entry.Name, e.Value));
            }
        }

        private void OnEntryCommand_Executing(object sender, CancelDataEventArgs e)
        {
            var handler = CommandExecuting;
            if (handler != null)
            {
                var entry = (CommandEntry)sender;
                var cancelArgs = new CommandExecutingEventArgs(entry.Name, e.Value);
                handler.Invoke(this, cancelArgs);
                e.Cancel = cancelArgs.Cancel;
            }
        }

        #endregion

        /// <summary>
        /// Вызывается перед началом выполнения команды. 
        /// Если в обработчике события, в параметрах свойству Cancel присвоить true, то команда выполнена не будет />
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Событие завершения выполнения команды
        /// </summary>
        public event EventHandler<CommandExecuteEventArgs> CommandExecuted;

        private sealed class CommandEntry
        {
            public string Name { get; }
            public IViewModelCommand Command { get; }

            /// <inheritdoc />
            public CommandEntry(string name, IViewModelCommand command)
            {
                Name = name;
                Command = command;

                Command.CommandExecuting += OnCommandExecuting;
                Command.CommandExecuted += OnCommandExecuted;
            }

            private void OnCommandExecuted(object sender, DataEventArgs e)
            {
                Executed?.Invoke(this, e);
            }

            private void OnCommandExecuting(object sender, CancelDataEventArgs e)
            {
                Executing?.Invoke(this, e);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public event EventHandler<CancelDataEventArgs> Executing;
            public event EventHandler<DataEventArgs> Executed;
        }
    }
}
