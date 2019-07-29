using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности фотография сотрудника
    /// </summary>
    [Serializable]
    public class EmployeePhoto : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Дата фотографировани
        /// </summary>
        public DateTime? DatePhoto { get; set; }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодФотографииСотрудника = dbReader.GetOrdinal("КодФотографииСотрудника");
            var colДатаФотографирования = dbReader.GetOrdinal("ДатаФотографирования");

            if (!dbReader.IsDBNull(colКодФотографииСотрудника))
                Id = dbReader.GetInt32(colКодФотографииСотрудника).ToString();
            if (!dbReader.IsDBNull(colДатаФотографирования)) DatePhoto = dbReader.GetDateTime(colДатаФотографирования);

            Unavailable = false;
        }
    }
}