using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс сущности-связи "СвязиДокументов"
    /// </summary>
    /// <example>
    /// Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}")]
    public class DocLink : Entity, ICloneable<DocLink>
    {
        #region Поля сущности "СвязиДокументов"

        /// <summary>
        ///  Поле КодСвязиДокументов,
        ///  Типизированный псевданим для Entity.Id
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int DocLinkId { get { return Id.ToInt(); } }

        /// <summary>
        ///  Поле КодДокументаОснования
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int BaseDocId { get; set; }

        /// <summary>
        ///  Поле КодДокументаВытекающего
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int SequelDocId { get; set; }

        /// <summary>
        /// Поле КодПоляДокумента
        /// </summary>
        /// <value>
        ///  int, null
        /// </value>
        public int? DocFieldId { get; set; }

        /// <summary>
        ///  Поле ПорядокОснования
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int SortParent { get; set; }

        /// <summary>
        ///  Поле ПорядокВытекающего
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int SortChild { get; set; }

        /// <summary>
        /// Изменил
        /// </summary>
        /// <value>
        ///  int, not null
        /// </value>
        public int ChangePersonId { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        /// <value>
        ///  datetime, not null
        /// </value>
        public DateTime ChangeDate { get; set; }

        #endregion

        /// <summary>
        ///  Конструктор без параметров
        /// </summary>
        public DocLink(){ }

        /// <summary>
        ///  Конструктор с параметрами
        /// </summary>
        /// <param name="id"></param>
        public DocLink(string id)
            : base(id)
        {
            Load();
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
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        /// Метод загрузки данных сущности "СвязиДокументов"
        /// </summary>
        public sealed override void Load()
        {
            FillData(DocLinkId);
        }

        /// <summary>
        /// Вытекающие документы по Коду Документа Основания
        /// </summary>
        public static List<int> LoadChildDocsById(int id)
        {
            var res = new List<int>();

            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиДокументов_ПоОснованию, id, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    var colКодЛица = dbReader.GetOrdinal("КодДокументаВытекающего");
                    while (dbReader.Read())
                    {
                        res.Add(dbReader.GetInt32(colКодЛица));
                    }
                }
            }

            return res;
        }

        /// <summary>
        ///  Функция аналогичная LoadBaseDocs в V3 
        /// </summary>
        /// <param name="docId">Id текущего документа</param>
        /// <param name="fieldId">Id поля</param>
        /// <returns></returns>
        public static List<DocLink> LoadBaseDocs(int docId, int fieldId)
        {
            var res = new List<DocLink>();

            var param = new Dictionary<string, object> {{"@IdDoc", docId}, {"@IDField", fieldId}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиДокументов_ПоВытекающему_ПоПолю, CommandType.Text, ConnString, param))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодСвязиДокументов = dbReader.GetOrdinal("КодСвязиДокументов");
                    int colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    int colКодДокументаВытекающего = dbReader.GetOrdinal("КодДокументаВытекающего");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colПорядокОснования = dbReader.GetOrdinal("ПорядокОснования");
                    int colПорядокВытекающего = dbReader.GetOrdinal("ПорядокВытекающего");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocLink
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодСвязиДокументов).ToString(),
                            BaseDocId = dbReader.GetInt32(colКодДокументаОснования),
                            SequelDocId = dbReader.GetInt32(colКодДокументаВытекающего),
                            SortParent = dbReader.GetInt32(colПорядокОснования),
                            SortChild = dbReader.GetInt32(colПорядокВытекающего),
                            ChangePersonId = dbReader.GetInt32(colИзменил),
                            ChangeDate = dbReader.GetDateTime(colИзменено)
                        };

                        if (!dbReader.IsDBNull(colКодПоляДокумента))
                        {
                            row.DocFieldId = dbReader.GetInt32(colКодПоляДокумента);
                        }

                        res.Add(row);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Вытекающие документы по коду вытекающего документа
        /// </summary>
        public static List<int> LoadBasisDocsIdByChildId(int id)
        {
            var res = new List<int>();

            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиДокументов_ПоВытекающему, id, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    var colКодЛица = dbReader.GetOrdinal("КодДокументаОснования");
                    while (dbReader.Read())
                    {
                        res.Add(dbReader.GetInt32(colКодЛица));
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Вытекающие документы по коду вытекающего документа
        /// </summary>
        /// <returns>(Not null) коллекция DocLink</returns>
        public static List<DocLink> LoadBasisDocsByChildId(int id)
        {
            List<DocLink> res = new List<DocLink>();

            if (id <= 0) return res;

            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиДокументов_ПоВытекающему, id, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодСвязиДокументов = dbReader.GetOrdinal("КодСвязиДокументов");
                    int colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    int colКодДокументаВытекающего = dbReader.GetOrdinal("КодДокументаВытекающего");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colПорядокОснования = dbReader.GetOrdinal("ПорядокОснования");
                    int colПорядокВытекающего = dbReader.GetOrdinal("ПорядокВытекающего");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocLink
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодСвязиДокументов).ToString(),
                            BaseDocId = dbReader.GetInt32(colКодДокументаОснования),
                            SequelDocId = dbReader.GetInt32(colКодДокументаВытекающего),
                            SortParent = dbReader.GetInt32(colПорядокОснования),
                            SortChild = dbReader.GetInt32(colПорядокВытекающего),
                            ChangePersonId = dbReader.GetInt32(colИзменил),
                            ChangeDate = dbReader.GetDateTime(colИзменено)
                        };

                        if (!dbReader.IsDBNull(colКодПоляДокумента)) { row.DocFieldId = dbReader.GetInt32(colКодПоляДокумента); }
 
                        res.Add(row);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Получить ID строки из таблицы СвязиДокументов
        /// </summary>
        public static int GetLinkId(int parentDocId, int childDocId)
        {
            var param = new Dictionary<string, object>();
            param.Add("@ParentDocID", parentDocId);
            param.Add("@ChildDocID", childDocId);

            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_СвязиДокументов_ПоОснованию_ПоВытекающему, CommandType.Text, ConnString, param);

            if (res is int)
                return (int) res;

            return 0;
        }

        /// <summary>
        /// Получить ID строки из таблицы СвязиДокументов
        /// </summary>
        public static int GetLinkId(int parentDocId, int childDocId, int? docFieldId)
        {
            var param = new Dictionary<string, object>();
            param.Add("@ParentDocID", parentDocId);
            param.Add("@ChildDocID", childDocId);
            param.Add("@FildID", docFieldId == null ? DBNull.Value : (object)docFieldId);

            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_СвязиДокументов_ПоОснованию_ПоВытекающему_ПоПолю, CommandType.Text, ConnString, param);

            if (res is int)
                return (int)res;

            return 0;
        }

        /// <summary>
        /// Метод загрузки и заполнения данных сущности "СвязиДокументов"
        /// </summary>
        public void FillData(int id)
        {
            if(id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_СвязиДокументов, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодСвязиДокументов = dbReader.GetOrdinal("КодСвязиДокументов");
                    int colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    int colКодДокументаВытекающего = dbReader.GetOrdinal("КодДокументаВытекающего");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colПорядокОснования = dbReader.GetOrdinal("ПорядокОснования");
                    int colПорядокВытекающего = dbReader.GetOrdinal("ПорядокВытекающего");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if(dbReader.Read())
                    {
                       Unavailable = false;
                       Id = dbReader.GetInt32(colКодСвязиДокументов).ToString();
                       BaseDocId = dbReader.GetInt32(colКодДокументаОснования);
                       SequelDocId = dbReader.GetInt32(colКодДокументаВытекающего);
                       if (!dbReader.IsDBNull(colКодПоляДокумента)) { DocFieldId = dbReader.GetInt32(colКодПоляДокумента); }
                       SortParent = dbReader.GetInt32(colПорядокОснования);
                       SortChild = dbReader.GetInt32(colПорядокВытекающего);
                       ChangePersonId = dbReader.GetInt32(colИзменил);
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
        ///  Метод создания связи документов(insert)
        /// </summary>
        public void Create(List<DBCommand> cmds = null)
        {
            var param = new Dictionary<string, object>();
            var outputParam = new Dictionary<string, object>();
            param.Add("@ParentDocID", BaseDocId);
            param.Add("@ChildDocID", SequelDocId);
            param.Add("@FildID", DocFieldId == null ? DBNull.Value :(object)DocFieldId);

            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Создание связи между документами",
                    Text = SQLQueries.SP_MakeDocsLink,
                    Type = CommandType.StoredProcedure,
                    ConnectionString = CN,
                    ParamsIn = param,
                    ParamsOut = outputParam
                });

                return;
            }
            if(BaseDocId <= 0)
                throw new ArgumentException("Код документа основания должен быть больше нуля");

            if(SequelDocId <= 0)
                throw new ArgumentException("Код вытекающего документа должен быть больше нуля");
            
            DBManager.ExecuteNonQuery(SQLQueries.SP_MakeDocsLink, CommandType.StoredProcedure, CN, param, outputParam);

            Id = outputParam["@RETURN_VALUE"].ToString();
        }

        /// <summary>
        ///  Метод удаления связи документов(delete)
        /// </summary>
        public void Delete(List<DBCommand> cmds = null)
        {
            if(Id.IsNullEmptyOrZero()) return;
            var param = new Dictionary<string, object> { { "@id", Id } };
            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Удаление связи документов",
                    Text = SQLQueries.DELETE_ID_СвязиДокументов,
                    Type = CommandType.Text,
                    ConnectionString = CN,
                    ParamsIn = param,
                    ParamsOut = null
                });
                return;
            }
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_СвязиДокументов, CommandType.Text, CN, param);
        }

        /// <summary>
        ///  Метод удаления связи документов по документам и полю
        /// </summary>
        public static void Delete(int parentDocId, int childDocId, int? docFieldId, List<DBCommand> cmds = null)
        {
            var param = new Dictionary<string, object>();
            param.Add("@ParentDocID", parentDocId);
            param.Add("@ChildDocID", childDocId);
            param.Add("@FildID", docFieldId == null ? DBNull.Value : (object)docFieldId);

            if (cmds != null)
            {
                cmds.Add(new DBCommand
                {
                    Appointment = "Удаление связи документа по коду документа и полю",
                    Text = SQLQueries.DELETE_ID_СвязиДокументов,
                    Type = CommandType.Text,
                    ConnectionString = ConnString,
                    ParamsIn = param,
                    ParamsOut = null
                });
                return;
            }
            if (parentDocId <= 0)
                throw new ArgumentException("Код документа основания должен больше нуля");

            if (childDocId <= 0)
                throw new ArgumentException("Код вытекающего документа должен быть больше нуля");

            DBManager.ExecuteNonQuery(SQLQueries.DELETE_СвязиДокументов_ПоОснованию_ПоВытекающему_ПоПолю, CommandType.Text, ConnString, param);
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public DocLink Clone()
        {
            return (DocLink) MemberwiseClone();
        }
    }
}
