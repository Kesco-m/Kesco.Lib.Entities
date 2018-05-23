using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности ПраваТипыЛиц
    /// </summary>
    [Serializable]
    public class EmployeePersonType : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     КодСотрудника
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     КодКаталога
        /// </summary>
        public int? CatalogId { get; set; }

        /// <summary>
        ///     Нзвание каталога
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        ///     КодКаталога
        /// </summary>
        public int? ThemeId { get; set; }

        /// <summary>
        ///     Нзвание темы
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        ///     Может давать права
        /// </summary>
        public byte CanGrant { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override sealed string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get
            {
                return string.IsNullOrEmpty(_connectionString)
                    ? (_connectionString = Config.DS_person)
                    : _connectionString;
            }
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодПраваТипыЛиц = dbReader.GetOrdinal("КодПраваТипыЛиц");
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colКодКаталога = dbReader.GetOrdinal("КодКаталога");
            var colКаталог = dbReader.GetOrdinal("Каталог");
            var colКодТемыЛица = dbReader.GetOrdinal("КодТемыЛица");
            var colТемаЛица = dbReader.GetOrdinal("ТемаЛица");
            var colМожетДаватьПрава = dbReader.GetOrdinal("МожетДаватьПрава");

            Unavailable = false;

            Id = dbReader.GetInt32(colКодПраваТипыЛиц).ToString();
            EmployeeId = dbReader.GetInt32(colКодСотрудника);

            if (!dbReader.IsDBNull(colКодКаталога))
            {
                CatalogId = dbReader.GetInt32(colКодКаталога);
            }
            if (!dbReader.IsDBNull(colКаталог))
            {
                CatalogName = dbReader.GetString(colКаталог);
            }
            if (!dbReader.IsDBNull(colКодТемыЛица))
            {
                ThemeId = dbReader.GetInt32(colКодТемыЛица);
            }
            if (!dbReader.IsDBNull(colТемаЛица))
            {
                ThemeName = dbReader.GetString(colТемаЛица);
            }
            CanGrant = dbReader.GetByte(colМожетДаватьПрава);
        }
    }
}