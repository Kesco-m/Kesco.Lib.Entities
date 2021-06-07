using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using ConvertEx = Kesco.Lib.ConvertExtention.Convert;

namespace Kesco.Lib.Entities.Corporate.Equipments
{
    /// <summary>
    ///     Класс сущности Модель оборудования
    /// </summary>
    [Serializable]
    [DBSource("МоделиОборудования")]
    public class ModelEquipment : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public ModelEquipment() { }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Код модели</param>
        public ModelEquipment(string id) : base(id)
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
        ///     Метод загрузки данных сущности Модель оборудования
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            var dt = DBManager.GetData(SQLQueries.SELECT_ID_МодельОборудования, CN, CommandType.Text, sqlParams);
            FillData(dt);
        }

        /// <summary>
        ///     Инициализация сущности Модель оборудования на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодМоделиОборудования"].ToString();
                ResourceId = dt.Rows[0]["КодРесурса"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодРесурса"]);
                TypeEquipmentId = Convert.ToInt32(dt.Rows[0]["КодТипаОборудования"]);
                Name = dt.Rows[0]["МодельОборудования"].ToString();
                PN = dt.Rows[0]["PN"].ToString();
                CPU = dt.Rows[0]["CPU"].ToString();
                MonitorSize = Convert.ToDouble(dt.Rows[0]["РазмерМонитора"]);
                MemoryTypeId = Convert.ToInt32(dt.Rows[0]["КодТипаПамяти"]);
                MemoryBanks = Convert.ToByte(dt.Rows[0]["БанковПамяти"]);
                IsMultimedia = Convert.ToByte(dt.Rows[0]["Мультимедиа"]);
                Net = Convert.ToByte(dt.Rows[0]["Net"]);
                NetName = dt.Rows[0]["НазваниеNet"].ToString();
                Driver = dt.Rows[0]["Драйвер"].ToString();
                HealthResource = dt.Rows[0]["Ресурс"].ToString();
                PhoneTemplate = dt.Rows[0]["ШаблонТелефона"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодРесурса", ResourceId },
                { "@КодТипаОборудования", TypeEquipmentId },
                { "@МодельОборудования", Name ?? string.Empty },
                { "@PN", PN ?? string.Empty },
                { "@CPU", CPU ?? string.Empty },
                { "@РазмерМонитора", MonitorSize },
                { "@КодТипаПамяти", MemoryTypeId },
                { "@БанковПамяти", MemoryBanks },
                { "@Мультимедиа", IsMultimedia },
                { "@Net", Net },
                { "@НазваниеNet", NetName ?? string.Empty },
                { "@Драйвер", Driver ?? string.Empty },
                { "@Ресурс", HealthResource ?? string.Empty },
                { "@ШаблонТелефона", PhoneTemplate ?? string.Empty }
            };

            if (IsNew)
            {
                var modId = DBManager.ExecuteScalar(SQLQueries.INSERT_МоделиОборудования,
                    CommandType.Text, Config.DS_user, sqlParams);

                if (modId != null)
                    Id = modId.ToString();
            }
            else
            {
                sqlParams.Add("@КодМоделиОборудования", Id);

                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_МоделиОборудования, CommandType.Text,
                    Config.DS_user, sqlParams, timeout: 120);
            }
        }

        /// <summary>
        ///     Метод сохранения шаблона телефона
        /// </summary>
        public void SavePhoneTemplate()
        {
            if (IsNew)
                return;

            var sqlParams = new Dictionary<string, object>
            {
                {"@КодМоделиОборудования", Id},
                {"@ШаблонТелефона", PhoneTemplate ?? string.Empty}
            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_МоделиОборудования_ШаблонТелефона, CommandType.Text,
                Config.DS_user, sqlParams, timeout: 120);
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
                    var colШаблонТелефона = dbReader.GetOrdinal("ШаблонТелефона");
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
                        row.MemoryTypeId = dbReader.GetInt32(colКодТипаПамяти);
                        row.MemoryBanks = dbReader.GetByte(colБанковПамяти);
                        row.IsMultimedia = dbReader.GetByte(colМультимедиа);
                        row.Net = dbReader.GetByte(colNet);
                        row.NetName = dbReader.GetString(colНазваниеNet);
                        row.Driver = dbReader.GetString(colДрайвер);
                        row.HealthResource = dbReader.GetString(colРесурс);
                        row.PhoneTemplate = dbReader.GetString(colШаблонТелефона);
                        row.ChangedId = dbReader.GetInt32(colИзменил);
                        row.ChangedTime = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        #region Поля сущности

        /// <summary>
        ///     Код ресурса
        /// </summary>
        /// <value>
        ///     КодРесурса (int, null)
        /// </value>
        [DBField("КодРесурса")]
        public int ResourceId
        {
            get { return string.IsNullOrEmpty(ResourceIdBind.Value) ? 0: int.Parse(ResourceIdBind.Value); }
            set { ResourceIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }

        /// <summary>
        ///     Binder для поля КодРесурса
        /// </summary>
        public BinderValue ResourceIdBind = new BinderValue();

        /// <summary>
        ///     Код типа оборудования
        /// </summary>
        /// <value>
        ///     КодТипаОборудования (int, not null)
        /// </value>
        [DBField("КодТипаОборудования")]
        public int TypeEquipmentId
        {
            get { return string.IsNullOrEmpty(TypeEquipmentIdBind.Value) ? 0 : int.Parse(TypeEquipmentIdBind.Value); }
            set { TypeEquipmentIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }

        /// <summary>
        ///     Binder для поля КодТипаОборудования
        /// </summary>
        public BinderValue TypeEquipmentIdBind = new BinderValue();

        /// <summary>
        ///     Возвращает объект типа TypeEquipment в зависимости от значения TypeEquipmentId
        /// </summary>
        public TypeEquipment TypeEquipment => TypeEquipmentId > 0 ? new TypeEquipment(TypeEquipmentId.ToString()) : null;

        /// <summary>
        ///     Название модели оборудования
        /// </summary>
        /// <value>
        ///     Название (varchar(100), not null)
        /// </value>
        [DBField("Название")]
        public new string Name
        {
            get { return NameBind.Value; }
            set { NameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Название
        /// </summary>
        public BinderValue NameBind = new BinderValue();

        /// <summary>
        ///     PartNumber
        /// </summary>
        /// <value>
        ///     PN (varchar(50), not null)
        /// </value>
        [DBField("PN")]
        public string PN
        {
            get { return PNBind.Value; }
            set { PNBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля PN
        /// </summary>
        public BinderValue PNBind = new BinderValue();

        /// <summary>
        ///     CPU
        /// </summary>
        /// <value>
        ///     CPU (varchar(50), not null)
        /// </value>
        [DBField("CPU")]
        public string CPU
        {
            get { return CPUBind.Value; }
            set { CPUBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля CPU
        /// </summary>
        public BinderValue CPUBind = new BinderValue();

        /// <summary>
        ///     РазмерМонитора
        /// </summary>
        /// <value>
        ///     РазмерМонитора (real(, not null)
        /// </value>
        [DBField("РазмерМонитора")]
        public double MonitorSize
        {
            get { return ConvertEx.Str2Double(MonitorSizeBind.Value, 0); }
            set { MonitorSizeBind.Value = value > 0 ? ConvertEx.Double2Str(value, 1) : string.Empty; }
        }

        /// <summary>
        ///     Binder для поля РазмерМонитора
        /// </summary>
        public BinderValue MonitorSizeBind = new BinderValue();

        /// <summary>
        ///     Код типа памяти
        /// </summary>
        /// <value>
        ///     КодТипаПамяти (int, not null)
        /// </value>
        [DBField("КодТипаПамяти")]
        public int MemoryTypeId
        {
            get { return string.IsNullOrEmpty(MemoryTypeIdBind.Value) ? 0 : int.Parse(MemoryTypeIdBind.Value); }
            set { MemoryTypeIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }

        /// <summary>
        ///     Binder для поля КодТипаПамяти
        /// </summary>
        public BinderValue MemoryTypeIdBind = new BinderValue();

        /// <summary>
        ///     Банков памяти
        /// </summary>
        /// <value>
        ///     БанковПамяти (tinyint, not null)
        /// </value>
        [DBField("БанковПамяти")]
        public byte MemoryBanks
        {
            get { return string.IsNullOrEmpty(MemoryBanksBind.Value) ? (byte)0 : Convert.ToByte(MemoryBanksBind.Value); }
            set { MemoryBanksBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля БанковПамяти
        /// </summary>
        public BinderValue MemoryBanksBind = new BinderValue();

        /// <summary>
        ///     Мультимедиа
        /// </summary>
        /// <value>
        ///     Мультимедиа (tinyint, not null)
        /// </value>
        [DBField("Мультимедиа")]
        public byte IsMultimedia
        {
            get { return string.IsNullOrEmpty(IsMultimediaBind.Value) ? (byte)0 : Convert.ToByte(IsMultimediaBind.Value); }
            set { IsMultimediaBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Мультимедиа
        /// </summary>
        public BinderValue IsMultimediaBind = new BinderValue();

        /// <summary>
        ///     Net
        /// </summary>
        /// <value>
        ///     Net (tinyint, not null)
        /// </value>
        [DBField("Net")]
        public byte Net
        {
            get { return string.IsNullOrEmpty(NetBind.Value) ? (byte)0 : Convert.ToByte(NetBind.Value); }
            set { NetBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Net
        /// </summary>
        public BinderValue NetBind = new BinderValue();

        /// <summary>
        ///     Название Net
        /// </summary>
        /// <value>
        ///     НазваниеNet (varchar(50), not null)
        /// </value>
        [DBField("НазваниеNet")]
        public string NetName
        {
            get { return NetNameBind.Value; }
            set { NetNameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля НазваниеNet
        /// </summary>
        public BinderValue NetNameBind = new BinderValue();

        /// <summary>
        ///     Драйвер
        /// </summary>
        /// <value>
        ///     Драйвер (varchar(300), not null)
        /// </value>
        [DBField("Драйвер")]
        public string Driver
        {
            get { return DriverBind.Value; }
            set { DriverBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Драйвер
        /// </summary>
        public BinderValue DriverBind = new BinderValue();

        /// <summary>
        ///     Ресурс
        /// </summary>
        /// <value>
        ///     Ресурс (varchar(50), not null)
        /// </value>
        [DBField("Ресурс")]
        public string HealthResource
        {
            get { return HealthResourceBind.Value; }
            set { HealthResourceBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Ресурс
        /// </summary>
        public BinderValue HealthResourceBind = new BinderValue();

        /// <summary>
        ///     Шаблон телефона
        /// </summary>
        /// <value>
        ///     ШаблонТелефона (varchar(max), not null)
        /// </value>
        [DBField("ШаблонТелефона")]
        public string PhoneTemplate
        {
            get { return PhoneTemplateBind.Value; }
            set { PhoneTemplateBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ШаблонТелефона
        /// </summary>
        public BinderValue PhoneTemplateBind = new BinderValue();

        /// <summary>
        ///     Изменил
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        [DBField("Изменил", "", false)]
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        [DBField("Изменено", "", false)]
        public DateTime ChangedTime { get; set; }

        #endregion
    }
}