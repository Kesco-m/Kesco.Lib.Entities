using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Attributes
{
    /// <summary>
    /// Класс сущности Тип формата атрибута
    /// </summary>
    [Serializable]
    public class AttributeFormatType: Entity
    {
        /// <summary>
        /// Текущий язык интерфеса
        /// </summary>
        private string LangName { get; set; }

        /// <summary>
        /// Имя типа атрибута на латинице
        /// </summary>
        public string AttributeFormatTypeNameLat { get; set; }

        /// <summary>
        /// Доступность типа формата атрибута для типа лица 1- юр. лица, 2-физ. лица, 0- для всех типов
        /// </summary>
        public int PersonTypeAvailability { get; set; }

        /// <summary>
        /// Порядковый номер вывода эллемента
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

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
        /// Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public AttributeFormatType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        /// /// <param name="lang">Текущий язык интерфейса</param>
        public AttributeFormatType(string id, string lang): base(id)
        {
            LangName = lang;
            Load();
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public AttributeFormatType()
        {
        }

        /// <summary>
        /// Метод загрузки данных сущности "Тип формата атрибута"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТипАтрибута, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Инициализация сущности Тип формата атрибута на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТипаАтрибута"].ToString();
                Name = LangName == "ru" ? dt.Rows[0]["ТипАтрибута"].ToString() : dt.Rows[0]["ТипАтрибутаЛат"].ToString();
                PersonTypeAvailability = (byte) dt.Rows[0]["ДоступностьДляТипаЛица"];
                Order = (int) dt.Rows[0]["ПорядокВыводаАтрибута"];
                AttributeFormatTypeNameLat = dt.Rows[0]["ТипАтрибутаЛат"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }


    }
}
