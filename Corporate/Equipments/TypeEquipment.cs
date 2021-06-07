using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Equipments
{
    /// <summary>
    ///     Класс сущности Тип оборудования
    /// </summary>
    [Serializable]
    [DBSource("ТипыОборудования")]
    public class TypeEquipment : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        /// Тип оборудования IPТелефон
        /// </summary>
        public const string IPТелефон = "30";

        /// <summary>
        ///     Конструктор
        /// </summary>
        public TypeEquipment()
        {
            Name = "";
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Код типа оборудования</param>
        public TypeEquipment(string id)
            : base(id)
        {
            Load();
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
        ///     Метод загрузки данных сущности Тип оборудования
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТипыОборудования, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Тип оборудования на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТипаОборудования"].ToString();
                Name = dt.Rows[0]["ТипОборудования"].ToString();
                NameLat = dt.Rows[0]["ТипОборудованияЛат"].ToString();
                IsSnRequired = (byte)dt.Rows[0]["SNобязателен"];
                WithoutAccuratePositioning = (byte)dt.Rows[0]["БезТочногоРасположения"];
                IsСharacteristicsComputer = (byte)dt.Rows[0]["ЕстьХарактеристикиКомпьютера"];
                IsСharacteristicsMonitor = (byte)dt.Rows[0]["ЕстьХарактеристикиМонитора"];
                IsСharacteristicsSim = (byte)dt.Rows[0]["ЕстьХарактеристикиSIM"];
                IsСharacteristicsSoft = (byte)dt.Rows[0]["ЕстьХарактеристикиSoft"];
                ExistPhoneNumber = (byte)dt.Rows[0]["ЕстьТелефонныйНомер"];
                ExistMacAddress = (byte)dt.Rows[0]["ЕстьMACадрес"];
                ExistNetworkName = (byte)dt.Rows[0]["ЕстьСетевоеИмя"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получение списка типов оборудования
        /// </summary>
        /// <param name="query">SQL-запрос</param>
        /// <returns>Список типов оборудования</returns>
        public List<TypeEquipment> GetTypeEquipmentList(string query)
        {
            List<TypeEquipment> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    list = new List<TypeEquipment>();

                    #region Получение порядкового номера столбца

                    var colКодТипаОборудования = dbReader.GetOrdinal("КодТипаОборудования");
                    var colТипОборудования = dbReader.GetOrdinal("ТипОборудования");
                    var colТипОборудованияЛат = dbReader.GetOrdinal("ТипОборудованияЛат");
                    var colSNобязателен = dbReader.GetOrdinal("SNобязателен");
                    var colБезТочногоРасположения = dbReader.GetOrdinal("БезТочногоРасположения");
                    var colЕстьХарактеристикиКомпьютера = dbReader.GetOrdinal("ЕстьХарактеристикиКомпьютера");
                    var colЕстьХарактеристикиМонитора = dbReader.GetOrdinal("ЕстьХарактеристикиМонитора");
                    var colЕстьХарактеристикиSIM = dbReader.GetOrdinal("ЕстьХарактеристикиSIM");
                    var colЕстьХарактеристикиSoft = dbReader.GetOrdinal("ЕстьХарактеристикиSoft");
                    var colЕстьТелефонныйНомер = dbReader.GetOrdinal("ЕстьТелефонныйНомер");
                    var colЕстьMACадрес = dbReader.GetOrdinal("ЕстьMACадрес");
                    var colЕстьСетевоеИмя = dbReader.GetOrdinal("ЕстьСетевоеИмя");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new TypeEquipment();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодТипаОборудования).ToString();
                        row.Name = dbReader.GetString(colТипОборудования);
                        row.NameLat = dbReader.GetString(colТипОборудованияЛат);
                        row.IsSnRequired = dbReader.GetByte(colSNобязателен);
                        row.WithoutAccuratePositioning = dbReader.GetByte(colБезТочногоРасположения);
                        row.IsСharacteristicsComputer = dbReader.GetByte(colЕстьХарактеристикиКомпьютера);
                        row.IsСharacteristicsMonitor = dbReader.GetByte(colЕстьХарактеристикиМонитора);
                        row.IsСharacteristicsSim = dbReader.GetByte(colЕстьХарактеристикиSIM);
                        row.IsСharacteristicsSoft = dbReader.GetByte(colЕстьХарактеристикиSoft);
                        row.ExistPhoneNumber = dbReader.GetByte(colЕстьТелефонныйНомер);
                        row.ExistMacAddress = dbReader.GetByte(colЕстьMACадрес);
                        row.ExistNetworkName = dbReader.GetByte(colЕстьСетевоеИмя);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        #region Поля сущности

        /// <summary>
        ///     ТипОборудованияЛат
        /// </summary>
        /// <value>
        ///     ТипОборудованияЛат (varchar(50), not null)
        /// </value>
        public string NameLat { get; set; }

        /// <summary>
        ///     SNобязателен
        /// </summary>
        /// <value>
        ///     SNобязателен (tinyint, not null)
        /// </value>
        public byte IsSnRequired { get; set; }

        /// <summary>
        ///     БезТочногоРасположения
        /// </summary>
        /// <value>
        ///     БезТочногоРасположения (tinyint, not null)
        /// </value>
        public byte WithoutAccuratePositioning { get; set; }

        /// <summary>
        ///     ЕстьХарактеристикиКомпьютера
        /// </summary>
        /// <value>
        ///     ЕстьХарактеристикиКомпьютера (tinyint, not null)
        /// </value>
        public byte IsСharacteristicsComputer { get; set; }

        /// <summary>
        ///     ЕстьХарактеристикиМонитора
        /// </summary>
        /// <value>
        ///     ЕстьХарактеристикиМонитора (tinyint, not null)
        /// </value>
        public byte IsСharacteristicsMonitor { get; set; }

        /// <summary>
        ///     ЕстьХарактеристикиSIM
        /// </summary>
        /// <value>
        ///     ЕстьХарактеристикиSIM (tinyint, not null)
        /// </value>
        public byte IsСharacteristicsSim { get; set; }

        /// <summary>
        ///     ЕстьХарактеристикиSoft
        /// </summary>
        /// <value>
        ///     ЕстьХарактеристикиSoft (tinyint, not null)
        /// </value>
        public byte IsСharacteristicsSoft { get; set; }

        /// <summary>
        ///     ЕстьТелефонныйНомер
        /// </summary>
        /// <value>
        ///     ЕстьТелефонныйНомер (tinyint, not null)
        /// </value>
        public byte ExistPhoneNumber { get; set; }

        /// <summary>
        ///     ЕстьMACадрес
        /// </summary>
        /// <value>
        ///     ЕстьMACадрес (tinyint, not null)
        /// </value>
        public byte ExistMacAddress { get; set; }

        /// <summary>
        ///     ЕстьСетевоеИмя
        /// </summary>
        /// <value>
        ///     ЕстьСетевоеИмя (tinyint, not null)
        /// </value>
        public byte ExistNetworkName { get; set; }

        #endregion
    }
}