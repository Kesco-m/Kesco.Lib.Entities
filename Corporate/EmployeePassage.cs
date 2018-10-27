using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности проход сотрудника
    /// </summary>
    [Serializable]
    public class EmployeePassage : Entity
    {
        /// <summary>
        /// Указывает местоположения последнего прохода сотрудника
        /// </summary>
        /// <value>
        /// Местоположение последнего прохода сотрудника
        /// </value>
        public string Point { get; set; }

        /// <summary>
        /// Указывает время(UTC) прохода сотрудника через пост
        /// </summary>
        /// <value>
        /// Время(UTC) прохода сотрудника через пост
        /// </value>
        public DateTime TimeAt { get; set; }
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        public EmployeePassage(string id):base(id)
        {
            Load();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", Id.ToInt() } };
            using (var dbReader = new DBReader(SQLQueries.SELECT_ПоследнийПроходСотрудника, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colPoint = dbReader.GetOrdinal("Считыватель");
                    int colTimeAt = dbReader.GetOrdinal("ПоследнийПроход");

                    #endregion
                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        if (!dbReader.IsDBNull(colPoint)) Point = dbReader.GetString(colPoint);
                        if (!dbReader.IsDBNull(colTimeAt)) TimeAt = dbReader.GetDateTime(colTimeAt);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }
    }
}
