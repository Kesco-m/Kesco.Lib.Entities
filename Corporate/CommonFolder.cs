using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "Обшая папка"
    /// </summary>
    public class CommonFolder : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">DomainName</param>
        public CommonFolder(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public CommonFolder()
        {
        }

        /// <summary>
        ///     Группа
        /// </summary>
        public string Group { get; set; }

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
        ///     Метод загрузки данных сущности "Обща папка"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_CommonFolder, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодОбщейПапки = dbReader.GetOrdinal("КодОбщейПапки");
            var colОбщаяПапка = dbReader.GetOrdinal("ОбщаяПапка");
            var colГруппа = dbReader.GetOrdinal("Группа");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодОбщейПапки)) Id = dbReader.GetInt32(colКодОбщейПапки).ToString();
            if (!dbReader.IsDBNull(colОбщаяПапка)) Name = dbReader.GetString(colОбщаяПапка);
            if (!dbReader.IsDBNull(colГруппа)) Group = dbReader.GetString(colГруппа);
        }
    }
}