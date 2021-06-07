using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    ///     Единица измерения дополнительная
    /// </summary>
    [Serializable]
    public class UnitAdv : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public UnitAdv()
        {
        }

        /// <summary>
        ///     Конструктор с загрузкой данных
        /// </summary>
        public UnitAdv(string id)
        {
            FillData(id);
        }

        /// <summary>
        ///  Конструктор с загрузкой данных
        /// </summary>
        /// <param name="id">Код единицы измерения</param>
        /// <param name="resourceId">Код ресурса</param>
        public UnitAdv(string id, string resourceId)
        {
            FillData(id, resourceId);
        }

        /// <summary>
        ///     Дополнительная ед. изм.
        /// </summary>
        public Unit Unit => new Unit(КодЕдиницыИзмерения.ToString());

        /// <summary>
        ///     Строка подключения к БД.
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
        ///     Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_resource
            : _connectionString;

        /// <summary>
        ///     Заполнить данные текущего
        /// </summary>
        public void FillData(string id, string resourceId = "")
        {
            if (id.IsNullEmptyOrZero()) return;

            var sqlParams = new Dictionary<string, object> { { "@id", id }, { "@КодРесурса", resourceId } };

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ЕдиницаИзмеренияДополнительные, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодЕдиницыИзмеренияДополнительной = dbReader.GetOrdinal("КодЕдиницыИзмеренияДополнительной");
                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colЕдиницаРус = dbReader.GetOrdinal("ЕдиницаРус");
                    var colЕдиницаЛат = dbReader.GetOrdinal("ЕдиницаЛат");
                    var colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    var colКоэффициент = dbReader.GetOrdinal("Коэффициент");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодЕдиницыИзмеренияДополнительной = dbReader.GetInt32(colКодЕдиницыИзмеренияДополнительной);
                        Id = КодЕдиницыИзмеренияДополнительной.ToString();
                        Name = ЕдиницаРус = dbReader.GetString(colЕдиницаРус);
                        КодРесурса = dbReader.GetInt32(colКодРесурса);
                        КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        Коэффициент = dbReader.GetDouble(colКоэффициент);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        #region Поля сущности

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодЕдиницыИзмеренияДополнительной (int, not null)
        /// </value>
        public int КодЕдиницыИзмеренияДополнительной { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодРесурса (int, not null)
        /// </value>
        public int КодРесурса { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     ЕдиницаРус (nvarchar(10), not null)
        /// </value>
        public string ЕдиницаРус { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     ЕдиницаЛат (nvarchar(10), not null)
        /// </value>
        public string ЕдиницаЛат { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодЕдиницыИзмерения (int, not null)
        /// </value>
        public int КодЕдиницыИзмерения { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Точность (tinyint, not null)
        /// </value>
        public int Точность { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КоличествоЕдиниц (decimal(18,4), not null
        /// </value>
        public decimal КоличествоЕдиниц { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КоличествоЕдиницОсновных (decimal(18,4), not null
        /// </value>
        public decimal КоличествоЕдиницОсновных { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Коэффициент (float, not null)
        /// </value>
        public double Коэффициент { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     МассаБрутто (decimal(18,3), null)
        /// </value>
        public decimal МассаБрутто { get; set; }

        #endregion
    }
}