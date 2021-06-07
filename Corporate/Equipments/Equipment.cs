using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Equipments
{
    /// <summary>
    ///     Класс сущности оборудование
    /// </summary>
    [Serializable]
    public class Equipment : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID</param>
        public Equipment(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Equipment()
        {
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Инициализация сущности "Оборудование" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных Оборудование</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодОборудования"].ToString();
                SN = (string) dt.Rows[0]["SN"];
                ModelId = (int) dt.Rows[0]["КодМоделиОборудования"];
                MacAddress = (string) dt.Rows[0]["MACадрес"];
                MacAddress2 = (string) dt.Rows[0]["MACадрес2"];
                MacAddressIlo = (string) dt.Rows[0]["MACадресILO"];
                NetworkName = (string) dt.Rows[0]["СетевоеИмя"];
                CameraIsConnected = (byte) dt.Rows[0]["ПодключенаКамера"];
                Description = (string) dt.Rows[0]["Примечания"];

                WriteOff = dt.Rows[0]["Списано"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(dt.Rows[0]["Списано"]);

                DocumentWriteOffId = dt.Rows[0]["КодДокументаСписания"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодДокументаСписания"]);

                PersonOwnerId = dt.Rows[0]["КодЛицаВладельца"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодЛицаВладельца"]);

                PersonTenantId = dt.Rows[0]["КодЛицаАрендатора"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодЛицаАрендатора"]);

                PersonShipperId = dt.Rows[0]["КодЛицаПоставщика"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодЛицаПоставщика"]);

                ChangeEmployeeId = (int) dt.Rows[0]["Изменил"];
                ChangedDate = (DateTime) dt.Rows[0]["Изменено"];
                Name = NetworkName.IsNullEmptyOrZero() ? dt.Rows[0]["МодельОборудования"] + " SN " + SN : NetworkName;
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Инициализация сущности "Оборудование" на основе таблицы данных о складе
        /// </summary>
        /// <param name="dt">Таблица данных склада</param>
        public static List<Equipment> GetEquipmentList(DataTable dt)
        {
            return (from object t in dt.Rows
                select new Equipment
                {
                    Unavailable = false,
                    Id = dt.Rows[0]["КодОборудования"].ToString(),
                    SN = (string) dt.Rows[0]["SN"],
                    ModelId = (int) dt.Rows[0]["КодМоделиОборудования"],
                    MacAddress = (string) dt.Rows[0]["MACадрес"],
                    MacAddress2 = (string) dt.Rows[0]["MACадрес2"],
                    MacAddressIlo = (string) dt.Rows[0]["MACадресILO"],
                    NetworkName = (string) dt.Rows[0]["СетевоеИмя"],
                    CameraIsConnected = (byte) dt.Rows[0]["ПодключенаКамера"],
                    Description = (string) dt.Rows[0]["Примечания"],

                    WriteOff = dt.Rows[0]["Списано"] == DBNull.Value
                        ? DateTime.MinValue
                        : Convert.ToDateTime(dt.Rows[0]["Списано"]),

                    DocumentWriteOffId = dt.Rows[0]["КодДокументаСписания"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(dt.Rows[0]["КодДокументаСписания"]),

                    PersonOwnerId = dt.Rows[0]["КодЛицаВладельца"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(dt.Rows[0]["КодЛицаВладельца"]),

                    PersonTenantId = dt.Rows[0]["КодЛицаАрендатора"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодЛицаАрендатора"]),

                    PersonShipperId = dt.Rows[0]["КодЛицаПоставщика"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодЛицаПоставщика"]),

                    ChangeEmployeeId = (int) dt.Rows[0]["Изменил"],
                    ChangedDate = (DateTime) dt.Rows[0]["Изменено"]
                }).ToList();
        }

        /// <summary>
        ///     Инициализация сущности "Оборудование" на основе DBReader
        /// </summary>
        /// <param name="dbReader">Объект типа DBReader</param>
        /// <returns>Список оборудования</returns>
        public static List<Equipment> GetEquipmentList(DBReader dbReader)
        {
            var list = new List<Equipment>();
            if (!dbReader.HasRows) return list;

            var colКодОборудования = dbReader.GetOrdinal("КодОборудования");
            var colКодТипаОборудования = dbReader.GetOrdinal("КодТипаОборудования");
            var colТипОборудования = dbReader.GetOrdinal("ТипОборудования");
            var colКодМоделиОборудования = dbReader.GetOrdinal("КодМоделиОборудования");
            var colМодельОборудования = dbReader.GetOrdinal("МодельОборудования");
            var colКодРасположения = dbReader.GetOrdinal("КодРасположения");
            var colРасположениеPath = dbReader.GetOrdinal("РасположениеPath");
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colСотрудник = dbReader.GetOrdinal("Сотрудник");

            while (dbReader.Read())
            {
                var row = new Equipment();
                row.Unavailable = false;
                row.Id = dbReader.GetInt32(colКодОборудования).ToString();
                row.TypeId = dbReader.GetInt32(colКодТипаОборудования);
                row.TypeName = dbReader.GetString(colТипОборудования);
                row.ModelId = dbReader.GetInt32(colКодМоделиОборудования);
                row.ModelName = dbReader.GetString(colМодельОборудования);
                if (!dbReader.IsDBNull(colКодРасположения))
                {
                    row.LocationId = dbReader.GetInt32(colКодРасположения);
                    row.LocationName = dbReader.GetString(colРасположениеPath);
                }

                if (!dbReader.IsDBNull(colКодСотрудника))
                {
                    row.EmployeeId = dbReader.GetInt32(colКодСотрудника);
                    row.EmployeeName = dbReader.GetString(colСотрудник);
                }

                list.Add(row);
            }

            return list;
        }


        /// <summary>
        ///     Метод загрузки данных сущности "Оборудование"
        /// </summary>
        public sealed override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Оборудование, CN, CommandType.Text, sqlParams));
        }

        #region Поля сущности

        /// <summary>
        ///     Серийный номер
        /// </summary>
        /// <value>
        ///     SN (varchar(50), not null)
        /// </value>
        public string SN { get; set; }

        /// <summary>
        ///     КодМоделиОборудования
        /// </summary>
        /// <value>
        ///     КодМоделиОборудования (int, not null)
        /// </value>
        public int ModelId { get; set; }

        /// <summary>
        ///     MACадрес
        /// </summary>
        /// <value>
        ///     MACадрес (varchar(12), not null)
        /// </value>
        public string MacAddress { get; set; }

        /// <summary>
        ///     MACадрес2
        /// </summary>
        /// <value>
        ///     MACадрес2 (varchar(12), not null)
        /// </value>
        public string MacAddress2 { get; set; }

        /// <summary>
        ///     MACадресILO
        /// </summary>
        /// <value>
        ///     MACадресILO (varchar(12), not null)
        /// </value>
        public string MacAddressIlo { get; set; }

        /// <summary>
        ///     СетевоеИмя
        /// </summary>
        /// <value>
        ///     СетевоеИмя (varchar(50), not null)
        /// </value>
        public string NetworkName { get; set; }

        /// <summary>
        ///     ПодключенаКамера
        /// </summary>
        /// <value>
        ///     ПодключенаКамера (tinyint, not null)
        /// </value>
        public byte CameraIsConnected { get; set; }

        /// <summary>
        ///     Списано
        /// </summary>
        /// <value>
        ///     Списано (datetime, null)
        /// </value>
        public DateTime? WriteOff { get; set; }

        /// <summary>
        ///     КодДокументаСписания
        /// </summary>
        /// <value>
        ///     КодДокументаСписания (int, null)
        /// </value>
        public int? DocumentWriteOffId { get; set; }

        /// <summary>
        ///     Примечания
        /// </summary>
        /// <value>
        ///     Примечания (varchar(300), not null)
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     КодЛицаВладельца
        /// </summary>
        /// <value>
        ///     КодЛицаВладельца (int, null)
        /// </value>
        public int? PersonOwnerId { get; set; }

        /// <summary>
        ///     КодЛицаАрендатора
        /// </summary>
        /// <value>
        ///     КодЛицаАрендатора (int, null)
        /// </value>
        public int? PersonTenantId { get; set; }

        /// <summary>
        ///     КодЛицаПоставщика
        /// </summary>
        /// <value>
        ///     КодЛицаПоставщика (int, null)
        /// </value>
        public int? PersonShipperId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        public int ChangeEmployeeId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        public DateTime ChangedDate { get; set; }

        #endregion

        #region Дополнительные поля

        /// <summary>
        ///     Наименование модели оборудования
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        ///     Код типа оборудования
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        ///     Наименование типа оборудования
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///     Код последнего расположения оборудования
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        ///     Полный путь к последнему расположению оборудования
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Полный путь к расположению с добавлением пробелов после /
        /// </summary>
        public string LocationName_WhiteSpace =>
            string.IsNullOrEmpty(LocationName) ? LocationName : LocationName.Replace("/", "/ ");

        /// <summary>
        ///     Код ответственного за оборудования
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     ФИО ответственного за оборудования
        /// </summary>
        public string EmployeeName { get; set; }

        #endregion
    }
}