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
    public class Role : Entity
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
        /// Метод загрузки данных сущности "Роли"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.String } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Роль, CN, CommandType.Text, sqlParams));
        }

        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id  = dt.Rows[0]["КодРоли"].ToString();
                Name = dt.Rows[0]["Роль"].ToString();
                Description = dt.Rows[0]["Описание"].ToString();
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
            int colКодРоли = dbReader.GetOrdinal("КодРоли");
            int colРоль = dbReader.GetOrdinal("Роль");
            int colОписание = dbReader.GetOrdinal("Описание");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодРоли)) Id = dbReader.GetInt32(colКодРоли).ToString();
            if (!dbReader.IsDBNull(colРоль)) Name = dbReader.GetString(colРоль);
            if (!dbReader.IsDBNull(colОписание)) Description = dbReader.GetString(colОписание);
        }


       

         /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">КодРоли</param>
        public Role(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор для загрузки по dbReader
        /// </summary>
        public Role()
        {

        }


    }
}
