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
    /// Класс сущности атрибут
    /// </summary>
    [Serializable]
    public class Attribute : Entity
    {
        #region Свойства
        /// <summary>
        /// КодЛица
        /// </summary>
        public int? PersonID { get; set; }
        /// <summary>
        /// КодТерритории
        /// </summary>
        public int? TerritoryID { get; set; }
        /// <summary>
        /// ДатаНачалаДействия
        /// </summary>
        public DateTime? DateStart { get; set; }
        /// <summary>
        /// ДатаОкончанияДействия
        /// </summary>
        public DateTime? DateEnd { get; set; }
        /// <summary>
        /// Формат атрибута
        /// </summary>
        public AttributeFormat AttributeFormat { get; set; }
        /// <summary>
        /// Значение атрибута 1
        /// </summary>
        public string AttributeValue1 { get; set; }
        /// <summary>
        /// Значение атрибута 2
        /// </summary>
        public string AttributeValue2 { get; set; }
        /// <summary>
        /// Значение атрибута 3
        /// </summary>
        public string AttributeValue3 { get; set; }
        /// <summary>
        /// Дата рождения лица
        /// </summary>
        public DateTime? DatePersonBegin { get; set; }
        /// <summary>
        /// Дата окончания лица
        /// </summary>
        public DateTime? DatePersonEnd { get; set; }
        /// <summary>
        /// ПорядокВыводаАтрибута
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// Изменено
        /// </summary>
        public new DateTime? Changed { get; set; }
        /// <summary>
        /// Изменил
        /// </summary>
        public int? ChangedBy { get; set; }
        /// <summary>
        /// Проверено
        /// </summary>
        public DateTime Checked { get; set; }

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
        #endregion

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Attribute()
        {
            ClearModel();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID атрибута</param>
        public Attribute(string id)
        {
            FillData(id);
        }

        /// <summary>
        /// Метод загрузки данных сущности "Атрибут лица"
        /// </summary>
        public override void Load()
        {
            FillData(Id);
        }

        /// <summary>
        /// Метод загрузки данных сущности "Атрибут лица"
        /// </summary>
        public void LoadWithOutID()
        {
            FillData(Id);
        }

        /// <summary>
        /// Метод удаления сущности "Атрибут лица"
        /// </summary>
        public DataTable Delete(byte check)
        {
            var sqlParams = new Dictionary<string, object>
                                {
                                    {"@КодЛица", new object[] {PersonID, DBManager.ParameterTypes.Int32}},
                                    {"@КодАтрибутаЛица", new object[] {Id, DBManager.ParameterTypes.Int32}},
                                    {"@ПровереныИзменения", new object[] {check, DBManager.ParameterTypes.Int32}}
                                };
            var result = DBManager.GetData(SQLQueries.SP_Лица_Атрибут_Del, CN, CommandType.StoredProcedure, sqlParams);
            return result;

        }

        /// <summary>
        /// Метод загрузки данных сущности "Атрибут лица"
        /// </summary>
        public object Create(byte check)
        {
            var parametersOut = new Dictionary<string, object>();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодЛица",  PersonID);
            sqlParams.Add("@ЗначениеАтрибута1",  AttributeValue1);
            sqlParams.Add("@ЗначениеАтрибута2", AttributeValue2);
            sqlParams.Add("@ЗначениеАтрибута3", AttributeValue3);
            sqlParams.Add("@КодТерритории", TerritoryID);
            sqlParams.Add("@КодТипаАтрибута", AttributeFormat.AttributeFormatBase.FormatTypeID);

            if (DateStart == null)
                sqlParams.Add("@От",  "");
            else
                sqlParams.Add("@От", DateStart);

            sqlParams.Add("@ПровереныИзменения", check);

            var result = new DataTable();

            #region СОздание колонок таблицы возврата результата

            DataColumn column = new DataColumn
                                    {DataType = System.Type.GetType("System.String"), ColumnName = "ТипАтрибута"};

            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ТипАтрибутаЛат"};
            result.Columns.Add(column);

            column = new DataColumn
                         {DataType = System.Type.GetType("System.DateTime"), ColumnName = "СтараяДатаОкончанияДействия"};
            result.Columns.Add(column);

            column = new DataColumn
                         {DataType = System.Type.GetType("System.DateTime"), ColumnName = "НоваяДатаОкончанияДействия"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаЛат1"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаЛат2"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаЛат3"};
            result.Columns.Add(column);

            column = new DataColumn
                         {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаНаЯзыкеСтраны1"};
            result.Columns.Add(column);

            column = new DataColumn
                         {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаНаЯзыкеСтраны2"};
            result.Columns.Add(column);

            column = new DataColumn
                         {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаНаЯзыкеСтраны3"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаРус1"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаРус2"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ИмяАтрибутаРус3"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ЗначениеАтрибута1"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ЗначениеАтрибута2"};
            result.Columns.Add(column);

            column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "ЗначениеАтрибута3"};
            result.Columns.Add(column);
            #endregion

            using (var dbReader = new DBReader(SQLQueries.SP_Лица_Атрибут_Ins, CommandType.StoredProcedure, CN, sqlParams, parametersOut))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colТипАтрибута = dbReader.GetOrdinal("ТипАтрибута");
                    int colТипАтрибутаЛат = dbReader.GetOrdinal("ТипАтрибутаЛат");
                    int colСтараяДатаОкончанияДействия = dbReader.GetOrdinal("СтараяДатаОкончанияДействия"); 
                    int colНоваяДатаОкончанияДействия = dbReader.GetOrdinal("НоваяДатаОкончанияДействия");
                    int colИмяАтрибутаЛат1 = dbReader.GetOrdinal("ИмяАтрибутаЛат1");
                    int colИмяАтрибутаЛат2 = dbReader.GetOrdinal("ИмяАтрибутаЛат2");
                    int colИмяАтрибутаЛат3 = dbReader.GetOrdinal("ИмяАтрибутаЛат3");
                    int colИмяАтрибутаНаЯзыкеСтраны1 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны1");
                    int colИмяАтрибутаНаЯзыкеСтраны2 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны2");
                    int colИмяАтрибутаНаЯзыкеСтраны3 = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны3");
                    int colИмяАтрибутаРус1 = dbReader.GetOrdinal("ИмяАтрибутаРус1");
                    int colИмяАтрибутаРус2 = dbReader.GetOrdinal("ИмяАтрибутаРус2");
                    int colИмяАтрибутаРус3 = dbReader.GetOrdinal("ИмяАтрибутаРус3");
                    int colЗначениеАтрибута1 = dbReader.GetOrdinal("ЗначениеАтрибута1");
                    int colЗначениеАтрибута2 = dbReader.GetOrdinal("ЗначениеАтрибута2");
                    int colЗначениеАтрибута3 = dbReader.GetOrdinal("ЗначениеАтрибута3");

                    #endregion
                    

                    while (dbReader.Read())
                    {
                        DataRow row = result.NewRow();

                        if (!dbReader.IsDBNull(colТипАтрибута)) { row["ТипАтрибута"] = dbReader.GetString(colТипАтрибута); }
                        if (!dbReader.IsDBNull(colТипАтрибутаЛат)) { row["ТипАтрибутаЛат"] = dbReader.GetString(colТипАтрибутаЛат); }
                        if (!dbReader.IsDBNull(colСтараяДатаОкончанияДействия)) { row["СтараяДатаОкончанияДействия"] = dbReader.GetDateTime(colСтараяДатаОкончанияДействия); }
                        if (!dbReader.IsDBNull(colНоваяДатаОкончанияДействия)) { row["НоваяДатаОкончанияДействия"] = dbReader.GetDateTime(colНоваяДатаОкончанияДействия); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаЛат1)) { row["ИмяАтрибутаЛат1"] = dbReader.GetString(colИмяАтрибутаЛат1); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаЛат2)) { row["ИмяАтрибутаЛат2"] = dbReader.GetString(colИмяАтрибутаЛат2); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаЛат3)) { row["ИмяАтрибутаЛат3"] = dbReader.GetString(colИмяАтрибутаЛат3); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаНаЯзыкеСтраны1)) { row["ИмяАтрибутаНаЯзыкеСтраны1"] = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны1); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаНаЯзыкеСтраны2)) { row["ИмяАтрибутаНаЯзыкеСтраны2"] = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны2); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаНаЯзыкеСтраны3)) { row["ИмяАтрибутаНаЯзыкеСтраны3"] = dbReader.GetString(colИмяАтрибутаНаЯзыкеСтраны3); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаРус1)) { row["ИмяАтрибутаРус1"] = dbReader.GetString(colИмяАтрибутаРус1); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаРус2)) { row["ИмяАтрибутаРус2"] = dbReader.GetString(colИмяАтрибутаРус2); }
                        if (!dbReader.IsDBNull(colИмяАтрибутаРус3)) { row["ИмяАтрибутаРус3"] = dbReader.GetString(colИмяАтрибутаРус3); }
                        if (!dbReader.IsDBNull(colЗначениеАтрибута1)) { row["ЗначениеАтрибута1"] = dbReader.GetString(colЗначениеАтрибута1); }
                        if (!dbReader.IsDBNull(colЗначениеАтрибута2)) { row["ЗначениеАтрибута2"] = dbReader.GetString(colЗначениеАтрибута2); }
                        if (!dbReader.IsDBNull(colЗначениеАтрибута3)) { row["ЗначениеАтрибута3"] = dbReader.GetString(colЗначениеАтрибута3); }
                        result.Rows.Add(row);
                    }
                }
                dbReader.Close();
                if (parametersOut["@RETURN_VALUE"] != null && Convert.ToInt32(parametersOut["@RETURN_VALUE"]) != 0)
                    return Convert.ToInt32(parametersOut["@RETURN_VALUE"]);
            }
            return result;

        }

        /// <summary>
        /// Метод загрузки данных сущности "Атрибут лица"
        /// </summary>
        public DataTable Edit(byte check)
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодЛица", new object[] { PersonID, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@КодАтрибута", new object[] { Id, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@ЗначениеАтрибута1", new object[] { AttributeValue1, DBManager.ParameterTypes.String });
            sqlParams.Add("@ЗначениеАтрибута2", new object[] { AttributeValue2, DBManager.ParameterTypes.String });
            sqlParams.Add("@ЗначениеАтрибута3", new object[] { AttributeValue3, DBManager.ParameterTypes.String });
            sqlParams.Add("@КодТерритории", new object[] { TerritoryID, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@КодТипаАтрибута", new object[] { AttributeFormat.AttributeFormatBase.FormatTypeID, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@От",
                          DateStart == null
                              ? new object[] { "", DBManager.ParameterTypes.String }
                              : new object[] { DateStart, DBManager.ParameterTypes.String });
            sqlParams.Add("@ПровереныИзменения", new object[] { check, DBManager.ParameterTypes.Int32 });
            var result = DBManager.GetData(SQLQueries.SP_Лица_Атрибут_Upd, CN, CommandType.StoredProcedure, sqlParams);

            return result;

        }

        /// <summary>
        /// Проверка атрибута
        /// </summary>
        public void Check()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодЛица", new object[] { PersonID, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@КодАтрибута", new object[] { Id, DBManager.ParameterTypes.Int32 });
            DBManager.GetData(SQLQueries.SP_Лица_Атрибут_Check, CN, CommandType.StoredProcedure, sqlParams);
        }

        /// <summary>
        /// Инициализация сущности атрибут лица на основе ID сущности
        /// </summary>
        /// <param name="id">ID сущности</param>
        protected void FillData(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ClearModel();
                return;
            }
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Атрибут, id.ToInt(), CommandType.Text, CN))
            {
                FillDataFromDataRow(dbReader);
            }
        }

        /// <summary>
        /// Инициализация сущности Формат атрибута на строки основе DBReader
        /// </summary>
        /// <param name="dbReader">Строка данных</param>
        /// <param name="fromOuterSourse">Метод вызывается извне</param>
        public void FillDataFromDataRow(DBReader dbReader, bool fromOuterSourse = false)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца
                int colId                           = dbReader.GetOrdinal("КодАтрибутовЛиц"); 
                int colPersonID                     = dbReader.GetOrdinal("КодЛица"); 
                int colTerritoryID                  = dbReader.GetOrdinal("КодТерритории");  
                int colDateStart                    = dbReader.GetOrdinal("ДатаНачалаДействия"); 
                int colDateEnd                      = dbReader.GetOrdinal("ДатаОкончанияДействия");  
                int colDatePersonBegin              = dbReader.GetOrdinal("ДатаРождения");  
                int colDatePersonEnd                = dbReader.GetOrdinal("ДатаКонца"); 
                int colChanged                      = dbReader.GetOrdinal("Изменено");  
                int colChangedBy                    = dbReader.GetOrdinal("Изменил"); 
                int colAttributeValue1              = dbReader.GetOrdinal("ЗначениеАтрибута1"); 
                int colAttributeValue2              = dbReader.GetOrdinal("ЗначениеАтрибута2");  
                int colAttributeValue3              = dbReader.GetOrdinal("ЗначениеАтрибута3");
                

                int colAttributeFormatNameRus1 = dbReader.GetOrdinal("ИмяАтрибутаРус1");
                int colAttributeFormatNameRus2 = dbReader.GetOrdinal("ИмяАтрибутаРус2");
                int colAttributeFormatNameRus3 = dbReader.GetOrdinal("ИмяАтрибутаРус3");
                int colAttributeFormatNameLat1 = dbReader.GetOrdinal("ИмяАтрибутаЛат1");
                int colAttributeFormatNameLat2 = dbReader.GetOrdinal("ИмяАтрибутаЛат2");
                int colAttributeFormatNameLat3 = dbReader.GetOrdinal("ИмяАтрибутаЛат3");
                int colFormatTypeID            = dbReader.GetOrdinal("КодТипаАтрибута");

                int colFormatId                     = dbReader.GetOrdinal("КодФорматаАтрибута");  
                int colAttributeFormatNameReg1      = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны1");  
                int colAttributeFormatNameReg2      = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны2");  
                int colAttributeFormatNameReg3      = dbReader.GetOrdinal("ИмяАтрибутаНаЯзыкеСтраны3"); 
                int colFormat1                      = dbReader.GetOrdinal("ФорматАтрибута1");  
                int colFormat2                      = dbReader.GetOrdinal("ФорматАтрибута2");  
                int colFormat3                      = dbReader.GetOrdinal("ФорматАтрибута3");  
                int colFormatTerritoryID            = dbReader.GetOrdinal("КодТерриторииФормата");  
                int colFormatAttributeUniqueness    = dbReader.GetOrdinal("УникаленВПределахТерритории");
                int colChecked                      = dbReader.GetOrdinal("Проверено"); 
                
                #endregion
                Unavailable = false;
                if (!fromOuterSourse) dbReader.Read();

                Id = dbReader.GetInt32(colId).ToString();
                PersonID = dbReader.GetInt32(colPersonID);
                if (!dbReader.IsDBNull(colPersonID)) { PersonID = dbReader.GetInt32(colPersonID); }
                if (!dbReader.IsDBNull(colTerritoryID)) { TerritoryID = dbReader.GetInt32(colTerritoryID); }
                if (!dbReader.IsDBNull(colDateStart)){ DateStart = dbReader.GetDateTime(colDateStart); }
                if (!dbReader.IsDBNull(colDateEnd)) { DateEnd = dbReader.GetDateTime(colDateEnd); }
                if (!dbReader.IsDBNull(colDatePersonBegin)) { DatePersonBegin = dbReader.GetDateTime(colDatePersonBegin); }
                if (!dbReader.IsDBNull(colDatePersonEnd)) { DatePersonEnd = dbReader.GetDateTime(colDatePersonEnd); }
                if (!dbReader.IsDBNull(colChanged)) { Changed = dbReader.GetDateTime(colChanged); }
                if (!dbReader.IsDBNull(colChangedBy)) { ChangedBy = dbReader.GetInt32(colChangedBy); }
                if (!dbReader.IsDBNull(colChecked)) { Checked = dbReader.GetDateTime(colChecked); }
                AttributeValue1 = dbReader.GetString(colAttributeValue1);
                AttributeValue2 = dbReader.GetString(colAttributeValue2);
                AttributeValue3 = dbReader.GetString(colAttributeValue3);

                AttributeFormat = new AttributeFormat
                {
                    Id = dbReader.GetInt32(colFormatId).ToString(),
                    AttributeFormatNameReg1 = dbReader.GetString(colAttributeFormatNameReg1),
                    AttributeFormatNameReg2 = dbReader.GetString(colAttributeFormatNameReg2),
                    AttributeFormatNameReg3 = dbReader.GetString(colAttributeFormatNameReg3),
                    Format1 = dbReader.GetString(colFormat1),
                    Format2 = dbReader.GetString(colFormat2),
                    Format3 = dbReader.GetString(colFormat3),
                    TerritoryID = !dbReader.IsDBNull(colFormatTerritoryID) ? dbReader.GetInt32(colFormatTerritoryID) : (int?) null,
                    FormatAttributeUniqueness = dbReader.GetByte(colFormatAttributeUniqueness) == 1
                };
                if (!dbReader.IsDBNull(colFormatTerritoryID)) { AttributeFormat.TerritoryID = dbReader.GetInt32(colFormatTerritoryID); }

                AttributeFormat.AttributeFormatBase = new AttributeFormatBase
                {
                    ИмяАтрибутаРус1 = dbReader.GetString(colAttributeFormatNameRus1),
                    ИмяАтрибутаРус2 = dbReader.GetString(colAttributeFormatNameRus2),
                    ИмяАтрибутаРус3 = dbReader.GetString(colAttributeFormatNameRus3),
                    ИмяАтрибутаЛат1 = dbReader.GetString(colAttributeFormatNameLat1),
                    ИмяАтрибутаЛат2 = dbReader.GetString(colAttributeFormatNameLat2),
                    ИмяАтрибутаЛат3 = dbReader.GetString(colAttributeFormatNameLat3)
                };
                if (!dbReader.IsDBNull(colFormatTypeID)) { AttributeFormat.AttributeFormatBase.FormatTypeID = dbReader.GetInt32(colFormatTypeID); }
            }
            else
            {
                Unavailable = true;
                ClearModel();
            }
        }

        /// <summary>
        /// Отчистка модели данных
        /// </summary>
        private void ClearModel()
        {
            AttributeFormat = new AttributeFormat();
            Unavailable = true;
            Id = null;
            PersonID = null;
            TerritoryID = null;
            DateStart = null;
            DateEnd = null;
            DatePersonBegin = null;
            DatePersonEnd = null;
            Changed = null;
            ChangedBy = null;
            AttributeValue1 = "";
            AttributeValue2 = "";
            AttributeValue3 = "";
        }
    }
}
