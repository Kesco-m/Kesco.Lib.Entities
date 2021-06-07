using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности роль сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeRole : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        private Person _person;

        /// <summary>
        ///     КодСотрудника
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     КодЛица
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        ///     Название лица
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        ///     Возвращает объект типа Person в зависимости от значения PersonId
        /// </summary>
        public Person PersonObject
        {
            get
            {
                if (PersonId == 0)
                    _person = null;
                else if (_person == null || _person.Id != PersonId.ToString())
                    _person = new Person(PersonId.ToString());

                return _person;
            }
        }

        /// <summary>
        ///     КодРоли
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        ///     Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///     Описание роли
        /// </summary>
        public string RoleDescription { get; set; }

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
            var colКодРоли = dbReader.GetOrdinal("КодРоли");
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colКодЛица = dbReader.GetOrdinal("КодЛица");
            var colНазваниеЛица = dbReader.GetOrdinal("НазваниеЛица");
            var colРоль = dbReader.GetOrdinal("Роль");
            var colОписание = dbReader.GetOrdinal("Описание");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодРоли)) RoleId = dbReader.GetInt32(colКодРоли);
            if (!dbReader.IsDBNull(colКодСотрудника)) EmployeeId = dbReader.GetInt32(colКодСотрудника);
            if (!dbReader.IsDBNull(colКодЛица)) PersonId = dbReader.GetInt32(colКодЛица);
            if (!dbReader.IsDBNull(colНазваниеЛица)) PersonName = dbReader.GetString(colНазваниеЛица);
            if (!dbReader.IsDBNull(colРоль)) {RoleName = dbReader.GetString(colРоль);}
            if (!dbReader.IsDBNull(colОписание)) RoleDescription = dbReader.GetString(colОписание);
        }
    }
}