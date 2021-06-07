using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Equipments
{

    /// <summary>
    ///     Класс сущности "SlBlAddress"
    /// </summary>
    [Serializable]
    public class SlBlAddress : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Адрес</param>
        public SlBlAddress(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public SlBlAddress()
        {
        }

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
        ///     Метод загрузки данных сущности "SlBlAddress"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_SlBlAddress, CN, CommandType.Text, sqlParams));
        }


        /// <summary>
        ///     Список исключений
        /// </summary>
        public string SList { get; set; }

        /// <summary>
        ///     Список ограничений
        /// </summary>
        public string BList { get; set; }


        /// <summary>
        ///     Инициализация сущности "Роли" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных Роли</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["Address"].ToString();
                Name = dt.Rows[0]["Address"].ToString();               
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colAddress = dbReader.GetOrdinal("Address");
           
            Unavailable = false;

            if (!dbReader.IsDBNull(colAddress))
            {
                Id = dbReader.GetString(colAddress);
                Name = dbReader.GetString(colAddress);
            }
        }

    }
}
