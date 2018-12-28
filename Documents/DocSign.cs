using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;
using Convert = System.Convert;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///  Класс сущности ПодписиДокументов
    /// </summary>
    /// <example>
    /// Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, DocId = {DocId}")]
    public class DocSign : Entity, ICloneable<DocSign>
    {

        #region Поля сущности "ПодписиДокументов"

        /// <summary>
        ///  Код подписи документа
        /// </summary>
        public int SignId { get { return Id.ToInt(); } }

        /// <summary>
        ///  Код документа(Document)
        /// </summary>
        public int DocId { get; set; }

        /// <summary>
        ///  Код изображения документа
        /// </summary>
        public int DocImageId { get; set; }

        /// <summary>
        ///  Код cотрудника
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///  Код cотрудника за
        /// </summary>
        public int EmployeeInsteadOf { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Тип подписи
        /// </summary>
        public byte SignType { get; set; }

        /// <summary>
        ///  Код штампа
        /// </summary>
        public int StampId { get; set; }

        /// <summary>
        ///  Поле Page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///  Поле X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///  Поле Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        ///  Поле Zoom
        /// </summary>
        public int Zoom { get; set; }

        /// <summary>
        ///  Поле Rotate
        /// </summary>
        public int Rotate { get; set; }

        /// <summary>
        ///  Завершено
        /// </summary>
        public bool IsFinal
        {
            get { return SignType == 1; }
        }

        /// <summary>
        ///  Поле Текст подписи для ХП sp_ПодписиДокумента
        /// </summary>
        /// <value>
        /// ТекстПодписи (varchar(100), null)
        /// </value>
        public string SignText { get; set; }

        /// <summary>
        ///  Поле ФИО для ХП sp_ПодписиДокумента
        /// </summary>
        /// <value>
        /// ФИО (varchar(100), null)
        /// </value>
        public string EmployeeFio { get; set; } 
        /// <summary>
        /// Поле ФИОЗА для ХП sp_ПодписиДокумента
        /// </summary>
        /// <value>
        /// ФИОЗА (varchar(100), null)
        /// </value>
        public string SubEmployeeFio { get; set; }

        /// <summary>
        /// МожноПодписать
        /// </summary>
        public Int16 CanSign { get; set; }

        /// <summary>
        ///  Поле МожноУдалить для ХП sp_ПодписиДокумента
        /// </summary>
        /// <value>
        /// МожноУдалить (tinyint, null)
        /// </value>
        public byte CanDelete { get; set; }

        #endregion

        /// <summary>
        /// Метод загрузки данных сущности "ПодписиДокументов"
        /// </summary>
        public sealed override void Load()
        {
            FillData(SignId);
        }

        /// <summary>
        ///  Конструктор без параметров
        /// </summary>
        public DocSign(){ }

        /// <summary>
        /// Метод загрузки данных сущности "ПодписиДокументов"
        /// </summary>
        public DocSign(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if(string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_document;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Удаление подписи
        /// </summary>
        /// <param name="id">Код подписи документа</param>
        public static void RemoveSign(string id)
        {
            try
            {
                var sqlParms = new Dictionary<string, object> { { "@SignId", id } };
                DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_ПодписьДокумента, CommandType.Text, ConnString, sqlParms);
            }
            catch (Exception e)
            {
                Logger.WriteEx(e);
                throw e;
            }
        }

        /// <summary>
        /// Получение завершающей надписи
        /// </summary>
        public static string GetSignText(int docTypeId, int signType)
        {
            string result = string.Empty;

            var parameters = new Dictionary<string, object> { { "@DocTypeId", docTypeId }, { "@SignType", signType } };

            var val = DBManager.ExecuteScalar(SQLQueries.SELECT_СообщенияПодписи, CommandType.Text, ConnString, parameters);

            if (val is string)
                result = val.ToString();

            return result;
        }

        /// <summary>
        ///  Получить подписи привязанные к документу используя хп sp_ПодписиДокумента
        /// </summary>
        public static List<DocSign> GetSignsByDocumentId(int docId)
        {
            List<DocSign> list = null;

            if(docId == 0)
                return null;

            var parameters = new Dictionary<string, object> { { "@КодДокумента", docId } };
            var parametersOut = new Dictionary<string, object> {{"@МожноПодписать", -1}};

            using (var dbReader = new DBReader("sp_ПодписиДокумента", CommandType.StoredProcedure, ConnString, parameters, parametersOut))
            {
                if (dbReader.HasRows)
                {
                    list = new List<DocSign>();

                    #region Получение порядкового номера столбца

                    int colКодПодписиДокумента = dbReader.GetOrdinal("КодПодписиДокумента");
                    int colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    int colКодСотрудникаЗА = dbReader.GetOrdinal("КодСотрудникаЗА");
                    int colДата = dbReader.GetOrdinal("Дата");
                    int colТекстПодписи = dbReader.GetOrdinal("ТекстПодписи");
                    int colФИО = dbReader.GetOrdinal("ФИО");
                    int colФИОЗА = dbReader.GetOrdinal("ФИОЗА");
                    int colТипПодписи = dbReader.GetOrdinal("ТипПодписи");
                    int colМожноУдалить = dbReader.GetOrdinal("МожноУдалить");
                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocSign { Unavailable = false, DocId = docId };
                        if (!dbReader.IsDBNull(colКодПодписиДокумента)) { row.Id = dbReader.GetInt32(colКодПодписиДокумента).ToString(); }
                        if (!dbReader.IsDBNull(colКодСотрудника)) { row.EmployeeId = dbReader.GetInt32(colКодСотрудника); }
                        if (!dbReader.IsDBNull(colКодСотрудникаЗА)) { row.EmployeeInsteadOf = dbReader.GetInt32(colКодСотрудникаЗА); }
                        if (!dbReader.IsDBNull(colДата)) { row.Date = dbReader.GetDateTime(colДата); }
                        if (!dbReader.IsDBNull(colТекстПодписи)) { row.SignText = dbReader.GetString(colТекстПодписи); }
                        if (!dbReader.IsDBNull(colФИО)) { row.EmployeeFio = dbReader.GetString(colФИО); }
                        if (!dbReader.IsDBNull(colФИОЗА)) { row.SubEmployeeFio = dbReader.GetString(colФИОЗА); }
                        if (!dbReader.IsDBNull(colТипПодписи)) { row.SignType = dbReader.GetByte(colТипПодписи); }
                        if (!dbReader.IsDBNull(colМожноУдалить)) { row.CanDelete = dbReader.GetByte(colМожноУдалить); }
                        list.Add(row);
                    }
                }

                dbReader.Close();
                var canSign = Convert.ToInt16(parametersOut["@МожноПодписать"]);

                if(list != null)
                  foreach (var l in list)
                    l.CanSign = canSign;
            }
            return list;
        }

        /// <summary>
        /// Метод загрузки и заполнения данных сущности "ПодписиДокументов"
        /// </summary>
        protected void FillData(int id)
        {
            if(id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ПодписьДокумента, id, CommandType.Text, CN))
            {

                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодПодписиДокумента = dbReader.GetOrdinal("КодПодписиДокумента");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colКодИзображенияДокумента = dbReader.GetOrdinal("КодИзображенияДокумента");
                    int colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    int colКодСотрудникаЗА = dbReader.GetOrdinal("КодСотрудникаЗА");
                    int colДата = dbReader.GetOrdinal("Дата");
                    int colТипПодписи = dbReader.GetOrdinal("ТипПодписи");
                    int colКодШтампа = dbReader.GetOrdinal("КодШтампа");
                    int colPage = dbReader.GetOrdinal("Page");
                    int colX = dbReader.GetOrdinal("X");
                    int colY = dbReader.GetOrdinal("Y");
                    int colZoom = dbReader.GetOrdinal("Zoom");
                    int colRotate = dbReader.GetOrdinal("Rotate");

                    #endregion

                    Unavailable = false;
                    dbReader.Read();
                    Id = dbReader.GetInt32(colКодПодписиДокумента).ToString();
                    DocId = dbReader.GetInt32(colКодДокумента);
                    if (!dbReader.IsDBNull(colКодИзображенияДокумента)) { DocImageId = dbReader.GetInt32(colКодИзображенияДокумента); }
                    EmployeeId = dbReader.GetInt32(colКодСотрудника);
                    EmployeeInsteadOf = dbReader.GetInt32(colКодСотрудникаЗА);
                    Date = dbReader.GetDateTime(colДата);
                    SignType = dbReader.GetByte(colТипПодписи);
                    if (!dbReader.IsDBNull(colКодШтампа)) { StampId = dbReader.GetInt32(colКодШтампа); }
                    if (!dbReader.IsDBNull(colPage)) { Page = dbReader.GetInt32(colPage); }
                    if (!dbReader.IsDBNull(colX)) { X = dbReader.GetInt32(colX); }
                    if (!dbReader.IsDBNull(colY)) { Y = dbReader.GetInt32(colY); }
                    if (!dbReader.IsDBNull(colZoom)) { Zoom = dbReader.GetInt32(colZoom); }
                    if (!dbReader.IsDBNull(colRotate)) { Rotate = dbReader.GetInt32(colRotate); }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///  Добавление подписи
        /// </summary>
        public void Create()
        {
            var param = new Dictionary<string, object> {{"@КодДокумента", DocId}, {"@КодСотрудника", EmployeeId}, {"@КодСотрудникаЗА", EmployeeInsteadOf}, {"@ТипПодписи", SignType}};

            DBManager.ExecuteNonQuery(SQLQueries.INSERT_ПодписьДокумента, CommandType.Text, CN, param);
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public DocSign Clone()
        {
            return (DocSign) MemberwiseClone();
        }
    }
}
