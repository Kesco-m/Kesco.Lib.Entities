using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Contacts
{
    
    /// <summary>
    /// Класс сущности Тип контакта
    /// </summary>
    [Serializable]
    public class ContactType : Entity
    {
        /// <summary>
        /// Язык пользователя
        /// </summary>
        private string LangName { get; set; }
        /// <summary>
        /// Наименование типа контакта
        /// </summary>
        public string ContactTypeName { get; set; }
        /// <summary>
        /// Наименование типа контакта на латинице
        /// </summary>
        public string ContactTypeNameLat { get; set; }
        /// <summary>
        /// Категория типа контакта
        /// </summary>
        public int? Categoty { get; set; }
        /// <summary>
        /// Иконка типа контакта
        /// </summary>
        public string Icon { get; set; }

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
        /// Конструктор сущности Тип контакта
        /// </summary>
        public ContactType()
            : base(null)
        {
            FillData(new DataTable());
        }

        /// <summary>
        /// Конструктор сущности Тип контакта
        /// </summary>
        public ContactType(string id,  string lang): base(id)
        {
            Id = id;
            LangName = lang;
            Load();
        }

        /// <summary>
        /// Конструктор сущности Тип контакта
        /// </summary>
        public ContactType(string id)
            : base(id)
        {
            Id = id;
            LangName = "ru";
            Load();
        }
       
        /// <summary>
        /// Метод загрузки данных сущности "Тип контакта"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТипКонтакта, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Инициализация сущности Територия на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТипаКонтакта"].ToString();
                ContactTypeName = dt.Rows[0]["ТипКонтакта"].ToString();
                ContactTypeNameLat = dt.Rows[0]["ТипКонтактаЛат"].ToString();
                Name = LangName == "ru" ? ContactTypeName : ContactTypeNameLat;
                Categoty = (int)dt.Rows[0]["Категория"];
                Icon = dt.Rows[0]["icon"].ToString();
            }
            else
            {
                Unavailable = true;
                Id = null;
                Name = null;
                Categoty = null;
                ContactTypeName = null;
                ContactTypeNameLat = null;
                Icon = null;
            }
        }
    }
}
