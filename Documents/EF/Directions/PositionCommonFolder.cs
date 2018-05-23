using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТПапки
    /// </summary>
    [DBSource("vwПозицииУказанийИТПапки")]
    public class PositionCommonFolder : DocumentPosition<PositionCommonFolder>
    {
        /// <summary>
        ///     Конструктор дл загрузки по коду документа
        /// </summary>
        public PositionCommonFolder()
        {
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PositionCommonFolder(int id)
        {
            Id = id.ToString();
            Load();
        }

        #region Поля

        /// <summary>
        ///     КодПозицииУказанийИТПапка
        /// </summary>
        [DBField("КодПозицииУказанийИТПапка", 0)]
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
        ///     КодОбщейПапки
        /// </summary>
        [DBField("КодОбщейПапки")]
        public int CommonFolderId { get; set; }

        /// <summary>
        ///     ОбщаяПапка
        /// </summary>
        [DBField("ОбщаяПапка")]
        public string CommonFolderName { get; set; }

        #endregion
    }
}