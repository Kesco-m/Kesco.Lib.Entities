using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности "Роли"
    /// </summary>
    [Serializable]
    public class VacationType : Entity
    {
        /// <summary>
        /// Описание роли
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public override sealed string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;
        /// <summary>
        /// Метод загрузки данных сущности "Виды отпуска"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.String } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ВидОтпуска, CN, CommandType.Text, sqlParams));
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
                Id = dt.Rows[0]["КодВидаОтпуска"].ToString();
                Name = dt.Rows[0]["ВидОтпуска"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            int colКодВидаОтпуска = dbReader.GetOrdinal("КодВидаОтпуска");
            int colВидОтпуска = dbReader.GetOrdinal("ВидОтпуска");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодВидаОтпуска)) Id = dbReader.GetInt32(colКодВидаОтпуска).ToString();
            if (!dbReader.IsDBNull(colВидОтпуска)) Name = dbReader.GetString(colВидОтпуска);
        }




        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">КодРоли</param>
        public VacationType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор для загрузки по dbReader
        /// </summary>
        public VacationType()
        {

        }


    }
}
