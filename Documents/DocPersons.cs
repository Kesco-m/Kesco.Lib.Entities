using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///  Сущность лица документов
    /// </summary>
    public class DocPersons : Entity
    {
        #region Поля сущности

        /// <summary>
        ///  Код лица документа
        /// </summary>
        /// <value>
        /// КодЛицаДокумента (int, not null)
        /// </value>
        public int DocPersonId { get; set; }

        /// <summary>
        ///  Код документа
        /// </summary>
        /// <value>
        /// КодДокумента (int, not null)
        /// </value>
        public int DocumentId { get; set; }

        /// <summary>
        ///  Код лица
        /// </summary>
        /// <value>
        /// КодЛица (int, not null)
        /// </value>
        public int PersonId { get; set; }

        /// <summary>
        ///  Положение
        /// </summary>
        /// <value>
        /// Положение (tinyint, not null)
        /// </value>
        public byte Position { get; set; }

        /// <summary>
        ///  Изменил
        /// </summary>
        /// <value>
        /// Изменил (int, not null)
        /// </value>
        public int ChangePersonID { get; set; }

        /// <summary>
        ///  Изменено
        /// </summary>
        /// <value>
        /// Изменено (datetime, not null)
        /// </value>
        public DateTime ChangeDate { get; set; }

        #endregion

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
        ///  Заполинть данными по ID
        /// </summary>
        public void FillData(int id)
        {
            if(id <= 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ЛицаДокументов, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    int colПоложение = dbReader.GetOrdinal("Положение");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = id.ToString();
                        DocumentId = dbReader.GetInt32(colКодДокумента);
                        PersonId = dbReader.GetInt32(colКодЛица);
                        Position = dbReader.GetByte(colПоложение);
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
        ///  Получить список документов лица по документу
        /// </summary>
        public static List<DocPersons> GetDocsPersonsByDocId(int docId)
        {
            var list = new List<DocPersons>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_ЛицаДокументов_ПоДокументу, docId, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодЛицаДокумента = dbReader.GetOrdinal("КодЛицаДокумента");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    int colПоложение = dbReader.GetOrdinal("Положение");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocPersons();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодЛицаДокумента).ToString();
                        row.DocumentId = dbReader.GetInt32(colКодДокумента);
                        row.PersonId = dbReader.GetInt32(colКодЛица);
                        row.Position = dbReader.GetByte(colПоложение);
                        row.ChangePersonID = dbReader.GetInt32(colИзменил);
                        row.ChangeDate = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Получение лиц по коду документа
        /// </summary>
        /// <param name="id">Код документа</param>
        /// <param name="isData">Признак для условия "Положение > 0"</param>
        /// <returns>Список кодов лиц</returns>
        public static List<int> LoadPersonsByDocId(int id, bool isData = false)
        {
            var query = isData ? SQLQueries.SELECT_ЛицаДокументов_ПоДокументу_ПоПоложению : SQLQueries.SELECT_ЛицаДокументов_ПоДокументу;

            var persArr = new List<int>();

            using (var dbReader = new DBReader(query, id, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    while (dbReader.Read())
                    {
                        persArr.Add(dbReader.GetInt32(colКодЛица));
                    }
                }
            }

            return persArr;
        }

        /// <summary>
        ///  Получить список документов лица по строке запроса
        /// </summary>
        public static List<DocPersons> GetDocPersonsList(string query)
        {
            var list = new List<DocPersons>();
            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодЛицаДокумента = dbReader.GetOrdinal("КодЛицаДокумента");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    int colПоложение = dbReader.GetOrdinal("Положение");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocPersons();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодЛицаДокумента).ToString();
                        row.DocumentId = dbReader.GetInt32(colКодДокумента);
                        row.PersonId = dbReader.GetInt32(colКодЛица);
                        row.Position = dbReader.GetByte(colПоложение);
                        row.ChangePersonID = dbReader.GetInt32(colИзменил);
                        row.ChangeDate = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }
            return list;
        }
    }
}
