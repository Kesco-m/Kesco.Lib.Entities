using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Attributes;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.Attributes
{
    /// <summary>
    ///     Класс сущности формат атрибута
    /// </summary>
    [Serializable]
    public class AttributeFormat : Entity
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public AttributeFormat()
        {
            FillData(null);
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public AttributeFormat(string id)
        {
            FillData(id);
        }

        /// <summary>
        ///     Конструктор поиска по территории, типу лица и типу формата
        /// </summary>
        /// <param name="personType">Тип лица</param>
        /// <param name="territoryID">Код территории</param>
        /// <param name="formatTypeID">Код типа формата атрибута</param>
        public AttributeFormat(int personType, int territoryID, int formatTypeID)
        {
            AttributeFormatBase = new AttributeFormatBase {PersonType = personType};
            TerritoryID = territoryID;
            AttributeFormatBase.FormatTypeID = formatTypeID;
            FillDataWIthOutID();
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Формат атрибута"
        /// </summary>
        public override void Load()
        {
            FillData(Id);
        }

        /// <summary>
        ///     Метод загрузки масок сущности "Формат атрибута"
        /// </summary>
        public void LoadMasks()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@formatTypeID", AttributeFormatBase.FormatTypeID}, {"@territoryID", TerritoryID},
                {"@personType", AttributeFormatBase.PersonType}
            };
            using (var dbReader = new DBReader(SQLQueries.SELECT_ФорматаАтрибута_Территория_ТипЛица_ТипАтрибута,
                CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colFormat1 = dbReader.GetOrdinal("ФорматАтрибута1");
                    var colFormat2 = dbReader.GetOrdinal("ФорматАтрибута2");
                    var colFormat3 = dbReader.GetOrdinal("ФорматАтрибута3");

                    #endregion

                    Unavailable = false;
                    dbReader.Read();
                    Format1 = dbReader.GetString(colFormat1);
                    Format2 = dbReader.GetString(colFormat2);
                    Format3 = dbReader.GetString(colFormat3);
                }
            }
        }

        /// <summary>
        ///     Метод сохранения изменений в сущности "Формат атрибута"
        /// </summary>
        public void Save()
        {
            var sqlParams = new Dictionary<string, object>(100)
            {
                {"@КодФорматаАтрибута", Id.ToInt()},
                {"@КодТерритории", TerritoryID},
                {"@МаскаАтрибута1", Format1},
                {"@МаскаАтрибута2", Format2},
                {"@МаскаАтрибута3", Format3},
                {"@НаименованиеАтрибута1Рус", AttributeFormatBase.ИмяАтрибутаРус1},
                {"@НаименованиеАтрибута2Рус", AttributeFormatBase.ИмяАтрибутаРус2},
                {"@НаименованиеАтрибута3Рус", AttributeFormatBase.ИмяАтрибутаРус3},
                {"@НаименованиеАтрибута1Лат", AttributeFormatBase.ИмяАтрибутаЛат1},
                {"@НаименованиеАтрибута2Лат", AttributeFormatBase.ИмяАтрибутаЛат2},
                {"@НаименованиеАтрибута3Лат", AttributeFormatBase.ИмяАтрибутаЛат3},
                {"@НаименованиеАтрибута1Рег", AttributeFormatNameReg1},
                {"@НаименованиеАтрибута2Рег", AttributeFormatNameReg2},
                {"@НаименованиеАтрибута3Рег", AttributeFormatNameReg3},
                {"@ПроверяемыйАтрибут", ChekedAttribute},
                {"@УникаленВПределахТерритории", FormatAttributeUniqueness}
            };

            var outputParams = new Dictionary<string, object> {{"@КодФорматаАтрибута", -1}};
            DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_ФорматАтрибута_Ins, CommandType.StoredProcedure, CN, sqlParams,
                outputParams);
            Id = outputParams["@КодФорматаАтрибута"].ToString();
        }

        /// <summary>
        ///     Метод создания сущности "Формат атрибута"
        /// </summary>
        public void Create()
        {
            var sqlParams = new Dictionary<string, object>(100)
            {
                {"@КодДокумента", null},
                {"@КодТипаАтрибута", AttributeFormatBase.FormatTypeID},
                {"@КодТерритории", TerritoryID},
                {"@ТипЛица", AttributeFormatBase.PersonType},
                {"@МаскаАтрибута1", Format1},
                {"@МаскаАтрибута2", Format2},
                {"@МаскаАтрибута3", Format3},
                {"@НаименованиеАтрибута1Рус", AttributeFormatBase.ИмяАтрибутаРус1},
                {"@НаименованиеАтрибута2Рус", AttributeFormatBase.ИмяАтрибутаРус2},
                {"@НаименованиеАтрибута3Рус", AttributeFormatBase.ИмяАтрибутаРус3},
                {"@НаименованиеАтрибута1Лат", AttributeFormatBase.ИмяАтрибутаЛат1},
                {"@НаименованиеАтрибута2Лат", AttributeFormatBase.ИмяАтрибутаЛат2},
                {"@НаименованиеАтрибута3Лат", AttributeFormatBase.ИмяАтрибутаЛат3},
                {"@НаименованиеАтрибута1Рег", AttributeFormatNameReg1},
                {"@НаименованиеАтрибута2Рег", AttributeFormatNameReg2},
                {"@НаименованиеАтрибута3Рег", AttributeFormatNameReg3},
                {"@ПроверяемыйАтрибут", ChekedAttribute},
                {"@УникаленВПределахТерритории", FormatAttributeUniqueness}
            };

            DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_ФорматАтрибута_Ins, CommandType.StoredProcedure, CN,
                sqlParams);
        }

        /// <summary>
        ///     Инициализация сущности Формат атрибута на основе таблицы данных
        /// </summary>
        /// <param name="id">id сущности</param>
        public void FillData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ClearModel();
                return;
            }

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ФорматАтрибута, id.ToInt(), CommandType.Text, CN))
            {
                FillDataFromDataRow(dbReader);
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Формат атрибута" по типу лица, типу формата и территории
        /// </summary>
        public void FillDataWIthOutID()
        {
            if (AttributeFormatBase.FormatTypeID == null || TerritoryID == null || AttributeFormatBase.PersonType == 0)
            {
                ClearModel();
                return;
            }

            var sqlParams = new Dictionary<string, object>
            {
                {"@formatTypeID", AttributeFormatBase.FormatTypeID}, {"@territoryID", TerritoryID},
                {"@personType", AttributeFormatBase.PersonType}
            };
            using (var dbReader = new DBReader(SQLQueries.SELECT_ФорматаАтрибута_Территория_ТипЛица_ТипАтрибута,
                CommandType.Text, CN, sqlParams))
            {
                FillDataFromDataRow(dbReader);
            }
        }

        /// <summary>
        ///     Инициализация сущности Формат атрибута на строки основе DBReader
        /// </summary>
        /// <param name="dbReader">Строка данных</param>
        private void FillDataFromDataRow(DBReader dbReader)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца

                var colИмяАтрибутаРус1 = dbReader.GetOrdinal("ИмяАтрибутаРус1");
                var colИмяАтрибутаРус2 = dbReader.GetOrdinal("ИмяАтрибутаРус2");
                var colИмяАтрибутаРус3 = dbReader.GetOrdinal("ИмяАтрибутаРус3");
                var colИмяАтрибутаЛат1 = dbReader.GetOrdinal("ИмяАтрибутаЛат1");
                var colИмяАтрибутаЛат2 = dbReader.GetOrdinal("ИмяАтрибутаЛат2");
                var colИмяАтрибутаЛат3 = dbReader.GetOrdinal("ИмяАтрибутаЛат3");
                var colИмяАтрибутаНаЯзыкеСтраны1 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны1");
                var colИмяАтрибутаНаЯзыкеСтраны2 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны2");
                var colИмяАтрибутаНаЯзыкеСтраны3 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны3");
                var colФорматАтрибута1 = dbReader.GetOrdinal("ФорматАтрибута1");
                var colФорматАтрибута2 = dbReader.GetOrdinal("ФорматАтрибута2");
                var colФорматАтрибута3 = dbReader.GetOrdinal("ФорматАтрибута3");
                var colКодФорматаАтрибута = dbReader.GetOrdinal("КодФорматаАтрибута");
                var colКодТерритории = dbReader.GetOrdinal("КодТерритории");
                var colТипЛица = dbReader.GetOrdinal("ТипЛица");
                var colКодТипаАтрибута = dbReader.GetOrdinal("КодТипаАтрибута");
                var colУникаленВПределахТерритории = dbReader.GetOrdinal("УникаленВПределахТерритории");
                var colПроверяемыйАтрибут = dbReader.GetOrdinal("ПроверяемыйАтрибут");
                var colИзменено = dbReader.GetOrdinal("Изменено");
                var colИзменил = dbReader.GetOrdinal("Изменил");

                #endregion

                Unavailable = false;
                dbReader.Read();

                AttributeFormatBase = new AttributeFormatBase
                {
                    ИмяАтрибутаРус1 = dbReader.GetString(colИмяАтрибутаРус1),
                    ИмяАтрибутаРус2 = dbReader.GetString(colИмяАтрибутаРус2),
                    ИмяАтрибутаРус3 = dbReader.GetString(colИмяАтрибутаРус3),
                    ИмяАтрибутаЛат1 = dbReader.GetString(colИмяАтрибутаЛат1),
                    ИмяАтрибутаЛат2 = dbReader.GetString(colИмяАтрибутаЛат2),
                    ИмяАтрибутаЛат3 = dbReader.GetString(colИмяАтрибутаЛат3),
                    PersonType = dbReader.GetByte(colТипЛица),
                    FormatTypeID = dbReader.GetInt32(colКодТипаАтрибута)
                };
                Name = dbReader.GetString(colИмяАтрибутаРус1);
                AttributeFormatNameReg1 = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны1);
                AttributeFormatNameReg2 = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны2);
                AttributeFormatNameReg3 = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны3);
                Format1 = dbReader.GetString(colФорматАтрибута1);
                Format2 = dbReader.GetString(colФорматАтрибута2);
                Format3 = dbReader.GetString(colФорматАтрибута3);
                Id = dbReader.GetInt32(colКодФорматаАтрибута).ToString();
                if (!dbReader.IsDBNull(colКодТерритории)) TerritoryID = dbReader.GetInt32(colКодТерритории);
                FormatAttributeUniqueness = dbReader.GetByte(colУникаленВПределахТерритории) == 1;
                ChekedAttribute = dbReader.GetByte(colПроверяемыйАтрибут) == 1;
                Changed = dbReader.GetDateTime(colИзменено).ToString();
                ChangedBy = dbReader.GetInt32(colИзменил);
            }
            else
            {
                Unavailable = true;
                ClearModel();
            }
        }

        /// <summary>
        ///     Отчистка модели данных
        /// </summary>
        private void ClearModel()
        {
            AttributeFormatBase = new AttributeFormatBase
            {
                ИмяАтрибутаРус1 = "",
                ИмяАтрибутаРус2 = "",
                ИмяАтрибутаРус3 = "",
                ИмяАтрибутаЛат1 = "",
                ИмяАтрибутаЛат2 = "",
                ИмяАтрибутаЛат3 = ""
            };
            AttributeFormatNameReg1 = "";
            AttributeFormatNameReg2 = "";
            AttributeFormatNameReg3 = "";
            Format1 = "";
            Format2 = "";
            Format3 = "";
            TerritoryID = null;
            AttributeFormatBase.FormatTypeID = null;
            FormatAttributeUniqueness = false;
            ChekedAttribute = false;
            Id = null;
            Unavailable = true;
            Changed = null;
            ChangedBy = null;
        }

        #region Свойства

        /// <summary>
        ///     Поля совпадающие с форматом атрибутов по умолчанию
        /// </summary>
        public AttributeFormatBase AttributeFormatBase { get; set; }

        /// <summary>
        ///     Имя формата атрибута1 на языке регистрации
        /// </summary>
        public string AttributeFormatNameReg1 { get; set; }

        /// <summary>
        ///     Имя формата атрибута2 на языке регистрации
        /// </summary>
        public string AttributeFormatNameReg2 { get; set; }

        /// <summary>
        ///     Имя формата атрибута3 на языке регистрации
        /// </summary>
        public string AttributeFormatNameReg3 { get; set; }

        /// <summary>
        ///     Формат атрибута1
        /// </summary>
        public string Format1 { get; set; }

        /// <summary>
        ///     Формат атрибута1
        /// </summary>
        public string Format2 { get; set; }

        /// <summary>
        ///     Формат атрибута1
        /// </summary>
        public string Format3 { get; set; }

        /// <summary>
        ///     Код территории формата атрибута
        /// </summary>
        public int? TerritoryID { get; set; }

        /// <summary>
        ///     Уникальность формата атрибута в пределах страны
        /// </summary>
        public bool FormatAttributeUniqueness { get; set; }

        /// <summary>
        ///     Проверяется ли атрибу этого формата
        /// </summary>
        public bool ChekedAttribute { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public new string Changed { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_person
            : _connectionString;

        #endregion
    }
}