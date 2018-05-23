using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.BusinessProject
{
    /// <summary>
    /// Класс сущности Бизнес проект
    /// </summary>
    [Serializable]
    public class BusinessProject : Entity
    {
        /// <summary>
        /// Имя бизнес проекта
        /// </summary>
        public string BusinessProjectName { get; set; }

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
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_person) : _connectionString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// Конструктор сущности Бизнес проект
        /// </summary>
        public BusinessProject()
            : base(null)
        {
            
        }

        /// <summary>
        /// Конструктор сущности Бизнес проект
        /// </summary>
        public BusinessProject(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Метод загрузки данных сущности "Контакт"
        /// </summary>
        public override sealed void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_БизнесПроект, CN, CommandType.Text, sqlParams));
        }

        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодБизнесПроекта"].ToString();
                Name = dt.Rows[0]["БизнесПроект"].ToString();
                BusinessProjectName = dt.Rows[0]["БизнесПроект"].ToString();
            }
            else
            {
                Unavailable = true;
                Id = null;
                Name = null;
                BusinessProjectName = null;
            }
        }
    }
}
