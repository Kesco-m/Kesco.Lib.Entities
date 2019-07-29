using System;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТТипыЛиц
    /// </summary>
    [Serializable]
    [DBSource("vwПозицииУказанийИТТипыЛиц", SQLQueries.SUBQUERY_ID_ПозицииУказанийИТТипы,
        SQLQueries.SUBQUERY_ID_DOC_ПозицииУказанийИТТипы)]
    public class PositionType : DocumentPosition<PositionType>
    {
        private PersonCatalog _catalog;
        private PersonTheme _theme;

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
        [DBField("КодПозицииУказанийИТТипЛица", 0)]
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
        ///     Возвращает объект типа PersonCatalog в зависимости от значения CatalogId
        /// </summary>
        public PersonCatalog CatalogObject
        {
            get
            {
                if (!CatalogId.HasValue)
                    _catalog = null;
                else if (_catalog == null || _catalog.Id != CatalogId.ToString())
                    _catalog = new PersonCatalog(CatalogId.ToString());

                return _catalog;
            }
        }


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
        ///     Возвращает объект типа PersonTheme в зависимости от значения ThemeId
        /// </summary>
        public PersonTheme ThemeObject
        {
            get
            {
                if (!ThemeId.HasValue)
                    _theme = null;
                else if (_theme == null || _theme.Id != ThemeId.ToString())
                    _theme = new PersonTheme(ThemeId.ToString());

                return _theme;
            }
        }


        /// <summary>
        ///     Название темы
        /// </summary>
        [DBField("ТемаЛица", "", false)]
        public string ThemeName { get; set; }

        #endregion
    }
}