namespace WpfToolset.Windows.Input
{
    /// <summary>
    /// Содержит данные для собития начала выполнения команды
    /// </summary>
    public class CommandExecutingEventArgs : CommandExecuteEventArgs
    {
        /// <inheritdoc />
        public CommandExecutingEventArgs(string name, object value) : base(name, value)
        {
        }

        /// <summary>
        /// Признак отмены выполнения команды
        /// </summary>
        public bool Cancel { get; set; }

    }
}