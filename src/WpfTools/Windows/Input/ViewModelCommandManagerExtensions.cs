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
    }
}