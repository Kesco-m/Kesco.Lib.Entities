﻿using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    ///     Класс валют
    /// </summary>
    /// <example>
    ///     Получить валюту по Id  Currency.GetCurrency(value);
    /// </example>
    [Serializable]
    public class Currency : Resource
    {
        /// <summary>
        ///     Рубли
        /// </summary>
        public enum Code
        {
            /// <summary>
            ///     RUR
            /// </summary>
            RUR = 183,

            /// <summary>
            ///     USD
            /// </summary>
            USD = 184,

            /// <summary>
            ///     EUR
            /// </summary>
            EUR = 193
        }

        /// <summary>
        ///     backing field для AllCurrencies
        /// </summary>
        private static Dictionary<int, Currency> _allCurrencies;

        /// <summary>
        ///     Запрещаем создание экземпляров
        ///     Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private Currency()
        {
        }

        /// <summary>
        ///     Запрещаем создание экземпляров
        ///     Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private Currency(int id)
        {
        }

        /// <summary>
        ///     Запрещаем создание экземпляров
        ///     Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private Currency(string id)
        {
        }

        /// <summary>
        ///     Получить все валюты
        /// </summary>
        /// <remarks>
        ///     Кешируется
        /// </remarks>
        public static Dictionary<int, Currency> GetAllCurrencies()
        {
            return _allCurrencies ?? (_allCurrencies = GetCurrencyList());
        }

        /// <summary>
        ///     Получить валюту по ID, в случае неудачи возвращает null
        /// </summary>
        public static Currency GetCurrency(int id)
        {
            var cur = GetAllCurrencies();
            if (cur != null && cur.ContainsKey(id))
                return cur[id];

            return null;
        }

        /// <summary>
        ///     Получить валюту по ID
        /// </summary>
        public static Currency GetCurrency(string id)
        {
            return GetCurrency(id.ToInt());
        }

        /// <summary>
        ///     Получить список всех валют
        /// </summary>
        private static Dictionary<int, Currency> GetCurrencyList()
        {
            Dictionary<int, Currency> list = null;
            using (var dbReader = new DBReader(SQLQueries.SELECT_Ресурсы_Валюты, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new Dictionary<int, Currency>();

                    #region Получение порядкового номера столбца

                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    var colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    var colЕдиницаРус = dbReader.GetOrdinal("ЕдиницаРус");
                    var colЕдиницаЛат = dbReader.GetOrdinal("ЕдиницаЛат");
                    var colТочность = dbReader.GetOrdinal("Точность");

                    #endregion

                    while (dbReader.Read())
                    {
                        var id = dbReader.GetInt32(colКодРесурса);
                        var row = new Currency
                        {
                            Unavailable = false,
                            Id = id.ToString(),
                            Name = dbReader.GetString(colРесурсРус),
                            ResourceLat = dbReader.GetString(colРесурсЛат)
                        };
                        if (!dbReader.IsDBNull(colЕдиницаРус)) row.UnitRus = dbReader.GetString(colЕдиницаРус);
                        if (!dbReader.IsDBNull(colЕдиницаЛат)) row.UnitEng = dbReader.GetString(colЕдиницаЛат);
                        if (!dbReader.IsDBNull(colТочность)) row.UnitScale = dbReader.GetByte(colТочность);
                        list.Add(id, row);
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Получить код валюты по имени
        /// </summary>
        /// <param name="ShortName">короткое наименование</param>
        /// <returns>Код валюты</returns>
        public static int GetCurrencyByName(string ShortName)
        {
            var sqlParams = new Dictionary<string, object> {{"@КодКлассификатораБукв", ShortName}};
            var dt = DBManager.GetData(SQLQueries.SELECT_ID_Валюты_ПоКодуКлассификатораБукв, Config.DS_resource,
                CommandType.Text, sqlParams);

            return dt.Rows.Count == 1 ? Convert.ToInt32(dt.Rows[0]["КодВалюты"].ToString()) : 0;
        }

        /// <summary>
        ///     Курс валюты на дату
        /// </summary>
        /// <param name="curId">Код валюты</param>
        /// <param name="d">Дата</param>
        /// <returns></returns>
        public static decimal GetKursCbrf(int curId, DateTime d)
        {
            decimal kurs;
            decimal scale;

            return GetKursCbrf(curId, d, out kurs, out scale);
        }

        /// <summary>
        ///     Курс валюты на дату
        /// </summary>
        /// <param name="curId">Код валюты</param>
        /// <param name="d">Дата</param>
        /// <param name="kurs">Курс</param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static decimal GetKursCbrf(int curId, DateTime d, out decimal kurs, out decimal scale)
        {
            var query = string.Format(SQLQueries.SELECT_LoadKursCbrf, curId, d.ToString("yyyyMMdd"));
            var dt = DBManager.GetData(query, Config.DS_resource);

            kurs = 0m;
            scale = 1m;
            if (dt.Rows.Count == 1)
            {
                kurs = ConvertExtention.Convert.Str2Decimal(dt.Rows[0]["Курс"].ToString());
                scale = ConvertExtention.Convert.Str2Decimal(dt.Rows[0]["Единиц"].ToString());
            }

            if (kurs == 0m)
                throw new DetailedException("Нет курса " + GetCurrency(curId).Name + " на дату " + d.ToLongDateString(),
                    null, false);

            return kurs / scale;
        }

        #region Поля сущности

        /// <summary>
        ///     Поле ЕдиницаРус
        /// </summary>
        /// <value>
        ///     ЕдиницаРус (nvarchar(10), null)
        /// </value>
        public string UnitRus { get; set; }

        /// <summary>
        ///     Поле ЕдиницаЛат
        /// </summary>
        /// <value>
        ///     ЕдиницаЛат (nvarchar(10), null)
        /// </value>
        public string UnitEng { get; set; }

        /// <summary>
        ///     Точность
        /// </summary>
        public new int UnitScale { get; set; }

        #endregion
    }
}