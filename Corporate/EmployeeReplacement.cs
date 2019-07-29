using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности замещение сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeReplacement : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     КодЗамещенияСотрудников
        /// </summary>
        public int ReplacementId { get; set; }

        /// <summary>
        ///     До
        /// </summary>
        public DateTime ForDate { get; set; }

        /// <summary>
        ///     КодСотрудникаЗамещаемого
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     Замещённый
        /// </summary>
        public string ReplacedEmployeeName { get; set; }

        /// <summary>
        ///     КодСотрудникаЗамещающего
        /// </summary>
        public int VicariousId { get; set; }

        /// <summary>
        ///     ИспОбязанности
        /// </summary>
        public string VicariousName { get; set; }

        /// <summary>
        ///     Примечания
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public string EmployeeWhoChange { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодЗамещенияСотрудников = dbReader.GetOrdinal("КодЗамещенияСотрудников");
            var colДо = dbReader.GetOrdinal("До");
            var colКодСотрудникаЗамещаемого = dbReader.GetOrdinal("КодСотрудникаЗамещаемого");
            var colЗамещённый = dbReader.GetOrdinal("Замещённый");
            var colКодСотрудникаЗамещающего = dbReader.GetOrdinal("КодСотрудникаЗамещающего");
            var colИспОбязанности = dbReader.GetOrdinal("ИспОбязанности");
            var colПримечания = dbReader.GetOrdinal("Примечания");
            var colИзменил = dbReader.GetOrdinal("Изменил");
            var colИзменено = dbReader.GetOrdinal("Изменено");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодЗамещенияСотрудников))
                ReplacementId = dbReader.GetInt32(colКодЗамещенияСотрудников);
            if (!dbReader.IsDBNull(colДо)) ForDate = dbReader.GetDateTime(colДо);
            if (!dbReader.IsDBNull(colКодСотрудникаЗамещаемого))
                EmployeeId = dbReader.GetInt32(colКодСотрудникаЗамещаемого);
            if (!dbReader.IsDBNull(colЗамещённый)) ReplacedEmployeeName = dbReader.GetString(colЗамещённый);
            if (!dbReader.IsDBNull(colКодСотрудникаЗамещающего))
                VicariousId = dbReader.GetInt32(colКодСотрудникаЗамещающего);
            if (!dbReader.IsDBNull(colИспОбязанности)) VicariousName = dbReader.GetString(colИспОбязанности);
            if (!dbReader.IsDBNull(colПримечания)) Comment = dbReader.GetString(colПримечания);
            if (!dbReader.IsDBNull(colИзменил)) EmployeeWhoChange = dbReader.GetString(colИзменил);
            if (!dbReader.IsDBNull(colИзменено)) ChangeDate = dbReader.GetDateTime(colИзменено);
        }
    }
}