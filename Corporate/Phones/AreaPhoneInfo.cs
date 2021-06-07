using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Phones
{
    /// <summary>
    ///     Класс получения информации о телефонном номере
    /// </summary>
    public class AreaPhoneInfo
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        public AreaPhoneInfo()
        {
            LengthPhoneCodeInCountry = 0;
            Direction =
                CountryPhoneCode =
                    PhoneCodeInCountry =
                        TerritoryName = string.Empty;
        }

        /// <summary>
        ///     Направление
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        ///     ТелКодСтраны
        /// </summary>
        public string CountryPhoneCode { get; set; }

        /// <summary>
        ///     ТелКодВСтране
        /// </summary>
        public string PhoneCodeInCountry { get; set; }

        /// <summary>
        ///     ДлинаКодаВСтране
        /// </summary>
        public int? LengthPhoneCodeInCountry { get; set; }

        /// <summary>
        ///     Территория
        /// </summary>
        public string TerritoryName { get; set; }

        /// <summary>
        ///     Получения частей телефонного номера
        /// </summary>
        /// <param name="phone">Телефонный номер</param>
        /// <returns>AreaPhoneInfo</returns>
        public static AreaPhoneInfo GetAreaPhoneInfo(string phone)
        {
            var api = new AreaPhoneInfo();
            var sqlParams = new Dictionary<string, object> {{"@Телефон", phone}};
            var dt = DBManager.GetData(SQLQueries.SELECT_ЧастиТелефонногоНомера, Config.DS_user, CommandType.Text,
                sqlParams);

            if (dt.Rows.Count != 1) return null;

            api.Direction = dt.Rows[0]["Направление"].ToString();
            api.CountryPhoneCode = dt.Rows[0]["ТелКодСтраны"].ToString();
            api.PhoneCodeInCountry = dt.Rows[0]["ТелКодВСтране"].ToString();
            if (!dt.Rows[0]["ДлинаКодаВСтране"].Equals(DBNull.Value))
                api.LengthPhoneCodeInCountry = int.Parse(dt.Rows[0]["ДлинаКодаВСтране"].ToString());
            api.TerritoryName = dt.Rows[0]["Территория"].ToString();

            return api;
        }
    }
}