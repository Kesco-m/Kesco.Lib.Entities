using System;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Attributes
{
    /// <summary>
    ///     Класс сущности Уникальности атрибута лица
    /// </summary>
    [Serializable]
    public class AttributeUnicness : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     ID атрибута
        /// </summary>
        public string AttributeId { get; set; }

        /// <summary>
        ///     ID типа формата атрибута
        /// </summary>
        public string AttributeFormatTypeId { get; set; }

        /// <summary>
        ///     ID страны атрибута
        /// </summary>
        public string TerritoryID { get; set; }

        /// <summary>
        ///     Дата начала
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        ///     Дата окончания
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_person
            : _connectionString;
    }
}