using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    /// ОказанныеУслуги
    /// </summary>
    public class FactUsl : Entity
    {
        #region Поля сущности

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодОказаннойУслуги (int, not null)
        /// </value>
        public int КодОказаннойУслуги { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// GuidОказаннойУслуги (uniqueidentifier, not null)
        /// </value>
        public Guid GuidОказаннойУслуги { get; set; }

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
        /// Агент1 (tinyint, not null)
        /// </value>
        public byte Агент1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Агент2 (tinyint, not null)
        /// </value>
        public byte Агент2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодДвиженияНаСкладе (int, null)
        /// </value>
        public int КодДвиженияНаСкладе { get; set; }

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
        /// КодУчасткаОтправкиВагона (int, null)
        /// </value>
        public int КодУчасткаОтправкиВагона { get; set; }

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
        /// КодЕдиницыИзмерения (int, null)
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
        /// ЦенаБезНДС (money, not null)
        /// </value>
        public decimal ЦенаБезНДС { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаБезНДС (money, not null)
        /// </value>
        public decimal SummaOutNDS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСтавкиНДС (int, not null)
        /// </value>
        public int КодСтавкиНДС { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаНДС (money, not null)
        /// </value>
        public decimal SummaNDS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Всего (money, not null)
        /// </value>
        public decimal Vsego { get; set; }

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
        ///  Конструктор c параметром
        /// </summary>
        public FactUsl(string id)
        {
            FillData(id);
        }

        /// <summary>
        ///  Конструктор
        /// </summary>
        public FactUsl()
        {
        }
        
        /// <summary>
        /// Получить данные по id
        /// </summary>
        public void FillData(string id)
        {
            if(id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ОказаннаяУслуга, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодОказаннойУслуги = dbReader.GetOrdinal("КодОказаннойУслуги");
                    int colGuidОказаннойУслуги = dbReader.GetOrdinal("GuidОказаннойУслуги");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colАгент1 = dbReader.GetOrdinal("Агент1");
                    int colАгент2 = dbReader.GetOrdinal("Агент2");
                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКодУчасткаОтправкиВагона = dbReader.GetOrdinal("КодУчасткаОтправкиВагона");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодОказаннойУслуги = dbReader.GetInt32(colКодОказаннойУслуги);
                        GuidОказаннойУслуги = dbReader.GetGuid(colGuidОказаннойУслуги);
                        КодДокумента = dbReader.GetInt32(colКодДокумента);
                        Агент1 = dbReader.GetByte(colАгент1);
                        Агент2 = dbReader.GetByte(colАгент2);
                        if (!dbReader.IsDBNull(colКодДвиженияНаСкладе)){КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);}
                        КодРесурса = dbReader.GetInt32(colКодРесурса);
                        РесурсРус = dbReader.GetString(colРесурсРус);
                        РесурсЛат = dbReader.GetString(colРесурсЛат);
                        if (!dbReader.IsDBNull(colКодУчасткаОтправкиВагона)){КодУчасткаОтправкиВагона = dbReader.GetInt32(colКодУчасткаОтправкиВагона);}
                        Количество = dbReader.GetDouble(colКоличество);
                        if (!dbReader.IsDBNull(colКодЕдиницыИзмерения)){КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);}
                        if (!dbReader.IsDBNull(colКоэффициент))
                        {Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);
                        SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС);
                        КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);
                        SummaNDS = dbReader.GetDecimal(colСуммаНДС);
                        Vsego = dbReader.GetDecimal(colВсего);
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
        ///  Получить список ОказанныеУслуги по Id
        /// </summary>
        /// <returns></returns>
        public static List<FactUsl> GetFactUslByDocId(string Id)
        {
            if(Id.IsNullEmptyOrZero()) return new List<FactUsl>();
            var query = string.Format("SELECT * FROM vwОказанныеУслуги (nolock) WHERE КодДокумента={0} ORDER BY Порядок", Id);
            return GetFactUslList(query);
        }

        /// <summary>
        /// Получить списко по строке запроса
        /// </summary>
        public static List<FactUsl> GetFactUslList(string query)
        {
            List<FactUsl> list = new List<FactUsl>();
            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new List<FactUsl>();

                    #region Получение порядкового номера столбца

                    int colКодОказаннойУслуги = dbReader.GetOrdinal("КодОказаннойУслуги");
                    int colGuidОказаннойУслуги = dbReader.GetOrdinal("GuidОказаннойУслуги");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colАгент1 = dbReader.GetOrdinal("Агент1");
                    int colАгент2 = dbReader.GetOrdinal("Агент2");
                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКодУчасткаОтправкиВагона = dbReader.GetOrdinal("КодУчасткаОтправкиВагона");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new FactUsl();
                        row.Unavailable = false;
                        row.КодОказаннойУслуги = dbReader.GetInt32(colКодОказаннойУслуги);
                        row.GuidОказаннойУслуги = dbReader.GetGuid(colGuidОказаннойУслуги);
                        row.КодДокумента = dbReader.GetInt32(colКодДокумента);
                        row.Агент1 = dbReader.GetByte(colАгент1);
                        row.Агент2 = dbReader.GetByte(colАгент2);
                        if (!dbReader.IsDBNull(colКодДвиженияНаСкладе)){row.КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);}
                        row.КодРесурса = dbReader.GetInt32(colКодРесурса);
                        row.РесурсРус = dbReader.GetString(colРесурсРус);
                        row.РесурсЛат = dbReader.GetString(colРесурсЛат);
                        if (!dbReader.IsDBNull(colКодУчасткаОтправкиВагона)){row.КодУчасткаОтправкиВагона = dbReader.GetInt32(colКодУчасткаОтправкиВагона);}
                        row.Количество = dbReader.GetDouble(colКоличество);
                        if (!dbReader.IsDBNull(colКодЕдиницыИзмерения)){row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);}
                        if (!dbReader.IsDBNull(colКоэффициент)){row.Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        row.ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);
                        row.SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС);
                        row.КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);
                        row.SummaNDS = dbReader.GetDecimal(colСуммаНДС);
                        row.Vsego = dbReader.GetDecimal(colВсего);
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
