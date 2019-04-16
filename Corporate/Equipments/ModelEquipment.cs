using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Equipments
{
    /// <summary>
    ///     Класс сущности Модель оборудования
    /// </summary>
    [Serializable]
    public class ModelEquipment : Entity
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
        ///     Заполнение объекта МодельОборудования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public void FillData(string id)
        {
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ID_МодельОборудования, int.Parse(id), CommandType.Text,
                    CN)
            )
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодМоделиОборудования = dbReader.GetOrdinal("КодМоделиОборудования");
                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colКодТипаОборудования = dbReader.GetOrdinal("КодТипаОборудования");
                    var colМодельОборудования = dbReader.GetOrdinal("МодельОборудования");
                    var colPN = dbReader.GetOrdinal("PN");
                    var colCPU = dbReader.GetOrdinal("CPU");
                    var colРазмерМонитора = dbReader.GetOrdinal("РазмерМонитора");
                    var colКодТипаПамяти = dbReader.GetOrdinal("КодТипаПамяти");
                    var colБанковПамяти = dbReader.GetOrdinal("БанковПамяти");
                    var colМультимедиа = dbReader.GetOrdinal("Мультимедиа");
                    var colNet = dbReader.GetOrdinal("Net");
                    var colНазваниеNet = dbReader.GetOrdinal("НазваниеNet");
                    var colДрайвер = dbReader.GetOrdinal("Драйвер");
                    var colРесурс = dbReader.GetOrdinal("Ресурс");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодМоделиОборудования).ToString();
                        if (!dbReader.IsDBNull(colКодРесурса)) ResourceId = dbReader.GetInt32(colКодРесурса);
                        TypeEquipmentId = dbReader.GetInt32(colКодТипаОборудования);
                        Name = dbReader.GetString(colМодельОборудования);
                        PN = dbReader.GetString(colPN);
                        CPU = dbReader.GetString(colCPU);
                        MonitorSize = dbReader.GetDouble(colРазмерМонитора);
                        TypeMemoryId = dbReader.GetInt32(colКодТипаПамяти);
                        BankMemory = dbReader.GetByte(colБанковПамяти);
                        IsMultimedia = dbReader.GetByte(colМультимедиа);
                        Net = dbReader.GetByte(colNet);
                        NetName = dbReader.GetString(colНазваниеNet);
                        Driver = dbReader.GetString(colДрайвер);
                        LongWill = dbReader.GetString(colРесурс);
                        ChangeEmployeeId = dbReader.GetInt32(colИзменил);
                        ChangedDate = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Формирования списка моделей оборудования
        /// </summary>
        /// <param name="query">SQL-Запрос</param>
        /// <returns>Список моделей оборудования</returns>
        public List<ModelEquipment> GetModelEquipmentList(string query)
        {
            List<ModelEquipment> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    list = new List<ModelEquipment>();

                    #region Получение порядкового номера столбца

                    var colКодМоделиОборудования = dbReader.GetOrdinal("КодМоделиОборудования");
                    var colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    var colКодТипаОборудования = dbReader.GetOrdinal("КодТипаОборудования");
                    var colМодельОборудования = dbReader.GetOrdinal("МодельОборудования");
                    var colPN = dbReader.GetOrdinal("PN");
                    var colCPU = dbReader.GetOrdinal("CPU");
                    var colРазмерМонитора = dbReader.GetOrdinal("РазмерМонитора");
                    var colКодТипаПамяти = dbReader.GetOrdinal("КодТипаПамяти");
                    var colБанковПамяти = dbReader.GetOrdinal("БанковПамяти");
                    var colМультимедиа = dbReader.GetOrdinal("Мультимедиа");
                    var colNet = dbReader.GetOrdinal("Net");
                    var colНазваниеNet = dbReader.GetOrdinal("НазваниеNet");
                    var colДрайвер = dbReader.GetOrdinal("Драйвер");
                    var colРесурс = dbReader.GetOrdinal("Ресурс");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new ModelEquipment();
                        row.Unavailable = false;
                        row.Id = dbReader.GetInt32(colКодМоделиОборудования).ToString();
                        if (!dbReader.IsDBNull(colКодРесурса)) row.ResourceId = dbReader.GetInt32(colКодРесурса);
                        row.TypeEquipmentId = dbReader.GetInt32(colКодТипаОборудования);
                        row.Name = dbReader.GetString(colМодельОборудования);
                        row.PN = dbReader.GetString(colPN);
                        row.CPU = dbReader.GetString(colCPU);
                        row.MonitorSize = dbReader.GetDouble(colРазмерМонитора);
                        row.TypeMemoryId = dbReader.GetInt32(colКодТипаПамяти);
                        row.BankMemory = dbReader.GetByte(colБанковПамяти);
                        row.IsMultimedia = dbReader.GetByte(colМультимедиа);
                        row.Net = dbReader.GetByte(colNet);
                        row.NetName = dbReader.GetString(colНазваниеNet);
                        row.Driver = dbReader.GetString(colДрайвер);
                        row.LongWill = dbReader.GetString(colРесурс);
                        row.ChangeEmployeeId = dbReader.GetInt32(colИзменил);
                        row.ChangedDate = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        #region Поля сущности

        /// <summary>
        ///     КодРесурса
        /// </summary>
        /// <value>
        ///     КодРесурса (int, null)
        /// </value>
        public int ResourceId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодТипаОборудования (int, not null)
        /// </value>
        public int TypeEquipmentId { get; set; }

        /// <summary>
        ///     PartNumber
        /// </summary>
        /// <value>
        ///     PN (varchar(50), not null)
        /// </value>
        public string PN { get; set; }

        /// <summary>
        ///     CPU
        /// </summary>
        /// <value>
        ///     CPU (varchar(50), not null)
        /// </value>
        public string CPU { get; set; }

        /// <summary>
        ///     РазмерМонитора
        /// </summary>
        /// <value>
        ///     РазмерМонитора (real(, not null)
        /// </value>
        public double MonitorSize { get; set; }

        /// <summary>
        ///     КодТипаПамяти
        /// </summary>
        /// <value>
        ///     КодТипаПамяти (int, not null)
        /// </value>
        public int TypeMemoryId { get; set; }

        /// <summary>
        ///     БанковПамяти
        /// </summary>
        /// <value>
        ///     БанковПамяти (tinyint, not null)
        /// </value>
        public byte BankMemory { get; set; }

        /// <summary>
        ///     Мультимедиа
        /// </summary>
        /// <value>
        ///     Мультимедиа (tinyint, not null)
        /// </value>
        public byte IsMultimedia { get; set; }

        /// <summary>
        ///     Net
        /// </summary>
        /// <value>
        ///     Net (tinyint, not null)
        /// </value>
        public byte Net { get; set; }

        /// <summary>
        ///     НазваниеNet
        /// </summary>
        /// <value>
        ///     НазваниеNet (varchar(50), not null)
        /// </value>
        public string NetName { get; set; }

        /// <summary>
        ///     Драйвер
        /// </summary>
        /// <value>
        ///     Драйвер (varchar(300), not null)
        /// </value>
        public string Driver { get; set; }

        /// <summary>
        ///     Ресурс
        /// </summary>
        /// <value>
        ///     Ресурс (varchar(50), not null)
        /// </value>
        public string LongWill { get; set; }

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
    }
}