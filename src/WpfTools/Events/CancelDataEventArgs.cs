namespace Algel.WpfTools.Events
{
    public class CancelDataEventArgs : CancelDataEventArgs<object>
    {
        /// <inheritdoc />
        public CancelDataEventArgs(object data) : base(data)
        {
        }
    }

    /// <summary>
    /// Предоставляет данные для отменяемого события
    /// </summary>
    /// <typeparam name="T">Тип данных над которыми производится действие</typeparam>
    public class CancelDataEventArgs<T> : DataEventArgs<T>
    {
        /// <summary>
        /// Получает или задает значение, показывающее, следует ли отменить событие
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса CancelDataEventArgs, устанавливая свойство Data  в заданное значение
        /// </summary>
        /// <param name="data">Данные над которыми производится действие</param>
        public CancelDataEventArgs(T data)
            : base(data)
        {
        }
    }
}