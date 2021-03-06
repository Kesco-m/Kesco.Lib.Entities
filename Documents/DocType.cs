﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///     Класс сущности "Тип документа"
    /// </summary>
    /// <example>
    ///     Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Name = {Name}")]
    public class DocType : TreeNodeEntity, ICloneable<DocType>
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Инициализация объекта Тип документа по ID
        /// </summary>
        /// <param name="id">ID лица</param>
        public DocType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Инициализация объекта Тип документа по типу
        /// </summary>
        /// <param name="DocType">тип документа из перечисления</param>
        public DocType(DocTypeEnum DocType)
            : base(Convert.ToString((int) DocType))
        {
            Load();
        }

        /// <summary>
        ///     Инициализация объекта Тип документа
        /// </summary>
        public DocType() : base("0")
        {
        }

        #region Свойства

        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public DocType ParentDocType => new DocType(Parent.ToString());

        #endregion

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_document
            : _connectionString;

        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public override TreeNodeEntity TreeNodeParent => Parent > 0 ? new DocType(Parent.ToString()) : null;

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public DocType Clone()
        {
            return (DocType) MemberwiseClone();
        }

        /// <summary>
        ///     Получение списка типов документов из таблицы данных
        /// </summary>
        /// <param name="query">Запрос на множество строк DocTypes</param>
        public static List<DocType> GetDocTypesList(string query)
        {
            var docTypesList = new List<DocType>();

            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                    var colTypeDoc = dbReader.GetOrdinal("TypeDoc");
                    var colТОписание = dbReader.GetOrdinal("ТОписание");
                    var colTDescription = dbReader.GetOrdinal("TDescription");
                    var colИмяПредставления = dbReader.GetOrdinal("ИмяПредставления");
                    var colNameExist = dbReader.GetOrdinal("NameExist");
                    var colНаличиеФормы = dbReader.GetOrdinal("НаличиеФормы");
                    var colURL = dbReader.GetOrdinal("URL");
                    var colSearchURL = dbReader.GetOrdinal("SearchURL");
                    var colHelpURL = dbReader.GetOrdinal("HelpURL");
                    var colТипАвтогенерацииНомера = dbReader.GetOrdinal("ТипАвтогенерацииНомера");
                    var colАлгоритмАвтогенерацииНомера = dbReader.GetOrdinal("АлгоритмАвтогенерацииНомера");
                    var colАвтоДата = dbReader.GetOrdinal("АвтоДата");
                    var colУсловиеПохожести = dbReader.GetOrdinal("УсловиеПохожести");
                    var colИсходящий = dbReader.GetOrdinal("Исходящий");
                    var colФинансовый = dbReader.GetOrdinal("Финансовый");
                    var colБухгалтерский = dbReader.GetOrdinal("Бухгалтерский");
                    var colБухгалтерскийСправочник = dbReader.GetOrdinal("БухгалтерскийСправочник");
                    var colСоздаватьЗащищеным = dbReader.GetOrdinal("СоздаватьЗащищеным");
                    var colТипОтвета = dbReader.GetOrdinal("ТипОтвета");
                    var colChanged = dbReader.GetOrdinal("Changed");
                    var colParent = dbReader.GetOrdinal("Parent");
                    var colL = dbReader.GetOrdinal("L");
                    var colR = dbReader.GetOrdinal("R");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var docTypeRow = new DocType();
                        docTypeRow.Unavailable = false;
                        docTypeRow.Id = dbReader.GetInt32(colКодТипаДокумента).ToString();
                        if (!dbReader.IsDBNull(colТипДокумента)) docTypeRow.Name = dbReader.GetString(colТипДокумента);
                        if (!dbReader.IsDBNull(colTypeDoc)) docTypeRow.TypeDocEn = dbReader.GetString(colTypeDoc);
                        docTypeRow.DescriptionRu = dbReader.GetString(colТОписание);
                        docTypeRow.DescriptionEn = dbReader.GetString(colTDescription);
                        docTypeRow.ViewName = dbReader.GetString(colИмяПредставления);
                        docTypeRow.NameExist = dbReader.GetByte(colNameExist) == 1;
                        docTypeRow.FormExist = dbReader.GetByte(colНаличиеФормы);
                        docTypeRow.URL = dbReader.GetString(colURL);
                        docTypeRow.SearchURL = dbReader.GetString(colSearchURL);
                        docTypeRow.HelpURL = dbReader.GetString(colHelpURL);
                        docTypeRow.AutoGenTypeID = dbReader.GetInt32(colТипАвтогенерацииНомера);
                        docTypeRow.AlgorithmAutoGenID = dbReader.GetInt32(colАлгоритмАвтогенерацииНомера);
                        docTypeRow.AutoDate = dbReader.GetByte(colАвтоДата);
                        docTypeRow.EquivCondition = dbReader.GetByte(colУсловиеПохожести);
                        docTypeRow.IsOutgoing = dbReader.GetBoolean(colИсходящий);
                        docTypeRow.Finance = dbReader.GetByte(colФинансовый);
                        docTypeRow.IsAccounting = dbReader.GetBoolean(colБухгалтерский);
                        if (!dbReader.IsDBNull(colБухгалтерскийСправочник))
                            docTypeRow.AccountDirectory = dbReader.GetInt32(colБухгалтерскийСправочник);
                        docTypeRow.IsCreateProtected = dbReader.GetBoolean(colСоздаватьЗащищеным);
                        if (!dbReader.IsDBNull(colТипОтвета)) docTypeRow.ResponseType = dbReader.GetInt32(colТипОтвета);
                        docTypeRow.IsChanged = dbReader.GetBoolean(colChanged);
                        if (!dbReader.IsDBNull(colParent)) docTypeRow.Parent = dbReader.GetInt32(colParent);
                        docTypeRow.L = dbReader.GetInt32(colL);
                        docTypeRow.R = dbReader.GetInt32(colR);
                        docTypeRow.ChangePersonID = dbReader.GetInt32(colИзменил);
                        docTypeRow.ChangeDate = dbReader.GetDateTime(colИзменено);

                        docTypesList.Add(docTypeRow);
                    }
                }
            }

            return docTypesList;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Тип документа"
        /// </summary>
        public sealed override void Load()
        {
            FillData(DocTypeId);
        }

        /// <summary>
        ///     Метод загрузки и заполнения данных сущности "Тип документа"
        /// </summary>
        public void FillData(int id)
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ТипДокумента, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                    var colTypeDoc = dbReader.GetOrdinal("TypeDoc");
                    var colТОписание = dbReader.GetOrdinal("ТОписание");
                    var colTDescription = dbReader.GetOrdinal("TDescription");
                    var colИмяПредставления = dbReader.GetOrdinal("ИмяПредставления");
                    var colNameExist = dbReader.GetOrdinal("NameExist");
                    var colНаличиеФормы = dbReader.GetOrdinal("НаличиеФормы");
                    var colURL = dbReader.GetOrdinal("URL");
                    var colSearchURL = dbReader.GetOrdinal("SearchURL");
                    var colHelpURL = dbReader.GetOrdinal("HelpURL");
                    var colТипАвтогенерацииНомера = dbReader.GetOrdinal("ТипАвтогенерацииНомера");
                    var colАлгоритмАвтогенерацииНомера = dbReader.GetOrdinal("АлгоритмАвтогенерацииНомера");
                    var colАвтоДата = dbReader.GetOrdinal("АвтоДата");
                    var colУсловиеПохожести = dbReader.GetOrdinal("УсловиеПохожести");
                    var colИсходящий = dbReader.GetOrdinal("Исходящий");
                    var colФинансовый = dbReader.GetOrdinal("Финансовый");
                    var colБухгалтерский = dbReader.GetOrdinal("Бухгалтерский");
                    var colБухгалтерскийСправочник = dbReader.GetOrdinal("БухгалтерскийСправочник");
                    var colСоздаватьЗащищеным = dbReader.GetOrdinal("СоздаватьЗащищеным");
                    var colТипОтвета = dbReader.GetOrdinal("ТипОтвета");
                    var colChanged = dbReader.GetOrdinal("Changed");
                    var colParent = dbReader.GetOrdinal("Parent");
                    var colL = dbReader.GetOrdinal("L");
                    var colR = dbReader.GetOrdinal("R");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    Unavailable = false;

                    dbReader.Read();
                    Id = dbReader.GetInt32(colКодТипаДокумента).ToString();
                    if (!dbReader.IsDBNull(colТипДокумента)) Name = dbReader.GetString(colТипДокумента);
                    if (!dbReader.IsDBNull(colTypeDoc)) TypeDocEn = dbReader.GetString(colTypeDoc);
                    DescriptionRu = dbReader.GetString(colТОписание);
                    DescriptionEn = dbReader.GetString(colTDescription);
                    ViewName = dbReader.GetString(colИмяПредставления);
                    NameExist = dbReader.GetByte(colNameExist) == 1;
                    FormExist = dbReader.GetByte(colНаличиеФормы);
                    URL = dbReader.GetString(colURL);
                    SearchURL = dbReader.GetString(colSearchURL);
                    HelpURL = dbReader.GetString(colHelpURL);
                    AutoGenTypeID = dbReader.GetInt32(colТипАвтогенерацииНомера);
                    AlgorithmAutoGenID = dbReader.GetInt32(colАлгоритмАвтогенерацииНомера);
                    AutoDate = dbReader.GetByte(colАвтоДата);
                    EquivCondition = dbReader.GetByte(colУсловиеПохожести);
                    IsOutgoing = dbReader.GetBoolean(colИсходящий);
                    Finance = dbReader.GetByte(colФинансовый);
                    IsAccounting = dbReader.GetBoolean(colБухгалтерский);
                    if (!dbReader.IsDBNull(colБухгалтерскийСправочник))
                        AccountDirectory = dbReader.GetInt32(colБухгалтерскийСправочник);
                    IsCreateProtected = dbReader.GetBoolean(colСоздаватьЗащищеным);
                    if (!dbReader.IsDBNull(colТипОтвета)) ResponseType = dbReader.GetInt32(colТипОтвета);
                    IsChanged = dbReader.GetBoolean(colChanged);
                    if (!dbReader.IsDBNull(colParent)) Parent = dbReader.GetInt32(colParent);
                    L = dbReader.GetInt32(colL);
                    R = dbReader.GetInt32(colR);
                    ChangePersonID = dbReader.GetInt32(colИзменил);
                    ChangeDate = dbReader.GetDateTime(colИзменено);
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Получение сущностей типов документов по наименованию и типам
        /// </summary>
        /// <param name="docTypeName">Строка/часть строки наименования типа документа</param>
        /// <param name="typesStr"></param>
        /// <returns></returns>
        public static List<DocType> GetDocTypesByNameAndTypes(string docTypeName, string typesStr = "")
        {
            var sqlWhere = string.Empty;
            var sqlByNamePat = @"' ' + ТипДокумента LIKE N'%{0}%'";
            var sqlByTypePat = @"КодТипаДокумента IN({0})";

            if (!string.IsNullOrEmpty(docTypeName) && !string.IsNullOrEmpty(typesStr))
                sqlWhere = string.Concat(string.Format(sqlByNamePat, docTypeName), " AND ",
                    string.Format(sqlByTypePat, typesStr));
            if (!string.IsNullOrEmpty(docTypeName))
                sqlWhere = string.Format(sqlByNamePat, docTypeName);
            else if (!string.IsNullOrEmpty(typesStr)) sqlWhere = string.Format(sqlByTypePat, typesStr);

            string query;
            //Все введенные символы могут оказаться недопустимыми и условие может остаться пустым
            if (string.IsNullOrWhiteSpace(sqlWhere))
                query = string.Format(SQLQueries.SELECT_ТипыДокументов, "");
            else
                query = string.Concat(string.Format(SQLQueries.SELECT_ТипыДокументов, ""),
                    string.Format(" WHERE {0}", sqlWhere));

            return GetDocTypesList(query);
        }

        /// <summary>
        ///     Сохранение нового типа документа(Insert)
        /// </summary>
        public void Create()
        {
            var sqlParams = SetSqlParams(true);

            DBManager.ExecuteNonQuery(SQLQueries.INSERT_ТипДокумента, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Обновление существующего типа документа (Update)
        /// </summary>
        public void UpdateData()
        {
            var sqlParams = SetSqlParams(false);

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ТипДокумента, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Установка параметров для записи в БД
        /// </summary>
        /// <param name="isNew">флаг true - сохраняем новое, false - обнавляем</param>
        /// <returns>Коллекция параметров - значений</returns>
        private Dictionary<string, object> SetSqlParams(bool isNew)
        {
            var sqlParams = new Dictionary<string, object>(25);

            if (!isNew)
                sqlParams.Add("@КодТипаДокумента", Id);

            sqlParams.Add("@ТипДокумента", TypeDocRu);
            sqlParams.Add("@TypeDoc", TypeDocEn);
            sqlParams.Add("@ТОписание", DescriptionRu);
            sqlParams.Add("@TDescription", DescriptionEn);
            sqlParams.Add("@ИмяПредставления", ViewName);
            sqlParams.Add("@NameExist", NameExist);
            sqlParams.Add("@НаличиеФормы", FormExist);
            sqlParams.Add("@URL", URL);
            sqlParams.Add("@SearchURL", SearchURL);
            sqlParams.Add("@HelpURL", HelpURL);
            sqlParams.Add("@ТипАвтогенерацииНомера", AutoGenTypeID);
            sqlParams.Add("@АлгоритмАвтогенерацииНомера", AlgorithmAutoGenID);
            sqlParams.Add("@АвтоДата", AutoDate);
            sqlParams.Add("@УсловиеПохожести", EquivCondition);
            sqlParams.Add("@Исходящий", IsOutgoing);
            sqlParams.Add("@Финансовый", Finance);
            sqlParams.Add("@Бухгалтерский", IsAccounting);
            sqlParams.Add("@БухгалтерскийСправочник", AccountDirectory);
            sqlParams.Add("@СоздаватьЗащищеным", IsCreateProtected);
            sqlParams.Add("@ТипОтвета", ResponseType);
            sqlParams.Add("@Changed", IsChanged);

            return sqlParams;
        }

        /// <summary>
        ///     Загрузить подчиненные узлы
        /// </summary>
        public override List<TreeNodeEntity> LoadChildren()
        {
            var query = string.Format(SQLQueries.SELECT_ТипыДокументов_НепосредственныеПотомки, Id);
            var types = GetDocTypesList(query);

            var tree = new List<TreeNodeEntity>(types.Count);
            tree.AddRange(types);

            return tree;
        }

        /// <summary>
        ///     Загрузить родительские узлы
        /// </summary>
        public override List<TreeNodeEntity> LoadParents()
        {
            var query = string.Format(SQLQueries.SELECT_ТипыДокументов_ВсеРодители, L, R);
            var types = GetDocTypesList(query);

            var tree = new List<TreeNodeEntity>(types.Count);
            tree.AddRange(types);

            return tree;
        }

        /// <summary>
        ///     Получение типов вытекющих
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSettingsLinkedDocsInfo(int baseTypeId)
        {
            var query = SQLQueries.SELECT_СвязиТиповДокументов_Вытекающие;
            var param = new Dictionary<string, object>
            {
                {"@id", new object[] {baseTypeId, DBManager.ParameterTypes.Int32}}
            };
            var dt = DBManager.GetData(query, Document.ConnString, CommandType.Text, param);
            return dt;
        }

        #region Поля сущности "Тип документа"

        /// <summary>
        ///     Типизированный псавданим ID Тип документа
        /// </summary>
        public int DocTypeId => Id.ToInt();

        /// <summary>
        ///     ID. Тип документа из перечисления
        /// </summary>
        /// <example>
        ///     Чтобы получить строковое значение: Type.ToString();
        /// </example>
        public DocTypeEnum Type => (DocTypeEnum) DocTypeId;

        /// <summary>
        ///     Описание типа документа RU. Поле ТипДокумента
        /// </summary>
        public new string TypeDocRu => Name;

        /// <summary>
        ///     Описание типа документа EN. Поле TypeDoc
        /// </summary>
        public new string TypeDocEn { get; set; }

        /// <summary>
        ///     Поле ТОписание
        /// </summary>
        public string DescriptionRu { get; set; }

        /// <summary>
        ///     Поле TDescription
        /// </summary>
        public string DescriptionEn { get; set; }

        /// <summary>
        ///     Поле ИмяПредставления
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        ///     Поле NameExist
        /// </summary>
        public bool NameExist { get; set; }

        /// <summary>
        ///     Поле Наличие формы
        /// </summary>
        public byte FormExist { get; set; }

        /// <summary>
        ///     Поле Наличие формы
        /// </summary>
        public bool HasForm => FormExist == 1;

        /// <summary>
        ///     Поле URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        ///     Поле SearchURL
        /// </summary>
        public string SearchURL { get; set; }

        /// <summary>
        ///     Поле HelpURL
        /// </summary>
        public string HelpURL { get; set; }

        /// <summary>
        ///     Поле ТипАвтогенерацииНомера
        /// </summary>
        public int AutoGenTypeID { get; set; }

        /// <summary>
        ///     Поле ТипАвтогенерацииНомера
        /// </summary>
        public NumGenTypes NumberGenType => (NumGenTypes) AutoGenTypeID;

        /// <summary>
        ///     Поле АлгоритмАвтогенерацииНомера
        /// </summary>
        public int AlgorithmAutoGenID { get; set; }

        /// <summary>
        ///     Поле АвтоДата
        /// </summary>
        public byte AutoDate { get; set; }

        /// <summary>
        ///     Поле УсловиеПохожести
        /// </summary>
        public byte EquivCondition { get; set; }

        /// <summary>
        ///     Поле Исходящий
        /// </summary>
        public bool IsOutgoing { get; set; }

        /// <summary>
        ///     Поле Финансовый
        /// </summary>
        public byte Finance { get; set; }

        /// <summary>
        ///     Поле Бухгалтерский
        /// </summary>
        public bool IsAccounting { get; set; }

        /// <summary>
        ///     Поле БухгалтерскийСправочник
        /// </summary>
        public int AccountDirectory { get; set; }

        /// <summary>
        ///     Поле СоздаватьЗащищеным
        /// </summary>
        public bool IsCreateProtected { get; set; }

        /// <summary>
        ///     Поле ТипОтвета
        /// </summary>
        public int ResponseType { get; set; }

        /// <summary>
        ///     Поле Changed
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        ///     Поле Изменил
        /// </summary>
        public int ChangePersonID { get; set; }

        /// <summary>
        ///     Поле Изменено
        /// </summary>
        public DateTime ChangeDate { get; set; }

        #endregion
    }
}