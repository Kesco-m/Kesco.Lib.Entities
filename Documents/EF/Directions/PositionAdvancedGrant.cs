using System;
using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТПрава
    /// </summary>
    [Serializable]
    [DBSource("vwПозицииУказанийИТДопПараметры")]
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
        [DBField("КодПозицииУказанийИТДопПараметры", 0)]
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
        ///     КодДопПараметраУказанийИТ
        /// </summary>
        [DBField("КодДопПараметраУказанийИТ")]
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

        /// <summary>
        ///     Порядок вывода
        /// </summary>
        public int OrderOutput { get; set; }

        /// <summary>
        ///     Относится к
        /// </summary>
        public int RefersTo { get; set; }

        #endregion
    }
}