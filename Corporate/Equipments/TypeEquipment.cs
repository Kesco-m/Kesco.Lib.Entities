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
    public class TypeEquipment : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

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
        ///     Заполение объекта ТипОборудования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public void FillData(string id)
        {
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ID_ТипыОборудования, int.Parse(id), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
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

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        id = dbReader.GetInt32(colКодТипаОборудования).ToString();
                        Name = dbReader.GetString(colТипОборудования);
                        NameLat = dbReader.GetString(colТипОборудованияЛат);
                        IsSnRequired = dbReader.GetByte(colSNобязателен);
                        WithoutAccuratePositioning = dbReader.GetByte(colБезТочногоРасположения);
                        IsСharacteristicsComputer = dbReader.GetByte(colЕстьХарактеристикиКомпьютера);
                        IsСharacteristicsMonitor = dbReader.GetByte(colЕстьХарактеристикиМонитора);
                        IsСharacteristicsSim = dbReader.GetByte(colЕстьХарактеристикиSIM);
                        IsСharacteristicsSoft = dbReader.GetByte(colЕстьХарактеристикиSoft);
                        ExistPhoneNumber = dbReader.GetByte(colЕстьТелефонныйНомер);
                        ExistMacAddress = dbReader.GetByte(colЕстьMACадрес);
                        ExistNetworkName = dbReader.GetByte(colЕстьСетевоеИмя);
                    }
                }
                else
                {
                    Unavailable = true;
                }
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