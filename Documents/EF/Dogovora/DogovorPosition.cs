using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Dogovora
{
    /// <summary>
    /// Движения на cкладах
    /// </summary>
    [Serializable]
    [DBSource("vwПозицииДоговоров", SQLQueries.SELECT_ID_ПозицияДоговора, SQLQueries.SELECT_ID_ПозицииДоговораПоРесурсу)]
    public class DogovorPosition : DocumentPosition<DogovorPosition>
    {
        #region Поля сущности

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     Документ
        /// </summary>
        public Document Document
        {
            get
            {
                return new Document(DocumentId.ToString());
            }
        }

        /// <summary>
        /// КодПозицииДоговора
        /// </summary>
        /// <value>
        /// КодПозицииДоговора (int, not null)
        /// </value>
        [DBField("КодПозицииДоговора",0)] 
        public override int? PositionId { get; set; }

        /// <summary>
        /// КодРесурса
        /// </summary>
        /// <value>
        /// КодРесурса (int, not null)
        /// </value>
        [DBField("КодРесурса")]
        public int ResourceId { get; set; }

        /// <summary>
        /// Ресурс
        /// </summary>
        public Resource Resource
        {
            get
            {
                return new Resource(ResourceId.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодЕдиницыИзмерения (int, not null)
        /// </value>
        [DBField("КодЕдиницыИзмерения")]
        public int UnitId { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public Unit Unit
        {
            get
            {
                return new Unit(UnitId.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Точность (ште, null
        /// </value>
        [DBField("Точность")]
        public int Scale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Количество (float, not null
        /// </value>
        [DBField("Количество")]
        public double Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Цена (money, null)
        /// </value>
        [DBField("Цена")]
        public decimal Cost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СтавкаНДС (float, not null)
        /// </value>
        [DBField("СтавкаНДС")]
        public double StavkaNDS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Сумма (money, null)
        /// </value>
        [DBField("Сумма")]
        public decimal Summa { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Формула цены (varchar(250), not null)
        /// </value>
        [DBField("ФормулаЦены")]
        public string CostFormula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Формула цены XML (varchar(250), not null)
        /// </value>
        [DBField("ФормулаЦеныXML")]
        public string CostFormulaXML { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Скидка (float, null)
        /// </value>
        [DBField("Скидка")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Наценка (float, null)
        /// </value>
        [DBField("Наценка")]
        public decimal Premium { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Опцион (float, null)
        /// </value>
        [DBField("Опцион")]
        public decimal Option { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Тип опциона (tinyint, null)
        /// </value>
        [DBField("ТипОпциона")]
        public int OptionType { get; set; }

        #endregion

        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        public DogovorPosition() {}

        /// <summary>
        ///  Конструктор с параметром
        /// </summary>
        public DogovorPosition(string id)
        {
            Id = id;
            Load();
        }

    }
}
