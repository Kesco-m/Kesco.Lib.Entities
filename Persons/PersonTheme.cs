using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Тема лица
    /// </summary>
    [Serializable]
    public class PersonTheme : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID темы</param>
        public PersonTheme(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonTheme()
        {
        }

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
        ///     Наименование темы лица.
        /// </summary>
        public string NameTheme { get; set; }

        /// <summary>
        ///     Метод загрузки данных сущности "Тема лица"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТемаЛица, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Тема лица
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТемыЛица"].ToString();
                Name = dt.Rows[0]["ТемаЛица"].ToString();
                NameTheme = dt.Rows[0]["ТемаЛица"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получение родительских и подчиненных тем
        /// </summary>
        /// <param name="themeId">КодТемыЛица</param>
        /// <param name="catalogId">КодКаталога, может быть NULL</param>
        /// <returns>Список объектов</returns>
        public static List<Item> GetParentAndChildThemes(int themeId, int catalogId)
        {
            var list = new List<Item>();
            var sqlParams = new Dictionary<string, object> { { "@Id", themeId }, { "@КодКаталога", catalogId } };
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ID_ТемыЛиц_ПотомкиИПодчиненные, CommandType.Text,
                    ConnString, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        list.Add(new Item {Id = dbReader.GetInt32(0).ToString(), Value = dbReader.GetString(1)});
                    }
                }
            }
            return list;
        }
    }
}