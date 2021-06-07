using Kesco.Lib.BaseExtention.Enums.Controls;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     Структура сущности Валидация-Предупреждение
    /// </summary>
    public struct Notification
    {
        /// <summary>
        ///     Идентификатор контейнера, куда будет помещено сообщение
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        ///     Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Подробное описание сообщения или что делать
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Статус сообщения, по-умолчанию Error
        /// </summary>
        public NtfStatus Status { get; set; }

        /// <summary>
        ///     Рисовать тире перед сообщением
        /// </summary>
        public bool DashSpace { get; set; }

        /// <summary>
        ///     Отображать сообщение в размере Ntf
        /// </summary>
        public bool SizeIsNtf { get; set; }

        /// <summary>
        ///     Дополнительный класс CSS
        /// </summary>
        public string CSSClass { get; set; }
    }
}