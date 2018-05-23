using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Attributes
{
    /// <summary>
    /// Класс сущности формат атрибута по умолчанию
    /// </summary>
    [Serializable]
    public class AttributeFormatBase : Entity
    {
        #region Поля сущности
        /// <summary>
        /// Имя формата атрибута1 на русском
        /// </summary>
        /// <value>
        /// ИмяАтрибутаРус1 (nvarchar(300), not null)
        /// </value>
        public string ИмяАтрибутаРус1 { get; set; }
        /// <summary>
        /// Имя формата атрибута2 на русском
        /// </summary>
        /// <value>
        /// ИмяАтрибутаРус2 (nvarchar(300), not null)
        /// </value>
        public string ИмяАтрибутаРус2 { get; set; }
        /// <summary>
        /// Имя формата атрибута3 на русском
        /// </summary>
        /// <value>
        /// ИмяАтрибутаРус3 (nvarchar(300), not null)
        /// </value>
        public string ИмяАтрибутаРус3 { get; set; }
        /// <summary>
        /// Имя формата атрибута1 на латинице
        /// </summary>
        /// <value>
        /// ИмяАтрибутаЛат1 (varchar(300), not null)
        /// </value>
        public string ИмяАтрибутаЛат1 { get; set; }
        /// <summary>
        /// Имя формата атрибута2 на латинице
        /// </summary>
        /// <value>
        /// ИмяАтрибутаЛат2 (varchar(300), not null)
        /// </value>
        public string ИмяАтрибутаЛат2 { get; set; }
        /// <summary>
        /// Имя формата атрибута3 на латинице
        /// </summary>
        /// <value>
        /// ИмяАтрибутаЛат3 (varchar(300), not null)
        /// </value>
        public string ИмяАтрибутаЛат3 { get; set; }
        /// <summary>
        /// Количество атрибутов в формате 
        /// </summary>
        /// <value>
        /// КоличествоПолей (int, not null)
        /// </value>
        public int КоличествоПолей { get; set; }
        /// <summary>
        /// Тип лица, для которого действует формат атрибута
        /// </summary>
        public int PersonType { get; set; }
        /// <summary>
        /// Код типа формата атрибута
        /// </summary>
        public int? FormatTypeID { get; set; }
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
        #endregion

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public AttributeFormatBase()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="formatTypeID">Тип формата атрибута</param>
        /// <param name="personType">Тип лица</param>
        public AttributeFormatBase(int formatTypeID, int personType)
        {
            FormatTypeID = formatTypeID;
            PersonType = personType;
            FillData();
        }

        /// <summary>
        /// Метод загрузки данных сущности "Формат атрибута по умолчанию"
        /// </summary>
        public override void Load()
        {
            FillData();
        }

        /// <summary>
        /// Заполнение сущности типа Класс сущности формат атрибута по умолчанию 
        /// </summary>
        public void FillData()
        {
            if (FormatTypeID == null || PersonType == 0) { ClearModel();  return;}

            var sqlParams = new Dictionary<string, object> { { "@formatTypeID",  FormatTypeID } , { "@personType",  PersonType } };
            using (var dbReader = new DBReader(SQLQueries.SELECT_ФорматаАтрибутаПоУмолчанию, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colИмяАтрибутаРус1 = dbReader.GetOrdinal("ИмяАтрибутаРус1");
                    int colИмяАтрибутаРус2 = dbReader.GetOrdinal("ИмяАтрибутаРус2");
                    int colИмяАтрибутаРус3 = dbReader.GetOrdinal("ИмяАтрибутаРус3");
                    int colИмяАтрибутаЛат1 = dbReader.GetOrdinal("ИмяАтрибутаЛат1");
                    int colИмяАтрибутаЛат2 = dbReader.GetOrdinal("ИмяАтрибутаЛат2");
                    int colИмяАтрибутаЛат3 = dbReader.GetOrdinal("ИмяАтрибутаЛат3");
                    int colКоличествоПолей = dbReader.GetOrdinal("КоличествоПолей");
                    #endregion
                    Unavailable = false;
                    dbReader.Read();
                    ИмяАтрибутаРус1 = dbReader.GetString(colИмяАтрибутаРус1);
                    ИмяАтрибутаРус2 = dbReader.GetString(colИмяАтрибутаРус2);
                    ИмяАтрибутаРус3 = dbReader.GetString(colИмяАтрибутаРус3);
                    ИмяАтрибутаЛат1 = dbReader.GetString(colИмяАтрибутаЛат1);
                    ИмяАтрибутаЛат2 = dbReader.GetString(colИмяАтрибутаЛат2);
                    ИмяАтрибутаЛат3 = dbReader.GetString(colИмяАтрибутаЛат3);
                    КоличествоПолей = dbReader.GetInt32(colКоличествоПолей);
                }
                else
                {
                    ClearModel();
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// Метод очищает модель данных сущности "Формат атрибута по умолчанию"
        /// </summary>
        public void ClearModel()
        {
            ИмяАтрибутаРус1 = "";
            ИмяАтрибутаРус2 = "";
            ИмяАтрибутаРус3 = "";
            ИмяАтрибутаЛат1 = "";
            ИмяАтрибутаЛат2 = "";
            ИмяАтрибутаЛат3 = "";
            FormatTypeID = null;
            Unavailable = true;
            КоличествоПолей = 0;
        }

        /// <summary>
        /// Метод получения списка сущностей типа Класс сущности формат атрибута по умолчанию 
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <returns>Возвращает перечисление сущности типа Класс сущности формат атрибута по умолчанию</returns>
        public List<AttributeFormatBase> GetAttributeFormatBaseList(string query)
        {
            var list = new List<AttributeFormatBase>(); using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colИмяАтрибутаРус1 = dbReader.GetOrdinal("ИмяАтрибутаРус1");
                    int colИмяАтрибутаРус2 = dbReader.GetOrdinal("ИмяАтрибутаРус2");
                    int colИмяАтрибутаРус3 = dbReader.GetOrdinal("ИмяАтрибутаРус3");
                    int colИмяАтрибутаЛат1 = dbReader.GetOrdinal("ИмяАтрибутаЛат1");
                    int colИмяАтрибутаЛат2 = dbReader.GetOrdinal("ИмяАтрибутаЛат2");
                    int colИмяАтрибутаЛат3 = dbReader.GetOrdinal("ИмяАтрибутаЛат3");
                    int colКоличествоПолей = dbReader.GetOrdinal("КоличествоПолей");
                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new AttributeFormatBase
                                      {
                                          Unavailable = false,
                                          ИмяАтрибутаРус1 = dbReader.GetString(colИмяАтрибутаРус1),
                                          ИмяАтрибутаРус2 = dbReader.GetString(colИмяАтрибутаРус2),
                                          ИмяАтрибутаРус3 = dbReader.GetString(colИмяАтрибутаРус3),
                                          ИмяАтрибутаЛат1 = dbReader.GetString(colИмяАтрибутаЛат1),
                                          ИмяАтрибутаЛат2 = dbReader.GetString(colИмяАтрибутаЛат2),
                                          ИмяАтрибутаЛат3 = dbReader.GetString(colИмяАтрибутаЛат3),
                                          КоличествоПолей = dbReader.GetInt32(colКоличествоПолей)
                                      };
                        list.Add(row);
                    }
                }
            }
            return list;
        }

    }

}
