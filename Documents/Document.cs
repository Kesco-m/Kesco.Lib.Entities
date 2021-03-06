﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///     Класс сущности "Документ"
    /// </summary>
    /// <example>
    ///     Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Number = {Number}, Desc = {Description}")]
    public class Document : Entity, ICloneable<Document>
    {
        
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        private bool _generateNumber;

        /// <summary>
        ///     Инициализация объекта Документ по ID
        /// </summary>
        /// <param name="id">ID документа</param>
        /// <param name="extendedLoad">
        ///     Расширенная загрузка данных - подгружать данные из ДокументыДанные в класс DocumentData
        ///     Поумолчанию - true, если дополнительные данные не нужны выставляем false, тогда дополнительное
        ///     обращение к таблице ДокументыДанные не происходит
        /// </param>
        public Document(string id, bool extendedLoad = true) : base(id)
        {
            LoadDocument(id, extendedLoad);
        }

        /// <summary>
        ///     Конструктор класа (Сущность документ)
        /// </summary>
        /// <param name="id"></param>
        public Document(string id) : base(id)
        {
            LoadDocument(id, true);
        }

        /// <summary>
        ///     Инициализация объекта Документ
        /// </summary>
        public Document()
        {
            LoadedExternalProperties = new Dictionary<string, DateTime>();
            DocumentData = new DocumentData {Id = Id};
        }

        /// <summary>
        ///     Состоит ли номер документа полностью из цифр
        /// </summary>
        public bool NumberIsDigital
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Number))
                {
                    var dNum = Number.Trim();
                    return dNum.All(char.IsDigit);
                }

                return false;
            }
        }

        /// <summary>
        ///     Доступность генерации номера документа
        /// </summary>
        public bool IsNumberGenerationAvailable => DocType.NumberGenType != NumGenTypes.CanNotBeGenerated && IsNew;

        /// <summary>
        ///     Доступность генерации номера документа
        /// </summary>
        public bool GenerateNumber
        {
            get { return _generateNumber; }
            set
            {
                if (value && DocType.NumberGenType == NumGenTypes.CanBeGenerated)
                    _generateNumber = true;
                else
                    _generateNumber = false;
            }
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_document
            : _connectionString;

        /// <summary>
        ///     Проверка на то, что текущий документ является договором
        /// </summary>
        public bool IsDogovor
        {
            get
            {
                var dt = DBManager.GetData(SQLQueries.SELECT_ТипыДоговоров, CN);
                if (dt.Rows.Count == 0) return false;

                var query = dt.AsEnumerable().Where(dr => dr.Field<int>("КодТипаДокумента").Equals(TypeId));

                return query.Any();
            }
        }

        /// <summary>
        ///     Проверка на то, что текущий документ является приложением
        /// </summary>
        public bool IsEnclosure
        {
            get
            {
                var dt = DBManager.GetData(SQLQueries.SELECT_ТипыПриложений, CN);
                if (dt.Rows.Count == 0) return false;

                var query = dt.AsEnumerable().Where(dr => dr.Field<int>("КодТипаДокумента").Equals(TypeId));

                return query.Any();
            }
        }

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public virtual Document Clone()
        {
            //var newObj = new Document();
            //  foreach (var pi in this.GetType().GetProperties().Where(pi => pi.CanRead && pi.CanWrite && pi.PropertyType.IsSerializable))
            //  {
            //      pi.SetValue(newObj, pi.GetValue(this, null), null);
            //  }

            //  newObj.DocumentData = DocumentData.Clone();

            //  if (_docType != null)
            //      newObj._docType = _docType.Clone();

            //  return newObj;

            using (var stream = new MemoryStream())
            {
                if (GetType().IsSerializable)
                {
                    var formatter = new BinaryFormatter();

                    formatter.Serialize(stream, this);

                    stream.Position = 0;

                    return (Document) formatter.Deserialize(stream);
                }

                return null;
            }

            /*
            var cloneDoc = (Document)MemberwiseClone();
            cloneDoc.DocumentData = DocumentData.Clone();

            if(_docType != null)
               cloneDoc._docType = _docType.Clone();

            // подписи не копируются - незачем.

            return cloneDoc;
            */
        }

        /// <summary>
        ///     Получение составного имени документа
        /// </summary>
        /// <returns>Имя документа</returns>
        public string GetFullDocumentName(Employee currentUser)
        {
            var docType = "";
            var dd = "";
            var lang = currentUser != null && !currentUser.Unavailable ? currentUser.Language : "";
            switch (lang)
            {
                case "ru":
                    docType = TypeDocRu;
                    dd = "от";
                    break;
                default:
                    docType = TypeDocEn;
                    dd = "dd";
                    break;
            }

            var docName = string.Format("{0}{1}{2}",
                string.IsNullOrEmpty(DocumentName) ? docType : DocumentName,
                string.IsNullOrEmpty(Number) ? string.Empty : string.Format(" № {0}", Number),
                Date == DateTime.MinValue ? string.Empty : string.Format(" {0} {1}", dd, Date.GetIndependenceDate())
            );

            return docName;
        }

        /// <summary>
        ///     Загрузить документ
        /// </summary>
        /// <param name="id">ID документа</param>
        /// <param name="extendedLoad">
        ///     Расширенная загрузка данных - подгружать данные из ДокументыДанные в класс DocumentData
        ///     Поумолчанию - true, если дополнительные данные не нужны выставляем false, тогда дополнительное
        ///     обращение к таблице ДокументыДанные не происходит
        /// </param>
        public void LoadDocument(string id, bool extendedLoad)
        {
            Id = id;
            DocumentData = extendedLoad ? new DocumentData(id) : new DocumentData();
            Load();
        }

        /// <summary>
        ///     Метод загрузки и заполнения данных сущности "Документ"
        /// </summary>
        public void FillData(int id)
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Документ, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    var colНазваниеДокумента = dbReader.GetOrdinal("НазваниеДокумента");
                    var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    var colОписание = dbReader.GetOrdinal("Описание");
                    var colКодИзображенияДокументаОсновного = dbReader.GetOrdinal("КодИзображенияДокументаОсновного");
                    var colНомерInt = dbReader.GetOrdinal("НомерInt");
                    var colНомерДокументаRL = dbReader.GetOrdinal("НомерДокументаRL");
                    var colНомерДокументаRLReverse = dbReader.GetOrdinal("НомерДокументаRLReverse");
                    var colЗащищен = dbReader.GetOrdinal("Защищен");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");
                    var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                    var colTypeDoc = dbReader.GetOrdinal("TypeDoc");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = id.ToString();
                        TypeId = dbReader.GetInt32(colКодТипаДокумента);
                        DocumentName = dbReader.GetString(colНазваниеДокумента);
                        Number = dbReader.GetString(colНомерДокумента);
                        if (!dbReader.IsDBNull(colДатаДокумента)) Date = dbReader.GetDateTime(colДатаДокумента);
                        Description = dbReader.GetString(colОписание);
                        if (!dbReader.IsDBNull(colКодИзображенияДокументаОсновного))
                            ImageCode = dbReader.GetInt32(colКодИзображенияДокументаОсновного);
                        if (!dbReader.IsDBNull(colНомерInt)) NumberInt = dbReader.GetInt32(colНомерInt);
                        NumberRL = dbReader.GetString(colНомерДокументаRL);
                        NumberRLReverse = dbReader.GetString(colНомерДокументаRLReverse);
                        Protected = dbReader.GetByte(colЗащищен);
                        ChangePersonID = dbReader.GetInt32(colИзменил);
                        Changed = dbReader.GetDateTime(colИзменено);
                        TypeDocRu = dbReader.GetString(colТипДокумента);
                        TypeDocEn = dbReader.GetString(colTypeDoc);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Получение списка документов из таблицы данных
        /// </summary>
        /// <param name="query">Запрос на множество строк Document</param>
        public static List<Document> GetDocumentsList(string query)
        {
            var docsList = new List<Document>();

            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    var colНазваниеДокумента = dbReader.GetOrdinal("НазваниеДокумента");
                    var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    var colОписание = dbReader.GetOrdinal("Описание");
                    var colКодИзображенияДокументаОсновного = dbReader.GetOrdinal("КодИзображенияДокументаОсновного");
                    var colНомерInt = dbReader.GetOrdinal("НомерInt");
                    var colНомерДокументаRL = dbReader.GetOrdinal("НомерДокументаRL");
                    var colНомерДокументаRLReverse = dbReader.GetOrdinal("НомерДокументаRLReverse");
                    var colЗащищен = dbReader.GetOrdinal("Защищен");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");
                    var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                    var colTypeDoc = dbReader.GetOrdinal("TypeDoc");

                    #endregion

                    while (dbReader.Read())
                    {
                        var docRow = new Document
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодДокумента).ToString(),
                            TypeId = dbReader.GetInt32(colКодТипаДокумента),
                            DocumentName = dbReader.GetString(colНазваниеДокумента),
                            Number = dbReader.GetString(colНомерДокумента),
                            Description = dbReader.GetString(colОписание),
                            NumberRL = dbReader.GetString(colНомерДокументаRL),
                            NumberRLReverse = dbReader.GetString(colНомерДокументаRLReverse),
                            Protected = dbReader.GetByte(colЗащищен),
                            ChangePersonID = dbReader.GetInt32(colИзменил),
                            Changed = dbReader.GetDateTime(colИзменено),
                            TypeDocRu = dbReader.GetString(colТипДокумента),
                            TypeDocEn = dbReader.GetString(colTypeDoc)
                        };

                        if (!dbReader.IsDBNull(colДатаДокумента)) docRow.Date = dbReader.GetDateTime(colДатаДокумента);
                        if (!dbReader.IsDBNull(colКодИзображенияДокументаОсновного))
                            docRow.ImageCode = dbReader.GetInt32(colКодИзображенияДокументаОсновного);
                        if (!dbReader.IsDBNull(colНомерInt)) docRow.NumberInt = dbReader.GetInt32(colНомерInt);

                        docsList.Add(docRow);
                    }
                }
            }

            return docsList;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Документ"
        /// </summary>
        public sealed override void Load()
        {
            if (!Id.IsNullEmptyOrZero())
                FillData(DocId);
        }

        /// <summary>
        ///     Получение количества похожих документов
        /// </summary>
        /// <param name="id">КодДокумента</param>
        /// <param name="type">КодТипаДокумента</param>
        /// <param name="number">НомерДокумента</param>
        /// <param name="date">ДатаДокумента</param>
        /// <param name="persons">КодыЛиц</param>
        /// <param name="equivCondition">УсловиеПохожести</param>
        /// <returns>Количество найденных похожих документов</returns>
        public static int SimilarCount(int id, int type, string number, DateTime date, string persons,
            byte equivCondition)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодТипаДокумента", type},
                {"@НомерДокумента", number},
                {"@ДатаДокумента", date},
                {"@КодыЛиц", persons},
                {"@КодДокумента", id},
                {"@УсловиеПохожести", equivCondition}
            };

            var dt = DBManager.GetData(SQLQueries.SP_ПохожиеДокументы, ConnString, CommandType.StoredProcedure,
                sqlParams);

            return dt.Rows.Count;
        }

        /// <summary>
        ///     Признак экспорта в 1С
        /// </summary>
        public bool Is1SExported()
        {
            var parameters = new Dictionary<string, object> {{"@id", DocId}};
            var result = (int) DBManager.ExecuteScalar(SQLQueries.SELECT_ID_ДокументЭкспортированВ1С, CommandType.Text,
                CN, parameters);

            return result == 1;
        }

        /// <summary>
        ///     Проверка на изменение документа
        /// </summary>
        /// <param name="hasFinishSign"></param>
        /// <param name="hasSign"></param>
        /// <param name="id"></param>
        /// <param name="dt"></param>
        /// <param name="usr"></param>
        /// <exception cref="DetailedException"></exception>
        public static void CheckForChanged(int id, ref DateTime dt, out bool hasFinishSign, out bool hasSign,
            out int usr)
        {
            hasFinishSign = false;
            hasSign = false;
            usr = 0;

            var query = string.Format(
                " SELECT TOP 1 Дата, КодСотрудника FROM ПодписиДокументов (nolock) WHERE КодИзображенияДокумента IS NULL AND ТипПодписи=1 AND КодДокумента={0} ORDER BY КодПодписиДокумента DESC" +
                " SELECT TOP 1 Дата, КодСотрудника FROM ПодписиДокументов (nolock) WHERE КодИзображенияДокумента IS NULL AND ТипПодписи=0 AND КодДокумента={0} ORDER BY КодПодписиДокумента DESC" +
                " SELECT Изменено, Изменил FROM vwДокументыДанные (nolock) WHERE КодДокумента={0} ", id);

            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.Read())
                {
                    hasFinishSign = true;
                    dt = dbReader.GetDateTime(0);
                    usr = dbReader.GetInt32(1);
                }
                else if (dbReader.NextResult())
                {
                    if (dbReader.Read())
                    {
                        hasSign = true;
                        dt = dbReader.GetDateTime(0);
                        usr = dbReader.GetInt32(1);
                    }
                }
                else if (dbReader.NextResult())
                {
                    dt = dbReader.GetDateTime(0);
                    usr = dbReader.GetInt32(1);
                }
            }
        }

        /// <summary>
        ///     Сохраняем текущий документ,
        ///     если новый то Insert
        ///     если существующий то Update
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            if (IsNew)
                Create(cmds);
            else
                UpdateData(cmds);

            try
            {
                SaveBaseDocs(cmds);
            }
            catch (Exception ex)
            {
                // возвращаем в первоначальнее состояние
                if (IsNew)
                    DeleteEForm(cmds);

                var dex = new DetailedException(ex.Message, ex);
                Logger.WriteEx(dex);
                throw dex;
            }
        }

        /// <summary>
        ///     Метод создания сущности документ(insert)
        /// </summary>
        public void Create(List<DBCommand> cmds)
        {
            SetDocumentNumber();

            var sqlParams = SetSqlParams(true);
            var outputParams = new Dictionary<string, object> {{"@КодДокумента", -1}};
            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Создание нового документа с электронной формой",
                    Text = SQLQueries.SP_ДокументыДанные_InsUpd,
                    Type = CommandType.StoredProcedure,
                    ConnectionString = CN,
                    ParamsIn = sqlParams,
                    ParamsOut = outputParams
                });
                return;
            }

            DBManager.ExecuteNonQuery(SQLQueries.SP_ДокументыДанные_InsUpd, CommandType.StoredProcedure, CN, sqlParams,
                outputParams);
            Id = outputParams["@КодДокумента"].ToString();
            Unavailable = string.IsNullOrEmpty(Id);
        }

        /// <summary>
        ///     Метод создания сущности документ(update)
        /// </summary>
        public void UpdateData(List<DBCommand> cmds)
        {
            var sqlParams = SetSqlParams(false);

            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Обновление данных документа",
                    Text = SQLQueries.SP_ДокументыДанные_InsUpd,
                    Type = CommandType.StoredProcedure,
                    ConnectionString = CN,
                    ParamsIn = sqlParams,
                    ParamsOut = null
                });
                return;
            }

            DBManager.ExecuteNonQuery(SQLQueries.SP_ДокументыДанные_InsUpd, CommandType.StoredProcedure, CN, sqlParams);
        }

        /// <summary>
        ///     Метод удаление документа
        /// </summary>
        /// <remarks>Удаляется электронная форма, если отсутствуют картинки, то запускается делитер и удаляет сам документ</remarks>
        public void DeleteEForm(ICollection<DBCommand> cmds = null)
        {
            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Удаление электронной формы документа",
                    Text = SQLQueries.DELETE_ID_ДокументДанные,
                    EntityId = DocId.ToString(),
                    Type = CommandType.Text,
                    ConnectionString = ConnString,
                    ParamsIn = null,
                    ParamsOut = null
                });
                return;
            }

            if (DocId == 0) return;
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_ДокументДанные, DocId, CommandType.Text, ConnString);
        }

        /// <summary>
        ///     Устанавливает номер документа если это необходимо
        /// </summary>
        private void SetDocumentNumber()
        {
            if (GenerateNumber)
                Number = GenerateDocumentNumber();
        }

        /// <summary>
        ///     Сгенирировать номер документа
        /// </summary>
        public string GenerateDocumentNumber()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодТипаДокумента", DocType.DocTypeId},
                {"@ДатаДокумента", Date},
                {"@КодЛица1", DocumentData.PersonId1},
                {"@КодЛица2", DocumentData.PersonId2},
                {"@КодЛица5", DocumentData.PersonId5}
            };

            var outPutparams = new Dictionary<string, object>();

            DBManager.ExecuteNonQuery(SQLQueries.SP_СозданиеНомераДокумента, CommandType.StoredProcedure, ConnString,
                sqlParams, outPutparams);

            var result = outPutparams["@RETURN_VALUE"];


            return result != null ? result.ToString() : "";
        }

        /// <summary>
        ///     Установка параметров для записи в БД
        /// </summary>
        /// <param name="isNew">флаг true - сохраняем новое, false - обнавляем</param>
        /// <param name="addToWork">Значение параметра ДобавитьВРаботу, по умолчанию null(в базе по умочанию 1)</param>
        /// <returns>Коллекция параметров - значений</returns>
        private Dictionary<string, object> SetSqlParams(bool isNew, byte? addToWork = null)
        {
            var sqlParams = new Dictionary<string, object>(100);

            if (!isNew)
                sqlParams.Add("@КодДокумента", DocId);

            sqlParams.Add("@КодТипаДокумента", TypeId);
            //sqlParams.Add("@КодТипаДокументаДанных", );
            sqlParams.Add("@НазваниеДокумента", string.IsNullOrWhiteSpace(DocumentName) ? "" : DocumentName);
            sqlParams.Add("@НомерДокумента", string.IsNullOrWhiteSpace(Number) ? "" : Number);
            sqlParams.Add("@ДатаДокумента", Date == DateTime.MinValue ? null : Date.ToString("yyyyMMdd"));
            sqlParams.Add("@Описание", string.IsNullOrWhiteSpace(Description) ? "" : Description);

            if (DocumentData != null)
            {
                sqlParams.Add("@КодЛица1", DocumentData.PersonId1);
                sqlParams.Add("@КодЛица2", DocumentData.PersonId2);
                sqlParams.Add("@КодЛица3", DocumentData.PersonId3);
                sqlParams.Add("@КодЛица4", DocumentData.PersonId4);
                sqlParams.Add("@КодЛица5", DocumentData.PersonId5);
                sqlParams.Add("@КодЛица6", DocumentData.PersonId6);
                sqlParams.Add("@КодСклада1", DocumentData.StockId1);
                sqlParams.Add("@КодСклада2", DocumentData.StockId2);
                sqlParams.Add("@КодСклада3", DocumentData.StockId3);
                sqlParams.Add("@КодСклада4", DocumentData.StockId4);
                sqlParams.Add("@КодРесурса1", DocumentData.ResourceId1);
                sqlParams.Add("@КодРесурса2", DocumentData.ResourceId2);
                sqlParams.Add("@КодСотрудника1", DocumentData.EmployeeId1);
                sqlParams.Add("@КодСотрудника2", DocumentData.EmployeeId2);
                sqlParams.Add("@КодСотрудника3", DocumentData.EmployeeId3);
                sqlParams.Add("@КодРасположения1", DocumentData.LocationId1);
                sqlParams.Add("@КодРасположения2", DocumentData.LocationId2);
                sqlParams.Add("@КодБазисаПоставки", DocumentData.BaseDeliveryId);
                sqlParams.Add("@КодВидаТранспорта", DocumentData.TransportTypeId);
                sqlParams.Add("@КодМестаХранения", DocumentData.StorageLocationId);
                sqlParams.Add("@КодЕдиницыИзмерения", DocumentData.UnitsId);
                sqlParams.Add("@КодСтавкиНДС", DocumentData.NDSrateId);
                sqlParams.Add("@КодТУзла1", DocumentData.UzelId1);
                sqlParams.Add("@КодТУзла2", DocumentData.UzelId2);
                sqlParams.Add("@КодТерритории", DocumentData.TerritoryId);
                sqlParams.Add("@КодСтатьиБюджета", DocumentData.BudgetId);
                sqlParams.Add("@Дата2", string.IsNullOrEmpty(DocumentData._Date2) ? null : DocumentData._Date2);
                sqlParams.Add("@Дата3", string.IsNullOrEmpty(DocumentData._Date3) ? null : DocumentData._Date3);
                sqlParams.Add("@Дата4", string.IsNullOrEmpty(DocumentData._Date4) ? null : DocumentData._Date4);
                sqlParams.Add("@Дата5", string.IsNullOrEmpty(DocumentData._Date5) ? null : DocumentData._Date5);
                sqlParams.Add("@Flag1", DocumentData.Flag1);
                sqlParams.Add("@Flag2", DocumentData.Flag2);
                sqlParams.Add("@Int1", DocumentData.Int1);
                sqlParams.Add("@Int2", DocumentData.Int2);
                sqlParams.Add("@Int3", DocumentData.Int3);
                sqlParams.Add("@Int4", DocumentData.Int4);
                sqlParams.Add("@Int5", DocumentData.Int5);
                sqlParams.Add("@Int6", DocumentData.Int6);
                sqlParams.Add("@Int7", DocumentData.Int7);
                sqlParams.Add("@Text50_1", DocumentData.Text50_1 ?? "");
                sqlParams.Add("@Text50_2", DocumentData.Text50_2 ?? "");
                sqlParams.Add("@Text50_3", DocumentData.Text50_3 ?? "");
                sqlParams.Add("@Text50_4", DocumentData.Text50_4 ?? "");
                sqlParams.Add("@Text50_5", DocumentData.Text50_5 ?? "");
                sqlParams.Add("@Text50_6", DocumentData.Text50_6 ?? "");
                sqlParams.Add("@Text50_7", DocumentData.Text50_7 ?? "");
                sqlParams.Add("@Text50_8", DocumentData.Text50_8 ?? "");
                sqlParams.Add("@Text50_9", DocumentData.Text50_9 ?? "");
                sqlParams.Add("@Text50_10", DocumentData.Text50_10 ?? "");
                sqlParams.Add("@Text50_11", DocumentData.Text50_11 ?? "");
                sqlParams.Add("@Text50_12", DocumentData.Text50_12 ?? "");
                sqlParams.Add("@Text50_13", DocumentData.Text50_13 ?? "");
                sqlParams.Add("@Text100_1", DocumentData.Text100_1 ?? "");
                sqlParams.Add("@Text100_2", DocumentData.Text100_2 ?? "");
                sqlParams.Add("@Text100_3", DocumentData.Text100_3 ?? "");
                sqlParams.Add("@Text100_4", DocumentData.Text100_4 ?? "");
                sqlParams.Add("@Text100_5", DocumentData.Text100_5 ?? "");
                sqlParams.Add("@Text100_6", DocumentData.Text100_6 ?? "");
                sqlParams.Add("@Text300_1", DocumentData.Text300_1 ?? "");
                sqlParams.Add("@Text300_2", DocumentData.Text300_2 ?? "");
                sqlParams.Add("@Text300_3", DocumentData.Text300_3 ?? "");
                sqlParams.Add("@Text300_4", DocumentData.Text300_4 ?? "");
                sqlParams.Add("@Text300_5", DocumentData.Text300_5 ?? "");
                sqlParams.Add("@Text300_6", DocumentData.Text300_6 ?? "");
                sqlParams.Add("@Text300_7", DocumentData.Text300_7 ?? "");
                sqlParams.Add("@Text300_8", DocumentData.Text300_8 ?? "");
                sqlParams.Add("@Text300_9", DocumentData.Text300_9 ?? "");
                sqlParams.Add("@Text1000_1", DocumentData.Text1000_1 ?? "");
                sqlParams.Add("@Text1000_2", DocumentData.Text1000_2 ?? "");
                sqlParams.Add("@Money1", DocumentData.Money1);
                sqlParams.Add("@Money2", DocumentData.Money2);
                sqlParams.Add("@Money3", DocumentData.Money3);
                sqlParams.Add("@Money4", DocumentData.Money4);
                sqlParams.Add("@Money5", DocumentData.Money5);
                sqlParams.Add("@Money6", DocumentData.Money6);
                sqlParams.Add("@Money7", DocumentData.Money7);
                sqlParams.Add("@Money8", DocumentData.Money8);
                sqlParams.Add("@Money9", DocumentData.Money9);
                sqlParams.Add("@Decimal1", DocumentData.Decimal1);
                sqlParams.Add("@Decimal2", DocumentData.Decimal2);
                sqlParams.Add("@Float1", DocumentData.Float1);
                sqlParams.Add("@ТекстДокумента", DocumentData.DocumentText ?? "");
            }

            if (addToWork != null)
                sqlParams.Add("@ДобавитьВРаботу", addToWork);

            return sqlParams;
        }

        /// <summary>
        ///     Сравнивает первоначальное состояние документа(original) и текущую версию
        /// </summary>
        /// <returns>
        ///     true - документ отличается,
        ///     false - документ не изменялся
        /// </returns>
        public virtual bool CompareToChanges(Document original)
        {
            if (original == null)
                return true;

            if (TypeId != original.TypeId)
                return true;

            if (DocumentName != original.DocumentName)
                return true;

            if (Number != original.Number)
                return true;

            if (Date != original.Date)
                return true;

            if (Description != original.Description)
                return true;

            if (ImageCode != original.ImageCode)
                return true;

            if (NumberInt != original.NumberInt)
                return true;

            if (NumberRL != original.NumberRL)
                return true;

            if (NumberRLReverse != original.NumberRLReverse)
                return true;

            if (Protected != original.Protected)
                return true;

            if (DocumentData.CompareToChanges(original.DocumentData))
                return true;

            if (!BaseDocsComparer(original.BaseDocsLinks))
                return true;

            // если дошло до конца, значит документ не менялся
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="guid"></param>
        public void ClearDeliveryTemporary(string guid)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@guid", guid}
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ОтправкаВагоновВыгрузка, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Получение иконокдокумента
        /// </summary>
        /// <param name="d"></param>
        /// <param name="col"></param>
        public static void FillAdvIcons(string d, NameValueCollection col)
        {
            if (string.IsNullOrEmpty(d) || col == null) return;

            try
            {
                var param = new Dictionary<string, object> {{"@КодДокумента", d}};
                var dt = DBManager.GetData("dbo.sp_СостояниеДокумента", Config.DS_document, CommandType.StoredProcedure,
                    param);
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    if (col.Get(dt.Rows[i][0].ToString()) != null) continue;
                    col.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new DetailedException("Ошибка при получении состояния документа", ex);
            }
        }

        /// <summary>
        ///     Метод получения даты последнего изменения
        /// </summary>
        public override DateTime GetLastChanged(string id)
        {
            var param = new Dictionary<string, object> {{"@Id", id}};
            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_Документ_LastChanged, CommandType.Text, CN, param);

            if (res is DateTime)
                return (DateTime) res;

            return DateTime.MinValue;
        }

        /// <summary>
        ///     Функция сохранения описания документа
        /// </summary>
        public void SaveDescription()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@Описание", string.IsNullOrWhiteSpace(Description) ? "" : Description);
            sqlParams.Add("@id", DocId);

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ID_Документы_Описание, CommandType.Text, CN, sqlParams);
        }

        #region Поля сущности "Документ"

        /// <summary>
        ///     ID. Поле КодДокумента
        /// </summary>
        /// <remarks>
        ///     Типизированный псевданим для ID
        /// </remarks>
        public int DocId => Id.ToInt();
        

        /// <summary>
        ///     Код типа документа
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        ///     Тип документа Enum
        /// </summary>
        /// <remarks>Название применяется для рефлексии, не менять</remarks>
        public DocTypeEnum Type
        {
            get { return (DocTypeEnum) TypeId; }
            set { TypeId = (int) value; }
        }

        /// <summary>
        ///     Название документа
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        ///     Номер документа
        /// </summary>
        public string Number
        {
            get { return NumberBind.Value; }
            set { NumberBind.Value = value; }
        }

        /// <summary>
        ///     Документ основание
        /// </summary>
        public BinderValue NumberBind = new BinderValue();

        /// <summary>
        ///     Дата документа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Описание
        /// </summary>
        public string Description
        {
            get { return DescriptionBinder.Value; }
            set { DescriptionBinder.Value = value; }
        }

        /// <summary>
        ///     Байдинг поля "Description"
        /// </summary>
        public BinderValue DescriptionBinder = new BinderValue();

        /// <summary>
        ///     Код изображения документа основного
        /// </summary>
        public int ImageCode { get; set; }

        /// <summary>
        ///     НомерInt
        /// </summary>
        public int NumberInt { get; set; }

        /// <summary>
        ///     НомерДокументаRL
        /// </summary>
        public string NumberRL { get; set; }

        /// <summary>
        ///     НомерДокументаRLReverse
        /// </summary>
        public string NumberRLReverse { get; set; }

        /// <summary>
        ///     Защищен
        /// </summary>
        public byte Protected { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int ChangePersonID { get; set; }

        /// <summary>
        ///     Дата изменения
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        ///     Язык документа
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Сущность "Документы данные"
        /// </summary>
        public DocumentData DocumentData { get; set; }

        /// <summary>
        ///     Доступность "Документы данные"
        /// </summary>
        /// <remarks>Недоступность</remarks>
        public bool DataUnavailable
        {
            get
            {
                if (DocumentData == null)
                    return true;

                return DocumentData.Unavailable;
            }
        }

        #endregion

        #region Подписи документа

        /// <summary>
        ///     Backing field для свойства docSigns
        /// </summary>
        private List<DocSign> _docSigns;

        /// <summary>
        ///     Коллекция всех подписей документа
        /// </summary>
        public List<DocSign> DocSigns
        {
            get
            {
                if (_docSigns == null) GetSignsFromDb();

                return _docSigns;
            }
        }

        /// <summary>
        ///     Обнуление подписи документа и присвоение новой ссылки
        /// </summary>
        public void DocSignsClear()
        {
            _docSigns = new List<DocSign>();
        }

        /// <summary>
        ///     Документ подписан, хотябы один раз
        /// </summary>
        public bool Signed
        {
            get
            {
                if (_docSigns == null) GetSignsFromDb();

                if (_docSigns == null || _docSigns.Count == 0)
                    return false;

                return _docSigns.Any(s => s.SignId > 0);
            }
        }

        /// <summary>
        ///     Последняя подпись завершено
        /// </summary>
        public bool Finished
        {
            get
            {
                if (_docSigns != null)
                    return _docSigns.Any(x => x.SignType == 1);

                return false;
            }
        }

        /// <summary>
        ///     Обновить и заново считать подписи документа
        /// </summary>
        public void GetSignsFromDb()
        {
            if (DocId > 0 && Available)
                _docSigns = DocSign.GetSignsByDocumentId(DocId);
        }

        /// <summary>
        ///     Получить подписантов
        /// </summary>
        public DataTable GetDocSigners()
        {
            return GetDocSigners(1);
        }

        /// <summary>
        ///     Получить подписантов
        /// </summary>
        public DataTable GetDocSigners(int PersonIndex)
        {
            var PersonCondition = "КодЛица1=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0
                                      ? "-1"
                                      : DocumentData.PersonId1.ToString());

            switch (PersonIndex)
            {
                case 1:
                    PersonCondition = "КодЛица1=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0
                                          ? "-1"
                                          : DocumentData.PersonId1.ToString());
                    break;
                case 2:
                    PersonCondition = "КодЛица2=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0
                                          ? "-1"
                                          : DocumentData.PersonId2.ToString());
                    break;
            }

            var query = string.Format(@"
SELECT TOP 1	Text50_1, Text50_2, Text50_3, Text50_4, Text50_11, Text50_12,
				Text100_1, Text100_2, Text100_3, Text100_4,Text100_5,Text100_6,
				КодЛица3, КодЛица4
FROM vwДокументыДокументыДанные (nolock)
WHERE {0} AND КодТипаДокумента={1} AND КодДокумента IS NOT NULL
ORDER BY Изменено DESC
", PersonCondition, TypeId);

            return DBManager.GetData(query, ConnString);
        }

        #endregion

        #region Тип документа

        /// <summary>
        ///     ТипДокумента
        /// </summary>
        public DocType DocType
        {
            get
            {
                if (_docType == null && TypeId > 0)
                    _docType = new DocType(TypeId.ToString());

                return _docType;
            }
        }

        /// <summary>
        ///     Backing field для свойства DocType
        /// </summary>
        private DocType _docType;

        #endregion

        #region Поля документа

        /// <summary>
        ///     Поля документа с ключем по ID
        /// </summary>
        public Dictionary<string, DocField> Fields
        {
            get
            {
                if (_fields == null && TypeId > 0) _fields = DocField.GetDocFieldsByDocTypeId(TypeId, this);

                return _fields;
            }
        }

        /// <summary>
        ///     Получает поле документа по ключу(таблица ПоляДокументов)
        /// </summary>
        public DocField GetDocField(string key)
        {
            var fields = Fields;
            if (fields != null)
                if (fields.ContainsKey(key))
                    return fields[key];

            // сюда доходит с случае ошибок программиста или баз данных
            if (fields == null)
                throw new MemberAccessException("Не удалось получить коллекцию полей документа по id = " + TypeId);
            throw new ArgumentOutOfRangeException(key, @"Не удалось получить значение поля документа по ключу: " + key);
        }

        /// <summary>
        ///     Backing field для свойства Fields
        /// </summary>
        private Dictionary<string, DocField> _fields;

        #endregion

        #region Связи документа

        /// <summary>
        ///     Получить связи документа для контрола select
        /// </summary>
        public List<Item> GetDocLinksItems(int fieldId, int? copyId = null)
        {
            var docId = copyId ?? DocId;
            if (docId > 0)
            {
                var sqlQuery = SQLQueries.SELECT_ВсеОснования + docId + " AND vwСвязиДокументов.КодПоляДокумента =" +
                               fieldId;

                var docs = GetDocumentsList(sqlQuery);
                if (docs != null) return docs.Select(d => new Item {Id = d.Id, Value = d}).ToList();
            }

            return new List<Item>();
        }

        /// <summary>
        ///     Получить документы основания
        /// </summary>
        /// <param name="fieldId">Id поля</param>
        public List<Document> GetBaseDocs(int fieldId)
        {
            var sqlQuery = SQLQueries.SELECT_ВсеОснования + DocId + " AND vwСвязиДокументов.КодПоляДокумента =" +
                           fieldId;

            var baseDoc = GetDocumentsList(sqlQuery);

            return baseDoc ?? new List<Document>();
        }

        /// <summary>
        ///     Получить все базовые документы
        /// </summary>
        /// <returns></returns>
        public List<Document> GetBaseDocsAll()
        {
            var sqlQuery = SQLQueries.SELECT_ВсеОснования + DocId;

            return GetDocumentsList(sqlQuery);
        }

        /// <summary>
        ///     Получить документ основание
        /// </summary>
        public string GetBaseDoc(int fieldId)
        {
            //var baseDocs = GetBaseDocs(fieldId);
            //return _document = baseDocs.FirstOrDefault() ?? new Document();
            var baseDoc = BaseDocsLinks.FirstOrDefault(b => b.DocFieldId == fieldId);
            return baseDoc == null ? "" : baseDoc.BaseDocId.ToString();
        }

        /// <summary>
        ///     Получить коллекцию документов
        /// </summary>
        public List<string> GetBaseDocs(int? fieldId)
        {
            //var baseDocs = GetBaseDocs(fieldId);
            //return _document = baseDocs.FirstOrDefault() ?? new Document();
            var baseDoc = BaseDocsLinks.Where(w => w.DocFieldId == fieldId).Select(b => b.BaseDocId.ToString())
                .ToList();
            return baseDoc;
        }

        /// <summary>
        ///     Установить или изменить документ основание
        /// </summary>
        public void SetBaseDoc(int fieldId, int baseDocId)
        {
            //if (_document == null)
            //    GetBaseDoc(fieldId);

            //if (_document == null)
            //{
            //    var link = new DocLink {ParentDocId = baseDocId, ChildDocId = DocId, DocFieldId = fieldId};
            //    link.Create();
            //    return;
            //}

            //// если изменился удаляем и создаем новый, если нет - ничего не далаем
            //if (_document.DocId != baseDocId)
            //{
            //    DocLink.Delete(_document.DocId, DocId, fieldId);
            //    var link = new DocLink { ParentDocId = baseDocId, ChildDocId = DocId, DocFieldId = fieldId };
            //    link.Create();
            //}

            AddBaseDoc(baseDocId.ToString(), fieldId);
        }

        /// <summary>
        ///     Добавить базовый документ
        /// </summary>
        public void AddBaseDoc(string baseDocId, int fieldId)
        {
            //var docs = GetBaseDocs(fieldId);
            //if (!docs.Exists(d => d.Id == baseDocId))
            //{
            //    var link = new DocLink { ParentDocId = baseDocId.ToInt(), ChildDocId = DocId, DocFieldId = fieldId };
            //    link.Create();
            //}

            if (baseDocId.IsNullEmptyOrZero())
            {
                RemoveAllBaseDocs(fieldId);
                return;
            }

            if (!BaseDocsLinks.Exists(d => d.BaseDocId == baseDocId.ToInt() && d.DocFieldId == fieldId))
            {
                var link = new DocLink {BaseDocId = baseDocId.ToInt(), SequelDocId = DocId, DocFieldId = fieldId};
                BaseDocsLinks.Add(link);
            }
        }

        /// <summary>
        ///     Удалить все базовые документы по полю
        /// </summary>
        /// <param name="fieldId"></param>
        public void RemoveAllBaseDocs(int fieldId)
        {
            BaseDocsLinks.RemoveAll(b => b.DocFieldId == fieldId);
        }

        /// <summary>
        ///     Удалить базовый документ
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="fieldId"></param>
        public void RemoveBaseDoc(int docId, int fieldId)
        {
            BaseDocsLinks.RemoveAll(b => b.BaseDocId == docId && b.DocFieldId == fieldId);
        }

        private List<DocLink> _baseDocsLinks;
        private List<DocSequels> _sequelDocs;

        /// <summary>
        ///     Базовые документы
        /// </summary>
        public List<DocLink> BaseDocsLinks =>
            _baseDocsLinks ?? (_baseDocsLinks = DocLink.LoadBasisDocsByChildId(DocId));

        /// <summary>
        ///     Получение списка документов оснований из БД
        /// </summary>
        public void RefreshBaseDocs(int docId = 0)
        {
            _baseDocsLinks = DocLink.LoadBasisDocsByChildId(docId == 0 ? DocId : docId);
        }

        private bool BaseDocsComparer(List<DocLink> originalList)
        {
            if ((_baseDocsLinks == null || _baseDocsLinks.Count == 0) &&
                (originalList == null || originalList.Count == 0))
                return true;

            if (_baseDocsLinks == null && originalList != null || _baseDocsLinks != null && originalList == null)
                return false;

            var ret = true;
            if (_baseDocsLinks != null)
                _baseDocsLinks.ForEach(bd =>
                {
                    if (originalList == null || !ret || bd.DocFieldId == 0) return;
                    var obj = originalList.FirstOrDefault(x =>
                        x.DocFieldId == bd.DocFieldId && x.SequelDocId == bd.SequelDocId &&
                        x.BaseDocId == bd.BaseDocId);
                    if (obj == null) ret = false;
                });

            if (!ret) return false;

            if (originalList != null)
                originalList.ForEach(bd =>
                {
                    if (_baseDocsLinks == null || !ret || bd.DocFieldId == 0) return;
                    var obj = _baseDocsLinks.FirstOrDefault(x =>
                        x.DocFieldId == bd.DocFieldId && x.SequelDocId == bd.SequelDocId &&
                        x.BaseDocId == bd.BaseDocId);
                    if (obj == null) ret = false;
                });

            return ret;
        }


        /// <summary>
        ///     Сохранение базовых документов
        /// </summary>
        private void SaveBaseDocs(List<DBCommand> cmds = null)
        {
            if (_baseDocsLinks == null) return;

            var origBaseDocs = DocLink.LoadBasisDocsByChildId(DocId);

            //докуметы после удаления(оставшиеся)
            var origDelDocs = new List<DocLink>(origBaseDocs.Count);

            // сохраняем документы в случае отката
            var delLinks = new Stack<DocLink>(origBaseDocs.Count);
            var newLinks = new Stack<DocLink>(origBaseDocs.Count);

            try
            {
                // удаление
                foreach (var o in origBaseDocs)
                {
                    // если в новой "версии" такого нет, то удаляем из базы
                    if (!_baseDocsLinks.Exists(b => b.Id == o.Id))
                    {
                        o.Delete(cmds);
                        delLinks.Push(o.Clone());
                        continue;
                    }

                    origDelDocs.Add(o);
                }
            }
            catch (Exception e)
            {
                // возвращаем в первоначальнее состояние
                while (delLinks.Count > 0)
                {
                    var d = delLinks.Pop();
                    d.Create(cmds);
                }

                var dex = new DetailedException(e.Message, e);
                Logger.WriteEx(dex);
                throw dex;
            }

            try
            {
                // добавляем
                foreach (var i in _baseDocsLinks)
                    // Если в базе такого нет, то добавляем
                    if (!origDelDocs.Exists(o => o.DocFieldId == i.DocFieldId && o.BaseDocId == i.BaseDocId))
                    {
                        i.SequelDocId = DocId;
                        i.Create(cmds);
                        newLinks.Push(i.Clone());
                    }
            }
            catch (Exception e)
            {
                // возвращаем в первоначальнее состояние
                while (delLinks.Count > 0)
                {
                    var d = delLinks.Pop();
                    d.Create(cmds);
                }

                while (newLinks.Count > 0)
                {
                    var i = newLinks.Pop();
                    i.Delete(cmds);
                }

                var dex = new DetailedException(e.Message, e);
                Logger.WriteEx(dex);
                throw dex;
            }
        }

        /// <summary>
        ///     Получить вытекающие связи документа для контрола select
        /// </summary>
        public List<Item> GetSequelItems(int fieldId = 0)
        {
            if (DocId <= 0) return new List<Item>();

            var sds = GetSequelDocs(fieldId);
            var items = new List<Item>();

            items.AddRange(sds.Select(d => new Item {Id = d.Id, Value = d}));

            return items;
        }

        /// <summary>
        ///     Получить все вытекающие документы
        /// </summary>
        /// <param name="fieldId">вызов без параметра получаем все вытекающие</param>
        public List<Document> GetSequelDocs(int fieldId = 0)
        {
            if (DocId <= 0) return new List<Document>();

            var cacheKey = $"document._sequelDocs_{fieldId}";

            DocSequels sd;

            if (LoadedExternalProperties.ContainsKey(cacheKey))
            {
                sd = _sequelDocs.FirstOrDefault(x => x.FieldId == fieldId);
                return sd?.Documents;
            }

            if (_sequelDocs == null)
            {
                sd = new DocSequels
                {
                    FieldId = fieldId,
                    Documents = GetDocumentsList(SQLQueries.SELECT_ВсеВытекающие(DocId, fieldId))
                };
                _sequelDocs = new List<DocSequels> {sd};
            }
            else
            {
                sd = _sequelDocs.FirstOrDefault(x => x.FieldId == fieldId);
                if (sd == null)
                {
                    sd = new DocSequels
                    {
                        FieldId = fieldId,
                        Documents = GetDocumentsList(SQLQueries.SELECT_ВсеВытекающие(DocId, fieldId))
                    };
                    _sequelDocs.Add(sd);
                }
                else
                    sd.Documents = GetDocumentsList(SQLQueries.SELECT_ВсеВытекающие(DocId, fieldId));
            }


            AddLoadedExternalProperties(cacheKey);
            return sd.Documents;
        }

        /// <summary>
        /// Загурзить все вытекающие документы
        /// </summary>
        /// <param name="typeId">Код типа документа</param>
        /// <param name="fieldId">Код поля документа</param>
        /// <param name="docId">КодДокумента</param>
        /// <returns></returns>
        public static DataTable LoadSequelDocs(string typeId, string fieldId, string docId)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодДокументаОснования", int.Parse(docId)},
                { "@КодТипаДокумента", typeId.Length > 0 ? int.Parse(typeId) : -1},
                { "@КодПоляДокумента", fieldId.Length > 0 ? int.Parse(fieldId) : -1}
            };

            return DBManager.GetData(SQLQueries.SELECT_ВсеВытекающие_ПоПолю_ПоТипу,ConnString, CommandType.Text, sqlParams);
        }

        /// <summary>
        ///     Загрузить все вытекающие документы
        /// </summary>
        public static DataTable LoadSequelDocs(string docId)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодДокументаОснования", int.Parse(docId)},
                { "@КодТипаДокумента",  -1},
                { "@КодПоляДокумента",  -1}
            };
            return DBManager.GetData(SQLQueries.SELECT_ВсеВытекающие_ПоПолю_ПоТипу, ConnString, CommandType.Text, sqlParams);
        }

        /// <summary>
        /// Загрузить данные по последним отметкам транспортного узла
        /// </summary>
        /// <param name="trWeselId">Код транспортного узла</param>
        /// <param name="typeId">Код типа документа</param>
        /// <param name="docId">Код документа</param>
        /// <param name="docDate">Дата документа</param>
        /// <returns>Отметка транспортного узла</returns>
        public static string GetTrWeselLastNote(string trWeselId, string typeId, int docId, DateTime docDate)
        {

            var sqlParams = new Dictionary<string, object>
            {
                { "@КодТУзла", int.Parse(trWeselId)},
                { "@КодТипаДокумента", int.Parse(typeId)},
                { "@ДатаДокумента", docDate},
                { "@КодДокумента", docId}
            };
            
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_ПоследниеОтметкиТранспортногоУзла, CommandType.Text, Config.DS_document, sqlParams);
            return result == null ? "" : result.ToString();
        }

        /// <summary>
        ///     Получить количество вытекающих документов по коду поля и коду документа
        /// </summary>
        /// <param name="docId">Код документа вытекающего</param>
        /// <param name="fieId">Код поля документа</param>
        /// <returns></returns>
        public static bool CheckExistsBasisDocs(string docId, string fieId)
        {
            var sqlParams = new Dictionary<string, object>
                {
                    { "@КодПоляДокумента", int.Parse(fieId)},
                    { "@КодДокументаВытекающего", int.Parse(docId)}};

            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_КоличествоДокументовОснований, CommandType.Text, Config.DS_document, sqlParams);

            if (result == null)
                return false;

            int count;
            int.TryParse(result.ToString(), out count);

            return count > 0;
        }

        #endregion

        #region Прочее

        /// <summary>
        ///     Получить всех наследников класса Document
        /// </summary>
        public static List<Type> DocumentInheritClasses
        {
            get
            {
                if (_documentInheritClasses == null)
                {
                    var ourtype = typeof(Document); // Базовый тип
                    return _documentInheritClasses = Assembly.GetAssembly(ourtype).GetTypes()
                        .Where(type => type.IsSubclassOf(ourtype)).ToList();
                }

                return _documentInheritClasses;
            }
        }

        private static List<Type> _documentInheritClasses;

        /// <summary>
        ///     Обобщенный метод создания класса на основе его типа
        /// </summary>
        // пример: CreateConcrete<TTN>(typeof(TTN), "123")
        public static D CreateConcrete<D>(Type type, string id = "") where D : Document
        {
            if (id.IsNullEmptyOrZero())
                return (D) Activator.CreateInstance(type);

            return (D) Activator.CreateInstance(type, id);
        }

        /// <summary>
        ///     Получить индекс лица
        /// </summary>
        public int GetPersonIndex(string person)
        {
            var personInt = person.ToInt();
            if (IsNew) return -1;
            if (!DataUnavailable)
            {
                if (DocumentData.PersonId1.Equals(personInt)) return 1;
                if (DocumentData.PersonId2.Equals(personInt)) return 2;
                if (DocumentData.PersonId3.Equals(personInt)) return 3;
                if (DocumentData.PersonId4.Equals(personInt)) return 4;
                if (DocumentData.PersonId5.Equals(personInt)) return 5;
                if (DocumentData.PersonId6.Equals(personInt)) return 6;
            }

            var persons = DocPersons.GetDocsPersonsByDocId(DocId);
            if (persons.Any(p => p.Id == person)) return 0;
            return -1;
        }

        #endregion

        #region Валюта и ее точность

        private int _currencyScale = -1;

        /// <summary>
        ///     Точность вывода валют. Кэшируется как свойство документа (для предотвращения доп запросов по ресурсу).
        /// </summary>
        public int CurrencyScale
        {
            get
            {
                if (_currencyScale == -1)
                    _currencyScale = Currency != null && !Currency.Unavailable ? Currency.UnitScale : 2;

                return _currencyScale;
            }
        }

        /// <summary>
        ///     Валюта
        /// </summary>
        public virtual Currency Currency => null;

        #endregion

        #region Документ не по проектам холдинга

        /// <summary>
        ///     Документ не по проектам холдинга
        ///     Определяет по количеству лиц по коду документа
        /// </summary>
        /// <remarks>
        ///     Тяжелый запрос, проверяется
        /// </remarks>
        public bool IsNoBProject()
        {
            if (IsNew) return _isNoBizProject;
            // если код лица изменился то проверим, если нет, то старое значение
            if (!PersonChanged()) return _isNoBizProject;

            _personId1 = DocumentData.PersonId1;
            _personId2 = DocumentData.PersonId2;
            _personId3 = DocumentData.PersonId3;
            _personId4 = DocumentData.PersonId4;
            _personId5 = DocumentData.PersonId5;
            _personId6 = DocumentData.PersonId6;


            var sqlParams = new Dictionary<string, object> {{"@id", int.Parse(Id)}};
            // если вернет null значит не одной записи нет, если 1 то есть хотябы одна запись
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_ID_ЛицаДокументаПоБизнесПроекту, CommandType.Text,
                CN, sqlParams);

            return _isNoBizProject = result == null;
        }

        private int? _personId1;
        private int? _personId2;
        private int? _personId3;
        private int? _personId4;
        private int? _personId5;
        private int? _personId6;
        private bool _isNoBizProject;

        /// <summary>
        ///     Код лица изменился
        /// </summary>
        /// <remarks>Метод нужен для уменьшения кол-ва обращения к БД с тяжелым запросом</remarks>
        /// <returns>true - изменился</returns>
        private bool PersonChanged()
        {
            if (DocumentData.PersonId1 != _personId1)
                return true;
            if (DocumentData.PersonId2 != _personId2)
                return true;
            if (DocumentData.PersonId3 != _personId3)
                return true;
            if (DocumentData.PersonId4 != _personId4)
                return true;
            if (DocumentData.PersonId5 != _personId5)
                return true;
            if (DocumentData.PersonId6 != _personId6)
                return true;

            return false;
        }

        #endregion
    }
}