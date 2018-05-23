using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности должность сотрудника
    /// </summary>
    [Serializable]
    public class EmployeePosition : Entity
    {
        /// <summary>
        /// КодЛица
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// Организация
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// Подразделение
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// Должность
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Совместитель
        /// </summary>
        public bool IsCombining { get; set; }
        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString; }
        }
        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            
                int colКодЛица = dbReader.GetOrdinal("КодЛица");
                int colОрганизация = dbReader.GetOrdinal("Организация");
                int colПодразделение = dbReader.GetOrdinal("Подразделение");
                int colДолжность = dbReader.GetOrdinal("Должность");
                int colСовместитель = dbReader.GetOrdinal("Совместитель");

                
                Unavailable = false;

                if (!dbReader.IsDBNull(colКодЛица)) PersonID = dbReader.GetInt32(colКодЛица).ToString();
                if (!dbReader.IsDBNull(colОрганизация)) Organization = dbReader.GetString(colОрганизация);
                if (!dbReader.IsDBNull(colПодразделение)) Department = dbReader.GetString(colПодразделение);
                if (!dbReader.IsDBNull(colДолжность)) Position = dbReader.GetString(colДолжность);
                if (!dbReader.IsDBNull(colСовместитель)) IsCombining = dbReader.GetValue(colСовместитель).ToString() == "1";
    

        }
    }
}
