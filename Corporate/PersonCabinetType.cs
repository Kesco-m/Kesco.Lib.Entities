using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "ТипЛичногоКабинета"
    /// </summary>
    [Serializable]
    public class PersonCabinetType : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">КодЛичногоКабинета</param>
        public PersonCabinetType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public PersonCabinetType()
        {
        }

        /// <summary>
        ///     Selenium script
        /// </summary>
        public string SeleniumScript { get; set; }

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
        ///     Метод загрузки данных сущности "Тип личного кабинета"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", Id}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ТипЛичногоКабинета, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТипаЛичногоКабинета = dbReader.GetOrdinal("КодТипаЛичногоКабинета");
                    var colТипЛичногоКабинета = dbReader.GetOrdinal("ТипЛичногоКабинета");
                    var colSeleniumScript = dbReader.GetOrdinal("SeleniumScript");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодТипаЛичногоКабинета).ToString();
                        Name = dbReader.GetString(colТипЛичногоКабинета);
                        SeleniumScript = dbReader.GetString(colSeleniumScript);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТипаЛичногоКабинета"].ToString();
                Name = dt.Rows[0]["ТипЛичногоКабинета"].ToString();
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
            var colКодТипаЛичногоКабинета = dbReader.GetOrdinal("КодТипаЛичногоКабинета");
            var colТипЛичногоКабинета = dbReader.GetOrdinal("ТипЛичногоКабинета");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодТипаЛичногоКабинета)) Id = dbReader.GetInt32(colКодТипаЛичногоКабинета).ToString();
            if (!dbReader.IsDBNull(colТипЛичногоКабинета)) Name = dbReader.GetString(colТипЛичногоКабинета);
        }
    }
}