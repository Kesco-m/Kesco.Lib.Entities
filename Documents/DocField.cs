using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс сущности "Поля документов"
    /// </summary>
    /// <example>
    /// Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Value = {Value}, Название поля = {DocumentField}")]
    public class DocField : Entity, ICloneable<DocField>
    {
        #region Поля сущности "Поля документов"

        /// <summary>
        ///  ID. Код поля документа
        ///  Типизированный псевданим для Entity.Id
        /// </summary>
        public int DocFieldId
        {
            get { return Id.ToInt(); }
        }

        /// <summary>
        ///  Связянный код типа документа из сущности "Тип документа"
        /// </summary>
        public int DocTypeId { get; set; }

        /// <summary>
        ///  Порядок поля документа для сортировки
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        ///  Поле документа - описание поля 
        /// </summary>
        public string DocumentField
        {
            get { return Name; }
        }

        /// <summary>
        ///  Описание поля по английски
        /// </summary>
        public string DocumentFieldEN { get; set; }

        /// <summary>
        /// Описание поля по эстонски
        /// </summary>
        public string DocumentFieldET { get; set; }

        /// <summary>
        ///  Колонка таблицы из таблицы ДокументыДанные
        /// </summary>
        public string DataColomnName { get; set; }

        /// <summary>
        ///  Обязательность
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        ///  Рассчетное, вычисляемое поле
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        ///  Код типа поля из таблицы "ТипыПолей"
        /// </summary>
        public int FieldTypeID { get; set; }

        /// <summary>
        ///  Число десятичных знаков
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        ///  URL Поиска
        /// </summary>
        public string SearchURL { get; set; }

        /// <summary>
        ///  Множественный выбор
        /// </summary>
        public bool IsMultipleSelect { get; set; }

        /// <summary>
        ///  Строгий поиск
        /// </summary>
        public bool IsStrictSearch { get; set; }

        /// <summary>
        ///  Параметры поиска
        /// </summary>
        public string SearchParameters { get; set; }

        /// <summary>
        ///  Режим поиска типов
        /// </summary>
        public byte TypesSearchMode { get; set; }

        /// <summary>
        ///  Заголовок формы поиска
        /// </summary>
        public string SearchFormTitle { get; set; }

        /// <summary>
        ///  Строка подключения
        /// </summary>
        /// <example>
        ///  @DS_resource
        /// </example>
        public string ConnectionString { get; set; }

        /// <summary>
        ///  SQL Запрос
        /// </summary>
        public string SQLquery { get; set; }

        /// <summary>
        ///  Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///  ID сотрудника изменившего
        /// </summary>
        public int UserChangeID { get; set; }

        /// <summary>
        ///  Изменено
        /// </summary>
        public DateTime ChangedDate { get; set; }

        #endregion

        /// <summary>
        ///  Ссылка на документы данные. Для возможности записи значения в класс DocumentData
        /// </summary>
        public DocumentData DocDataMapping
        {
            get { return _document.DocumentData; }
       }

        /// <summary>
        ///  Ссылка на документ
        /// </summary>
        private readonly Document _document;

        /// <summary>
        /// Если у поля нет маппинга значит значения сохраняются/берутся в СвязиДокументов, эту функцию берет на себя этот класс
        /// </summary>
        public BaseDocFacade BaseDocSaver; 

        /// <summary>
        ///  Значение
        /// </summary>
        public object Value
        {
            get { return GetValueFromDocumentData(); }
            set
            {
                var newVal = ValueToString(value);

                if (newVal != _oldValue)
                {
                    SaveValue(value == null ? "" : value.ToString());
                    ValueChangedEvent_Invoke(newVal, _oldValue);
                }

                _oldValue = newVal;
            }
        }

        /// <summary>
        ///  Передыдущее значение
        /// </summary>
        public string _oldValue;

        /// <summary>
        ///  Получает строковое значение, либо пустое значение ("") 
        ///  в случае неудачи или отсутсвия значения
        /// </summary>
        public string ValueString
        {
            get { return ValueToString(Value); }
            set { Value = value; }
        }

        /// <summary>
        ///  Преобразование Value to string
        /// </summary>
        private string ValueToString(object objVal)
        {
            try
            {
                if (objVal != null)
                    return objVal.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        ///  Конвертирует в int значение, в случае неудачи возращает дефолтное значение для типа int - 0.
        /// </summary>
        /// <remarks>
        ///  Это свойство со строгой типизацией. Если значение другого типа, то свойство может ничего не вернуть
        /// </remarks>
        public int ValueInt
        {
            get
            {
                if (Value == null)
                    return 0;

                if (Value is int)
                    return (int)Value;

                int integer;
                int.TryParse(Value.ToString(), out integer);

                return integer;
            }
        }

        /// <summary>
        ///  Конвертирует в DateTime значение Value
        /// </summary>
        public DateTime? DateTimeValue
        {
            get { return (DateTime?) Value; }
        }

        /// <summary>
        ///  Указывает, является ли значение равным null или Empty
        /// </summary>
        /// <remarks>
        ///  Корректно может обрабатывать поля связанные с колонкой таблицы ДокументыДанные,
        ///  Для остальных нужно исключать из проверки, либо доваить делегат
        /// </remarks>
        public bool IsValueEmpty
        {
            get
            {
                var val = Value;
                if (val == null)
                    return true;

                var type = val.GetType();

                switch (type.Name)
                {
                    case "String":
                        if (string.IsNullOrEmpty((string) val))
                            return true;
                        break;
                    case "Int32":
                        var i = (int?) val;
                        if (i == 0)
                            return true;
                        break;
                    case "Decimal":
                        var m = (decimal?)val;
                        if (m == 0)
                            return true;
                        break;
                    case "DateTime":
                        var dt = (DateTime?) val;
                        if (dt == DateTime.MinValue)
                            return true;
                        break;
                    case "Byte":
                        var b = (byte?) val;
                        if (b == 0)
                            return true;
                        break;
                    case "Double":
                        var d = (double?) val;
                        if (d == 0)
                            return true;
                        break;
                    default:
                        throw new ArgumentException("Не реализовано для типа: " + type.Name);
                }

                return false;
            }
        }

        /// <summary>
        /// Стирает значение
        /// </summary>
        public void ClearValue()
        {
            if(Value == null) return;

            var type = Value.GetType();

            switch (type.Name)
            {
                case "String":
                    Value = "";
                    break;
                case "Int32":
                    Value = 0;
                    break;
                case "Decimal":
                    Value = 0.0M;
                    break;
                case "DateTime":
                    Value = default(DateTime);
                    break;
                case "Byte":
                    Value = 0;
                    break;
                case "Double":
                    Value = 0.0D;
                    break;
                default:
                    throw new ArgumentException("Не реализовано ClearValue() для типа: " + type.Name);
            }
        }

        /// <summary>
        ///  Приватный конструктор - запрет для создания объекта без параметров(без ссылки на документ он будет работать неправильно) 
        /// </summary>
        private DocField()
        {
        }

        /// <summary>
        ///  Конструктор класса DocField
        /// </summary>
        /// <param name="id">Параметр для немедленной загрузки из базы</param>
        /// <param name="document">Ссылка на Document иначе маппинг будет некорректно работать</param>
        public DocField(string id, Document document) : base(id)
        {
            _document = document;
            Load();
            BaseDocSaver = new BaseDocFacade(_document, this, IsMultipleSelect ? BaseSetBehavior.SetBaseDoc : BaseSetBehavior.RemoveAllAndAddDoc);
        }

        /// <summary>
        /// Конструктор класса DocField
        /// </summary>
        /// <param name="document">Ссылка на Document иначе маппинг будет некорректно работать</param>
        public DocField(Document document)
        {
            _document = document;
        }

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
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Загрузка данных по Id
        /// </summary>
        public sealed override void Load()
        {
            FillData(DocFieldId);
        }

        /// <summary>
        ///  Получить поля документа по типу документа
        /// </summary>
        public static Dictionary<string, DocField> GetDocFieldsByDocId(int id, Document document)
        {
            Dictionary<string, DocField> list = null;
            using (var dbReader = new DBReader(SQLQueries.SELECT_ПоляДокументов_ТипДокумента, id , CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new Dictionary<string, DocField>();
                    #region Получение порядкового номера столбца

                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colПорядокПоляДокумента = dbReader.GetOrdinal("ПорядокПоляДокумента");
                    int colПолеДокумента = dbReader.GetOrdinal("ПолеДокумента");
                    int colПолеДокументаEN = dbReader.GetOrdinal("ПолеДокументаEN");
                    int colПолеДокументаET = dbReader.GetOrdinal("ПолеДокументаET");
                    int colКолонкаТаблицы = dbReader.GetOrdinal("КолонкаТаблицы");
                    int colОбязательность = dbReader.GetOrdinal("Обязательность");
                    int colРассчетное = dbReader.GetOrdinal("Рассчетное");
                    int colКодТипаПоля = dbReader.GetOrdinal("КодТипаПоля");
                    int colЧислоДесятичныхЗнаков = dbReader.GetOrdinal("ЧислоДесятичныхЗнаков");
                    int colURLПоиска = dbReader.GetOrdinal("URLПоиска");
                    int colМножественныйВыбор = dbReader.GetOrdinal("МножественныйВыбор");
                    int colСтрогийПоиск = dbReader.GetOrdinal("СтрогийПоиск");
                    int colПараметрыПоиска = dbReader.GetOrdinal("ПараметрыПоиска");
                    int colРежимПоискаТипов = dbReader.GetOrdinal("РежимПоискаТипов");
                    int colЗаголовокФормыПоиска = dbReader.GetOrdinal("ЗаголовокФормыПоиска");
                    int colСтрокаПодключения = dbReader.GetOrdinal("СтрокаПодключения");
                    int colSQLЗапрос = dbReader.GetOrdinal("SQLЗапрос");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");
                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocField(document)
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодПоляДокумента).ToString(),
                            DocTypeId = id, // нет смысла получать из базы, то что у нас уже есть
                            SortNumber = dbReader.GetInt32(colПорядокПоляДокумента),
                            Name = dbReader.GetString(colПолеДокумента),
                            DocumentFieldEN = dbReader.GetString(colПолеДокументаEN),
                            DocumentFieldET = dbReader.GetString(colПолеДокументаET),
                            DataColomnName = dbReader.GetString(colКолонкаТаблицы),
                            IsMandatory = dbReader.GetBoolean(colОбязательность),
                            IsCalculated = dbReader.GetBoolean(colРассчетное),
                            FieldTypeID = dbReader.GetInt32(colКодТипаПоля),
                            Decimals = dbReader.GetInt32(colЧислоДесятичныхЗнаков),
                            SearchURL = dbReader.GetString(colURLПоиска),
                            IsMultipleSelect = dbReader.GetBoolean(colМножественныйВыбор),
                            IsStrictSearch = dbReader.GetBoolean(colСтрогийПоиск),
                            SearchParameters = dbReader.GetString(colПараметрыПоиска),
                            TypesSearchMode = dbReader.GetByte(colРежимПоискаТипов),
                            SearchFormTitle = dbReader.GetString(colЗаголовокФормыПоиска),
                            ConnectionString = dbReader.GetString(colСтрокаПодключения),
                            SQLquery = dbReader.GetString(colSQLЗапрос),
                            Description = dbReader.GetString(colОписание),
                            UserChangeID = dbReader.GetInt32(colИзменил),
                            ChangedDate = dbReader.GetDateTime(colИзменено)
                        };

                        row.BaseDocSaver = new BaseDocFacade(document, row, row.IsMultipleSelect ? BaseSetBehavior.SetBaseDoc : BaseSetBehavior.RemoveAllAndAddDoc);

                        list.Add(row.Id, row);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Метод загрузки и заполнения данных сущности "Тип документа"
        /// </summary>
        public void FillData(int id)
        {
            if(id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ПолеДокумента, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    int colПорядокПоляДокумента = dbReader.GetOrdinal("ПорядокПоляДокумента");
                    int colПолеДокумента = dbReader.GetOrdinal("ПолеДокумента");
                    int colПолеДокументаEN = dbReader.GetOrdinal("ПолеДокументаEN");
                    int colПолеДокументаET = dbReader.GetOrdinal("ПолеДокументаET");
                    int colКолонкаТаблицы = dbReader.GetOrdinal("КолонкаТаблицы");
                    int colОбязательность = dbReader.GetOrdinal("Обязательность");
                    int colРассчетное = dbReader.GetOrdinal("Рассчетное");
                    int colКодТипаПоля = dbReader.GetOrdinal("КодТипаПоля");
                    int colЧислоДесятичныхЗнаков = dbReader.GetOrdinal("ЧислоДесятичныхЗнаков");
                    int colURLПоиска = dbReader.GetOrdinal("URLПоиска");
                    int colМножественныйВыбор = dbReader.GetOrdinal("МножественныйВыбор");
                    int colСтрогийПоиск = dbReader.GetOrdinal("СтрогийПоиск");
                    int colПараметрыПоиска = dbReader.GetOrdinal("ПараметрыПоиска");
                    int colРежимПоискаТипов = dbReader.GetOrdinal("РежимПоискаТипов");
                    int colЗаголовокФормыПоиска = dbReader.GetOrdinal("ЗаголовокФормыПоиска");
                    int colСтрокаПодключения = dbReader.GetOrdinal("СтрокаПодключения");
                    int colSQLЗапрос = dbReader.GetOrdinal("SQLЗапрос");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");
                    #endregion

                   
                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодПоляДокумента).ToString();
                        DocTypeId = dbReader.GetInt32(colКодТипаДокумента);
                        SortNumber = dbReader.GetInt32(colПорядокПоляДокумента);
                        Name = dbReader.GetString(colПолеДокумента);
                        DocumentFieldEN = dbReader.GetString(colПолеДокументаEN);
                        DocumentFieldET = dbReader.GetString(colПолеДокументаET);
                        DataColomnName = dbReader.GetString(colКолонкаТаблицы);
                        IsMandatory = dbReader.GetBoolean(colОбязательность);
                        IsCalculated = dbReader.GetBoolean(colРассчетное);
                        FieldTypeID = dbReader.GetInt32(colКодТипаПоля);
                        Decimals = dbReader.GetInt32(colЧислоДесятичныхЗнаков);
                        SearchURL = dbReader.GetString(colURLПоиска);
                        IsMultipleSelect = dbReader.GetBoolean(colМножественныйВыбор);
                        IsStrictSearch = dbReader.GetBoolean(colСтрогийПоиск);
                        SearchParameters = dbReader.GetString(colПараметрыПоиска);
                        TypesSearchMode = dbReader.GetByte(colРежимПоискаТипов);
                        SearchFormTitle = dbReader.GetString(colЗаголовокФормыПоиска);
                        ConnectionString = dbReader.GetString(colСтрокаПодключения);
                        SQLquery = dbReader.GetString(colSQLЗапрос);
                        Description = dbReader.GetString(colОписание);
                        UserChangeID = dbReader.GetInt32(colИзменил);
                        ChangedDate = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///  Сохранение нового поля документа(Insert)
        /// </summary>
        public void Create()
        {
            var sqlParams = SetSqlParams(isNew: true);

            DBManager.ExecuteNonQuery(SQLQueries.INSERT_ПолеДокумента, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///  Обновление существующего поля документа (Update)
        /// </summary>
        public void UpdateData()
        {
            var sqlParams = SetSqlParams(isNew: false);

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ПолеДокумента, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///  Установка параметров для записи в БД
        /// </summary>
        /// <param name="isNew">флаг true - сохраняем новое, false - обнавляем</param>
        /// <returns>Коллекция параметров - значений</returns>
        private Dictionary<string, object> SetSqlParams(bool isNew)
        {
            var sqlParams = new Dictionary<string, object>();

            if (!isNew)
                sqlParams.Add("@КодПоляДокумента", Id);

            sqlParams.Add("@КодТипаДокумента", DocTypeId);
            sqlParams.Add("@ПорядокПоляДокумента", SortNumber);
            sqlParams.Add("@ПолеДокумента", Name);
            sqlParams.Add("@ПолеДокументаEN", DocumentFieldEN);
            sqlParams.Add("@ПолеДокументаET", DocumentFieldET);
            sqlParams.Add("@КолонкаТаблицы", DataColomnName);
            sqlParams.Add("@Обязательность", IsMandatory);
            sqlParams.Add("@Рассчетное", IsCalculated);
            sqlParams.Add("@КодТипаПоля", FieldTypeID);
            sqlParams.Add("@ЧислоДесятичныхЗнаков", Decimals);
            sqlParams.Add("@URLПоиска", SearchURL);
            sqlParams.Add("@МножественныйВыбор", IsMultipleSelect);
            sqlParams.Add("@СтрогийПоиск", IsStrictSearch);
            sqlParams.Add("@ПараметрыПоиска", SearchParameters);
            sqlParams.Add("@РежимПоискаТипов", TypesSearchMode);
            sqlParams.Add("@ЗаголовокФормыПоиска", SearchFormTitle);
            sqlParams.Add("@СтрокаПодключения", ConnectionString);
            sqlParams.Add("@SQLЗапрос", SQLquery);
            sqlParams.Add("@Описание", Description);

            return sqlParams;
        }

        #region Методы для меппинга на DocumentData
      
        /// <summary>
        ///  Сохраняет значение с приведением типов
        /// </summary>
        public void SaveValue(string value)
        {
            if (DocDataMapping == null)
                throw new MemberAccessException("Ошибка программиста DocDataMapping не инициализирован в классе DocField");
            try
            {
                int i;
                byte b;
                decimal d;
                DateTime dt;

                switch (DataColomnName)
                {
                    case "":
                    case null:
                        SetLinkedDocValue(value);
                        break;
                    case "КодЛица1":
                        DocDataMapping.PersonId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЛица2":
                        DocDataMapping.PersonId2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЛица3":
                        DocDataMapping.PersonId3 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЛица4":
                        DocDataMapping.PersonId4 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЛица5":
                        DocDataMapping.PersonId5 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЛица6":
                        DocDataMapping.PersonId6 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСклада1":
                        DocDataMapping.StockId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСклада2":
                        DocDataMapping.StockId2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСклада3":
                        DocDataMapping.StockId3 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСклада4":
                        DocDataMapping.StockId4 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодРесурса1":
                        DocDataMapping.ResourceId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодРесурса2":
                        DocDataMapping.ResourceId2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСотрудника1":
                        DocDataMapping.EmployeeId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСотрудника2":
                        DocDataMapping.EmployeeId2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСотрудника3":
                        DocDataMapping.EmployeeId3 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодРасположения1":
                        DocDataMapping.LocationId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодБазисаПоставки":
                        DocDataMapping.BaseDeliveryId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодВидаТранспорта":
                        DocDataMapping.TransportTypeId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодМестаХранения":
                        DocDataMapping.StorageLocationId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодЕдиницыИзмерения":
                        DocDataMapping.UnitsId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСтавкиНДС":
                        DocDataMapping.NDSrateId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодТУзла1":
                        DocDataMapping.UzelId1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодТУзла2":
                        DocDataMapping.UzelId2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодТерритории":
                        DocDataMapping.TerritoryId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "КодСтатьиБюджета":
                        DocDataMapping.BudgetId = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Дата2":
                        DocDataMapping.Date2 = DateTime.TryParse(value, out dt) ? dt : (DateTime?) null; 
                        break;
                    case "Дата3":
                        DocDataMapping.Date3 = DateTime.TryParse(value, out dt) ? dt : (DateTime?) null;
                        break;
                    case "Дата4":
                        DocDataMapping.Date4 = DateTime.TryParse(value, out dt) ? dt : (DateTime?) null;
                        break;
                    case "Дата5":
                        DocDataMapping.Date5 = DateTime.TryParse(value, out dt) ? dt : (DateTime?) null;
                        break;
                    case "Flag1":
                        DocDataMapping.Flag1 = byte.TryParse(value, out b) ? b : (byte?) null;
                        break;
                    case "Flag2":
                        DocDataMapping.Flag1 = byte.TryParse(value, out b) ? b : (byte?)null;
                        break;
                    case "Int1":
                        DocDataMapping.Int1 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int2":
                        DocDataMapping.Int2 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int3":
                        DocDataMapping.Int3 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int4":
                        DocDataMapping.Int4 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int5":
                        DocDataMapping.Int5 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int6":
                        DocDataMapping.Int6 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Int7":
                        DocDataMapping.Int7 = int.TryParse(value, out i) ? i : (int?) null;
                        break;
                    case "Text50_1":
                        DocDataMapping.Text50_1 = value;
                        break;
                    case "Text50_2":
                        DocDataMapping.Text50_2 = value;
                        break;
                    case "Text50_3":
                        DocDataMapping.Text50_3 = value;
                        break;
                    case "Text50_4":
                        DocDataMapping.Text50_4 = value;
                        break;
                    case "Text50_5":
                        DocDataMapping.Text50_5 = value;
                        break;
                    case "Text50_6":
                        DocDataMapping.Text50_6 = value;
                        break;
                    case "Text50_7":
                        DocDataMapping.Text50_7 = value;
                        break;
                    case "Text50_8":
                        DocDataMapping.Text50_8 = value;
                        break;
                    case "Text50_9":
                        DocDataMapping.Text50_9 = value;
                        break;
                    case "Text50_10":
                        DocDataMapping.Text50_10 = value;
                        break;
                    case "Text50_11":
                        DocDataMapping.Text50_11 = value;
                        break;
                    case "Text50_12":
                        DocDataMapping.Text50_12 = value;
                        break;
                    case "Text50_13":
                        DocDataMapping.Text50_13 = value;
                        break;
                    case "Text100_1":
                        DocDataMapping.Text100_1 = value;
                        break;
                    case "Text100_2":
                        DocDataMapping.Text100_2 = value;
                        break;
                    case "Text100_3":
                        DocDataMapping.Text100_3 = value;
                        break;
                    case "Text100_4":
                        DocDataMapping.Text100_4 = value;
                        break;
                    case "Text100_5":
                        DocDataMapping.Text100_5 = value;
                        break;
                    case "Text100_6":
                        DocDataMapping.Text100_6 = value;
                        break;
                    case "Text300_1":
                        DocDataMapping.Text300_1 = value;
                        break;
                    case "Text300_2":
                        DocDataMapping.Text300_2 = value;
                        break;
                    case "Text300_3":
                        DocDataMapping.Text300_3 = value;
                        break;
                    case "Text300_4":
                        DocDataMapping.Text300_4 = value;
                        break;
                    case "Text300_5":
                        DocDataMapping.Text300_5 = value;
                        break;
                    case "Text300_6":
                        DocDataMapping.Text300_6 = value;
                        break;
                    case "Text300_7":
                        DocDataMapping.Text300_7 = value;
                        break;
                    case "Text300_8":
                        DocDataMapping.Text300_8 = value;
                        break;
                    case "Text300_9":
                        DocDataMapping.Text300_9 = value;
                        break;
                    case "Text1000_1":
                        DocDataMapping.Text1000_1 = value;
                        break;
                    case "Text1000_2":
                        DocDataMapping.Text1000_2 = value;
                        break;
                    case "Money1":
                        DocDataMapping.Money1 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money2":
                        DocDataMapping.Money2 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money3":
                        DocDataMapping.Money3 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money4":
                        DocDataMapping.Money4 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money5":
                        DocDataMapping.Money5 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money6":
                        DocDataMapping.Money6 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money7":
                        DocDataMapping.Money7 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money8":
                        DocDataMapping.Money8 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Money9":
                        DocDataMapping.Money9 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Decimal1":
                        DocDataMapping.Decimal1 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Decimal2":
                        DocDataMapping.Decimal2 = decimal.TryParse(value, out d) ? d : (decimal?) null;
                        break;
                    case "Float1":
                        double f;
                        DocDataMapping.Float1 = double.TryParse(value, out f) ? f : (double?) null;
                        break;
                    case "ТекстДокумента":
                        DocDataMapping.DocumentText = value;
                        break;
                }
            }
            catch (Exception ex)
            {
                const string errMsg = "Ошибка в классе DocField методе SetValueToDocumentData, вероятно данные не отвалидированы или выбран неверное поле для данных";
                Logger.WriteEx(new DetailedException(errMsg, ex, ex.Message + " поле " + DataColomnName));
                // ReSharper disable once PossibleIntendedRethrow
                throw ex;
            }
        }

        /// <summary>
        ///  Получение данных из DocumentData опираясь на mapping
        /// </summary>
        public object GetValueFromDocumentData()
        {
            if (DocDataMapping == null)
                throw new MemberAccessException("Ошибка программиста DocDataMapping не инициализирован в классе DocField");
            try
            {
                switch (DataColomnName)
                {
                    case "":
                    case null:
                        return GetLinkedDocValue();
                    case "КодДокумента":
                        return DocDataMapping.Id;
                    case "КодЛица1":
                        return DocDataMapping.PersonId1;
                    case "КодЛица2":
                        return DocDataMapping.PersonId2;
                    case "КодЛица3":
                        return DocDataMapping.PersonId3;
                    case "КодЛица4":
                        return DocDataMapping.PersonId4;
                    case "КодЛица5":
                        return DocDataMapping.PersonId5;
                    case "КодЛица6":
                        return DocDataMapping.PersonId6;
                    case "КодСклада1":
                        return DocDataMapping.StockId1;
                    case "КодСклада2":
                        return DocDataMapping.StockId2;
                    case "КодСклада3":
                        return DocDataMapping.StockId3;
                    case "КодСклада4":
                        return DocDataMapping.StockId4;
                    case "КодРесурса1":
                        return DocDataMapping.ResourceId1;
                    case "КодРесурса2":
                        return DocDataMapping.ResourceId2;
                    case "КодСотрудника1":
                        return DocDataMapping.EmployeeId1;
                    case "КодСотрудника2":
                        return DocDataMapping.EmployeeId2;
                    case "КодСотрудника3":
                        return DocDataMapping.EmployeeId3;
                    case "КодРасположения1":
                        return DocDataMapping.LocationId1;
                    case "КодБазисаПоставки":
                        return DocDataMapping.BaseDeliveryId;
                    case "КодВидаТранспорта":
                        return DocDataMapping.TransportTypeId;
                    case "КодМестаХранения":
                        return DocDataMapping.StorageLocationId;
                    case "КодЕдиницыИзмерения":
                        return DocDataMapping.UnitsId;
                    case "КодСтавкиНДС":
                        return DocDataMapping.NDSrateId;
                    case "КодТУзла1":
                        return DocDataMapping.UzelId1;
                    case "КодТУзла2":
                        return DocDataMapping.UzelId2;
                    case "КодТерритории":
                        return DocDataMapping.TerritoryId;
                    case "КодСтатьиБюджета":
                        return DocDataMapping.BudgetId;
                    case "Дата2":
                        return DocDataMapping.Date2;
                    case "Дата3":
                        return DocDataMapping.Date3;
                    case "Дата4":
                        return DocDataMapping.Date4;
                    case "Дата5":
                        return DocDataMapping.Date5;
                    case "Flag1":
                        return DocDataMapping.Flag1;
                    case "Flag2":
                        return DocDataMapping.Flag2;
                    case "Int1":
                        return DocDataMapping.Int1;
                    case "Int2":
                        return DocDataMapping.Int2;
                    case "Int3":
                        return DocDataMapping.Int3;
                    case "Int4":
                        return DocDataMapping.Int4;
                    case "Int5":
                        return DocDataMapping.Int5;
                    case "Int6":
                        return DocDataMapping.Int6;
                    case "Int7":
                        return DocDataMapping.Int7;
                    case "Text50_1":
                        return DocDataMapping.Text50_1;
                    case "Text50_2":
                        return DocDataMapping.Text50_2;
                    case "Text50_3":
                        return DocDataMapping.Text50_3;
                    case "Text50_4":
                        return DocDataMapping.Text50_4;
                    case "Text50_5":
                        return DocDataMapping.Text50_5;
                    case "Text50_6":
                        return DocDataMapping.Text50_6;
                    case "Text50_7":
                        return DocDataMapping.Text50_7;
                    case "Text50_8":
                        return DocDataMapping.Text50_8;
                    case "Text50_9":
                        return DocDataMapping.Text50_9;
                    case "Text50_10":
                        return DocDataMapping.Text50_10;
                    case "Text50_11":
                        return DocDataMapping.Text50_11;
                    case "Text50_12":
                        return DocDataMapping.Text50_12;
                    case "Text50_13":
                        return DocDataMapping.Text50_13;
                    case "Text100_1":
                        return DocDataMapping.Text100_1;
                    case "Text100_2":
                        return DocDataMapping.Text100_2;
                    case "Text100_3":
                        return DocDataMapping.Text100_3;
                    case "Text100_4":
                        return DocDataMapping.Text100_4;
                    case "Text100_5":
                        return DocDataMapping.Text100_5;
                    case "Text100_6":
                        return DocDataMapping.Text100_6;
                    case "Text300_1":
                        return DocDataMapping.Text300_1;
                    case "Text300_2":
                        return DocDataMapping.Text300_2;
                    case "Text300_3":
                        return DocDataMapping.Text300_3;
                    case "Text300_4":
                        return DocDataMapping.Text300_4;
                    case "Text300_5":
                        return DocDataMapping.Text300_5;
                    case "Text300_6":
                        return DocDataMapping.Text300_6;
                    case "Text300_7":
                        return DocDataMapping.Text300_7;
                    case "Text300_8":
                        return DocDataMapping.Text300_8;
                    case "Text300_9":
                        return DocDataMapping.Text300_9;
                    case "Text1000_1":
                        return DocDataMapping.Text1000_1;
                    case "Text1000_2":
                        return DocDataMapping.Text1000_2;
                    case "Money1":
                        return DocDataMapping.Money1;
                    case "Money2":
                        return DocDataMapping.Money2;
                    case "Money3":
                        return DocDataMapping.Money3;
                    case "Money4":
                        return DocDataMapping.Money4;
                    case "Money5":
                        return DocDataMapping.Money5;
                    case "Money6":
                        return DocDataMapping.Money6;
                    case "Money7":
                        return DocDataMapping.Money7;
                    case "Money8":
                        return DocDataMapping.Money8;
                    case "Money9":
                        return DocDataMapping.Money9;
                    case "Decimal1":
                        return DocDataMapping.Decimal1;
                    case "Decimal2":
                        return DocDataMapping.Decimal2;
                    case "Float1":
                        return DocDataMapping.Float1;
                    case "ТекстДокумента":
                        return DocDataMapping.DocumentText;
                    default:
                        throw new ArgumentException(@"Маппинг на DocumentData не реализован для поля " + DataColomnName, DataColomnName);
                }
            }
            catch (Exception ex)
            {
                const string errMsg = "Ошибка в классе DocField методе SetValueToDocumentData, вероятно данные не отвалидированы или выбран неверное поле для данных";
                Logger.WriteEx(new DetailedException(errMsg, ex, ex.Message + " поле " + DataColomnName));
                // ReSharper disable once PossibleIntendedRethrow
                throw ex;
            }
        }

        #endregion

        /// <summary>
        ///  Получение специального значения не входящий DocumentData
        /// </summary>
        public object GetLinkedDocValue()
        {
            if (Id.IsNullEmptyOrZero()) return "";

            if(BaseDocSaver == null)
                throw new Exception("Ошибка в классе DocField(ID=" + Id + ") методе GetSpecialValue, BaseDocSaver не инициализирован");

            return BaseDocSaver.Value;
        }

        /// <summary>
        ///  Установка специального значения не входящий DocumentData
        /// </summary>
        public void SetLinkedDocValue(object value)
        {
            if(Id.IsNullEmptyOrZero()) return;

            if (BaseDocSaver == null)
                throw new Exception("Ошибка в классе DocField(ID=" + Id + ") методе GetSpecialValue, BaseDocSaver не инициализирован");

            BaseDocSaver.Value = value == null ? "" : value.ToString();
        }

        /// <summary>
        ///  Переопределенный метод сравнения DocField по  Value значениям
        /// </summary>
        public override bool Equals(object obj)
        {
            var field = obj as DocField;
            return field != null && EqualsHelper(this, field);
        }

        /// <summary>
        /// метод сравнения DocField по  Value значениям
        /// </summary>
        public bool Equals(DocField a)
        {
            return EqualsHelper(this, a);
        }

        /// <summary>
        ///  Переопределенный метод взятия хеша
        /// </summary>
        public override int GetHashCode()
        {
            var val = Value;
            return DocFieldId ^ (val != null ? val.GetHashCode() : 1);
        }

        /// <summary>
        ///  Сравненене двух Value значений для DocField
        /// </summary>
        private static bool EqualsHelper(DocField a, DocField b)
        {
            if (a == null || b == null)
                return false;

            if (ReferenceEquals(a, b))
                return true;

            var val = a.Value;
            var otherVal = b.Value;

            if (val == null || otherVal == null)
                return false;

            var type = val.GetType();
            var type2 = otherVal.GetType();

            if (type != type2)
                return false;

            switch (type.Name)
            {
                case "String":
                    var str1 = (string)val;
                    var str2 = (string)otherVal;
                    if (str1.Equals(str2))
                        return true;

                    return false;
                case "Int32":
                    var int1 = (int?)val;
                    var int2 = (int?)otherVal;
                    if (int1 == int2)
                        return true;

                    return false;
                case "Decimal":
                    var d1 = (decimal?)val;
                    var d2 = (decimal?)otherVal;
                    if (d1 == d2)
                        return true;

                    return false;
                case "DateTime":
                    var dt1 = (DateTime?)val;
                    var dt2 = (DateTime?)otherVal;
                    if (dt1 == dt2)
                        return true;

                    return false;
                case "Byte":
                    var b1 = (byte?)val;
                    var b2 = (byte?)otherVal;
                    if (b1 == b2)
                        return true;

                    return false;
                case "Double":
                    var f1 = (double?)val;
                    var f2 = (double?)otherVal;
                    if (f1 == f2)
                        return true;
                    break;
                default:
                    throw new ArgumentException("DocField: Не реализовано сравнение для типа: " + type.Name);
            }

            return false;

        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public DocField Clone()
        {
            return (DocField)MemberwiseClone();
        }
    }
}
