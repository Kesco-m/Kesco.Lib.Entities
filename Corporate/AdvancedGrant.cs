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
    /// Класс сущности "Дополнительные права для указаний"
    /// </summary>
    public class AdvancedGrant : Entity
    {
         /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">AdvancedGrant</param>
        public AdvancedGrant(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public AdvancedGrant()
        {
        }

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
        /// Описание права на английском
        /// </summary>
        public string NameEn { get; set; }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;
        /// <summary>
        /// Метод загрузки данных сущности "Дополнительное право"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.String } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_AdvancedGrant, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКод = dbReader.GetOrdinal("КодПраваДляУказанийIT");
            var colОписание = dbReader.GetOrdinal("Описание");
            var colNameEn = dbReader.GetOrdinal("ОписаниеЛат");
            Unavailable = false;

            if (!dbReader.IsDBNull(colКод)) Id = dbReader.GetInt32(colКод).ToString();
            if (!dbReader.IsDBNull(colОписание)) Name = dbReader.GetString(colОписание);
            if (!dbReader.IsDBNull(colNameEn)) NameEn = dbReader.GetString(colNameEn);
            
        }
    }
}
