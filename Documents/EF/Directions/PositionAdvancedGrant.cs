using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТПрава
    /// </summary>
    [DBSource("vwПозицииУказанийИТПрава")]
    public class PositionAdvancedGrant : DocumentPosition<PositionAdvancedGrant>
    {
        /// <summary>
        ///     Конструктор дл загрузки по коду документа
        /// </summary>
        public PositionAdvancedGrant()
        {
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PositionAdvancedGrant(int id)
        {
            Id = id.ToString();
            Load();
        }

        #region Поля

        /// <summary>
        ///     КодПозицииУказанийИТПрава
        /// </summary>
        [DBField("КодПозицииУказанийИТПрава", 0)]
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
        ///     КодПраваДляУказанийIT
        /// </summary>
        [DBField("КодПраваДляУказанийIT")]
        public int GrantId { get; set; }

        /// <summary>
        ///     Описание
        /// </summary>
        [DBField("Описание")]
        public string GrantDescription { get; set; }

        /// <summary>
        ///     ОписаниеЛат
        /// </summary>
        [DBField("ОписаниеЛат")]
        public string GrantDescriptionEn { get; set; }

        #endregion
    }
}