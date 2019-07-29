using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.MTR
{
    /// <summary>
    ///     Класс для обеспечения функциональности MTRClaimDoc
    ///     и получения вытекающих документов МТР
    /// </summary>
    /// <remarks>
    ///     Не явялется ральной сущностью БД, разные данные и разных таблиц
    /// </remarks>
    [DebuggerDisplay("DocId = {DocId}, LinkType = {LinkType}")]
    [Serializable]
    public class MtrChildDoc : Entity, ICloneable<MtrChildDoc>
    {
        /// <summary>
        ///     Получить все платежные документы основания
        /// </summary>
        private const string GetPayDoc = @"
DECLARE @СчетаДоговоры TABLE(КодДокумента INT, КодТипаДокумента INT, ТипДокумента varchar(100), НомерДокумента varchar(50), ДатаДокумента datetime, Money1 money, КодДокументаОснования int, КодРесурса1 int)
DECLARE @ТипыДоговоров TABLE(КодТипаДокумента INT)
DECLARE @ПриложденияТипы TABLE(КодТипаДокумента INT)

-- Порядок поиска
-- 1) Ищем счета по МТР
-- 2) ищем договора и приложения по счетам из п.1
-- 3) Ищем приложение по МТР
-- 4) Удаляем и наденного в п.3 позиции, те которые уже есть еще найденны в п.2
-- 5) Ищем договора по МТР
-- 6) Удаляем из наденных договоров в п.5, те которые уже есть еще найденны в п.2

-- получаем всех наследников договора
  INSERT @ТипыДоговоров
  SELECT Child.КодТипаДокумента
  FROM [ТипыДокументов] Child
  INNER JOIN [ТипыДокументов] Parent ON Child.L >= Parent.L AND Child.R <= Parent.R
  WHERE Parent.КодТипаДокумента = 2039 AND Child.ТипДокумента IS NOT NULL  
  
  INSERT INTO @ПриложденияТипы(КодТипаДокумента)VALUES(2110)
  INSERT INTO @ПриложденияТипы(КодТипаДокумента)VALUES(2324)
  
-- ищем счета и инвойс проформа привязаные к МТР
INSERT @СчетаДоговоры(КодДокумента, КодТипаДокумента, ТипДокумента, НомерДокумента, ДатаДокумента, Money1, КодДокументаОснования, КодРесурса1)
SELECT ДД._КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1
FROM vwСвязиДокументов Связи (nolock)
		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента
		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента =  ТД.КодТипаДокумента	 
WHERE КодДокументаОснования = @Id AND ДД.КодТипаДокумента IN(2283, 2284)

-- ищем договора и приложения связанные со счетом 
IF EXISTS(SELECT 1 FROM @СчетаДоговоры)
  INSERT @СчетаДоговоры(КодДокумента, КодТипаДокумента, ТипДокумента, НомерДокумента, ДатаДокумента, Money1, КодДокументаОснования, КодРесурса1)
  SELECT ДД._КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1
  FROM vwСвязиДокументов Связи (nolock)
		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента 
		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента =  ТД.КодТипаДокумента	 
  WHERE КодДокументаОснования IN (SELECT s.КодДокумента FROM @СчетаДоговоры s)
  AND ДД.КодТипаДокумента IN(SELECT КодТипаДокумента FROM @ТипыДоговоров UNION SELECT КодТипаДокумента FROM @ПриложденияТипы)
 
-- отдельно ищем приложения по МТР  
DECLARE @ПриложенияМТР TABLE(КодДокумента INT, КодТипаДокумента INT, ТипДокумента varchar(100), НомерДокумента varchar(50), ДатаДокумента datetime, Money1 money, КодДокументаОснования int, КодРесурса1 int)

INSERT @ПриложенияМТР(КодДокумента, КодТипаДокумента, ТипДокумента, НомерДокумента, ДатаДокумента, Money1, КодДокументаОснования, КодРесурса1)
SELECT ДД._КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1
FROM vwСвязиДокументов Связи (nolock)
		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента 
		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента =  ТД.КодТипаДокумента	 
WHERE КодДокументаОснования = @Id AND ДД.КодТипаДокумента IN (SELECT КодТипаДокумента FROM @ПриложденияТипы) 

-- Удаляем и наденного в п.3 позиции, те которые уже есть еще найденны в п.2
DELETE p
FROM @ПриложенияМТР p
WHERE p.КодДокумента IN(SELECT КодДокумента FROM @СчетаДоговоры)

-- Ищем договора по МТР
DECLARE @ДоговораМТР TABLE(КодДокумента INT, КодТипаДокумента INT, ТипДокумента varchar(100), НомерДокумента varchar(50), ДатаДокумента datetime, Money1 money, КодДокументаОснования int, КодРесурса1 int)

INSERT @ДоговораМТР(КодДокумента, КодТипаДокумента, ТипДокумента, НомерДокумента, ДатаДокумента, Money1, КодДокументаОснования, КодРесурса1)
SELECT ДД._КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1
FROM vwСвязиДокументов Связи (nolock)
		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента
		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента =  ТД.КодТипаДокумента	 
WHERE КодДокументаОснования = @Id AND ДД.КодТипаДокумента IN (SELECT КодТипаДокумента FROM @ТипыДоговоров) 

-- Удаляем из наденных договоров в п.5, те которые уже есть еще найденны в п.2

DELETE d
FROM @ДоговораМТР d
WHERE d.КодДокумента IN (SELECT КодДокумента FROM @СчетаДоговоры)

--Pезультирующий селект для оснований
SELECT * FROM @СчетаДоговоры
UNION ALL
SELECT * FROM @ПриложенияМТР
UNION ALL
SELECT * FROM @ДоговораМТР";

        /// <summary>
        ///     Получить платежные документы(документы оплаты) и ТТН
        /// </summary>
        public const string SQLGetPayDocs =
            @"SELECT ДД._КодДокумента as КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1
  FROM vwСвязиДокументов Связи (nolock)
  		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента
  		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента = ТД.КодТипаДокумента	 
  WHERE КодДокументаОснования = @Id AND ДД.КодТипаДокумента IN(2066, 2259, 2145) -- Платежное поручение, swift, ТТН";

        /// <summary>
        ///     SQL запрос: Получить все связаные документы с позицией
        /// </summary>
        /// <remarks>столбец ТипСвязанногоДокумента это соответствие MtrChildType</remarks>
        public const string SQLGetAllLinkedDocs = @"
DECLARE @ОснованиеОплаты TABLE (ДокументОснованиеОплаты INT, КодПозицииЗаявокМТРСвязи INT, КодПозицииЗаявокМТР INT, Количество decimal(15,4))

INSERT INTO @ОснованиеОплаты
SELECT КодДокументаОснованияОплаты, КодПозицииЗаявокМТРСвязи, КодПозицииЗаявокМТР, Количество
FROM dbo.vwПозицииЗаявокМТРСвязи 
WHERE КодПозицииЗаявокМТР IN (SELECT [КодПозицииЗаявокМТР] FROM [dbo].[vwПозицииЗаявокМТР] (nolock) WHERE КодДокумента = @id)


IF EXISTS(SELECT * FROM @ОснованиеОплаты)
BEGIN
	DECLARE @СчетаДоговоры TABLE(КодДокумента INT, КодТипаДокумента INT, ТипДокумента varchar(100), НомерДокумента varchar(50), ДатаДокумента datetime, Money1 money, КодДокументаОснования int, КодРесурса1 INT, КодПозицииЗаявокМТРСвязи INT, КодПозицииЗаявокМТР INT, ТипСвязанногоДокумента INT, Количество decimal(15,4))
	
	INSERT INTO @СчетаДоговоры
	SELECT ДД._КодДокумента as КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, @id, ДД.КодРесурса1, O.КодПозицииЗаявокМТРСвязи, O.КодПозицииЗаявокМТР, 1 AS ТипСвязанногоДокумента, O.Количество
	FROM    vwДокументыДокументыДанные ДД (nolock)
			INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента =  ТД.КодТипаДокумента
			INNER JOIN @ОснованиеОплаты AS O ON ДД._КодДокумента = O.ДокументОснованиеОплаты

	SELECT * FROM @СчетаДоговоры
	
	UNION ALL
	
    SELECT ДД._КодДокумента, ТД.КодТипаДокумента, ТД.ТипДокумента, ДД.НомерДокумента, ДД.ДатаДокумента, ДД.Money1, Связи.КодДокументаОснования, ДД.КодРесурса1, 0 as КодПозицииЗаявокМТРСвязи, СЧ.КодПозицииЗаявокМТР,
    CASE WHEN ДД.КодТипаДокумента IN (2066, 2259) THEN 2
         WHEN ДД.КодТипаДокумента = 2145 THEN 3
         ELSE 0
         END AS ТипСвязанногоДокумента,
         0 as Количество
    FROM vwСвязиДокументов Связи (nolock)
  		INNER JOIN vwДокументыДокументыДанные ДД (nolock) ON Связи.КодДокументаВытекающего = ДД._КодДокумента
  		INNER JOIN ТипыДокументов ТД (nolock) ON ДД.КодТипаДокумента = ТД.КодТипаДокумента
  		INNER JOIN @СчетаДоговоры СЧ ON Связи.КодДокументаОснования = СЧ.КодДокумента	 
    WHERE ДД.КодТипаДокумента IN(2066, 2259, 2145) -- Платежное поручение, swift, ТТН
END";

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;


        private string _documentName;

        /// <summary>
        ///     ID документа
        /// </summary>
        public int DocId { get; set; }

        /// <summary>
        ///     Тип связанного документа
        /// </summary>
        public MtrChildType LinkType { get; set; }

        /// <summary>
        ///     ID типа документа
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        ///     Дата документа
        /// </summary>
        public DateTime DocDate { get; set; }

        /// <summary>
        ///     Сумма по документу
        /// </summary>
        public decimal? Money { get; set; }

        /// <summary>
        ///     Сумма в строке
        /// </summary>
        public string MoneyString
        {
            get
            {
                if (Money != null)
                    return Money.Value.ToString("N2");

                return "";
            }
        }

        /// <summary>
        ///     Код документа основания
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        ///     Код ресурса - код валюты
        /// </summary>
        public int? ResourceId { get; set; }

        /// <summary>
        ///     Наименование валюты рус
        /// </summary>
        public string CurrencyRur { get; set; }

        /// <summary>
        ///     Наименование валюты англ
        /// </summary>
        public string CurrencyEng { get; set; }

        /// <summary>
        ///     Наименование типа
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///     Номер документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     Название документа
        /// </summary>
        public string DocumentName
        {
            get
            {
                return string.IsNullOrEmpty(_documentName)
                    ? TypeName + " №" + Number + " от " + DocDate.ToString("dd.MM.yyyy")
                    : _documentName;
            }
            set { _documentName = value; }
        }

        /// <summary>
        ///     Интерфейсное свойство
        ///     Частичная оплата, количество
        /// </summary>
        public decimal PartialQuantity { get; set; }

        /// <summary>
        ///     Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_document
            : _connectionString;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public MtrChildDoc Clone()
        {
            return (MtrChildDoc) MemberwiseClone();
        }

        /// <summary>
        ///     Получить все связаные документы с позицией
        /// </summary>
        /// <param name="mtrDocId">Код документа МТР</param>
        public static List<MtrChildDoc> GetAllLinkedDocs(int mtrDocId)
        {
            var allDocs = new List<MtrChildDoc>();

            if (mtrDocId == 0) return allDocs;

            using (var dbReader = new DBReader(SQLGetAllLinkedDocs, mtrDocId, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                    var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                    var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    var colMoney1 = dbReader.GetOrdinal("Money1");
                    var colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                    var colКодРесурса1 = dbReader.GetOrdinal("КодРесурса1");
                    var colКодПозицииЗаявокМТРСвязи = dbReader.GetOrdinal("КодПозицииЗаявокМТРСвязи");
                    var colКодПозицииЗаявокМТР = dbReader.GetOrdinal("КодПозицииЗаявокМТР");
                    var colТипСвязанногоДокумента = dbReader.GetOrdinal("ТипСвязанногоДокумента");
                    var colКоличество = dbReader.GetOrdinal("Количество");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new MtrChildDoc();
                        if (!dbReader.IsDBNull(colКодДокумента)) row.DocId = dbReader.GetInt32(colКодДокумента);
                        if (!dbReader.IsDBNull(colКодТипаДокумента))
                            row.TypeID = dbReader.GetInt32(colКодТипаДокумента);
                        if (!dbReader.IsDBNull(colТипДокумента)) row.TypeName = dbReader.GetString(colТипДокумента);
                        if (!dbReader.IsDBNull(colНомерДокумента)) row.Number = dbReader.GetString(colНомерДокумента);
                        if (!dbReader.IsDBNull(colДатаДокумента)) row.DocDate = dbReader.GetDateTime(colДатаДокумента);
                        if (!dbReader.IsDBNull(colMoney1)) row.Money = dbReader.GetDecimal(colMoney1);
                        if (!dbReader.IsDBNull(colКодДокументаОснования))
                            row.ParentId = dbReader.GetInt32(colКодДокументаОснования);
                        if (!dbReader.IsDBNull(colКодРесурса1)) row.ResourceId = dbReader.GetInt32(colКодРесурса1);
                        if (!dbReader.IsDBNull(colКодПозицииЗаявокМТРСвязи))
                            row.PositionLinkId = dbReader.GetInt32(colКодПозицииЗаявокМТРСвязи);
                        if (!dbReader.IsDBNull(colКодПозицииЗаявокМТР))
                            row.MtrPositionId = dbReader.GetInt32(colКодПозицииЗаявокМТР);
                        if (!dbReader.IsDBNull(colТипСвязанногоДокумента))
                            row.LinkType = (MtrChildType) dbReader.GetInt32(colТипСвязанногоДокумента);
                        if (!dbReader.IsDBNull(colКоличество)) row.PartialQuantity = dbReader.GetDecimal(colКоличество);

                        allDocs.Add(row);
                    }
                }
            }

            return allDocs;
        }

        /// <summary>
        ///     Получить платежные документы, ТТН на основе документа
        /// </summary>
        /// <param name="payBasisDocId">Документ основание платежа(счет, договор, приложение к договору)</param>
        public static List<MtrChildDoc> GetLinkedDocs(int payBasisDocId)
        {
            var pl = new List<MtrChildDoc>();

            if (payBasisDocId == 0) return pl;

            using (var dbReader = new DBReader(SQLGetPayDocs, payBasisDocId, CommandType.Text, ConnString))
            {
                #region Получение порядкового номера столбца

                var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                var colMoney1 = dbReader.GetOrdinal("Money1");
                var colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                var colКодРесурса1 = dbReader.GetOrdinal("КодРесурса1");

                #endregion

                while (dbReader.Read())
                {
                    var row = new MtrChildDoc();
                    if (!dbReader.IsDBNull(colКодДокумента)) row.DocId = dbReader.GetInt32(colКодДокумента);
                    if (!dbReader.IsDBNull(colКодТипаДокумента)) row.TypeID = dbReader.GetInt32(colКодТипаДокумента);
                    if (!dbReader.IsDBNull(colТипДокумента)) row.TypeName = dbReader.GetString(colТипДокумента);
                    if (!dbReader.IsDBNull(colНомерДокумента)) row.Number = dbReader.GetString(colНомерДокумента);
                    if (!dbReader.IsDBNull(colДатаДокумента)) row.DocDate = dbReader.GetDateTime(colДатаДокумента);
                    if (!dbReader.IsDBNull(colMoney1)) row.Money = dbReader.GetDecimal(colMoney1);
                    if (!dbReader.IsDBNull(colКодДокументаОснования))
                        row.ParentId = dbReader.GetInt32(colКодДокументаОснования);
                    if (!dbReader.IsDBNull(colКодРесурса1)) row.ResourceId = dbReader.GetInt32(colКодРесурса1);

                    if (row.TypeID == (int) DocTypeEnum.ТоварноТранспортнаяНакладная)
                        row.LinkType = MtrChildType.ДокументТТН;
                    else
                        row.LinkType = MtrChildType.ДокументОплаты;

                    pl.Add(row);
                }
            }

            return pl;
        }

        /// <summary>
        ///     Получить платежные документы (счета, договора, приложения) не распределенные
        /// </summary>
        /// <returns>Связаные документы</returns>
        public static List<MtrChildDoc> GetPayDocumentsNotDistributed(int docId)
        {
            var sl = new List<MtrChildDoc>();

            if (docId == 0) return sl;

            using (var dbReader = new DBReader(GetPayDoc, docId, CommandType.Text, ConnString))
            {
                #region Получение порядкового номера столбца

                var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                var colКодТипаДокумента = dbReader.GetOrdinal("КодТипаДокумента");
                var colТипДокумента = dbReader.GetOrdinal("ТипДокумента");
                var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                var colMoney1 = dbReader.GetOrdinal("Money1");
                var colКодДокументаОснования = dbReader.GetOrdinal("КодДокументаОснования");
                var colКодРесурса1 = dbReader.GetOrdinal("КодРесурса1");

                #endregion

                while (dbReader.Read())
                {
                    var row = new MtrChildDoc();
                    if (!dbReader.IsDBNull(colКодДокумента)) row.DocId = dbReader.GetInt32(colКодДокумента);
                    if (!dbReader.IsDBNull(colКодТипаДокумента)) row.TypeID = dbReader.GetInt32(colКодТипаДокумента);
                    if (!dbReader.IsDBNull(colТипДокумента)) row.TypeName = dbReader.GetString(colТипДокумента);
                    row.Number = dbReader.GetString(colНомерДокумента);
                    if (!dbReader.IsDBNull(colДатаДокумента)) row.DocDate = dbReader.GetDateTime(colДатаДокумента);
                    if (!dbReader.IsDBNull(colMoney1)) row.Money = dbReader.GetDecimal(colMoney1);
                    if (!dbReader.IsDBNull(colКодДокументаОснования))
                        row.ParentId = dbReader.GetInt32(colКодДокументаОснования);
                    if (!dbReader.IsDBNull(colКодРесурса1)) row.ResourceId = dbReader.GetInt32(colКодРесурса1);
                    sl.Add(row);
                }
            }

            SetCurrencies(sl);

            return sl;
        }

        /// <summary>
        ///     Установить значение валюты
        /// </summary>
        private static void SetCurrencies(List<MtrChildDoc> docs)
        {
            if (docs.Count > 0)
            {
                // валюты кешируются
                var curs = Currency.GetAllCurrencies();

                foreach (var d in docs)
                    if (d.ResourceId != null && curs.ContainsKey(d.ResourceId.Value))
                    {
                        var c = curs[d.ResourceId.Value];
                        d.CurrencyRur = c.UnitRus;
                        d.CurrencyEng = c.UnitEng;
                    }
            }
        }

        /// <summary>
        ///     Метод создания связи документов МТР
        /// </summary>
        public void Create()
        {
            if (MtrPositionId == 0)
                throw new ArgumentException("Ошибка создания связи МТР: MtrPositionId == 0");

            if (DocId == 0)
                throw new ArgumentException("Ошибка создания связи МТР: DocId == 0");

            var param = new Dictionary<string, object>();
            param.Add("@КодПозицииЗаявокМТР", MtrPositionId);
            param.Add("@КодДокументаОснованияОплаты", DocId);
            param.Add("@Количество", PartialQuantity);

            var output = new Dictionary<string, object>();
            output.Add("@КодПозицииЗаявокМТРСвязи", -1);

            DBManager.ExecuteNonQuery(SQLQueries.INSERT_ПозицииЗаявокМТРСвязи, CommandType.Text, CN, param, output);

            PositionLinkId = Convert.ToInt32(output["@КодПозицииЗаявокМТРСвязи"]);
        }

        /// <summary>
        ///     Удалить связи документов МТР
        /// </summary>
        public void Delete()
        {
            if (PositionLinkId == 0)
                throw new ArgumentException("Ошибка удаления связи МТР: PositionLinkId == 0");

            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_ПозицияЗаявкиМТРСвязи, PositionLinkId, CommandType.Text, CN);
        }

        /// <summary>
        ///     Создает новый объекты и новую коллекцию, с идентичными данными но разными ссылками
        /// </summary>
        public static List<MtrChildDoc> CloneCollection(List<MtrChildDoc> col)
        {
            if (col != null && col.Count > 0)
            {
                var cloneCol = new List<MtrChildDoc>(col.Count);
                cloneCol.AddRange(col.Select(c => c.Clone()));

                return cloneCol;
            }

            return new List<MtrChildDoc>();
        }


        #region Поля сущности таблицы связей позиций и документов

        /// <summary>
        ///     Код связи документов и позиции заявок МТР
        /// </summary>
        /// <value>
        ///     КодПозицииЗаявокМТРСвязи (int, not null)
        /// </value>
        public int PositionLinkId { get; set; }

        /// <summary>
        ///     Код позиции заявок МТР
        /// </summary>
        /// <value>
        ///     КодПозицииЗаявокМТР (int, not null)
        /// </value>
        public int MtrPositionId { get; set; }

        #endregion
    }
}