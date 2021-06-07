using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "ВидОтпуска"
    /// </summary>
    [Serializable]
    public class VacationType : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ВидОтпуска</param>
        public VacationType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public VacationType()
        {
        }

        /// <summary>
        ///     Описание роли
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Краткое название
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Виды отпуска"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", Id}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ВидОтпуска, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодВидаОтпуска = dbReader.GetOrdinal("КодВидаОтпуска");
                    var colВидОтпуска = dbReader.GetOrdinal("ВидОтпуска");
                    var colКраткоеНазвание = dbReader.GetOrdinal("КраткоеНазвание");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодВидаОтпуска).ToString();
                        Name = dbReader.GetString(colВидОтпуска);
                        ShortName = dbReader.GetString(colКраткоеНазвание);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }

            //FillData(DBManager.GetData(SQLQueries.SELECT_ID_ВидОтпуска, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодВидаОтпуска"].ToString();
                Name = dt.Rows[0]["ВидОтпуска"].ToString();
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
            var colКодВидаОтпуска = dbReader.GetOrdinal("КодВидаОтпуска");
            var colВидОтпуска = dbReader.GetOrdinal("ВидОтпуска");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодВидаОтпуска)) Id = dbReader.GetInt32(colКодВидаОтпуска).ToString();
            if (!dbReader.IsDBNull(colВидОтпуска)) Name = dbReader.GetString(colВидОтпуска);
        }
    }
}