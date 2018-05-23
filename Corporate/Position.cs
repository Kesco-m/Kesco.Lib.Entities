using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
   
    /// <summary>
    /// Класс сущности Должность
    /// </summary>
    [Serializable]
    public class Position : Entity
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public Position(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Position() { }

        /// <summary>
        /// Метод загрузки данных сущности "Должность"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Должность, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Инициализация сущности Подразделение на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодДолжности"].ToString();
                Name = dt.Rows[0]["Должность"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}
