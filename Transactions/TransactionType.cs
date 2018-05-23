using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Transactions
{
    /// <summary>
    /// Бизнес-объект - типы транзакций
    /// </summary>
    [DebuggerDisplay("ID = {Id}, Name = {TransactionNameRu}")]
    public class TransactionType : Entity
    {
        #region Поля сущности
        /// <summary>
        /// Поле КодТипаТранзакции
        /// </summary>
        /// <value>
        /// КодТипаТранзакции (int, not null)
        /// </value>
        /// <remarks>
        ///  Типизированный псевданим для ID
        /// </remarks>
        public int CodeType { get { return Convert.ToInt32(Id); } }
        /// <summary>
        /// Поле ТипТранзакции
        /// </summary>
        /// <value>
        /// ТипТранзакции (varchar(50), not null)
        /// </value>
        public string TransactionNameRu { get { return Name; } }
        /// <summary>
        /// Поле ТипТранзакцииEN
        /// </summary>
        /// <value>
        /// ТипТранзакцииEN (varchar(50), not null)
        /// </value>
        public string TransactionNameEN { get; set; }
        /// <summary>
        /// Поле КодГруппыТиповТранзакций
        /// </summary>
        /// <value>
        /// КодГруппыТиповТранзакций (int, not null)
        /// </value>
        public int CodeTypeGroup { get; set; }
        /// <summary>
        /// Поле ГруппаАктаСверки
        /// </summary>
        /// <value>
        /// ГруппаАктаСверки (int, not null)
        /// </value>
        public int ReviseActGroup { get; set; }
        /// <summary>
        /// Поле ПорядокАктаСверки
        /// </summary>
        /// <value>
        /// ПорядокАктаСверки (int, not null)
        /// </value>
        public int ReviseActOrder { get; set; }
        /// <summary>
        /// Поле ОписаниеАктаСверки
        /// </summary>
        /// <value>
        /// ОписаниеАктаСверки (varchar(100), not null)
        /// </value>
        public string ReviseActDescr { get; set; }
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
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Конструктор с параметром для сущности "типы транзакций"
        /// </summary>
        /// <param name="id">КодТипаТранзакции</param>
        public TransactionType(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///  Конструктор по умолчанию для сущности "типы транзакций"
        /// </summary>
        public TransactionType(){}

        /// <summary>
        /// Метод загрузки данных сущности "типы транзакций"
        /// </summary>
        public sealed override void Load()
        {
            FillData(CodeType);
        }


        /// <summary>
        /// Метод загрузки и заполнения данных сущности "типы транзакций"
        /// </summary>
        /// <param name="id">КодТипаТранзакции</param>
        public void FillData(int id)
        {
            if(id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ТипТранзакции, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодТипаТранзакции = dbReader.GetOrdinal("КодТипаТранзакции");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colТипТранзакцииEN = dbReader.GetOrdinal("ТипТранзакцииEN");
                    int colКодГруппыТиповТранзакций = dbReader.GetOrdinal("КодГруппыТиповТранзакций");
                    int colГруппаАктаСверки = dbReader.GetOrdinal("ГруппаАктаСверки");
                    int colПорядокАктаСверки = dbReader.GetOrdinal("ПорядокАктаСверки");
                    int colОписаниеАктаСверки = dbReader.GetOrdinal("ОписаниеАктаСверки");
                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодТипаТранзакции).ToString();
                        Name = dbReader.GetString(colТипТранзакции);
                        TransactionNameEN = dbReader.GetString(colТипТранзакцииEN);
                        CodeTypeGroup = dbReader.GetInt32(colКодГруппыТиповТранзакций);
                        ReviseActGroup = dbReader.GetInt32(colГруппаАктаСверки);
                        ReviseActOrder = dbReader.GetInt32(colПорядокАктаСверки);
                        ReviseActDescr = dbReader.GetString(colОписаниеАктаСверки);
                    }
                    else { Unavailable = true; }
                }
            }
        }

        /// <summary>
        /// Получить типы транзакций по строке запроса
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TransactionType> GetTransactionTypeList(string query)
        {
            var list = new List<TransactionType>();
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодТипаТранзакции = dbReader.GetOrdinal("КодТипаТранзакции");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colТипТранзакцииEN = dbReader.GetOrdinal("ТипТранзакцииEN");
                    int colКодГруппыТиповТранзакций = dbReader.GetOrdinal("КодГруппыТиповТранзакций");
                    int colГруппаАктаСверки = dbReader.GetOrdinal("ГруппаАктаСверки");
                    int colПорядокАктаСверки = dbReader.GetOrdinal("ПорядокАктаСверки");
                    int colОписаниеАктаСверки = dbReader.GetOrdinal("ОписаниеАктаСверки");
                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new TransactionType();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодТипаТранзакции).ToString();
                        row.Name = dbReader.GetString(colТипТранзакции);
                        row.TransactionNameEN = dbReader.GetString(colТипТранзакцииEN);
                        row.CodeTypeGroup = dbReader.GetInt32(colКодГруппыТиповТранзакций);
                        row.ReviseActGroup = dbReader.GetInt32(colГруппаАктаСверки);
                        row.ReviseActOrder = dbReader.GetInt32(colПорядокАктаСверки);
                        row.ReviseActDescr = dbReader.GetString(colОписаниеАктаСверки);
                        list.Add(row);
                    }
                }
            }
            return list;
        }

    }

}
