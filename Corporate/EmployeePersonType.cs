using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons;
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

        private PersonCatalog _catalog;
        private PersonTheme _theme;

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
        ///     Возвращает объект типа PersonCatalog в зависимости от значения CatalogId
        /// </summary>
        public PersonCatalog CatalogObject
        {
            get
            {
                if (!CatalogId.HasValue)
                    _catalog = null;
                else if (_catalog == null || _catalog.Id != CatalogId.ToString())
                    _catalog = new PersonCatalog(CatalogId.ToString());

                return _catalog;
            }
        }

        /// <summary>
        ///     КодТемы
        /// </summary>
        public int? ThemeId { get; set; }

        /// <summary>
        ///     Нзвание темы
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        ///     Возвращает объект типа PersonTheme в зависимости от значения ThemeId
        /// </summary>
        public PersonTheme ThemeObject
        {
            get
            {
                if (!ThemeId.HasValue)
                    _theme = null;
                else if (_theme == null || _theme.Id != ThemeId.ToString())
                    _theme = new PersonTheme(ThemeId.ToString());

                return _theme;
            }
        }

        /// <summary>
        ///     Может давать права
        /// </summary>
        public byte CanGrant { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString =>
            string.IsNullOrEmpty(_connectionString)
                ? _connectionString = Config.DS_person
                : _connectionString;

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

            if (!dbReader.IsDBNull(colКодКаталога)) CatalogId = dbReader.GetInt32(colКодКаталога);
            if (!dbReader.IsDBNull(colКаталог)) CatalogName = dbReader.GetString(colКаталог);
            if (!dbReader.IsDBNull(colКодТемыЛица)) ThemeId = dbReader.GetInt32(colКодТемыЛица);
            if (!dbReader.IsDBNull(colТемаЛица)) ThemeName = dbReader.GetString(colТемаЛица);
            CanGrant = dbReader.GetByte(colМожетДаватьПрава);
        }
    }
}