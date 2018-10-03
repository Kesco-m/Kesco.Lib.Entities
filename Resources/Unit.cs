using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    /// Единица измерения
    /// </summary>
public class Unit : Entity
{
        #region Поля сущности

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
        /// ЕдиницаРус (nvarchar(10), not null)
        /// </value>
        public string ЕдиницаРус { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ЕдиницаЛат (nvarchar(10), not null)
        /// </value>
        public string ЕдиницаЛат { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодГруппыЕдиниц (int, null)
        /// </value>
        public int КодГруппыЕдиниц { get; set; }

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
        /// Описание (varchar(100), not null)
        /// </value>
        public string Описание { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодОКЕИ (char(3), null)
        /// </value>
        public string КодОКЕИ { get; set; }

        #endregion

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Unit()
        {}

        /// <summary>
        ///  Конструктор с загрузкой данных
        /// </summary>
        public Unit(string id)
        {
            FillData(id);
        }

        /// <summary>
        /// Заполнить данные текущего 
        /// </summary>
        public void FillData(string id)
        {
            if(id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ЕдиницаИзмерения, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colЕдиницаРус = dbReader.GetOrdinal("ЕдиницаРус");
                    int colЕдиницаЛат = dbReader.GetOrdinal("ЕдиницаЛат");
                    int colКодГруппыЕдиниц = dbReader.GetOrdinal("КодГруппыЕдиниц");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colКодОКЕИ = dbReader.GetOrdinal("КодОКЕИ");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        Id = КодЕдиницыИзмерения.ToString();
                        Name = ЕдиницаРус = dbReader.GetString(colЕдиницаРус);
                        ЕдиницаЛат = dbReader.GetString(colЕдиницаЛат);
                        if (!dbReader.IsDBNull(colКодГруппыЕдиниц)) { КодГруппыЕдиниц = dbReader.GetInt32(colКодГруппыЕдиниц); }
                        if (!dbReader.IsDBNull(colКоэффициент)) { Коэффициент = dbReader.GetDouble(colКоэффициент); }
                        Описание = dbReader.GetString(colОписание);
                        if (!dbReader.IsDBNull(colКодОКЕИ)) { КодОКЕИ = dbReader.GetString(colКодОКЕИ); }
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
        public override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_resource) : _connectionString; }
        }

        /// <summary>
        /// Получить список запросом
        /// </summary>
        public List<Unit> GetUnitList(string query)
        {
            List<Unit> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    list = new List<Unit>();

                    #region Получение порядкового номера столбца

                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colЕдиницаРус = dbReader.GetOrdinal("ЕдиницаРус");
                    int colЕдиницаЛат = dbReader.GetOrdinal("ЕдиницаЛат");
                    int colКодГруппыЕдиниц = dbReader.GetOrdinal("КодГруппыЕдиниц");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colОписание = dbReader.GetOrdinal("Описание");
                    int colКодОКЕИ = dbReader.GetOrdinal("КодОКЕИ");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new Unit();
                        row.Unavailable = false;
                        row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        row.ЕдиницаРус = dbReader.GetString(colЕдиницаРус);
                        row.ЕдиницаЛат = dbReader.GetString(colЕдиницаЛат);
                        if (!dbReader.IsDBNull(colКодГруппыЕдиниц)) { row.КодГруппыЕдиниц = dbReader.GetInt32(colКодГруппыЕдиниц); }
                        if (!dbReader.IsDBNull(colКоэффициент)) { row.Коэффициент = dbReader.GetDouble(colКоэффициент); }
                        row.Описание = dbReader.GetString(colОписание);
                        if (!dbReader.IsDBNull(colКодОКЕИ)) { row.КодОКЕИ = dbReader.GetString(colКодОКЕИ); }
                        list.Add(row);
                    }
                }
            }
            return list;
        }


}
}
