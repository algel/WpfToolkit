using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Algel.WpfTools.Windows.Input
{
    public static class ViewModelCommandManagerExtensions
    {
        public static IViewModelCommand Get(this ViewModelCommandManager commandManager, [CallerMemberName]string name = null)
        {
            return commandManager[name];
        }

        #region CreateCommand

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, [CallerMemberName] string name = null)
        {
            return commandManager.CreateCommand(name);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Action executeMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Action<object> executeMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Action executeMethod, Func<bool> canExecuteMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, canExecuteMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Action<object> executeMethod, Func<object, bool> canExecuteMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, canExecuteMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Func<Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, exceptionHandlerMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Func<object, Task> executeMethod, Action<Task, Exception> exceptionHandlerMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, exceptionHandlerMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Func<Task> executeMethod, Func<bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, canExecuteMethod, exceptionHandlerMethod);
        }

        public static IViewModelCommand CreateCommand(this ViewModelCommandManager commandManager, Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod, Action<Task, Exception> exceptionHandlerMethod, [CallerMemberName]string name = null)
        {
            return commandManager.CreateCommand(name, executeMethod, canExecuteMethod, exceptionHandlerMethod);
        }

        #endregion

        #region TryExecute

        public static void TryExecute(this IViewModelCommand command)
        {
            if (command != null && command.CanExecute())
                command.Execute();
        }

        public static void TryExecute(this IViewModelCommand command, object arg)
        {
            if (command != null && command.CanExecute(arg))
                command.Execute(arg);
        }

        public static async Task TryExecuteAsync(this IViewModelCommand command)
        {
            if (command != null && command.CanExecute())
                await command.ExecuteAsync();
        }

        public static async Task TryExecuteAsync(this IViewModelCommand command, object arg)
        {
            if (command != null && command.CanExecute(arg))
                await command.ExecuteAsync(arg);
        }

        public static void TryExecute(this ViewModelCommandManager commandManager, string commandName)
        {
            commandManager.Get(commandName).TryExecute();
        }

        public static void TryExecute(this ViewModelCommandManager commandManager, string commandName, object arg)
        {
            commandManager.Get(commandName).TryExecute(arg);
        }

        public static async Task TryExecuteAsync(this ViewModelCommandManager commandManager, string commandName)
        {
            await commandManager.Get(commandName).TryExecuteAsync();
        }

        public static async Task TryExecuteAsync(this ViewModelCommandManager commandManager, string commandName, object arg)
        {
            await commandManager.Get(commandName).TryExecuteAsync(arg);
        }

        #endregion
    }
}