using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Transactions
{
    /// <summary>
    ///     Бизнес-объект - транзакции
    ///     Не путать с транзакциями в БД
    /// </summary>
    [DebuggerDisplay("ID = {Id}")]
    public class Transaction : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Backing field для свойства TranType
        /// </summary>
        private TransactionType _tranType;

        /// <summary>
        ///     Конструктор с праметором
        /// </summary>
        /// <param name="id">КодТранзакции</param>
        public Transaction(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Инициализация объекта "Транзакции"
        /// </summary>
        public Transaction()
        {
        }

        /// <summary>
        ///     Получение типа транзакции
        /// </summary>
        public TransactionType TranType
        {
            get
            {
                if (_tranType == null && CodeType > 0)
                    _tranType = new TransactionType(Convert.ToString(CodeType));

                return _tranType;
            }
        }

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
        ///     Метод загрузки данных сущности "Транзакции"
        /// </summary>
        public sealed override void Load()
        {
            FillData(Code);
        }

        /// <summary>
        ///     Удаляет все связанные транзакции по коду документа
        /// </summary>
        /// <param name="id">КодДокумента</param>
        public static void RemoveTrans(string id)
        {
            try
            {
                var sqlParms = new Dictionary<string, object> {{"@CodeDoc", id.ToInt()}};
                DBManager.ExecuteNonQuery(SQLQueries.DELETE_Транзакции_ПоДокументуПодтверждения, CommandType.Text,
                    ConnString, sqlParms);
            }
            catch (Exception e)
            {
                Logger.WriteEx(e);
                throw e;
            }
        }

        /// <summary>
        ///     Получить данные о транзакции по КодуТранзакции
        /// </summary>
        /// <param name="id">КодТранзакции</param>
        public void FillData(int id)
        {
            if (id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Транзакция, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТранзакции = dbReader.GetOrdinal("КодТранзакции");
                    var colКодТипаТранзакции = dbReader.GetOrdinal("КодТипаТранзакции");
                    var colКодГруппыТиповТранзакций = dbReader.GetOrdinal("КодГруппыТиповТранзакций");
                    var colДата = dbReader.GetOrdinal("Дата");
                    var colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    var colКодДокументаПодтверждения = dbReader.GetOrdinal("КодДокументаПодтверждения");
                    var colКодДокументаДоговора = dbReader.GetOrdinal("КодДокументаДоговора");
                    var colКодДокументаПриложения = dbReader.GetOrdinal("КодДокументаПриложения");
                    var colКодДокументаСФ = dbReader.GetOrdinal("КодДокументаСФ");
                    var colКодЛицаДО = dbReader.GetOrdinal("КодЛицаДО");
                    var colКодЛицаПОСЛЕ = dbReader.GetOrdinal("КодЛицаПОСЛЕ");
                    var colКодСкладаДО = dbReader.GetOrdinal("КодСкладаДО");
                    var colКодСкладаПОСЛЕ = dbReader.GetOrdinal("КодСкладаПОСЛЕ");
                    var colКодХранителяДо = dbReader.GetOrdinal("КодХранителяДо");
                    var colКодХранителяПосле = dbReader.GetOrdinal("КодХранителяПосле");
                    var colКодБизнесПроектаДО = dbReader.GetOrdinal("КодБизнесПроектаДО");
                    var colКодБизнесПроектаПОСЛЕ = dbReader.GetOrdinal("КодБизнесПроектаПОСЛЕ");
                    var colСуммаРуб = dbReader.GetOrdinal("СуммаРуб");
                    var colКодВалюты = dbReader.GetOrdinal("КодВалюты");
                    var colСумма = dbReader.GetOrdinal("Сумма");
                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colКоличество = dbReader.GetOrdinal("Количество");
                    var colПримечание = dbReader.GetOrdinal("Примечание");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    Unavailable = false;
                    if (dbReader.Read())
                    {
                        Id = dbReader.GetInt32(colКодТранзакции).ToString();
                        CodeType = dbReader.GetInt32(colКодТипаТранзакции);
                        CodeTypeGroup = dbReader.GetInt32(colКодГруппыТиповТранзакций);
                        Date = dbReader.GetDateTime(colДата);
                        if (!dbReader.IsDBNull(colКодДокументаОснования))
                            CodeDocBase = dbReader.GetInt32(colКодДокументаОснования);
                        if (!dbReader.IsDBNull(colКодДокументаПодтверждения))
                            CodeDocConfirm = dbReader.GetInt32(colКодДокументаПодтверждения);
                        if (!dbReader.IsDBNull(colКодДокументаДоговора))
                            CodeDocContract = dbReader.GetInt32(colКодДокументаДоговора);
                        if (!dbReader.IsDBNull(colКодДокументаПриложения))
                            CodeDocAttachment = dbReader.GetInt32(colКодДокументаПриложения);
                        if (!dbReader.IsDBNull(colКодДокументаСФ)) CodeDocSF = dbReader.GetInt32(colКодДокументаСФ);
                        CodePersonBefore = dbReader.GetInt32(colКодЛицаДО);
                        CodePersonAfter = dbReader.GetInt32(colКодЛицаПОСЛЕ);
                        if (!dbReader.IsDBNull(colКодСкладаДО)) CodeStoreBefore = dbReader.GetInt32(colКодСкладаДО);
                        if (!dbReader.IsDBNull(colКодСкладаПОСЛЕ))
                            CodeStoreAfter = dbReader.GetInt32(colКодСкладаПОСЛЕ);
                        if (!dbReader.IsDBNull(colКодХранителяДо))
                            CodeKeeperBefore = dbReader.GetInt32(colКодХранителяДо);
                        if (!dbReader.IsDBNull(colКодХранителяПосле))
                            CodeKeeperAfter = dbReader.GetInt32(colКодХранителяПосле);
                        if (!dbReader.IsDBNull(colКодБизнесПроектаДО))
                            CodeProjectBefore = dbReader.GetInt32(colКодБизнесПроектаДО);
                        if (!dbReader.IsDBNull(colКодБизнесПроектаПОСЛЕ))
                            CodeProjectAfter = dbReader.GetInt32(colКодБизнесПроектаПОСЛЕ);
                        SumRub = dbReader.GetDecimal(colСуммаРуб);
                        CodeCurrency = dbReader.GetInt32(colКодВалюты);
                        Sum = dbReader.GetDecimal(colСумма);
                        CodeResource = dbReader.GetInt32(colКодРесурса);
                        Amount = dbReader.GetDecimal(colКоличество);
                        Description = dbReader.GetString(colПримечание);
                        ChangeBy = dbReader.GetInt32(colИзменил);
                        ChangeDate = dbReader.GetDateTime(colИзменено);
                    }
                    else
                    {
                        Unavailable = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Получить список транзакций по коду документа
        /// </summary>
        /// <param name="id">КодДокумента</param>
        public static List<Transaction> GetTransactionsByDocId(int id)
        {
            var sqlParams = new Dictionary<string, object> {{"@CodeDoc", id}};
            return GetTransactionList(SQLQueries.SELECT_Транзакции_ПоДокументуПодтверждения, sqlParams);
        }

        /// <summary>
        ///     Получить список транзакций
        /// </summary>
        /// <param name="query">Строка запроса</param>
        /// <param name="sqlParams">Параметры запроса</param>
        public static List<Transaction> GetTransactionList(string query, Dictionary<string, object> sqlParams = null)
        {
            var list = new List<Transaction>();

            using (var dbReader = new DBReader(query, CommandType.Text, ConnString, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодТранзакции = dbReader.GetOrdinal("КодТранзакции");
                    var colКодТипаТранзакции = dbReader.GetOrdinal("КодТипаТранзакции");
                    var colКодГруппыТиповТранзакций = dbReader.GetOrdinal("КодГруппыТиповТранзакций");
                    var colДата = dbReader.GetOrdinal("Дата");
                    var colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    var colКодДокументаПодтверждения = dbReader.GetOrdinal("КодДокументаПодтверждения");
                    var colКодДокументаДоговора = dbReader.GetOrdinal("КодДокументаДоговора");
                    var colКодДокументаПриложения = dbReader.GetOrdinal("КодДокументаПриложения");
                    var colКодДокументаСФ = dbReader.GetOrdinal("КодДокументаСФ");
                    var colКодЛицаДО = dbReader.GetOrdinal("КодЛицаДО");
                    var colКодЛицаПОСЛЕ = dbReader.GetOrdinal("КодЛицаПОСЛЕ");
                    var colКодСкладаДО = dbReader.GetOrdinal("КодСкладаДО");
                    var colКодСкладаПОСЛЕ = dbReader.GetOrdinal("КодСкладаПОСЛЕ");
                    var colКодХранителяДо = dbReader.GetOrdinal("КодХранителяДо");
                    var colКодХранителяПосле = dbReader.GetOrdinal("КодХранителяПосле");
                    var colКодБизнесПроектаДО = dbReader.GetOrdinal("КодБизнесПроектаДО");
                    var colКодБизнесПроектаПОСЛЕ = dbReader.GetOrdinal("КодБизнесПроектаПОСЛЕ");
                    var colСуммаРуб = dbReader.GetOrdinal("СуммаРуб");
                    var colКодВалюты = dbReader.GetOrdinal("КодВалюты");
                    var colСумма = dbReader.GetOrdinal("Сумма");
                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colКоличество = dbReader.GetOrdinal("Количество");
                    var colПримечание = dbReader.GetOrdinal("Примечание");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new Transaction();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодТранзакции).ToString();
                        row.CodeType = dbReader.GetInt32(colКодТипаТранзакции);
                        row.CodeTypeGroup = dbReader.GetInt32(colКодГруппыТиповТранзакций);
                        row.Date = dbReader.GetDateTime(colДата);
                        if (!dbReader.IsDBNull(colКодДокументаОснования))
                            row.CodeDocConfirm = dbReader.GetInt32(colКодДокументаОснования);
                        if (!dbReader.IsDBNull(colКодДокументаПодтверждения))
                            row.CodeDocConfirm = dbReader.GetInt32(colКодДокументаПодтверждения);
                        if (!dbReader.IsDBNull(colКодДокументаДоговора))
                            row.CodeDocContract = dbReader.GetInt32(colКодДокументаДоговора);
                        if (!dbReader.IsDBNull(colКодДокументаПриложения))
                            row.CodeDocAttachment = dbReader.GetInt32(colКодДокументаПриложения);
                        if (!dbReader.IsDBNull(colКодДокументаСФ)) row.CodeDocSF = dbReader.GetInt32(colКодДокументаСФ);
                        row.CodePersonBefore = dbReader.GetInt32(colКодЛицаДО);
                        row.CodePersonAfter = dbReader.GetInt32(colКодЛицаПОСЛЕ);
                        if (!dbReader.IsDBNull(colКодСкладаДО)) row.CodeStoreBefore = dbReader.GetInt32(colКодСкладаДО);
                        if (!dbReader.IsDBNull(colКодСкладаПОСЛЕ))
                            row.CodeStoreAfter = dbReader.GetInt32(colКодСкладаПОСЛЕ);
                        if (!dbReader.IsDBNull(colКодХранителяДо))
                            row.CodeKeeperBefore = dbReader.GetInt32(colКодХранителяДо);
                        if (!dbReader.IsDBNull(colКодХранителяПосле))
                            row.CodeKeeperAfter = dbReader.GetInt32(colКодХранителяПосле);
                        if (!dbReader.IsDBNull(colКодБизнесПроектаДО))
                            row.CodeProjectBefore = dbReader.GetInt32(colКодБизнесПроектаДО);
                        if (!dbReader.IsDBNull(colКодБизнесПроектаПОСЛЕ))
                            row.CodeProjectAfter = dbReader.GetInt32(colКодБизнесПроектаПОСЛЕ);
                        row.SumRub = dbReader.GetDecimal(colСуммаРуб);
                        row.CodeCurrency = dbReader.GetInt32(colКодВалюты);
                        row.Sum = dbReader.GetDecimal(colСумма);
                        row.CodeResource = dbReader.GetInt32(colКодРесурса);
                        row.Amount = dbReader.GetDecimal(colКоличество);
                        row.Description = dbReader.GetString(colПримечание);
                        row.ChangeBy = dbReader.GetInt32(colИзменил);
                        row.ChangeDate = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        #region Поля сущности

        /// <summary>
        ///     Поле Код транзакции
        /// </summary>
        /// <value>
        ///     КодТранзакции (int, not null)
        /// </value>
        /// <remarks>
        ///     Типизированный псевданим для ID
        /// </remarks>
        public int Code => Id.ToInt();

        /// <summary>
        ///     Поле КодТипаТранзакции
        /// </summary>
        /// <value>
        ///     КодТипаТранзакции (int, not null)
        /// </value>
        public int CodeType { get; set; }

        /// <summary>
        ///     Поле КодГруппыТиповТранзакций
        /// </summary>
        /// <value>
        ///     КодГруппыТиповТранзакций (int, not null)
        /// </value>
        public int CodeTypeGroup { get; set; }

        /// <summary>
        ///     Поле Дата
        /// </summary>
        /// <value>
        ///     Дата (datetime, not null)
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Поле КодДокументаОснования
        /// </summary>
        /// <value>
        ///     КодДокументаОснования (int, null)
        /// </value>
        public int CodeDocBase { get; set; }

        /// <summary>
        ///     Поле КодДокументаПодтверждения
        /// </summary>
        /// <value>
        ///     КодДокументаПодтверждения (int, null)
        /// </value>
        public int CodeDocConfirm { get; set; }

        /// <summary>
        ///     Поле КодДокументаДоговора
        /// </summary>
        /// <value>
        ///     КодДокументаДоговора (int, null)
        /// </value>
        public int CodeDocContract { get; set; }

        /// <summary>
        ///     Поле КодДокументаПриложения
        /// </summary>
        /// <value>
        ///     КодДокументаПриложения (int, null)
        /// </value>
        public int CodeDocAttachment { get; set; }

        /// <summary>
        ///     Поле КодДокументаСФ
        /// </summary>
        /// <value>
        ///     КодДокументаСФ (int, null)
        /// </value>
        public int CodeDocSF { get; set; }

        /// <summary>
        ///     Поле КодЛицаДО
        /// </summary>
        /// <value>
        ///     КодЛицаДО (int, not null)
        /// </value>
        public int CodePersonBefore { get; set; }

        /// <summary>
        ///     Поле КодЛицаПОСЛЕ
        /// </summary>
        /// <value>
        ///     КодЛицаПОСЛЕ (int, not null)
        /// </value>
        public int CodePersonAfter { get; set; }

        /// <summary>
        ///     Поле КодСкладаДО
        /// </summary>
        /// <value>
        ///     КодСкладаДО (int, null)
        /// </value>
        public int CodeStoreBefore { get; set; }

        /// <summary>
        ///     Поле КодСкладаПОСЛЕ
        /// </summary>
        /// <value>
        ///     КодСкладаПОСЛЕ (int, null)
        /// </value>
        public int CodeStoreAfter { get; set; }

        /// <summary>
        ///     Поле КодХранителяДо
        /// </summary>
        /// <value>
        ///     КодХранителяДо (int, null)
        /// </value>
        public int CodeKeeperBefore { get; set; }

        /// <summary>
        ///     Поле КодХранителяПосле
        /// </summary>
        /// <value>
        ///     КодХранителяПосле (int, null)
        /// </value>
        public int CodeKeeperAfter { get; set; }

        /// <summary>
        ///     Поле КодБизнесПроектаДО
        /// </summary>
        /// <value>
        ///     КодБизнесПроектаДО (int, null)
        /// </value>
        public int CodeProjectBefore { get; set; }

        /// <summary>
        ///     Поле КодБизнесПроектаПОСЛЕ
        /// </summary>
        /// <value>
        ///     КодБизнесПроектаПОСЛЕ (int, null)
        /// </value>
        public int CodeProjectAfter { get; set; }

        /// <summary>
        ///     Поле СуммаРуб
        /// </summary>
        /// <value>
        ///     СуммаРуб (money, not null)
        /// </value>
        public decimal SumRub { get; set; }

        /// <summary>
        ///     Поле КодВалюты
        /// </summary>
        /// <value>
        ///     КодВалюты (int, not null)
        /// </value>
        public int CodeCurrency { get; set; }

        /// <summary>
        ///     Поле Сумма
        /// </summary>
        /// <value>
        ///     Сумма (money, not null)
        /// </value>
        public decimal Sum { get; set; }

        /// <summary>
        ///     Поле КодРесурса
        /// </summary>
        /// <value>
        ///     КодРесурса (int, not null)
        /// </value>
        public int CodeResource { get; set; }

        /// <summary>
        ///     Поле Количество
        /// </summary>
        /// <value>
        ///     Количество (money, not null)
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        ///     Поле Примечание
        /// </summary>
        /// <value>
        ///     Примечание (varchar(1000), not null)
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     Поле Изменил
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        public int ChangeBy { get; set; }

        /// <summary>
        ///     Поле  Изменено
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        public DateTime ChangeDate { get; set; }

        #endregion
    }
}