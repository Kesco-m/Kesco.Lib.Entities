using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности Доменное имя
    /// </summary>
    public class DomainName : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">DomainName</param>
        public DomainName(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public DomainName()
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
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Коды лиц через ','
        /// </summary>
        public string PersonIds { get; set; }

        /// <summary>
        ///     Тип
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     Метод загрузки данных сущности "Доменные имена"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_DomainName, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colDomainName = dbReader.GetOrdinal("DomainName");
            var colКодыЛиц = dbReader.GetOrdinal("КодыЛиц");
            var colТип = dbReader.GetOrdinal("Тип");

            Unavailable = false;

            if (!dbReader.IsDBNull(colDomainName)) Id = dbReader.GetString(colDomainName);
            if (!dbReader.IsDBNull(colКодыЛиц)) Name = dbReader.GetString(colКодыЛиц);
            if (!dbReader.IsDBNull(colТип)) Type = dbReader.GetByte(colТип);
        }
    }
}