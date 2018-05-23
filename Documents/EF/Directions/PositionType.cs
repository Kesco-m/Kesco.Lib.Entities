using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТТипыЛиц
    /// </summary>
    [DBSource("vwПозицииУказанийИТТипыЛиц", SQLQueries.SUBQUERY_ID_ПозицииУказанийИТТипы,
        SQLQueries.SUBQUERY_ID_DOC_ПозицииУказанийИТТипы)]
    public class PositionType : DocumentPosition<PositionType>
    {
        /// <summary>
        ///     Конструктор дл загрузки по коду документа
        /// </summary>
        public PositionType()
        {

        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PositionType(int id)
        {
            Id = id.ToString();
            Load();
        }

        #region Поля

        /// <summary>
        ///     КодПозицииУказанийИТТипЛица
        /// </summary>
        [DBField("КодПозицииУказанийИТТипЛица",0)]
        public override int? PositionId
        {
            get { return base.PositionId; }
            set { base.PositionId = value; }
        }

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодКаталога")]
        public int? CatalogId { get; set; }

        /// <summary>
        ///     Название каталога
        /// </summary>
        [DBField("Каталог", "", false)]
        public string CatalogName { get; set; }

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодТемыЛица")]
        public int? ThemeId { get; set; }

        /// <summary>
        ///     Название темы
        /// </summary>
        [DBField("ТемаЛица", "", false)]
        public string ThemeName { get; set; }

        #endregion
    }
}