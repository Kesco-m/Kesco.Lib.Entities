using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности должность сотрудника
    /// </summary>
    [Serializable]
    public class EmployeePosition : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     КодЛица
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        ///     Организация
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        ///     Подразделение
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        ///     Должность
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        ///     Совместитель
        /// </summary>
        public bool IsCombining { get; set; }

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
            var colКодЛица = dbReader.GetOrdinal("КодЛица");
            var colОрганизация = dbReader.GetOrdinal("Организация");
            var colПодразделение = dbReader.GetOrdinal("Подразделение");
            var colДолжность = dbReader.GetOrdinal("Должность");
            var colСовместитель = dbReader.GetOrdinal("Совместитель");


            Unavailable = false;

            if (!dbReader.IsDBNull(colКодЛица)) PersonID = dbReader.GetInt32(colКодЛица).ToString();
            if (!dbReader.IsDBNull(colОрганизация)) Organization = dbReader.GetString(colОрганизация);
            if (!dbReader.IsDBNull(colПодразделение)) Department = dbReader.GetString(colПодразделение);
            if (!dbReader.IsDBNull(colДолжность)) Position = dbReader.GetString(colДолжность);
            if (!dbReader.IsDBNull(colСовместитель)) IsCombining = dbReader.GetValue(colСовместитель).ToString() == "1";
        }
    }
}