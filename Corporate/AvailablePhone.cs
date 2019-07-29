using System;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Бизнес-объект - доступный телефон
    /// </summary>
    [Serializable]
    public class AvailablePhone : Entity
    {
        /// <summary>
        ///     КодОборудования
        /// </summary>
        public int CodeEquipment { get; set; }

        /// <summary>
        ///     Исходящий
        /// </summary>
        public int Outgoing { get; set; }

        /// <summary>
        ///     ТелефонныйНомер
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Ведомый
        /// </summary>
        public string Driven { get; set; }

        /// <summary>
        ///     СетевоеИмя
        /// </summary>
        public string NetName { get; set; }

        /// <summary>
        ///     КодТелефоннойСтанции
        /// </summary>
        public int CodePhoneStation { get; set; }

        /// <summary>
        ///     CTI
        /// </summary>
        public string CTI { get; set; }

        /// <summary>
        ///     Тип
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     ОписаниеАТС
        /// </summary>
        public string DescriptionATS { get; set; }

        /// <summary>
        ///     Оборудование
        /// </summary>
        public string Equipment { get; set; }

        /// <summary>
        ///     Расположение
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     КодТипаОборудования
        /// </summary>
        public int EquipmentTypeCode { get; set; }
    }
}