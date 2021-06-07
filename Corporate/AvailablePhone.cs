using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Бизнес-объект - доступный телефон
    /// </summary>
    [Serializable]
    public class AvailablePhone : Entity
    {
        /// <summary>
        /// Объект звонилки
        /// </summary>
        public object Zvonilka { get; set; }
        /// <summary>
        /// Строка подключения
        /// </summary>
        public override string CN => Kesco.Lib.Web.Settings.Config.DS_user;

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
        ///     ОтображаемыйНомер
        /// </summary>
        public string DisplayNumber { get; set; }

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

        /// <summary>
        /// Получение списка доступных телефонов
        /// </summary>
        /// <param name="clientName">Имя компьютера</param>
        /// <returns></returns>
        public static List<AvailablePhone> GetClientPhoneNumbers(string clientName)
        {
            var sqlParams = new Dictionary<string, object> { { "@СетевоеИмя", clientName } };
                       
            var dt = Kesco.Lib.DALC.DBManager.GetData(SQLQueries.SP_ДоступныеТелефоны, Kesco.Lib.Web.Settings.Config.DS_user, CommandType.StoredProcedure, sqlParams);
            
            var result = dt.AsEnumerable().Select(dr => new AvailablePhone
            {
                CodeEquipment = dr.Field<int>("КодОборудования"),
                Outgoing = dr.Field<int>("Исходящий"),
                PhoneNumber = dr.Field<string>("ТелефонныйНомер"),
                DisplayNumber = dr.Field<string>("ОтображаемыйНомер"),
                Driven = dr.Field<string>("Ведомый"),
                NetName = dr.Field<string>("СетевоеИмя"),
                CodePhoneStation = dr.Field<int>("КодТелефоннойСтанции"),
                CTI = dr.Field<string>("CTI"),
                Type = dr.Field<string>("Тип"),
                DescriptionATS = dr.Field<string>("ОписаниеАТС"),
                Equipment = dr.Field<string>("Оборудование"),
                Location = dr.Field<string>("Расположение"),
                EquipmentTypeCode = dr.Field<int>("КодТипаОборудования")
            }).ToList().FindAll(x => x.Outgoing == 1);

            return result;
        }

        /// <summary>
        /// Получение номера набора из номера в международном формате
        /// </summary>
        /// <param name="interNumber">Номер в международном формате</param>
        /// <returns>Номер набора</returns>
        public string GetDialingNumber(string interNumber)
        {
            var sqlParams = new Dictionary<string, object> { { "@НомерМеждународный", interNumber }, { "@КодТелефоннойСтанции", CodePhoneStation }, { "@КодНомернойЁмкости", DBNull.Value }};

            var dtN = DALC.DBManager.GetData(SQLQueries.SELECT_НомерМеждународный2НомерНабора, Web.Settings.Config.DS_user, CommandType.Text, sqlParams);

            return dtN.Rows[0][0].ToString();
            
        }
    }
}