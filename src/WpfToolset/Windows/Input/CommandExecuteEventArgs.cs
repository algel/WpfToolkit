using System;

namespace WpfToolset.Windows.Input
{
    /// <summary>
    /// Содержит данные для событий выполнения команды
    /// </summary>
    public class CommandExecuteEventArgs : EventArgs
    {
        /// <inheritdoc />
        public CommandExecuteEventArgs(string name, object parameter)
        {
            Name = name;
            Parameter = parameter;
        }

        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Значение аргумента команды
        /// </summary>
        public object Parameter { get; }
    }
}