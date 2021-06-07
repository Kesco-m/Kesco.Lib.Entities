using System;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///     Класс сущности "Документы данные"
    /// </summary>
    /// <example>
    ///     Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}")]
    public class DocumentData : Entity, ICloneable<DocumentData>
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Инициализация объекта Документ по ID
        /// </summary>
        /// <param name="id">ID документа</param>
        public DocumentData(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Инициализация объекта Документ
        /// </summary>
        public DocumentData()
        {
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_document;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public DocumentData Clone()
        {
            return (DocumentData) MemberwiseClone();
        }

        /// <summary>
        ///     Метод загрузки данных сущности "ДокументыДанные"
        /// </summary>
        public sealed override void Load()
        {
            if (!Id.IsNullEmptyOrZero())
                FillData(DocCode);
        }

        /// <summary>
        ///     Метод загрузки и заполнения данных сущности "ДокументыДанные"
        /// </summary>
        public void FillData(int id)
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ДокументДанные, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");
                    var colКодЛица1 = dbReader.GetOrdinal("КодЛица1");
                    var colКодЛица2 = dbReader.GetOrdinal("КодЛица2");
                    var colКодЛица3 = dbReader.GetOrdinal("КодЛица3");
                    var colКодЛица4 = dbReader.GetOrdinal("КодЛица4");
                    var colКодЛица5 = dbReader.GetOrdinal("КодЛица5");
                    var colКодЛица6 = dbReader.GetOrdinal("КодЛица6");
                    var colКодСклада1 = dbReader.GetOrdinal("КодСклада1");
                    var colКодСклада2 = dbReader.GetOrdinal("КодСклада2");
                    var colКодСклада3 = dbReader.GetOrdinal("КодСклада3");
                    var colКодСклада4 = dbReader.GetOrdinal("КодСклада4");
                    var colКодРесурса1 = dbReader.GetOrdinal("КодРесурса1");
                    var colКодРесурса2 = dbReader.GetOrdinal("КодРесурса2");
                    var colКодСотрудника1 = dbReader.GetOrdinal("КодСотрудника1");
                    var colКодСотрудника2 = dbReader.GetOrdinal("КодСотрудника2");
                    var colКодСотрудника3 = dbReader.GetOrdinal("КодСотрудника3");
                    var colКодРасположения1 = dbReader.GetOrdinal("КодРасположения1");
                    var colКодРасположения2 = dbReader.GetOrdinal("КодРасположения2");
                    var colКодБазисаПоставки = dbReader.GetOrdinal("КодБазисаПоставки");
                    var colКодВидаТранспорта = dbReader.GetOrdinal("КодВидаТранспорта");
                    var colКодМестаХранения = dbReader.GetOrdinal("КодМестаХранения");
                    var colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    var colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    var colКодТУзла1 = dbReader.GetOrdinal("КодТУзла1");
                    var colКодТУзла2 = dbReader.GetOrdinal("КодТУзла2");
                    var colКодТерритории = dbReader.GetOrdinal("КодТерритории");
                    var colКодСтатьиБюджета = dbReader.GetOrdinal("КодСтатьиБюджета");
                    var colДата2 = dbReader.GetOrdinal("Дата2");
                    var colДата3 = dbReader.GetOrdinal("Дата3");
                    var colДата4 = dbReader.GetOrdinal("Дата4");
                    var colДата5 = dbReader.GetOrdinal("Дата5");
                    var colFlag1 = dbReader.GetOrdinal("Flag1");
                    var colFlag2 = dbReader.GetOrdinal("Flag2");
                    var colInt1 = dbReader.GetOrdinal("Int1");
                    var colInt2 = dbReader.GetOrdinal("Int2");
                    var colInt3 = dbReader.GetOrdinal("Int3");
                    var colInt4 = dbReader.GetOrdinal("Int4");
                    var colInt5 = dbReader.GetOrdinal("Int5");
                    var colInt6 = dbReader.GetOrdinal("Int6");
                    var colInt7 = dbReader.GetOrdinal("Int7");
                    var colText50_1 = dbReader.GetOrdinal("Text50_1");
                    var colText50_2 = dbReader.GetOrdinal("Text50_2");
                    var colText50_3 = dbReader.GetOrdinal("Text50_3");
                    var colText50_4 = dbReader.GetOrdinal("Text50_4");
                    var colText50_5 = dbReader.GetOrdinal("Text50_5");
                    var colText50_6 = dbReader.GetOrdinal("Text50_6");
                    var colText50_7 = dbReader.GetOrdinal("Text50_7");
                    var colText50_8 = dbReader.GetOrdinal("Text50_8");
                    var colText50_9 = dbReader.GetOrdinal("Text50_9");
                    var colText50_10 = dbReader.GetOrdinal("Text50_10");
                    var colText50_11 = dbReader.GetOrdinal("Text50_11");
                    var colText50_12 = dbReader.GetOrdinal("Text50_12");
                    var colText50_13 = dbReader.GetOrdinal("Text50_13");
                    var colText100_1 = dbReader.GetOrdinal("Text100_1");
                    var colText100_2 = dbReader.GetOrdinal("Text100_2");
                    var colText100_3 = dbReader.GetOrdinal("Text100_3");
                    var colText100_4 = dbReader.GetOrdinal("Text100_4");
                    var colText100_5 = dbReader.GetOrdinal("Text100_5");
                    var colText100_6 = dbReader.GetOrdinal("Text100_6");
                    var colText300_1 = dbReader.GetOrdinal("Text300_1");
                    var colText300_2 = dbReader.GetOrdinal("Text300_2");
                    var colText300_3 = dbReader.GetOrdinal("Text300_3");
                    var colText300_4 = dbReader.GetOrdinal("Text300_4");
                    var colText300_5 = dbReader.GetOrdinal("Text300_5");
                    var colText300_6 = dbReader.GetOrdinal("Text300_6");
                    var colText300_7 = dbReader.GetOrdinal("Text300_7");
                    var colText300_8 = dbReader.GetOrdinal("Text300_8");
                    var colText300_9 = dbReader.GetOrdinal("Text300_9");
                    var colText1000_1 = dbReader.GetOrdinal("Text1000_1");
                    var colText1000_2 = dbReader.GetOrdinal("Text1000_2");
                    var colMoney1 = dbReader.GetOrdinal("Money1");
                    var colMoney2 = dbReader.GetOrdinal("Money2");
                    var colMoney3 = dbReader.GetOrdinal("Money3");
                    var colMoney4 = dbReader.GetOrdinal("Money4");
                    var colMoney5 = dbReader.GetOrdinal("Money5");
                    var colMoney6 = dbReader.GetOrdinal("Money6");
                    var colMoney7 = dbReader.GetOrdinal("Money7");
                    var colMoney8 = dbReader.GetOrdinal("Money8");
                    var colMoney9 = dbReader.GetOrdinal("Money9");
                    var colDecimal1 = dbReader.GetOrdinal("Decimal1");
                    var colDecimal2 = dbReader.GetOrdinal("Decimal2");
                    var colFloat1 = dbReader.GetOrdinal("Float1");
                    var colТекстДокумента = dbReader.GetOrdinal("ТекстДокумента");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодДокумента).ToString();
                        ChangePersonID = dbReader.GetInt32(colИзменил);
                        ChangeDate = dbReader.GetDateTime(colИзменено);
                        if (!dbReader.IsDBNull(colКодЛица1)) PersonId1 = dbReader.GetInt32(colКодЛица1);
                        if (!dbReader.IsDBNull(colКодЛица2)) PersonId2 = dbReader.GetInt32(colКодЛица2);
                        if (!dbReader.IsDBNull(colКодЛица3)) PersonId3 = dbReader.GetInt32(colКодЛица3);
                        if (!dbReader.IsDBNull(colКодЛица4)) PersonId4 = dbReader.GetInt32(colКодЛица4);
                        if (!dbReader.IsDBNull(colКодЛица5)) PersonId5 = dbReader.GetInt32(colКодЛица5);
                        if (!dbReader.IsDBNull(colКодЛица6)) PersonId6 = dbReader.GetInt32(colКодЛица6);
                        if (!dbReader.IsDBNull(colКодСклада1)) StockId1 = dbReader.GetInt32(colКодСклада1);
                        if (!dbReader.IsDBNull(colКодСклада2)) StockId2 = dbReader.GetInt32(colКодСклада2);
                        if (!dbReader.IsDBNull(colКодСклада3)) StockId3 = dbReader.GetInt32(colКодСклада3);
                        if (!dbReader.IsDBNull(colКодСклада4)) StockId4 = dbReader.GetInt32(colКодСклада4);
                        if (!dbReader.IsDBNull(colКодРесурса1)) ResourceId1 = dbReader.GetInt32(colКодРесурса1);
                        if (!dbReader.IsDBNull(colКодРесурса2)) ResourceId2 = dbReader.GetInt32(colКодРесурса2);
                        if (!dbReader.IsDBNull(colКодСотрудника1)) EmployeeId1 = dbReader.GetInt32(colКодСотрудника1);
                        if (!dbReader.IsDBNull(colКодСотрудника2)) EmployeeId2 = dbReader.GetInt32(colКодСотрудника2);
                        if (!dbReader.IsDBNull(colКодСотрудника3)) EmployeeId3 = dbReader.GetInt32(colКодСотрудника3);
                        if (!dbReader.IsDBNull(colКодРасположения1))
                            LocationId1 = dbReader.GetInt32(colКодРасположения1);
                        if (!dbReader.IsDBNull(colКодРасположения2))
                            LocationId2 = dbReader.GetInt32(colКодРасположения2);
                        if (!dbReader.IsDBNull(colКодБазисаПоставки))
                            BaseDeliveryId = dbReader.GetInt32(colКодБазисаПоставки);
                        if (!dbReader.IsDBNull(colКодВидаТранспорта))
                            TransportTypeId = dbReader.GetInt32(colКодВидаТранспорта);
                        if (!dbReader.IsDBNull(colКодМестаХранения))
                            StorageLocationId = dbReader.GetInt32(colКодМестаХранения);
                        if (!dbReader.IsDBNull(colКодЕдиницыИзмерения))
                            UnitsId = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        if (!dbReader.IsDBNull(colКодСтавкиНДС)) NDSrateId = dbReader.GetInt32(colКодСтавкиНДС);
                        if (!dbReader.IsDBNull(colКодТУзла1)) UzelId1 = dbReader.GetInt32(colКодТУзла1);
                        if (!dbReader.IsDBNull(colКодТУзла2)) UzelId2 = dbReader.GetInt32(colКодТУзла2);
                        if (!dbReader.IsDBNull(colКодТерритории)) TerritoryId = dbReader.GetInt32(colКодТерритории);
                        if (!dbReader.IsDBNull(colКодСтатьиБюджета)) BudgetId = dbReader.GetInt32(colКодСтатьиБюджета);
                        if (!dbReader.IsDBNull(colДата2)) Date2 = dbReader.GetDateTime(colДата2);
                        if (!dbReader.IsDBNull(colДата3)) Date3 = dbReader.GetDateTime(colДата3);
                        if (!dbReader.IsDBNull(colДата4)) Date4 = dbReader.GetDateTime(colДата4);
                        if (!dbReader.IsDBNull(colДата5)) Date5 = dbReader.GetDateTime(colДата5);
                        if (!dbReader.IsDBNull(colFlag1)) Flag1 = dbReader.GetByte(colFlag1);
                        if (!dbReader.IsDBNull(colFlag2)) Flag2 = dbReader.GetByte(colFlag2);
                        if (!dbReader.IsDBNull(colInt1)) Int1 = dbReader.GetInt32(colInt1);
                        if (!dbReader.IsDBNull(colInt2)) Int2 = dbReader.GetInt32(colInt2);
                        if (!dbReader.IsDBNull(colInt3)) Int3 = dbReader.GetInt32(colInt3);
                        if (!dbReader.IsDBNull(colInt4)) Int4 = dbReader.GetInt32(colInt4);
                        if (!dbReader.IsDBNull(colInt5)) Int5 = dbReader.GetInt32(colInt5);
                        if (!dbReader.IsDBNull(colInt6)) Int6 = dbReader.GetInt32(colInt6);
                        if (!dbReader.IsDBNull(colInt7)) Int7 = dbReader.GetInt32(colInt7);
                        Text50_1 = dbReader.GetString(colText50_1);
                        Text50_2 = dbReader.GetString(colText50_2);
                        Text50_3 = dbReader.GetString(colText50_3);
                        Text50_4 = dbReader.GetString(colText50_4);
                        Text50_5 = dbReader.GetString(colText50_5);
                        Text50_6 = dbReader.GetString(colText50_6);
                        Text50_7 = dbReader.GetString(colText50_7);
                        Text50_8 = dbReader.GetString(colText50_8);
                        Text50_9 = dbReader.GetString(colText50_9);
                        Text50_10 = dbReader.GetString(colText50_10);
                        Text50_11 = dbReader.GetString(colText50_11);
                        Text50_12 = dbReader.GetString(colText50_12);
                        Text50_13 = dbReader.GetString(colText50_13);
                        Text100_1 = dbReader.GetString(colText100_1);
                        Text100_2 = dbReader.GetString(colText100_2);
                        Text100_3 = dbReader.GetString(colText100_3);
                        Text100_4 = dbReader.GetString(colText100_4);
                        Text100_5 = dbReader.GetString(colText100_5);
                        Text100_6 = dbReader.GetString(colText100_6);
                        Text300_1 = dbReader.GetString(colText300_1);
                        Text300_2 = dbReader.GetString(colText300_2);
                        Text300_3 = dbReader.GetString(colText300_3);
                        Text300_4 = dbReader.GetString(colText300_4);
                        Text300_5 = dbReader.GetString(colText300_5);
                        Text300_6 = dbReader.GetString(colText300_6);
                        Text300_7 = dbReader.GetString(colText300_7);
                        Text300_8 = dbReader.GetString(colText300_8);
                        Text300_9 = dbReader.GetString(colText300_9);
                        Text1000_1 = dbReader.GetString(colText1000_1);
                        Text1000_2 = dbReader.GetString(colText1000_2);
                        if (!dbReader.IsDBNull(colMoney1)) Money1 = dbReader.GetDecimal(colMoney1);
                        if (!dbReader.IsDBNull(colMoney2)) Money2 = dbReader.GetDecimal(colMoney2);
                        if (!dbReader.IsDBNull(colMoney3)) Money3 = dbReader.GetDecimal(colMoney3);
                        if (!dbReader.IsDBNull(colMoney4)) Money4 = dbReader.GetDecimal(colMoney4);
                        if (!dbReader.IsDBNull(colMoney5)) Money5 = dbReader.GetDecimal(colMoney5);
                        if (!dbReader.IsDBNull(colMoney6)) Money6 = dbReader.GetDecimal(colMoney6);
                        if (!dbReader.IsDBNull(colMoney7)) Money7 = dbReader.GetDecimal(colMoney7);
                        if (!dbReader.IsDBNull(colMoney8)) Money8 = dbReader.GetDecimal(colMoney8);
                        if (!dbReader.IsDBNull(colMoney9)) Money9 = dbReader.GetDecimal(colMoney9);
                        if (!dbReader.IsDBNull(colDecimal1)) Decimal1 = dbReader.GetDecimal(colDecimal1);
                        if (!dbReader.IsDBNull(colDecimal2)) Decimal2 = dbReader.GetDecimal(colDecimal2);
                        if (!dbReader.IsDBNull(colFloat1)) Float1 = dbReader.GetDouble(colFloat1);
                        if (!dbReader.IsDBNull(colТекстДокумента)) DocumentText = dbReader.GetString(colТекстДокумента);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Сравнивает первоначальное состояние документа(original) и текущую версию
        /// </summary>
        /// <returns>
        ///     true - документ отличается,
        ///     false - документ не изменялся
        /// </returns>
        public virtual bool CompareToChanges(DocumentData original)
        {
            if (original == null)
                return true;

            if (original.PersonId1 != PersonId1)
                return true;

            if (original.PersonId2 != PersonId2)
                return true;

            if (original.PersonId3 != PersonId3)
                return true;

            if (original.PersonId4 != PersonId4)
                return true;

            if (original.PersonId5 != PersonId5)
                return true;

            if (original.PersonId6 != PersonId6)
                return true;

            if (original.StockId1 != StockId1)
                return true;

            if (original.StockId2 != StockId2)
                return true;

            if (original.StockId3 != StockId3)
                return true;

            if (original.StockId4 != StockId4)
                return true;

            if (original.ResourceId1 != ResourceId1)
                return true;

            if (original.ResourceId2 != ResourceId2)
                return true;

            if (original.EmployeeId1 != EmployeeId1)
                return true;

            if (original.EmployeeId2 != EmployeeId2)
                return true;

            if (original.EmployeeId3 != EmployeeId3)
                return true;

            if (original.LocationId1 != LocationId1)
                return true;

            //if (original.LocationId2 != LocationId2)
            //    return true;

            if (original.BaseDeliveryId != BaseDeliveryId)
                return true;

            if (original.TransportTypeId != TransportTypeId)
                return true;

            if (original.StorageLocationId != StorageLocationId)
                return true;

            if (original.UnitsId != UnitsId)
                return true;

            if (original.NDSrateId != NDSrateId)
                return true;

            if (original.UzelId1 != UzelId1)
                return true;

            if (original.UzelId2 != UzelId2)
                return true;

            if (original.TerritoryId != TerritoryId)
                return true;

            if (original.BudgetId != BudgetId)
                return true;

            if (original.Date2 != Date2)
                return true;

            if (original.Date3 != Date3)
                return true;

            if (original.Date4 != Date4)
                return true;

            if (original.Date5 != Date5)
                return true;

            if (original.Flag1 != Flag1)
                return true;

            if (original.Flag2 != Flag2)
                return true;

            if (original.Int1 != Int1)
                return true;

            if (original.Int2 != Int2)
                return true;

            if (original.Int3 != Int3)
                return true;

            if (original.Int4 != Int4)
                return true;

            if (original.Int5 != Int5)
                return true;

            if (original.Int6 != Int6)
                return true;

            if (original.Int7 != Int7)
                return true;

            if (original.Text50_1 != Text50_1)
                return true;

            if (original.Text50_2 != Text50_2)
                return true;

            if (original.Text50_3 != Text50_3)
                return true;

            if (original.Text50_4 != Text50_4)
                return true;

            if (original.Text50_5 != Text50_5)
                return true;

            if (original.Text50_6 != Text50_6)
                return true;

            if (original.Text50_7 != Text50_7)
                return true;

            if (original.Text50_8 != Text50_8)
                return true;

            if (original.Text50_9 != Text50_9)
                return true;

            if (original.Text50_10 != Text50_10)
                return true;

            if (original.Text50_11 != Text50_11)
                return true;

            if (original.Text50_12 != Text50_12)
                return true;

            if (original.Text50_13 != Text50_13)
                return true;

            if (original.Text100_1 != Text100_1)
                return true;

            if (original.Text100_2 != Text100_2)
                return true;

            if (original.Text100_3 != Text100_3)
                return true;

            if (original.Text100_4 != Text100_4)
                return true;

            if (original.Text100_5 != Text100_5)
                return true;

            if (original.Text100_6 != Text100_6)
                return true;

            if (original.Text300_1 != Text300_1)
                return true;

            if (original.Text300_2 != Text300_2)
                return true;

            if (original.Text300_3 != Text300_3)
                return true;

            if (original.Text300_4 != Text300_4)
                return true;

            if (original.Text300_5 != Text300_5)
                return true;

            if (original.Text300_6 != Text300_6)
                return true;

            if (original.Text300_7 != Text300_7)
                return true;

            if (original.Text300_8 != Text300_8)
                return true;

            if (original.Text300_9 != Text300_9)
                return true;

            if (original.Text1000_1 != Text1000_1)
                return true;

            if (original.Text1000_2 != Text1000_2)
                return true;

            if (original.Money1 != Money1)
                return true;

            if (original.Money2 != Money2)
                return true;

            if (original.Money3 != Money3)
                return true;

            if (original.Money4 != Money4)
                return true;

            if (original.Money5 != Money5)
                return true;

            if (original.Money6 != Money6)
                return true;

            if (original.Money7 != Money7)
                return true;

            if (original.Money8 != Money8)
                return true;

            if (original.Money9 != Money9)
                return true;

            if (original.Decimal1 != Decimal1)
                return true;

            if (original.Decimal2 != Decimal2)
                return true;

            if (original.Float1 != Float1)
                return true;

            if (original.DocumentText != DocumentText)
                return true;

            return false;
        }

        #region Поля сущности "Документы данные"

        /// <summary>
        ///     ID. Поле КодДокумента
        /// </summary>
        public int DocCode => Id.ToInt();

        /// <summary>
        ///     Поле Изменил [int] NOT NULL
        /// </summary>
        public int ChangePersonID { get; set; }

        /// <summary>
        ///     Изменено [datetime] NOT NULL
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        ///     Поле КодЛица1 [int] NULL
        /// </summary>
        public int? PersonId1 { get; set; }

        /// <summary>
        ///     Поле КодЛица2 [int] NULL
        /// </summary>
        public int? PersonId2 { get; set; }

        /// <summary>
        ///     Поле КодЛица3 [int] NULL
        /// </summary>
        public int? PersonId3 { get; set; }

        /// <summary>
        ///     Поле КодЛица4 [int] NULL
        /// </summary>
        public int? PersonId4 { get; set; }

        /// <summary>
        ///     Поле КодЛица5 [int] NULL
        /// </summary>
        public int? PersonId5 { get; set; }

        /// <summary>
        ///     Поле КодЛица6 [int] NULL
        /// </summary>
        public int? PersonId6 { get; set; }

        /// <summary>
        ///     Поле КодСклада1 [int] NULL
        /// </summary>
        public int? StockId1 { get; set; }

        /// <summary>
        ///     Поле КодСклада2 [int] NULL
        /// </summary>
        public int? StockId2 { get; set; }

        /// <summary>
        ///     Поле КодСклада3 [int] NULL
        /// </summary>
        public int? StockId3 { get; set; }

        /// <summary>
        ///     Поле КодСклада4 [int] NULL
        /// </summary>
        public int? StockId4 { get; set; }

        /// <summary>
        ///     Поле КодРесурса1 [int] NULL
        /// </summary>
        public int? ResourceId1 { get; set; }

        /// <summary>
        ///     Поле КодРесурса2 [int] NULL
        /// </summary>
        public int? ResourceId2 { get; set; }

        /// <summary>
        ///     Поле КодСотрудника1 [int] NULL
        /// </summary>
        public int? EmployeeId1 { get; set; }

        /// <summary>
        ///     Поле КодСотрудника2 [int] NULL
        /// </summary>
        public int? EmployeeId2 { get; set; }

        /// <summary>
        ///     Поле КодСотрудника3 [int] NULL
        /// </summary>
        public int? EmployeeId3 { get; set; }

        /// <summary>
        ///     Поле КодРасположения1[int] NULL
        /// </summary>
        public int? LocationId1 { get; set; }

        /// <summary>
        ///     Поле КодРасположения2[int] NULL
        /// </summary>
        public int? LocationId2 { get; set; }

        /// <summary>
        ///     Поле КодБазисаПоставки [int] NULL
        /// </summary>
        public int? BaseDeliveryId { get; set; }

        /// <summary>
        ///     Поле КодВидаТранспорта [int] NULL
        /// </summary>
        public int? TransportTypeId { get; set; }

        /// <summary>
        ///     Поле  КодМестаХранения [int] NULL
        /// </summary>
        public int? StorageLocationId { get; set; }

        /// <summary>
        ///     Поле КодЕдиницыИзмерения [int] NULL
        /// </summary>
        public int? UnitsId { get; set; }

        /// <summary>
        ///     Поле КодСтавкиНДС[int] NULL
        /// </summary>
        public int? NDSrateId { get; set; }

        /// <summary>
        ///     Поле КодТУзла1[int] NULL
        /// </summary>
        public int? UzelId1 { get; set; }

        /// <summary>
        ///     Поле КодТУзла2[int] NULL
        /// </summary>
        public int? UzelId2 { get; set; }

        /// <summary>
        ///     Поле КодТерритории[int] NULL
        /// </summary>
        public int? TerritoryId { get; set; }

        /// <summary>
        ///     Поле КодСтатьиБюджета [int] NULL
        /// </summary>
        public int? BudgetId { get; set; }

        /// <summary>
        ///     Поле Дата2 [smalldatetime] NULL
        /// </summary>
        public DateTime? Date2 { get; set; }

        /// <summary>
        ///     Поле Дата2 [string] yyyyMMdd
        /// </summary>
        public string _Date2 => !Date2.HasValue ? "" : Date2.Value.ToString("yyyyMMdd");

        /// <summary>
        ///     Поле Дата3 [smalldatetime] NULL
        /// </summary>
        public DateTime? Date3 { get; set; }

        /// <summary>
        ///     Поле _Date3 [string] yyyyMMdd
        /// </summary>
        public string _Date3 => !Date3.HasValue ? "" : Date3.Value.ToString("yyyyMMdd");

        /// <summary>
        ///     Поле Дата4 [smalldatetime] NULL
        /// </summary>
        public DateTime? Date4 { get; set; }

        /// <summary>
        ///     Поле _Date4 [string] yyyyMMdd
        /// </summary>
        public string _Date4 => !Date4.HasValue ? "" : Date4.Value.ToString("yyyyMMdd");

        /// <summary>
        ///     Поле Дата5 [smalldatetime] NULL
        /// </summary>
        public DateTime? Date5 { get; set; }

        /// <summary>
        ///     Поле _Date5 [string] yyyyMMdd
        /// </summary>
        public string _Date5 => !Date5.HasValue ? "" : Date5.Value.ToString("yyyyMMdd");

        /// <summary>
        ///     Поле Flag1 [tinyint] NULL
        /// </summary>
        public byte? Flag1 { get; set; }

        /// <summary>
        ///     Поле Flag2 [tinyint] NULL
        /// </summary>
        public byte? Flag2 { get; set; }

        /// <summary>
        ///     Поле Int1 [int] NULL
        /// </summary>
        public int? Int1 { get; set; }

        /// <summary>
        ///     Поле Int2 [int] NULL
        /// </summary>
        public int? Int2 { get; set; }

        /// <summary>
        ///     Поле Int3 [int] NULL
        /// </summary>
        public int? Int3 { get; set; }

        /// <summary>
        ///     Поле Int4 [int] NULL
        /// </summary>
        public int? Int4 { get; set; }

        /// <summary>
        ///     Поле Int5 [int] NULL
        /// </summary>
        public int? Int5 { get; set; }

        /// <summary>
        ///     Поле Int6 [int] NULL
        /// </summary>
        public int? Int6 { get; set; }

        /// <summary>
        ///     Поле Int7 [int] NULL
        /// </summary>
        public int? Int7 { get; set; }

        /// <summary>
        ///     Поле Text50_1 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_1 { get; set; }

        /// <summary>
        ///     Поле Text50_2 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_2 { get; set; }

        /// <summary>
        ///     Поле Text50_3  [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_3 { get; set; }

        /// <summary>
        ///     Поле Text50_4  [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_4 { get; set; }

        /// <summary>
        ///     Поле Text50_5 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_5 { get; set; }

        /// <summary>
        ///     Поле Text50_6 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_6 { get; set; }

        /// <summary>
        ///     Поле Text50_7 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_7 { get; set; }

        /// <summary>
        ///     Поле Text50_8 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_8 { get; set; }

        /// <summary>
        ///     Поле Text50_9 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_9 { get; set; }

        /// <summary>
        ///     Поле Text50_10 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_10 { get; set; }

        /// <summary>
        ///     Поле Text50_11 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_11 { get; set; }

        /// <summary>
        ///     Поле Text50_12 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_12 { get; set; }

        /// <summary>
        ///     Поле Text50_13 [nvarchar(50)] NOT NULL
        /// </summary>
        public string Text50_13 { get; set; }

        /// <summary>
        ///     Поле Text100_1 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_1 { get; set; }

        /// <summary>
        ///     Поле Text100_2 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_2 { get; set; }

        /// <summary>
        ///     Поле Text100_3 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_3 { get; set; }

        /// <summary>
        ///     Поле Text100_4 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_4 { get; set; }

        /// <summary>
        ///     Поле  Text100_5 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_5 { get; set; }

        /// <summary>
        ///     Поле Text100_6 [nvarchar(100)] NOT NULL
        /// </summary>
        public string Text100_6 { get; set; }

        /// <summary>
        ///     Поле Text300_1 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_1 { get; set; }

        /// <summary>
        ///     Поле Text300_2  [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_2 { get; set; }

        /// <summary>
        ///     Поле  Text300_3 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_3 { get; set; }

        /// <summary>
        ///     Поле Text300_4 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_4 { get; set; }

        /// <summary>
        ///     Поле Text300_4 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_5 { get; set; }

        /// <summary>
        ///     Поле Text300_6 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_6 { get; set; }

        /// <summary>
        ///     Поле Text300_7 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_7 { get; set; }

        /// <summary>
        ///     Поле Text300_8 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_8 { get; set; }

        /// <summary>
        ///     Поле Text300_9 [nvarchar(300)] NOT NULL
        /// </summary>
        public string Text300_9 { get; set; }

        /// <summary>
        ///     Поле Text1000_1 [nvarchar(1000)] NOT NULL
        /// </summary>
        public string Text1000_1 { get; set; }

        /// <summary>
        ///     Поле Text1000_2  [nvarchar(1000)] NOT NULL
        /// </summary>
        public string Text1000_2 { get; set; }

        /// <summary>
        ///     Поле Money1 [money] NULL
        /// </summary>
        public decimal? Money1 { get; set; }

        /// <summary>
        ///     Поле Money2 [money] NULL
        /// </summary>
        public decimal? Money2 { get; set; }

        /// <summary>
        ///     Поле Money3 [money] NULL
        /// </summary>
        public decimal? Money3 { get; set; }

        /// <summary>
        ///     Поле Money4 [money] NULL
        /// </summary>
        public decimal? Money4 { get; set; }

        /// <summary>
        ///     Поле Money5  [money] NULL
        /// </summary>
        public decimal? Money5 { get; set; }

        /// <summary>
        ///     Поле Money6 [money] NULL
        /// </summary>
        public decimal? Money6 { get; set; }

        /// <summary>
        ///     Поле Money7 [money] NULL
        /// </summary>
        public decimal? Money7 { get; set; }

        /// <summary>
        ///     Поле Money8 [money] NULL
        /// </summary>
        public decimal? Money8 { get; set; }

        /// <summary>
        ///     Поле Money9 [money] NULL
        /// </summary>
        public decimal? Money9 { get; set; }

        /// <summary>
        ///     Поле Decimal1 [decimal(18,8)] NULL
        /// </summary>
        public decimal? Decimal1 { get; set; }

        /// <summary>
        ///     Поле Decimal2 [decimal(18,8)] NULL
        /// </summary>
        public decimal? Decimal2 { get; set; }

        /// <summary>
        ///     Поле Float1 [float] NULL
        /// </summary>
        public double? Float1 { get; set; }

        /// <summary>
        ///     Поле ТекстДокумента [text] NULL
        /// </summary>
        public string DocumentText { get; set; }

        #endregion
    }
}