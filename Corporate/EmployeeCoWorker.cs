using System;
using System.Threading;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности сотрудники замещающие сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeCoWorker : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     КодРасположения
        /// </summary>
        public string CoWorker
        {
            get
            {
                var coWorkerName = string.Empty;

                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ru")
                    coWorkerName = CoWorkerRU;
                else
                    coWorkerName = CoWorkerEN;

                return coWorkerName ?? "#" + Id;
            }
        }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        public string CoWorkerRU { get; set; }

        /// <summary>
        ///     Employee
        /// </summary>
        public string CoWorkerEN { get; set; }

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
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colСотрудник = dbReader.GetOrdinal("Сотрудник");
            var colEmployee = dbReader.GetOrdinal("Employee");
            if (!dbReader.IsDBNull(colКодСотрудника)) Id = dbReader.GetInt32(colКодСотрудника).ToString();
            if (!dbReader.IsDBNull(colСотрудник)) CoWorkerRU = dbReader.GetString(colСотрудник);
            if (!dbReader.IsDBNull(colEmployee)) CoWorkerEN = dbReader.GetString(colEmployee);
            Unavailable = false;
        }
    }
}