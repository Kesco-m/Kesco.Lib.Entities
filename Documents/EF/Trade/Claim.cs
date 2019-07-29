using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///     Документ Претензия
    /// </summary>
    public class Claim : Document
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        public Claim()
        {
            Initialization();
        }

        /// <summary>
        ///     Конструктор с инициализацией документа
        /// </summary>
        public Claim(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        ///     Заявитель
        /// </summary>
        public DocField ZayavitelField { get; private set; }

        /// <summary>
        ///     Нарушитель
        /// </summary>
        public DocField NarushitelField { get; private set; }

        /// <summary>
        ///     Договор
        /// </summary>
        public DocField DogovorField { get; private set; }

        /// <summary>
        ///     Сотрудник заявителя
        /// </summary>
        public DocField ZayavitelEmlField { get; private set; }

        /// <summary>
        ///     Сотрудник нарушителя
        /// </summary>
        public DocField NarushitelEmlField { get; private set; }

        /// <summary>
        ///     Р/С заявителя
        /// </summary>
        public DocField ZayavitelRSField { get; private set; }

        /// <summary>
        ///     Валюта оплаты
        /// </summary>
        public DocField CurrencyField { get; private set; }

        /// <summary>
        ///     HTMLТекст
        /// </summary>
        public DocField HTMLText { get; private set; }

        /// <summary>
        ///     Претензии
        /// </summary>
        public DocField PositionField { get; private set; }

        /// <summary>
        ///     Приложение
        /// </summary>
        public DocField PrilozhenieField { get; private set; }

        /// <summary>
        ///     Описание формулы расчета у.е.
        /// </summary>
        public DocField FormulaDescrField { get; private set; }

        /// <summary>
        ///     Курс
        /// </summary>
        public DocField KursField { get; private set; }

        /// <summary>
        ///     Сумма
        /// </summary>
        public DocField Sum { get; private set; }

        /// <summary>
        ///     Счет, Инвойс проформа
        /// </summary>
        public DocField SchetPredField { get; private set; }

        /// <summary>
        ///     Платежные документы
        /// </summary>
        public DocField PlatezhkiField { get; private set; }

        /// <summary>
        ///     Коносамент
        /// </summary>
        public DocField BillOfLading { get; private set; }

        /// <summary>
        ///     Дата подтверждения претензии
        /// </summary>
        public DocField DateProvodkiField { get; private set; }

        /// <summary>
        ///     Валюта притензии
        /// </summary>
        public override Currency Currency => Currency.GetCurrency((int) CurrencyField.Value);

        /// <summary>
        ///     Договор
        /// </summary>
        public string _Dogovor
        {
            get { return GetBaseDoc(DogovorField.DocFieldId); }
            set { SetBaseDoc(DogovorField.DocFieldId, value.ToInt()); }
        }

        /// <summary>
        ///     Приложение
        /// </summary>
        public string _Prilozhenie
        {
            get { return GetBaseDoc(PrilozhenieField.DocFieldId); }
            set { SetBaseDoc(PrilozhenieField.DocFieldId, value.ToInt()); }
        }

        /// <summary>
        ///     Инициализация документа Претензия
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.Претензия;
            ZayavitelField = GetDocField("1351");
            NarushitelField = GetDocField("1352");
            DogovorField = GetDocField("1353");
            ZayavitelEmlField = GetDocField("1354");
            NarushitelEmlField = GetDocField("1355");
            ZayavitelRSField = GetDocField("1356");
            CurrencyField = GetDocField("1357");
            PositionField = GetDocField("1359");
            PrilozhenieField = GetDocField("1360");
            FormulaDescrField = GetDocField("1361");
            KursField = GetDocField("1362");
            SchetPredField = GetDocField("1635");
            PlatezhkiField = GetDocField("1636");
            DateProvodkiField = GetDocField("1784");

            BillOfLading = GetDocField("1637");
            Sum = GetDocField("1424");
            HTMLText = GetDocField("1358");
        }

        /// <summary>
        ///     Позиции претензий из таблицы: ПозицииПретензий
        /// </summary>
        public class Position : Entity
        {
            /// <summary>
            ///     Загрузить данные по Id
            /// </summary>
            /// <param name="id"></param>
            public void FillData(int id)
            {
                using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ПозицияПретензии, id, CommandType.Text, CN))
                {
                    if (dbReader.HasRows)
                    {
                        #region Получение порядкового номера столбца

                        var colКодПозицииПретензии = dbReader.GetOrdinal("КодПозицииПретензии");
                        var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                        var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                        var colРесурс = dbReader.GetOrdinal("Ресурс");
                        var colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                        var colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                        var colКоличество = dbReader.GetOrdinal("Количество");
                        var colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                        var colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                        var colПорядок = dbReader.GetOrdinal("Порядок");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        if (dbReader.Read())
                        {
                            Unavailable = false;
                            PositionId = dbReader.GetInt32(colКодПозицииПретензии);
                            ClmId = dbReader.GetInt32(colКодДокумента);
                            ResourceId = dbReader.GetInt32(colКодРесурса);
                            ResourceText = dbReader.GetString(colРесурс);
                            if (!dbReader.IsDBNull(colКодЕдиницыИзмерения))
                                UnitId = dbReader.GetInt32(colКодЕдиницыИзмерения);
                            if (!dbReader.IsDBNull(colКоэффициент)) Coefficient = dbReader.GetDouble(colКоэффициент);
                            Quantity = dbReader.GetDouble(colКоличество);
                            CostOutNds = dbReader.GetDecimal(colЦенаБезНДС);
                            SummaOutNds = dbReader.GetDecimal(colСуммаБезНДС);
                            Order = dbReader.GetInt32(colПорядок);
                            ChangedBy = dbReader.GetInt32(colИзменил);
                            ChangeDateTime = dbReader.GetDateTime(colИзменено);
                        }
                    }
                    else
                    {
                        Unavailable = true;
                    }
                }
            }

            /// <summary>
            ///     Получить позиции по Id претензии
            /// </summary>
            public static List<Position> GetPositionsByClaimId(int id)
            {
                if (id == 0) return null;

                var query = string.Format("SELECT * FROM vwПозицииПретензий WHERE КодДокумента={0}", id);

                return GetPositionList(query);
            }

            /// <summary>
            ///     Произвольный запрос
            /// </summary>
            public static List<Position> GetPositionList(string query)
            {
                var list = new List<Position>();
                using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
                {
                    if (dbReader.HasRows)
                    {
                        #region Получение порядкового номера столбца

                        var colКодПозицииПретензии = dbReader.GetOrdinal("КодПозицииПретензии");
                        var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                        var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                        var colРесурс = dbReader.GetOrdinal("Ресурс");
                        var colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                        var colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                        var colКоличество = dbReader.GetOrdinal("Количество");
                        var colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                        var colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                        var colПорядок = dbReader.GetOrdinal("Порядок");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        while (dbReader.Read())
                        {
                            var row = new Position();
                            row.Unavailable = false;
                            row.PositionId = dbReader.GetInt32(colКодПозицииПретензии);
                            row.Id = row.PositionId.ToString();
                            row.ClmId = dbReader.GetInt32(colКодДокумента);
                            row.ResourceId = dbReader.GetInt32(colКодРесурса);
                            row.Name = row.ResourceText = dbReader.GetString(colРесурс);
                            if (!dbReader.IsDBNull(colКодЕдиницыИзмерения))
                                row.UnitId = dbReader.GetInt32(colКодЕдиницыИзмерения);
                            if (!dbReader.IsDBNull(colКоэффициент))
                                row.Coefficient = dbReader.GetDouble(colКоэффициент);
                            row.Quantity = dbReader.GetDouble(colКоличество);
                            row.CostOutNds = dbReader.GetDecimal(colЦенаБезНДС);
                            row.SummaOutNds = dbReader.GetDecimal(colСуммаБезНДС);
                            row.Order = dbReader.GetInt32(colПорядок);
                            row.ChangedBy = dbReader.GetInt32(colИзменил);
                            row.ChangeDateTime = dbReader.GetDateTime(colИзменено);
                            list.Add(row);
                        }
                    }
                }

                return list;
            }

            #region Поля сущности

            /// <summary>
            ///     Поле КодПозицииПретензии
            /// </summary>
            /// <value>
            ///     КодПозицииПретензии (int, not null)
            /// </value>
            public int PositionId { get; set; }

            /// <summary>
            ///     Поле КодДокумента
            /// </summary>
            /// <value>
            ///     КодДокумента (int, not null)
            /// </value>
            public int ClmId { get; set; }

            /// <summary>
            ///     Поле КодРесурса
            /// </summary>
            /// <value>
            ///     КодРесурса (int, not null)
            /// </value>
            public int ResourceId { get; set; }

            /// <summary>
            ///     Поле Ресурс
            /// </summary>
            /// <value>
            ///     Ресурс (varchar(300), not null)
            /// </value>
            public string ResourceText { get; set; }

            /// <summary>
            ///     Поле КодЕдиницыИзмерения
            /// </summary>
            /// <value>
            ///     КодЕдиницыИзмерения (int, null)
            /// </value>
            public int UnitId { get; set; }

            /// <summary>
            ///     Валюта
            /// </summary>
            public Unit Unit => new Unit(UnitId.ToString());

            /// <summary>
            ///     Поле коэффициент
            /// </summary>
            /// <value>
            ///     Коэффициент (float, null)
            /// </value>
            public double Coefficient { get; set; }

            /// <summary>
            ///     Поле количество
            /// </summary>
            /// <value>
            ///     Количество (float, not null)
            /// </value>
            public double Quantity { get; set; }

            /// <summary>
            ///     Поле ценаБезНДС
            /// </summary>
            /// <value>
            ///     ЦенаБезНДС (money, not null)
            /// </value>
            public decimal CostOutNds { get; set; }

            /// <summary>
            ///     Поле суммаБезНДС
            /// </summary>
            /// <value>
            ///     СуммаБезНДС (money, not null)
            /// </value>
            public decimal SummaOutNds { get; set; }

            /// <summary>
            ///     Поле порядок
            /// </summary>
            /// <value>
            ///     Порядок (int, not null)
            /// </value>
            public int Order { get; set; }

            /// <summary>
            ///     Поле изменил
            /// </summary>
            /// <value>
            ///     Изменил (int, not null)
            /// </value>
            public int ChangedBy { get; set; }

            /// <summary>
            ///     Поле изменено
            /// </summary>
            /// <value>
            ///     Изменено (datetime, not null)
            /// </value>
            public DateTime ChangeDateTime { get; set; }

            #endregion
        }
    }
}