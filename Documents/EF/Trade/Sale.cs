using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    /// Движения на cкладах
    /// </summary>
    public class Sale : Entity
    {
        #region Поля сущности

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодДвиженияНаСкладе (int, not null)
        /// </value>
        public int КодДвиженияНаСкладе { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ТипТранзакции (int, not null)
        /// </value>
        public int ТипТранзакции { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодДокумента (int, not null)
        /// </value>
        public int КодДокумента { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ДатаДвижения (datetime, not null)
        /// </value>
        public DateTime ДатаДвижения { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСкладаОтправителя (int, null)
        /// </value>
        public int КодСкладаОтправителя { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСкладаПолучателя (int, null)
        /// </value>
        public int КодСкладаПолучателя { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодОтправкиВагона (int, null)
        /// </value>
        public int КодОтправкиВагона { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодРесурса (int, not null)
        /// </value>
        public int КодРесурса { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// РесурсРус (varchar(300), not null)
        /// </value>
        public string РесурсРус { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// РесурсЛат (varchar(300), not null)
        /// </value>
        public string РесурсЛат { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Количество (float, not null
        /// </value>
        public double Количество { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодЕдиницыИзмерения (int, not null)
        /// </value>
        public int КодЕдиницыИзмерения { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Коэффициент (float, null
        /// </value>
        public double Коэффициент { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодУпаковки (int, null)
        /// </value>
        public int КодУпаковки { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ЦенаБезНДС (money, null)
        /// </value>
        public decimal ЦенаБезНДС { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаБезНДС (money, null)
        /// </value>
        public decimal SummaOutNDS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСтавкиНДС (int, null)
        /// </value>
        public int КодСтавкиНДС { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаНДС (money, null)
        /// </value>
        public decimal SummaNDS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Акциз (money, null)
        /// </value>
        public decimal Акциз { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Всего (money, null)
        /// </value>
        public decimal Vsego { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСтраныПроисхождения (int, null)
        /// </value>
        public int КодСтраныПроисхождения { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодТаможеннойДекларации (int, null)
        /// </value>
        public int КодТаможеннойДекларации { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Порядок (int, not null)
        /// </value>
        public int Порядок { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Изменил (int, not null)
        /// </value>
        public int Изменил { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Изменено (datetime, not null)
        /// </value>
        public DateTime Изменено { get; set; }

        #endregion

        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        public Sale() {}

        /// <summary>
        ///  Конструктор с параметром
        /// </summary>
        public Sale(string id)
        {
            FillData(id);
        }

        /// <summary>
        ///  Получить данные по Id
        /// </summary>
        public void FillData(string id)
        {
            if(id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ДвижениеНаСкладе, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colДатаДвижения = dbReader.GetOrdinal("ДатаДвижения");
                    int colКодСкладаОтправителя = dbReader.GetOrdinal("КодСкладаОтправителя");
                    int colКодСкладаПолучателя = dbReader.GetOrdinal("КодСкладаПолучателя");
                    int colКодОтправкиВагона = dbReader.GetOrdinal("КодОтправкиВагона");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colКодУпаковки = dbReader.GetOrdinal("КодУпаковки");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colАкциз = dbReader.GetOrdinal("Акциз");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colКодСтраныПроисхождения = dbReader.GetOrdinal("КодСтраныПроисхождения");
                    int colКодТаможеннойДекларации = dbReader.GetOrdinal("КодТаможеннойДекларации");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);
                        ТипТранзакции = dbReader.GetInt32(colТипТранзакции);
                        КодДокумента = dbReader.GetInt32(colКодДокумента);
                        ДатаДвижения = dbReader.GetDateTime(colДатаДвижения);
                        if (!dbReader.IsDBNull(colКодСкладаОтправителя)){КодСкладаОтправителя = dbReader.GetInt32(colКодСкладаОтправителя);}
                        if (!dbReader.IsDBNull(colКодСкладаПолучателя)){КодСкладаПолучателя = dbReader.GetInt32(colКодСкладаПолучателя);}
                        if (!dbReader.IsDBNull(colКодОтправкиВагона)){КодОтправкиВагона = dbReader.GetInt32(colКодОтправкиВагона);}
                        КодРесурса = dbReader.GetInt32(colКодРесурса);
                        РесурсРус = dbReader.GetString(colРесурсРус);
                        РесурсЛат = dbReader.GetString(colРесурсЛат);
                        Количество = dbReader.GetDouble(colКоличество);
                        КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        if (!dbReader.IsDBNull(colКоэффициент)){Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        if (!dbReader.IsDBNull(colКодУпаковки)){КодУпаковки = dbReader.GetInt32(colКодУпаковки);}
                        if (!dbReader.IsDBNull(colЦенаБезНДС)){ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);}
                        if (!dbReader.IsDBNull(colСуммаБезНДС)) { SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС); }
                        if (!dbReader.IsDBNull(colКодСтавкиНДС)){КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);}
                        if (!dbReader.IsDBNull(colСуммаНДС)){SummaNDS = dbReader.GetDecimal(colСуммаНДС);}
                        if (!dbReader.IsDBNull(colАкциз)){Акциз = dbReader.GetDecimal(colАкциз);}
                        if (!dbReader.IsDBNull(colВсего)){Vsego = dbReader.GetDecimal(colВсего);}
                        if (!dbReader.IsDBNull(colКодСтраныПроисхождения)){КодСтраныПроисхождения = dbReader.GetInt32(colКодСтраныПроисхождения);}
                        if (!dbReader.IsDBNull(colКодТаможеннойДекларации)){КодТаможеннойДекларации = dbReader.GetInt32(colКодТаможеннойДекларации);}
                        Порядок = dbReader.GetInt32(colПорядок);
                        Изменил = dbReader.GetInt32(colИзменил);
                        Изменено = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///  Получить ДвиженияНаСкладах до id документа
        /// </summary>
        public static List<Sale> GetSalesByDocId(string id)
        {
            var query = string.Format("SELECT * FROM vwДвиженияНаСкладах (nolock) WHERE КодДокумента={0} ORDER BY Порядок", id);
            return GetSaleList(query);
        }


        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Получить список на основании запроса
        /// </summary>
        public static List<Sale> GetSaleList(string query)
        {
            List<Sale> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new List<Sale>();

                    #region Получение порядкового номера столбца

                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colДатаДвижения = dbReader.GetOrdinal("ДатаДвижения");
                    int colКодСкладаОтправителя = dbReader.GetOrdinal("КодСкладаОтправителя");
                    int colКодСкладаПолучателя = dbReader.GetOrdinal("КодСкладаПолучателя");
                    int colКодОтправкиВагона = dbReader.GetOrdinal("КодОтправкиВагона");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colКодУпаковки = dbReader.GetOrdinal("КодУпаковки");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colАкциз = dbReader.GetOrdinal("Акциз");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colКодСтраныПроисхождения = dbReader.GetOrdinal("КодСтраныПроисхождения");
                    int colКодТаможеннойДекларации = dbReader.GetOrdinal("КодТаможеннойДекларации");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new Sale();
                        row.Unavailable = false;
                        row.КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);
                        row.ТипТранзакции = dbReader.GetInt32(colТипТранзакции);
                        row.КодДокумента = dbReader.GetInt32(colКодДокумента);
                        row.ДатаДвижения = dbReader.GetDateTime(colДатаДвижения);
                        if (!dbReader.IsDBNull(colКодСкладаОтправителя)){row.КодСкладаОтправителя = dbReader.GetInt32(colКодСкладаОтправителя);}
                        if (!dbReader.IsDBNull(colКодСкладаПолучателя)){row.КодСкладаПолучателя = dbReader.GetInt32(colКодСкладаПолучателя);}
                        if (!dbReader.IsDBNull(colКодОтправкиВагона)){row.КодОтправкиВагона = dbReader.GetInt32(colКодОтправкиВагона);}
                        row.КодРесурса = dbReader.GetInt32(colКодРесурса);
                        row.РесурсРус = dbReader.GetString(colРесурсРус);
                        row.РесурсЛат = dbReader.GetString(colРесурсЛат);
                        row.Количество = dbReader.GetDouble(colКоличество);
                        row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        if (!dbReader.IsDBNull(colКоэффициент)){row.Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        if (!dbReader.IsDBNull(colКодУпаковки)){row.КодУпаковки = dbReader.GetInt32(colКодУпаковки);}
                        if (!dbReader.IsDBNull(colЦенаБезНДС)){row.ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);}
                        if (!dbReader.IsDBNull(colСуммаБезНДС)) { row.SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС); }
                        if (!dbReader.IsDBNull(colКодСтавкиНДС)){row.КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);}
                        if (!dbReader.IsDBNull(colСуммаНДС)){row.SummaNDS = dbReader.GetDecimal(colСуммаНДС);}
                        if (!dbReader.IsDBNull(colАкциз)){row.Акциз = dbReader.GetDecimal(colАкциз);}
                        if (!dbReader.IsDBNull(colВсего)){row.Vsego = dbReader.GetDecimal(colВсего);}
                        if (!dbReader.IsDBNull(colКодСтраныПроисхождения)){row.КодСтраныПроисхождения = dbReader.GetInt32(colКодСтраныПроисхождения);}
                        if (!dbReader.IsDBNull(colКодТаможеннойДекларации)){row.КодТаможеннойДекларации = dbReader.GetInt32(colКодТаможеннойДекларации);}
                        row.Порядок = dbReader.GetInt32(colПорядок);
                        row.Изменил = dbReader.GetInt32(colИзменил);
                        row.Изменено = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }
            return list;
        }
    }
}
