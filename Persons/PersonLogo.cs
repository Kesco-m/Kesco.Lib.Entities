using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности логотип лица
    /// </summary>
    [Serializable]
    public class PersonLogo : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_person
            : _connectionString;

       

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодЛоготипаЛица = dbReader.GetOrdinal("КодЛоготипаЛица");
            

            if (!dbReader.IsDBNull(colКодЛоготипаЛица))
                Id = dbReader.GetInt32(colКодЛоготипаЛица).ToString();           

            Unavailable = false;
        }
    }
}