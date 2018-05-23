using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    /// Класс валют
    /// </summary>
    /// <example>
    ///  Получить валюту по Id  Currency.GetCurrency(value);
    /// </example>
    public class Currency : Resource
    {
        /// <summary>
        ///  Запрещаем создание экземпляров
        ///  Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private Currency(){}
        /// <summary>
        ///  Запрещаем создание экземпляров
        ///  Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private Currency(int id) {}
        /// <summary>
        ///  Запрещаем создание экземпляров
        ///  Использование Получить валюту по Id Currency.GetCurrency(value);
        /// </summary>
        private  Currency(string id) {}

        #region Поля сущности

        /// <summary>
        ///  Поле ЕдиницаРус
        /// </summary>
        /// <value>
        /// ЕдиницаРус (nvarchar(10), null)
        /// </value>
        public string  UnitRus { get; set; }

        /// <summary>
        ///  Поле ЕдиницаЛат
        /// </summary>
        /// <value>
        /// ЕдиницаЛат (nvarchar(10), null)
        /// </value>
        public string UnitEng { get; set; }

        /// <summary>
        ///  Точность
        /// </summary>
        public int UnitScale { get; set; }

        #endregion

        /// <summary>
        ///  backing field для AllCurrencies
        /// </summary>
        private static Dictionary<int, Currency> _allCurrencies;

        /// <summary>
        /// Получить все валюты
        /// </summary>
        /// <remarks>
        ///  Кешируется
        /// </remarks>
        public static Dictionary<int, Currency> GetAllCurrencies()
        {
            return _allCurrencies ?? (_allCurrencies = GetCurrencyList()); 
        }



        /// <summary>
        ///  Получить валюту по ID, в случае неудачи возвращает null
        /// </summary>
        public static Currency GetCurrency(int id)
        {
            var cur = GetAllCurrencies();
            if (cur != null && cur.ContainsKey(id))
                return cur[id];

            return null;
        }

        /// <summary>
        ///  Получить валюту по ID
        /// </summary>
        public static Currency GetCurrency(string id)
        {
            return GetCurrency(id.ToInt());
        }

        /// <summary>
        ///  Получить список всех валют
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

                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colЕдиницаРус = dbReader.GetOrdinal("ЕдиницаРус");
                    int colЕдиницаЛат = dbReader.GetOrdinal("ЕдиницаЛат");
                    int colТочность = dbReader.GetOrdinal("Точность");

                    #endregion

                    while (dbReader.Read())
                    {
                        var id = dbReader.GetInt32(colКодРесурса);
                        var row = new Currency
                        {
                            Unavailable = false,
                            Id = id.ToString(),
                            Name = dbReader.GetString(colРесурсРус)
                        };
                        if (!dbReader.IsDBNull(colЕдиницаРус)) { row.UnitRus = dbReader.GetString(colЕдиницаРус); }
                        if (!dbReader.IsDBNull(colЕдиницаЛат)) { row.UnitEng = dbReader.GetString(colЕдиницаЛат); }
                        if (!dbReader.IsDBNull(colТочность))   { row.UnitScale = dbReader.GetByte(colТочность); }
                        list.Add(id, row);
                    }
                }
            }
            return list;
        }

    }
}
