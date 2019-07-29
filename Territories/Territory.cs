using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Territories
{
    /// <summary>
    ///     Класс сущности Територия
    /// </summary>
    [Serializable]
    public class Territory : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID территории</param>
        public Territory(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID территории</param>
        /// <param name="langName">Текущий язык интерфейса</param>
        public Territory(string id, string langName) : base(id)
        {
            LangName = langName;
            Load();
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Territory()
        {
        }

        /// <summary>
        ///     Язык пользователя
        /// </summary>
        private string LangName { get; }

        /// <summary>
        ///     Код территории
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        ///     Аббревиатура территории
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        ///     Телефонный код территории
        /// </summary>
        public string TelephoneCode { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Територия"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Территории_Страна, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Територия на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТерритории"].ToString();
                Name = LangName == "ru" ? dt.Rows[0]["Территория"].ToString() : (string) dt.Rows[0]["Caption"];
                Caption = (string) dt.Rows[0]["Caption"];
                Abbreviation = (string) dt.Rows[0]["Аббревиатура"];
                TelephoneCode = (string) dt.Rows[0]["ТелКодСтраны"];
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}