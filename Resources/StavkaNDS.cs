using System;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    ///     Ставка НДС
    /// </summary>
    [Serializable]
    public class StavkaNDS : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public StavkaNDS()
        {
        }

        /// <summary>
        ///     Конструктор с загрузкой данных
        /// </summary>
        public StavkaNDS(string id)
        {
            FillData(id);
        }

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
        public void FillData(string id)
        {
            if (id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_СтавкаНДС, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    var colСтавкаНДС = dbReader.GetOrdinal("СтавкаНДС");
                    var colСтавкаНДСЛат = dbReader.GetOrdinal("СтавкаНДСЛат");
                    var colВеличина = dbReader.GetOrdinal("Величина");
                    var colПриоритет = dbReader.GetOrdinal("Приоритет");
                    var colСпецНДС = dbReader.GetOrdinal("СпецНДС");
                    var colКодТерритории = dbReader.GetOrdinal("КодТерритории");
                    var colДействует = dbReader.GetOrdinal("Действует");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);
                        Id = КодСтавкиНДС.ToString();
                        Name = СтавкаНДС = dbReader.GetString(colСтавкаНДС);
                        СтавкаНДСЛат = dbReader.GetString(colСтавкаНДСЛат);
                        Величина = dbReader.GetDouble(colВеличина);
                        Приоритет = dbReader.GetInt32(colПриоритет);

                        if (!dbReader.IsDBNull(colСпецНДС)) СпецНДС = dbReader.GetByte(colСпецНДС);
                        if (!dbReader.IsDBNull(colКодТерритории)) КодТерритории = dbReader.GetInt32(colКодТерритории);

                        Действует = dbReader.GetInt32(colДействует);
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
        ///     КодСтавкиНДС (int, not null)
        /// </value>
        public int КодСтавкиНДС { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СтавкаНДС (nvarchar(50), not null)
        /// </value>
        public string СтавкаНДС { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СтавкаНДСЛат (nvarchar(50), not null)
        /// </value>
        public string СтавкаНДСЛат { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Величина (float, not null)
        /// </value>
        public double Величина { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Приоритет (int, not null)
        /// </value>
        public int Приоритет { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СпецНДС (tinyint, null)
        /// </value>
        public int СпецНДС { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодТерритории (int, null)
        /// </value>
        public int КодТерритории { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Действует (tinyint, not null)
        /// </value>
        public int Действует { get; set; }

        #endregion
    }
}