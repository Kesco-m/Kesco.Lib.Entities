using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности "Каталоги"
    /// </summary>
    [Serializable]
    public class PersonCatalog : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">КодКаталога</param>
        public PersonCatalog(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public PersonCatalog(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public PersonCatalog()
        {
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override sealed string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_person;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Каталог"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Каталог, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодКаталога"].ToString();
                Name = dt.Rows[0]["Каталог"].ToString();
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
            var colКодКаталога = dbReader.GetOrdinal("КодКаталога");
            var colКаталог = dbReader.GetOrdinal("Каталог");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодКаталога)) Id = dbReader.GetInt32(colКодКаталога).ToString();
            if (!dbReader.IsDBNull(colКаталог)) Name = dbReader.GetString(colКаталог);
        }
    }
}