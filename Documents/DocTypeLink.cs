using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///  Класс сущности "СвязиТиповДокументов"
    /// </summary>
    [DebuggerDisplay("DocBasisId = {DocBasisId}, ChildDocId = {ChildDocId}, DocFieldId = {DocFieldId}")]
    public class DocTypeLink : Entity
    {
        #region Поля сущности

        /// <summary>
        ///  Поле Код типа документа основания
        /// </summary>
        /// <value>
        /// КодТипаДокументаОснования (int, not null)
        /// </value>
        public int DocBasisId { get; set; }

        /// <summary>
        ///  Поле Код типа документа вытекающего
        /// </summary>
        /// <value>
        /// КодТипаДокументаВытекающего (int, not null)
        /// </value>
        public int ChildDocId { get; set; }

        /// <summary>
        ///  Поле Тип связи
        /// </summary>
        /// <value>
        /// ТипСвязи (tinyint, not null)
        /// </value>
        public byte LinkType { get; set; }

        /// <summary>
        ///  Поле Код поля документа
        /// </summary>
        /// <value>
        /// КодПоляДокумента (int, not null)
        /// </value>
        public int DocFieldId { get; set; }

        /// <summary>
        /// Поле Выводить список в основании
        /// </summary>
        /// <value>
        /// ВыводитьСписокВОсновании (tinyint, not null)
        /// </value>
        public byte ShowListInBasis { get; set; }

        /// <summary>
        ///  Поле Порядок вывода в основании
        /// </summary>
        /// <value>
        /// ПорядокВыводаВОсновании (int, not null)
        /// </value>
        public int BasisOrder { get; set; }

        /// <summary>
        ///  Поле Режим поиска основания
        /// </summary>
        /// <value>
        /// РежимПоискаОснования (tinyint, not null)
        /// </value>
        public byte BasisSearchMode { get; set; }

        /// <summary>
        ///  Поле Строгий поиск основания
        /// </summary>
        /// <value>
        /// СтрогийПоискОснования (tinyint, not null)
        /// </value>
        public byte StrictBasisSearch { get; set; }

        /// <summary>
        ///  Поле Запрет изменения основания
        /// </summary>
        /// <value>
        /// ЗапретИзмененияОснования (tinyint, not null)
        /// </value>
        public byte BasisChangeBan { get; set; }

        /// <summary>
        ///  Поле Тип документа основания
        /// </summary>
        /// <value>
        /// ТипДокументаОснования (varchar(100), not null)
        /// </value>
        public string DocBasisType { get; set; }

        /// <summary>
        ///  Поле Тип документа вытекающего
        /// </summary>
        /// <value>
        /// ТипДокументаВытекающего (varchar(100), not null)
        /// </value>
        public string DocChildType { get; set; }


        /// <summary>
        ///  Описание поля вытекающего по русски
        /// </summary>
        public string DocChildField { get; set; }

        /// <summary>
        ///  Описание поля вытекающего по английски
        /// </summary>
        public string DocChildFieldEN { get; set; }

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

        #endregion

        /// <summary>
        ///  Получить список СвязиТиповДокументов
        /// </summary>
        public List<DocTypeLink> GetDocTypeLinkList(string query)
        {
            List<DocTypeLink> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    list = new List<DocTypeLink>();

                    #region Получение порядкового номера столбца

                    int colКодТипаДокументаОснования = dbReader.GetOrdinal("КодТипаДокументаОснования");
                    int colКодТипаДокументаВытекающего = dbReader.GetOrdinal("КодТипаДокументаВытекающего");
                    int colТипСвязи = dbReader.GetOrdinal("ТипСвязи");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colВыводитьСписокВОсновании = dbReader.GetOrdinal("ВыводитьСписокВОсновании");
                    int colПорядокВыводаВОсновании = dbReader.GetOrdinal("ПорядокВыводаВОсновании");
                    int colРежимПоискаОснования = dbReader.GetOrdinal("РежимПоискаОснования");
                    int colСтрогийПоискОснования = dbReader.GetOrdinal("СтрогийПоискОснования");
                    int colЗапретИзмененияОснования = dbReader.GetOrdinal("ЗапретИзмененияОснования");
                    int colТипДокументаОснования = dbReader.GetOrdinal("ТипДокументаОснования");
                    int colТипДокументаВытекающего = dbReader.GetOrdinal("ТипДокументаВытекающего");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocTypeLink
                        {
                            Unavailable = false,
                            DocBasisId = dbReader.GetInt32(colКодТипаДокументаОснования),
                            ChildDocId = dbReader.GetInt32(colКодТипаДокументаВытекающего),
                            LinkType = dbReader.GetByte(colТипСвязи),
                            DocFieldId = dbReader.GetInt32(colКодПоляДокумента),
                            ShowListInBasis = dbReader.GetByte(colВыводитьСписокВОсновании),
                            BasisOrder = dbReader.GetInt32(colПорядокВыводаВОсновании),
                            BasisSearchMode = dbReader.GetByte(colРежимПоискаОснования),
                            StrictBasisSearch = dbReader.GetByte(colСтрогийПоискОснования),
                            BasisChangeBan = dbReader.GetByte(colЗапретИзмененияОснования),
                            DocBasisType = dbReader.GetString(colТипДокументаОснования),
                            DocChildType = dbReader.GetString(colТипДокументаВытекающего)
                        };
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Получение ограничений по типам и полям документов оснований
        /// </summary>
        /// <remarks>
        ///  Данные статичные. Можно один раз получить и закешировать
        /// </remarks>
        public static List<DocTypeLink> GetControlFilter(int docTypeId)
        {
            if (docTypeId == 0) return null;

            List<DocTypeLink> list = null;

            var param = new Dictionary<string, object> {{"@id", docTypeId}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиТиповДокументов_ТипВытекающего, CommandType.Text, ConnString, param))
            {
                if (dbReader.HasRows)
                {
                    list = new List<DocTypeLink>();
                    #region Получение порядкового номера столбца

                    int colКодТипаДокументаОснования = dbReader.GetOrdinal("КодТипаДокументаОснования");
                    int colРежимПоискаОснования = dbReader.GetOrdinal("РежимПоискаОснования");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocTypeLink
                        {
                            Unavailable = false,
                            DocBasisId = dbReader.GetInt32(colКодТипаДокументаОснования),
                            ChildDocId = docTypeId,
                            BasisSearchMode = dbReader.GetByte(colРежимПоискаОснования),
                            DocFieldId = dbReader.GetInt32(colКодПоляДокумента)
                        };
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///  Получение всех вытекающих типов документа 
        /// </summary>
        /// <returns>List никогда не бывает null</returns>
        public static List<DocTypeLink> GetAllChildTypes(int DocTypeId)
        {
            List<DocTypeLink> list = new List<DocTypeLink>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_СвязиТиповДокументов_Вытекающие, DocTypeId, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодТипаДокументаОснования = dbReader.GetOrdinal("КодТипаДокументаОснования");
                    int colТипДокументаОснования = dbReader.GetOrdinal("ТипДокументаОснования");
                    int colКодТипаДокументаВытекающего = dbReader.GetOrdinal("КодТипаДокументаВытекающего");
                    int colТипДокументаВытекающего = dbReader.GetOrdinal("ТипДокументаВытекающего");
                    int colВыводитьСписокВОсновании = dbReader.GetOrdinal("ВыводитьСписокВОсновании");
                    int colПорядокВыводаВОсновании = dbReader.GetOrdinal("ПорядокВыводаВОсновании");
                    int colТипСвязи = dbReader.GetOrdinal("ТипСвязи");
                    int colРежимПоискаОснования = dbReader.GetOrdinal("РежимПоискаОснования");
                    int colКодПоляДокумента = dbReader.GetOrdinal("КодПоляДокумента");
                    int colПолеДокумента = dbReader.GetOrdinal("ПолеДокумента");
                    int colПолеДокументаEn = dbReader.GetOrdinal("ПолеДокументаEn");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new DocTypeLink {Unavailable = false};
                        if (!dbReader.IsDBNull(colКодТипаДокументаОснования))
                        {
                            row.DocBasisId = dbReader.GetInt32(colКодТипаДокументаОснования);
                        }
                        row.DocBasisType = dbReader.GetString(colТипДокументаОснования);
                        row.ChildDocId = dbReader.GetInt32(colКодТипаДокументаВытекающего);
                        row.DocChildType = dbReader.GetString(colТипДокументаВытекающего);
                        row.ShowListInBasis = dbReader.GetByte(colВыводитьСписокВОсновании);
                        row.BasisOrder = dbReader.GetInt32(colПорядокВыводаВОсновании);
                        row.LinkType = dbReader.GetByte(colТипСвязи);
                        row.BasisSearchMode = dbReader.GetByte(colРежимПоискаОснования);
                        row.DocFieldId = dbReader.GetInt32(colКодПоляДокумента);
                        row.DocChildField = dbReader.GetString(colПолеДокумента);
                        row.DocChildFieldEN = dbReader.GetString(colПолеДокументаEn);
                        list.Add(row);
                    }
                }
            }

            list.Sort((a, b) => a.BasisOrder.CompareTo(b.BasisOrder));

            return list;
        } 



    }
}
