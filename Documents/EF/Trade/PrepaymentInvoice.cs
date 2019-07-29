using System;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;
using Convert = Kesco.Lib.ConvertExtention.Convert;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///     Позиции Счетов На Предоплату
    /// </summary>
    [Serializable]
    [DBSource("vwПозицииСчетовНаПредоплату", SQLQueries.SUBQUERY_ID_ПозицииСчетовНаПредоплату,
        SQLQueries.SUBQUERY_ID_DOC_ПозицииСчетовНаПредоплату)]
    public class PrepaymentInvoice : DocumentPosition<PrepaymentInvoice>
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PrepaymentInvoice()
        {
        }

        /// <summary>
        ///     Конструктор с параметром
        /// </summary>
        public PrepaymentInvoice(string id)
        {
            Id = id;
            Load();
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        #region Поля сущности

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     ПозицииСчетовНаПредоплату
        /// </summary>
        [DBField("КодПозицииСчетаНаПредоплату", 0)]
        public override int? PositionId
        {
            get { return base.PositionId; }
            set { base.PositionId = value; }
        }

        /// <summary>
        ///     Документ
        /// </summary>
        private Document document { get; set; }

        /// <summary>
        ///     Документ
        /// </summary>
        public Document Document
        {
            get
            {
                if (document != null && DocumentId.ToString() == document.Id) return document;

                document = new Document(DocumentId.ToString());
                return document;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодРесурса (int, not null)
        /// </value>
        [DBField("КодРесурса")]
        public int ResourceId
        {
            get { return string.IsNullOrEmpty(ResourceIdBind.Value) ? 0 : int.Parse(ResourceIdBind.Value); }
            set { ResourceIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодРесурса
        /// </summary>
        public BinderValue ResourceIdBind = new BinderValue();

        /// <summary>
        ///     Ресурс
        /// </summary>
        private Resource resource { get; set; }

        /// <summary>
        ///     Ресурс
        /// </summary>
        public Resource Resource
        {
            get
            {
                if (resource != null && ResourceId.ToString() == resource.Id) return resource;

                resource = new Resource(ResourceId.ToString());
                return resource;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Ресурс (varchar(300), not null)
        /// </value>
        [DBField("Ресурс")]
        public string ResourceRus
        {
            get { return string.IsNullOrEmpty(ResourceRusBind.Value) ? "" : ResourceRusBind.Value; }
            set { ResourceRusBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Binder для поля Ресурс
        /// </summary>
        public BinderValue ResourceRusBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     РесурсЛат (varchar(300), not null)
        /// </value>
        [DBField("РесурсЛат")]
        public string ResourceLat
        {
            get { return string.IsNullOrEmpty(ResourceLatBind.Value) ? "" : ResourceLatBind.Value; }
            set { ResourceLatBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Binder для поля РесурсЛат
        /// </summary>
        public BinderValue ResourceLatBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Количество (float, not null
        /// </value>
        [DBField("Количество")]
        public double Count
        {
            get { return string.IsNullOrEmpty(CountBind.Value) ? 0 : (double) Convert.Str2Decimal(CountBind.Value); }
            set { CountBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Количество
        /// </summary>
        public BinderValue CountBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодЕдиницыИзмерения (int, not null)
        /// </value>
        [DBField("КодЕдиницыИзмерения")]
        public int? UnitId
        {
            get { return string.IsNullOrEmpty(UnitIdBind.Value) ? (int?) null : int.Parse(UnitIdBind.Value); }
            set { UnitIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля
        /// </summary>
        /// КодЕдиницыИзмерения
        public BinderValue UnitIdBind = new BinderValue();

        /// <summary>
        ///     Единица измерения
        /// </summary>
        private Unit unit { get; set; }

        /// <summary>
        ///     Единица измерения
        /// </summary>
        public Unit Unit
        {
            get
            {
                if (unit != null && UnitId.ToString() == unit.Id) return unit;

                unit = new Unit(UnitId.ToString());
                return unit;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Коэффициент (float, null
        /// </value>
        [DBField("Коэффициент")]
        public double? Coef
        {
            get { return string.IsNullOrEmpty(CoefBind.Value) ? 0 : (double) Convert.Str2Decimal(CoefBind.Value); }
            set { CoefBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Коэффициент
        /// </summary>
        public BinderValue CoefBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     ЦенаБезНДС (money, null)
        /// </value>
        [DBField("ЦенаБезНДС")]
        public decimal CostOutNDS
        {
            get { return string.IsNullOrEmpty(CostOutNDSBind.Value) ? 0 : Convert.Str2Decimal(CostOutNDSBind.Value); }
            set { CostOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ЦенаБезНДС
        /// </summary>
        public BinderValue CostOutNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     СуммаБезНДС (money, null)
        /// </value>
        [DBField("СуммаБезНДС")]
        public decimal SummaOutNDS
        {
            get { return string.IsNullOrEmpty(SummaOutNDSBind.Value) ? 0 : Convert.Str2Decimal(SummaOutNDSBind.Value); }
            set { SummaOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СуммаБезНДС
        /// </summary>
        public BinderValue SummaOutNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодСтавкиНДС (int, null)
        /// </value>
        [DBField("КодСтавкиНДС")]
        public int? StavkaNDSId
        {
            get { return string.IsNullOrEmpty(StavkaNDSIdBind.Value) ? (int?) null : int.Parse(StavkaNDSIdBind.Value); }
            set { StavkaNDSIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодСтавкиНДС
        /// </summary>
        public BinderValue StavkaNDSIdBind = new BinderValue();

        /// <summary>
        ///     СтавкаНДС
        /// </summary>
        private StavkaNDS stavkaNDS { get; set; }

        /// <summary>
        ///     СтавкаНДС
        /// </summary>
        public StavkaNDS StavkaNDS
        {
            get
            {
                if (stavkaNDS != null && StavkaNDSId.ToString() == stavkaNDS.Id) return stavkaNDS;

                stavkaNDS = new StavkaNDS(StavkaNDSId.ToString());
                return stavkaNDS;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СуммаНДС (money, null)
        /// </value>
        [DBField("СуммаНДС")]
        public decimal SummaNDS
        {
            get { return string.IsNullOrEmpty(SummaNDSBind.Value) ? 0 : Convert.Str2Decimal(SummaNDSBind.Value); }
            set { SummaNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СуммаНДС
        /// </summary>
        public BinderValue SummaNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Всего (money, null)
        /// </value>
        [DBField("Всего")]
        public decimal Vsego
        {
            get { return string.IsNullOrEmpty(VsegoBind.Value) ? 0 : Convert.Str2Decimal(VsegoBind.Value); }
            set { VsegoBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Всего
        /// </summary>
        public BinderValue VsegoBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Порядок (int, not null)
        /// </value>
        [DBField("Порядок")]
        public int Order { get; set; }

        #endregion
    }
}