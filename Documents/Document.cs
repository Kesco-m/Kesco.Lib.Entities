using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс сущности "Документ"
    /// </summary>
    /// <example>
    /// Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Number = {Number}, Desc = {Description}")]
    public class Document : Entity, ICloneable<Document>
    {
        #region Поля сущности "Документ"

        /// <summary>
        ///  ID. Поле КодДокумента
        /// </summary>
        /// <remarks>
        ///  Типизированный псевданим для ID
        /// </remarks>
        public int DocId { get { return Id.ToInt(); } }

        /// <summary>
        ///  Признак нового документа
        /// </summary>
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(Id) || Id == "0"; }
        }

        /// <summary>
        /// Код типа документа
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        ///  Тип документа Enum
        /// </summary>
        /// <remarks>Название применяется для рефлексии, не менять</remarks>
        public  DocTypeEnum Type
        {
            get { return (DocTypeEnum) TypeID; }
            set { TypeID = (int) value; }
        }

        /// <summary>
        ///  Название документа
        /// </summary>
        public string DocumentName { get { return Name; } }

        /// <summary>
        /// Номер документа
        /// </summary>
        public string Number
        {
            get { return NumberBind.Value; }
            set { NumberBind.Value = value; }
        }

        /// <summary>
        ///  Документ основание
        /// </summary>
        public BinderValue NumberBind = new BinderValue();

        /// <summary>
        /// Дата документа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get { return DescriptionBinder.Value; }
            set { DescriptionBinder.Value = value; }
        }

        /// <summary>
        ///  Байдинг поля "Description"
        /// </summary>
        public BinderValue DescriptionBinder = new BinderValue();

        /// <summary>
        /// Код изображения документа основного
        /// </summary>
        public int ImageCode { get; set; }

        /// <summary>
        /// НомерInt
        /// </summary>
        public int NumberInt { get; set; }

        /// <summary>
        /// НомерДокументаRL
        /// </summary>
        public string NumberRL { get; set; }

        /// <summary>
        /// НомерДокументаRLReverse
        /// </summary>
        public string NumberRLReverse { get; set; }

        /// <summary>
        /// Защищен
        /// </summary>
        public byte Protected { get; set; }

        /// <summary>
        /// Изменил
        /// </summary>
        public int ChangePersonID { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// Тип документа Рус
        /// </summary>
       // public string DocTypeNameRus { get; set; }

        /// <summary>
        /// Тип документа Eng
        /// </summary>
       // public string DocTypeNameEng { get; set; }

        /// <summary>
        /// Язык документа
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///  Сущность "Документы данные"
        /// </summary>
        public DocumentData DocumentData { get; set; }

        /// <summary>
        ///  Доступность "Документы данные"
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

        /// <summary>
        /// Состоит ли номер документа полностью из цифр
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
        /// Доступность генерации номера документа
        /// </summary>
        public bool IsNumberGenerationAvailable
        {
            get { return DocType.NumberGenType != NumGenTypes.CanNotBeGenerated && IsNew; }
        }

        private bool _generateNumber;

        /// <summary>
        /// Доступность генерации номера документа
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

        #region Подписи документа

        /// <summary>
        ///  Backing field для свойства docSigns
        /// </summary>
        private List<DocSign> _docSigns;

        /// <summary>
        ///  Коллекция всех подписей документа
        /// </summary>
        public List<DocSign> DocSigns
        {
            get
            {
                if (_docSigns == null)
                {
                    GetSignsFromDb();
                }

                return _docSigns;
            }
        }

        /// <summary>
        ///  Обнуление подписи документа и присвоение новой ссылки
        /// </summary>
        public void DocSignsClear()
        {
            _docSigns = new List<DocSign>();
        }

        /// <summary>
        ///  Документ подписан, хотябы один раз
        /// </summary>
        public bool Signed
        {
            get
            {
                if (_docSigns == null)
                {
                    GetSignsFromDb();
                }

                if (_docSigns == null)
                    return false;

                return _docSigns.Any(s => s.SignId > 0);
            }
        }

        /// <summary>
        /// Последняя подпись завершено
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
        ///  Обновить и заново считать подписи документа
        /// </summary>
        public void GetSignsFromDb()
        {
            if (DocId > 0 && Available)
                _docSigns = DocSign.GetSignsByDocumentId(DocId);
        }

        /// <summary>
        ///  Получить подписантов 
        /// </summary>
        public DataTable GetDocSigners()
        {
            return GetDocSigners(1);
        }

        /// <summary>
        ///  Получить подписантов 
        /// </summary>
        public DataTable GetDocSigners(int PersonIndex)
        {
            string PersonCondition = "КодЛица1=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0 ? "-1" : DocumentData.PersonId1.ToString());

            switch (PersonIndex)
            {
                case 1:
                    PersonCondition = "КодЛица1=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0 ? "-1" : DocumentData.PersonId1.ToString());
                    break;
                case 2:
                    PersonCondition = "КодЛица2=" + (DocumentData.PersonId1 == null || DocumentData.PersonId1 == 0 ? "-1" : DocumentData.PersonId2.ToString());
                    break;
            }

            string query = string.Format(@"
SELECT TOP 1	Text50_1, Text50_2, Text50_3, Text50_4, Text50_11, Text50_12,
				Text100_1, Text100_2, Text100_3, Text100_4,Text100_5,Text100_6,
				КодЛица3, КодЛица4
FROM vwДокументыДокументыДанные (nolock)
WHERE {0} AND КодТипаДокумента={1} AND КодДокумента IS NOT NULL
ORDER BY Изменено DESC
", PersonCondition, TypeID);

            return DBManager.GetData(query, ConnString);
        }

        #endregion

        #region Тип документа
        ///<summary>
        /// ТипДокумента
        ///</summary>
        public DocType DocType
        {
            get
            {
                if (_docType == null && TypeID > 0)
                    _docType = new DocType(TypeID.ToString());

                return _docType;
            }
        }

        /// <summary>
        ///  Backing field для свойства DocType
        /// </summary>
        private DocType _docType;
        
        #endregion

        #region Поля документа
        /// <summary>
        ///  Поля документа с ключем по ID
        /// </summary>
        public Dictionary<string, DocField> Fields
        {
            get
            {
                if (_fields == null && TypeID > 0)
                {
                    _fields = DocField.GetDocFieldsByDocId(TypeID, this);
                }

                return _fields;
            }
        }

        /// <summary>
        ///  Получает поле документа по ключу(таблица ПоляДокументов)
        /// </summary>
        public DocField GetDocField(string key)
        {
            var fields = Fields;
            if(fields != null)
                if (fields.ContainsKey(key))
                    return fields[key];

            // сюда доходит с случае ошибок программиста или баз данных
            if(fields == null)
                throw new MemberAccessException("Не удалось получить коллекцию полей документа по id = " + TypeID);
            else
                throw new ArgumentOutOfRangeException(key, @"Не удалось получить значение поля документа по ключу: " + key);
        }

        /// <summary>
        ///  Backing field для свойства Fields
        /// </summary>
        private Dictionary<string, DocField> _fields;

        #endregion

        #region Связи документа


        /// <summary>
        ///  Получить связи документа для контрола select
        /// </summary>
        public List<Item> GetDocLinksItems(int fieldId)
        {
            if (DocId > 0)
            {
                var sqlQuery = SQLQueries.SELECT_ВсеОснования + DocId + " AND vwСвязиДокументов.КодПоляДокумента =" + fieldId; 

                var docs = GetDocumentsList(sqlQuery);
                if (docs != null)
                {
                    return docs.Select(d => new Item {Id = d.Id, Value = d}).ToList();
                }
            }
            return new List<Item>();
        }

        /// <summary>
        ///  Получить документы основания
        /// </summary>
        /// <param name="fieldId">Id поля</param>
        public List<Document> GetBaseDocs(int fieldId)
        {
            var sqlQuery = SQLQueries.SELECT_ВсеОснования + DocId + " AND vwСвязиДокументов.КодПоляДокумента =" + fieldId; 

            List<Document> baseDoc = GetDocumentsList(sqlQuery);

            return baseDoc ?? new List<Document>();
        }

        /// <summary>
        ///  Получить все базовые документы
        /// </summary>
        /// <returns></returns>
        public List<Document> GetBaseDocsAll()
        {
            var sqlQuery = SQLQueries.SELECT_ВсеОснования + DocId;

            return  GetDocumentsList(sqlQuery);
        }

        /// <summary>
        ///  Получить документ основание
        /// </summary>
        public string GetBaseDoc(int fieldId)
        {
            //var baseDocs = GetBaseDocs(fieldId);
            //return _document = baseDocs.FirstOrDefault() ?? new Document();
            var baseDoc = BaseDocs.FirstOrDefault(b => b.DocFieldId == fieldId);
            return baseDoc == null ? "" : baseDoc.BaseDocId.ToString();
        }

        /// <summary>
        ///  Получить коллекцию документов
        /// </summary>
        public List<string> GetBaseDocs(int? fieldId)
        {
            //var baseDocs = GetBaseDocs(fieldId);
            //return _document = baseDocs.FirstOrDefault() ?? new Document();
            var baseDoc = BaseDocs.Where(w => w.DocFieldId == fieldId).Select(b => b.BaseDocId.ToString()).ToList();
            return baseDoc;
        }

        /// <summary>
        ///  Установить или изменить документ основание
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
        ///  Добавить базовый документ
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

            if (!BaseDocs.Exists(d => d.BaseDocId == baseDocId.ToInt() && d.DocFieldId == fieldId))
            {
                var link = new DocLink { BaseDocId = baseDocId.ToInt(), SequelDocId = DocId, DocFieldId = fieldId };
                BaseDocs.Add(link);
            }
        }

        /// <summary>
        ///  Удалить все базовые документы по полю
        /// </summary>
        /// <param name="fieldId"></param>
        public void RemoveAllBaseDocs(int fieldId)
        {
            BaseDocs.RemoveAll(b => b.DocFieldId == fieldId);
        }

        /// <summary>
        ///  Удалить базовый документ
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="fieldId"></param>
        public void RemoveBaseDoc(int docId, int fieldId)
        {
            BaseDocs.RemoveAll(b => b.BaseDocId == docId && b.DocFieldId == fieldId);
        }
        
        /// <summary>
        ///  Базовые документы
        /// </summary>
        public List<DocLink> BaseDocs
        {
            get { return _baseDocs ?? (_baseDocs = DocLink.LoadBasisDocsByChildId(DocId)); }
        }

        private List<DocLink> _baseDocs;

        /// <summary>
        ///  Сохранение базовых документов
        /// </summary>
        private void SaveBaseDocs(List<DBCommand> cmds = null)
        {
            if(_baseDocs == null) return;

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
                    if (!_baseDocs.Exists(b => b.Id == o.Id))
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

                DetailedException dex = new DetailedException(e.Message, e);
                Logger.WriteEx(dex);
                throw dex;
            }

            try
            {
                // добавляем
                foreach (var i in _baseDocs)
                {
                    // Если в базе такого нет, то добавляем
                    if (!origDelDocs.Exists(o => o.DocFieldId == i.DocFieldId && o.BaseDocId == i.BaseDocId))
                    {
                        i.SequelDocId = DocId;
                        i.Create(cmds);
                        newLinks.Push(i.Clone());
                    }
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

                DetailedException dex = new DetailedException(e.Message, e);
                Logger.WriteEx(dex);
                throw dex;
            }
        }

        /// <summary>
        ///  Получить вытекающие связи документа для контрола select
        /// </summary>
        public List<Item> GetSequelItems(int fieldId = 0)
        {
            var items = new List<Item>();

            if (DocId > 0)
            {
                var query = SQLQueries.SELECT_ВсеВытекающие(DocId, fieldId);
                var docs = GetDocumentsList(query);
                if (docs != null)
                {
                    items.AddRange(docs.Select(d => new Item { Id = d.Id, Value = d }));
                }
            }

            return items;
        }

        /// <summary>
        ///  Получить все вытекающие документы
        /// </summary>
        /// <param name="fieldId">вызов без параметра получаем все вытекающие</param>
        public List<Document> GetSequelDocs(int fieldId = 0)
        {
            if (DocId > 0)
            {
                var query = SQLQueries.SELECT_ВсеВытекающие(DocId, fieldId);
                var docs = GetDocumentsList(query);

                return docs;
            }

            return new List<Document>();
        }

        /// <summary>
        ///  Загрузить вытекающие документы
        /// </summary>
        public static DataTable LoadSequelDocs(string _type, string _field, string docId)
        {
            string query = string.Format(@"
SELECT D._КодДокумента КодДокумента, D.ДатаДокумента, D.КодТипаДокумента, D.КодРесурса1,ISNULL(D.Money1,0) Money1
FROM vwСвязиДокументов S (nolock)
			INNER JOIN vwДокументыДокументыДанные D (nolock) ON S.КодДокументаВытекающего=D.КодДокумента
WHERE КодДокументаОснования={0} {1} {2}", docId,
    _type.Length > 0 ? string.Format(" AND КодТипаДокумента IN ({0})", _type) : "",
    _field.Length > 0 ? string.Format(" AND КодПоляДокумента IN ({0})", _field) : ""
);
            return DBManager.GetData(query, ConnString);
        }

        #endregion

        #region Прочее

        /// <summary>
        ///  Получить всех наследников класса Document
        /// </summary>
        public static List<Type> DocumentInheritClasses
        {
            get
            {
                if (_documentInheritClasses == null)
                {
                    Type ourtype = typeof(Document); // Базовый тип
                    return _documentInheritClasses = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype)).ToList();   
                }

                return _documentInheritClasses;
            }
        }

        private static List<Type> _documentInheritClasses;

        /// <summary>
        ///  Обобщенный метод создания класса на основе его типа
        /// </summary>
        // пример: CreateConcrete<TTN>(typeof(TTN), "123")
        public static D CreateConcrete<D>(Type type, string id = "") where D : Document
        {
            if(id.IsNullEmptyOrZero())
                return (D)Activator.CreateInstance(type);

            return (D)Activator.CreateInstance(type, id);
        }

        /// <summary>
        ///  Получить индекс лица
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
            //if (Regex.IsMatch(_Persons, "(^|,)" + person + "(,|$)")) return 0;
            return -1;
        }

        #endregion

        #region Валюта и ее точность

        private int _currencyScale = -1;

        /// <summary>
        /// Точность вывода валют. Кэшируется как свойство документа (для предотвращения доп запросов по ресурсу).
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
        ///  Валюта
        /// </summary>
        public virtual Resources.Currency Currency
        {
            get { return null; }
        }


        #endregion

        /// <summary>
        /// Составное имя документа
        /// </summary>
        public string FullDocName
        {
            get
            {
                return GetFullDocumentName(this);
            }
        }

        /// <summary>
        /// Получение составного имени документа
        /// </summary>
        /// <param name="doc">Документ</param>
        /// <returns>Имя документа</returns>
        public static string GetFullDocumentName(Document doc)
        {
            string docType = null;

            if (doc.DocType != null)
                switch (doc.Language)
                {
                    case "en":
                    case "et":
                        docType = doc.DocType.TypeDocEn;
                        break;
                    case "ru":
                        docType = doc.DocType.TypeDocRu;
                        break;
                    default:
                        docType = doc.DocType.TypeDocRu;
                        break;
                }

            string docName =
                    string.IsNullOrEmpty(docType) ?
                        doc.Name :
                        string.Format("{0}{1}{2}",
                                        docType,
                                        string.IsNullOrEmpty(doc.Number) ? string.Empty : string.Format(" № {0}", doc.Number),
                                        doc.Date == DateTime.MinValue ? string.Empty : string.Format(" от {0}", doc.Date.GetIndependenceDate())
                        );
            return docName; 
        }

        /// <summary>
        /// Инициализация объекта Документ по ID
        /// </summary>
        /// <param name="id">ID документа</param>
        /// <param name="extendedLoad">Расширенная загрузка данных - подгружать данные из ДокументыДанные в класс DocumentData
        ///                            Поумолчанию - true, если дополнительные данные не нужны выставляем false, тогда дополнительное
        ///                            обращение к таблице ДокументыДанные не происходит</param>
        public Document(string id, bool extendedLoad = true) : base(id)
        {
            LoadDocument(id, extendedLoad);
        }

        public Document(string id) : base(id)
        {
            LoadDocument(id, true);
        }

        /// <summary>
        ///  Загрузить документ
        /// </summary>
        /// <param name="id">ID документа</param>
        /// <param name="extendedLoad">Расширенная загрузка данных - подгружать данные из ДокументыДанные в класс DocumentData
        ///                            Поумолчанию - true, если дополнительные данные не нужны выставляем false, тогда дополнительное
        ///                            обращение к таблице ДокументыДанные не происходит</param>
        public void LoadDocument(string id, bool extendedLoad)
        {
            Id = id;
            DocumentData = extendedLoad ? new DocumentData(id) : new DocumentData();
            Load();
        }

        /// <summary>
        /// Инициализация объекта Документ
        /// </summary>
        public Document()
        {
            DocumentData = new DocumentData {Id = Id};
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        /// Метод загрузки и заполнения данных сущности "Документ"
        /// </summary>
        public void FillData(int id)
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Документ, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    int colНазваниеДокумента = dbReader.GetOrdinal("НазваниеДокумента");
                    int colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    int colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colКодИзображенияДокументаОсновного = dbReader.GetOrdinal("КодИзображенияДокументаОсновного");
                    int colНомерInt = dbReader.GetOrdinal("НомерInt");
                    int colНомерДокументаRL = dbReader.GetOrdinal("НомерДокументаRL");
                    int colНомерДокументаRLReverse = dbReader.GetOrdinal("НомерДокументаRLReverse");
                    int colЗащищен = dbReader.GetOrdinal("Защищен");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if(dbReader.Read())
                    {
                       Unavailable = false;
                       Id = id.ToString();
                       TypeID = dbReader.GetInt32(colКодТипаДокумента);
                       Name = dbReader.GetString(colНазваниеДокумента);
                       Number = dbReader.GetString(colНомерДокумента);
                       if (!dbReader.IsDBNull(colДатаДокумента)) { Date = dbReader.GetDateTime(colДатаДокумента); }
                       Description = dbReader.GetString(colОписание);
                       if (!dbReader.IsDBNull(colКодИзображенияДокументаОсновного)) { ImageCode = dbReader.GetInt32(colКодИзображенияДокументаОсновного); }
                       if (!dbReader.IsDBNull(colНомерInt)) { NumberInt = dbReader.GetInt32(colНомерInt); }
                       NumberRL = dbReader.GetString(colНомерДокументаRL);
                       NumberRLReverse = dbReader.GetString(colНомерДокументаRLReverse);
                       Protected = dbReader.GetByte(colЗащищен);
                       ChangePersonID = dbReader.GetInt32(colИзменил);
                       ChangeDate = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// Получение списка документов из таблицы данных
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

                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    int colНазваниеДокумента = dbReader.GetOrdinal("НазваниеДокумента");
                    int colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    int colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colКодИзображенияДокументаОсновного = dbReader.GetOrdinal("КодИзображенияДокументаОсновного");
                    int colНомерInt = dbReader.GetOrdinal("НомерInt");
                    int colНомерДокументаRL = dbReader.GetOrdinal("НомерДокументаRL");
                    int colНомерДокументаRLReverse = dbReader.GetOrdinal("НомерДокументаRLReverse");
                    int colЗащищен = dbReader.GetOrdinal("Защищен");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");
                    #endregion

                    while (dbReader.Read())
                    {
                        var docRow = new Document
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодДокумента).ToString(),
                            TypeID = dbReader.GetInt32(colКодТипаДокумента),
                            Name = dbReader.GetString(colНазваниеДокумента),
                            Number = dbReader.GetString(colНомерДокумента),
                            Description = dbReader.GetString(colОписание),
                            NumberRL = dbReader.GetString(colНомерДокументаRL),
                            NumberRLReverse = dbReader.GetString(colНомерДокументаRLReverse),
                            Protected = dbReader.GetByte(colЗащищен),
                            ChangePersonID = dbReader.GetInt32(colИзменил),
                            ChangeDate = dbReader.GetDateTime(colИзменено)
                        };

                        if (!dbReader.IsDBNull(colДатаДокумента)) { docRow.Date = dbReader.GetDateTime(colДатаДокумента); }
                        if (!dbReader.IsDBNull(colКодИзображенияДокументаОсновного)) { docRow.ImageCode = dbReader.GetInt32(colКодИзображенияДокументаОсновного); }
                        if (!dbReader.IsDBNull(colНомерInt)) { docRow.NumberInt = dbReader.GetInt32(colНомерInt); }

                        docsList.Add(docRow);
                    }
                }
            }

            return docsList;
        }

        /// <summary>
        /// Метод загрузки данных сущности "Документ"
        /// </summary>
        public sealed override void Load()
        {   
            if(!Id.IsNullEmptyOrZero())
                FillData(DocId);
        }

        /// <summary>
        /// Получение количества похожих документов
        /// </summary>
        /// <param name="id">КодДокумента</param>
        /// <param name="type">КодТипаДокумента</param>
        /// <param name="number">НомерДокумента</param>
        /// <param name="date">ДатаДокумента</param>
        /// <param name="persons">КодыЛиц</param>
        /// <param name="equivCondition">УсловиеПохожести</param>
        /// <returns>Количество найденных похожих документов</returns>
        public static int SimilarCount(int id, int type, string number, DateTime date, string persons, byte equivCondition)
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

            var dt = DBManager.GetData(SQLQueries.SP_ПохожиеДокументы, ConnString, CommandType.StoredProcedure, sqlParams);

            return dt.Rows.Count;
        }

        /// <summary>
        /// Признак экспорта в 1С
        /// </summary>
        public bool Is1SExported()
        {
            var parameters = new Dictionary<string, object> {{"@id", DocId}};
            var result = (int) DBManager.ExecuteScalar(SQLQueries.SELECT_ID_ДокументЭкспортированВ1С, CommandType.Text, CN, parameters);

            return result == 1;
        }

        /// <summary>
        ///  Проверка на изменение документа
        /// </summary>
        /// <param name="hasFinishSign"></param>
        /// <param name="hasSign"></param>
        /// <param name="id"></param>
        /// <param name="dt"></param>
        /// <param name="usr"></param>
        /// <exception cref="DetailedException"></exception>
        public static void CheckForChanged(int id, ref DateTime dt, out bool hasFinishSign, out bool hasSign, out int usr)
        {
            hasFinishSign = false;
            hasSign = false;
            usr = 0;

            var query = string.Format(" SELECT TOP 1 Дата, КодСотрудника FROM ПодписиДокументов (nolock) WHERE КодИзображенияДокумента IS NULL AND ТипПодписи=1 AND КодДокумента={0} ORDER BY КодПодписиДокумента DESC" +
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

        #region Документ не по проектам холдинга
        /// <summary>
        /// Документ не по проектам холдинга
        /// Определяет по количеству лиц по коду документа
        /// </summary>
        /// <remarks>
        ///  Тяжелый запрос, проверяется
        /// </remarks>
        public bool IsNoBProject()
        {
            // если код лица изменился то проверим, если нет, то старое значение
            if (PersonChanged())
            {
                _personId1 = DocumentData.PersonId1;
                _personId2 = DocumentData.PersonId2;
                _personId3 = DocumentData.PersonId3;
                _personId4 = DocumentData.PersonId4;
                _personId5 = DocumentData.PersonId5;
                _personId6 = DocumentData.PersonId6;

                // запрос по сути как EXIST если есть, то возвращает 1
                string sql = String.Format(@"
                SELECT 1 FROM vwЛицаДокументов
	            INNER JOIN Справочники.dbo.vwЛица Л ON Л.КодЛица = vwЛицаДокументов.КодЛица
                WHERE vwЛицаДокументов.КодДокумента = {0} AND КодБизнесПроекта IS NOT NULL", Id);

                // если вернет null значит не одной записи нет, если 1 то есть хотябы одна запись
                var result = DBManager.ExecuteScalar(sql, CommandType.Text, CN);

                return _isNoBizProject = result == null;
            }

            return _isNoBizProject;
        }

        private int? _personId1;
        private int? _personId2;
        private int? _personId3;
        private int? _personId4;
        private int? _personId5;
        private int? _personId6;
        private bool _isNoBizProject;

        /// <summary>
        ///  Код лица изменился
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

        /// <summary>
        ///  Сохраняем текущий документ, 
        ///  если новый то Insert 
        ///  если существующий то Update
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
                    DeleteDocument(cmds);

                DetailedException dex = new DetailedException(ex.Message, ex);
                Logger.WriteEx(dex);
                throw dex;
            }
        }
        

        /// <summary>
        ///  Метод создания сущности документ(insert)
        /// </summary>
        public void Create(List<DBCommand> cmds)
        {
            SetDocumentNumber();

            var sqlParams = SetSqlParams(isNew: true);
            var outputParams = new Dictionary<string, object> { { "@КодДокумента", -1 } };
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
            DBManager.ExecuteNonQuery(SQLQueries.SP_ДокументыДанные_InsUpd, CommandType.StoredProcedure, CN, sqlParams, outputParams);
            Id = outputParams["@КодДокумента"].ToString();
            Unavailable = string.IsNullOrEmpty(Id);
        }

        /// <summary>
        ///  Метод создания сущности документ(update)
        /// </summary>
        public void UpdateData(List<DBCommand> cmds)
        {
            var sqlParams = SetSqlParams(isNew: false);

            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment ="Обновление данных документа",
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
        /// Метод удаление документа
        /// </summary>
        /// <remarks>Удаляется электронная форма, если отсутствуют картинки, то запускается делитер и удаляет сам документ</remarks>
        private void DeleteDocument(List<DBCommand> cmds = null)
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
        ///  Устанавливает номер документа если это необходимо
        /// </summary>
        private void SetDocumentNumber()
        {
            if (GenerateNumber)
                Number = GenerateDocumentNumber();
        }

        /// <summary>
        ///  Сгенирировать номер документа
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

            DBManager.ExecuteNonQuery(SQLQueries.SP_СозданиеНомераДокумента, CommandType.StoredProcedure, ConnString, sqlParams, outPutparams);

            var result = outPutparams["@RETURN_VALUE"];
            

            return result != null ? result.ToString() : "";
        }

        /// <summary>
        ///  Установка параметров для записи в БД
        /// </summary>
        /// <param name="isNew">флаг true - сохраняем новое, false - обнавляем</param>
        /// <param name="addToWork">Значение параметра ДобавитьВРаботу, по умолчанию null(в базе по умочанию 1)</param>
        /// <returns>Коллекция параметров - значений</returns>
        private Dictionary<string, object> SetSqlParams(bool isNew, byte? addToWork = null)
        {

            var sqlParams = new Dictionary<string, object>(100);

            if (!isNew)
                sqlParams.Add("@КодДокумента", DocId);

            sqlParams.Add("@КодТипаДокумента", TypeID);
            //sqlParams.Add("@КодТипаДокументаДанных", );
            sqlParams.Add("@НазваниеДокумента", string.IsNullOrWhiteSpace(DocumentName) ? "" : DocumentName);
            sqlParams.Add("@НомерДокумента", string.IsNullOrWhiteSpace(Number) ? "" : Number);
            sqlParams.Add("@ДатаДокумента", Date == DateTime.MinValue? (object) null : Date);
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
                sqlParams.Add("@КодБазисаПоставки", DocumentData.BaseDeliveryId);
                sqlParams.Add("@КодВидаТранспорта", DocumentData.TransportTypeId);
                sqlParams.Add("@КодМестаХранения", DocumentData.StorageLocationId);
                sqlParams.Add("@КодЕдиницыИзмерения", DocumentData.UnitsId);
                sqlParams.Add("@КодСтавкиНДС", DocumentData.NDSrateId);
                sqlParams.Add("@КодТУзла1", DocumentData.UzelId1);
                sqlParams.Add("@КодТУзла2", DocumentData.UzelId2);
                sqlParams.Add("@КодТерритории", DocumentData.TerritoryId);
                sqlParams.Add("@КодСтатьиБюджета", DocumentData.BudgetId);
                sqlParams.Add("@Дата2", DocumentData.Date2);
                sqlParams.Add("@Дата3", DocumentData.Date3);
                sqlParams.Add("@Дата4", DocumentData.Date4);
                sqlParams.Add("@Дата5", DocumentData.Date5);
                sqlParams.Add("@Flag1", DocumentData.Flag1);
                sqlParams.Add("@Flag2", DocumentData.Flag2);
                sqlParams.Add("@Int1", DocumentData.Int1);
                sqlParams.Add("@Int2", DocumentData.Int2);
                sqlParams.Add("@Int3", DocumentData.Int3);
                sqlParams.Add("@Int4", DocumentData.Int4);
                sqlParams.Add("@Int5", DocumentData.Int5);
                sqlParams.Add("@Int6", DocumentData.Int6);
                sqlParams.Add("@Int7", DocumentData.Int7);
                sqlParams.Add("@Text50_1", DocumentData.Text50_1);
                sqlParams.Add("@Text50_2", DocumentData.Text50_2);
                sqlParams.Add("@Text50_3", DocumentData.Text50_3);
                sqlParams.Add("@Text50_4", DocumentData.Text50_4);
                sqlParams.Add("@Text50_5", DocumentData.Text50_5);
                sqlParams.Add("@Text50_6", DocumentData.Text50_6);
                sqlParams.Add("@Text50_7", DocumentData.Text50_7);
                sqlParams.Add("@Text50_8", DocumentData.Text50_8);
                sqlParams.Add("@Text50_9", DocumentData.Text50_9);
                sqlParams.Add("@Text50_10", DocumentData.Text50_10);
                sqlParams.Add("@Text50_11", DocumentData.Text50_11);
                sqlParams.Add("@Text50_12", DocumentData.Text50_12);
                sqlParams.Add("@Text50_13", DocumentData.Text50_13);
                sqlParams.Add("@Text100_1", DocumentData.Text100_1);
                sqlParams.Add("@Text100_2", DocumentData.Text100_2);
                sqlParams.Add("@Text100_3", DocumentData.Text100_3);
                sqlParams.Add("@Text100_4", DocumentData.Text100_4);
                sqlParams.Add("@Text100_5", DocumentData.Text100_5);
                sqlParams.Add("@Text100_6", DocumentData.Text100_6);
                sqlParams.Add("@Text300_1", DocumentData.Text300_1);
                sqlParams.Add("@Text300_2", DocumentData.Text300_2);
                sqlParams.Add("@Text300_3", DocumentData.Text300_3);
                sqlParams.Add("@Text300_4", DocumentData.Text300_4);
                sqlParams.Add("@Text300_5", DocumentData.Text300_5);
                sqlParams.Add("@Text300_6", DocumentData.Text300_6);
                sqlParams.Add("@Text300_7", DocumentData.Text300_7);
                sqlParams.Add("@Text300_8", DocumentData.Text300_8);
                sqlParams.Add("@Text300_9", DocumentData.Text300_9);
                sqlParams.Add("@Text1000_1", DocumentData.Text1000_1);
                sqlParams.Add("@Text1000_2", DocumentData.Text1000_2);
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
                sqlParams.Add("@ТекстДокумента", DocumentData.DocumentText);
            }
            if (addToWork != null)
                sqlParams.Add("@ДобавитьВРаботу", addToWork);

            return sqlParams;
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public virtual Document Clone()
        {
            var cloneDoc = (Document)MemberwiseClone();
            cloneDoc.DocumentData = DocumentData.Clone();

            if(_docType != null)
               cloneDoc._docType = _docType.Clone();

            // подписи не копируются - незачем.

            return cloneDoc;
        }

        /// <summary>
        ///  Сравнивает первоначальное состояние документа(original) и текущую версию
        /// </summary>
        /// <returns>
        /// true - документ отличается, 
        /// false - документ не изменялся
        /// </returns>
        public virtual bool CompareToChanges(Document original)
        {
            if (original == null)
                return true;

            if (TypeID != original.TypeID)
                return true;

            if (Name != original.Name)
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

            // если дошло до конца, значит документ не менялся
            return false;
        }

        public void ClearDeliveryTemporary(string guid)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@guid", guid}
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ОтправкаВагоновВыгрузка, CommandType.Text, CN, sqlParams);
        }
    }
}