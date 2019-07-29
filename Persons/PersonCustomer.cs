using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Лица заказчики
    /// </summary>
    [Serializable]
    public class PersonCustomer : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PersonCustomer(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonCustomer()
        {
        }

        /// <summary>
        ///     Наименование Лат.
        /// </summary>
        public string NameLat { get; set; }

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
        ///     Метод загрузки данных сущности "Компания"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ЛицоЗаказчик, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Компания на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодЛица"].ToString();
                Name = dt.Rows[0]["Кличка"].ToString();
                NameLat = dt.Rows[0]["КраткоеНазваниеЛат"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}