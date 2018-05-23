using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности рабочее место сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeWorkPlace : Entity
    {
        /// <summary>
        /// РасположениеPath
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// РабочееМесто
        /// </summary>
        public int WorkPlacePar { get; set; }
        /// <summary>
        /// Замещающие сотрудники
        /// </summary>
        public List<EmployeeCoWorker> CoWorkers { get; set; }

       
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
            int colКодРасположения = dbReader.GetOrdinal("КодРасположения");
            int colPath = dbReader.GetOrdinal("РасположениеPath");
            int colWorkPlacePar = dbReader.GetOrdinal("РабочееМесто");
            if (!dbReader.IsDBNull(colКодРасположения)) Id = dbReader.GetInt32(colКодРасположения).ToString();
            if (!dbReader.IsDBNull(colPath)) Path = dbReader.GetString(colPath);
            if (!dbReader.IsDBNull(colWorkPlacePar)) WorkPlacePar = dbReader.GetByte(colWorkPlacePar);
            Unavailable = false;
        }
    }
}
