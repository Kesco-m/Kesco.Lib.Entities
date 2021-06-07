using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Nets
{
    /// <summary>
    ///     Класс сущности сеть
    /// </summary>
    [Serializable]
    public class Net : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID</param>
        public Net(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Net()
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
        ///     Метод загрузки данных сущности "Сеть"
        /// </summary>
        public sealed override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Сеть, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности "Сеть" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных Сеть</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодЛокальнойСети"].ToString();
                EquipmentId = (int)dt.Rows[0]["КодОборудования"];
                Port = (byte)dt.Rows[0]["Порт"];
                POE = (byte)dt.Rows[0]["PoE"];
                PersonCustomerId = (int)dt.Rows[0]["КодЛицаЗаказчика"];
                Connected = (string)dt.Rows[0]["Подключено"];
                Description = (string)dt.Rows[0]["Примечания"];
                SocketId = dt.Rows[0]["КодРозетки"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодРозетки"]);
                ReturnDate = dt.Rows[0]["СрокВозврата"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["СрокВозврата"]);
                EmployeeId = dt.Rows[0]["КодСотрудника"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодСотрудника"]);
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedDate = (DateTime)dt.Rows[0]["Изменено"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Инициализация сущности "Сеть" на основе DBReader
        /// </summary>
        /// <param name="dbReader">Объект типа DBReader</param>
        /// <returns>Список оборудования</returns>
        public static List<Net> GetNetList(DBReader dbReader)
        {
            var list = new List<Net>();
            if (!dbReader.HasRows) return list;

            var colКодЛокальнойСети = dbReader.GetOrdinal("КодЛокальнойСети");
            var colКодОборудования = dbReader.GetOrdinal("КодОборудования");
            var colПорт = dbReader.GetOrdinal("Порт");
            var colPoE = dbReader.GetOrdinal("PoE");
            var colКодРозетки = dbReader.GetOrdinal("КодРозетки");
            var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colПодключено = dbReader.GetOrdinal("Подключено");
            var colПримечания = dbReader.GetOrdinal("Примечания");
            var colСрокВозврата = dbReader.GetOrdinal("СрокВозврата");

            while (dbReader.Read())
            {
                var row = new Net();
                row.Unavailable = false;
                row.Id = dbReader.GetInt32(colКодЛокальнойСети).ToString();
                row.EquipmentId = dbReader.GetInt32(colКодОборудования);
                row.Port = dbReader.GetByte(colПорт);
                row.POE = dbReader.GetByte(colPoE);
                row.PersonCustomerId = dbReader.GetInt32(colКодЛицаЗаказчика);
                row.Connected = dbReader.GetString(colПодключено);
                row.Description = dbReader.GetString(colПримечания);
                if (!dbReader.IsDBNull(colКодРозетки)) row.SocketId = dbReader.GetInt32(colКодРозетки);
                if (!dbReader.IsDBNull(colКодСотрудника)) row.EmployeeId = dbReader.GetInt32(colКодСотрудника);
                if (!dbReader.IsDBNull(colСрокВозврата)) row.ReturnDate = dbReader.GetDateTime(colСрокВозврата);
                list.Add(row);
            }

            return list;
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодОборудования", EquipmentId },
                { "@Порт", Port },
                { "@PoE", POE },
                { "@КодЛицаЗаказчика", PersonCustomerId },
                { "@Подключено", Connected },
                { "@Примечания", Description },
                { "@КодРозетки", SocketId },
                { "@КодСотрудника", EmployeeId },
                { "@СрокВозврата", ReturnDate }
            };

            if (IsNew)
            {
                var netId = DBManager.ExecuteScalar(SQLQueries.INSERT_ЛокальнаяСеть, CommandType.Text, Config.DS_user, sqlParams);

                if (netId != null)
                    Id = netId.ToString();
            }
            else
            {
                sqlParams.Add("@КодЛокальнойСети", Id);
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ЛокальнаяСеть, CommandType.Text, Config.DS_user, sqlParams);
            }
        }

        #region Поля сущности

        /// <summary>
        ///     КодОборудования
        /// </summary>
        [DBField("КодОборудования")]
        public int EquipmentId
        {
            get { return string.IsNullOrEmpty(EquipmentIdBind.Value) ? 0 : int.Parse(EquipmentIdBind.Value); }
            set { EquipmentIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодОборудования
        /// </summary>
        public BinderValue EquipmentIdBind = new BinderValue();

        /// <summary>
        ///     Порт
        /// </summary>
        [DBField("Порт")]
        public byte Port
        {
            get { return string.IsNullOrEmpty(PortBind.Value) ? (byte)0 : byte.Parse(PortBind.Value); }
            set { PortBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля Порт
        /// </summary>
        public BinderValue PortBind = new BinderValue();

        /// <summary>
        ///     PoE
        /// </summary>
        [DBField("PoE")]
        public byte POE
        {
            get { return string.IsNullOrEmpty(POEBind.Value) ? (byte)0 : byte.Parse(POEBind.Value); }
            set { POEBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля Порт
        /// </summary>
        public BinderValue POEBind = new BinderValue();

        /// <summary>
        ///     КодЛицаЗаказчика
        /// </summary>
        [DBField("КодЛицаЗаказчика")]
        public int PersonCustomerId
        {
            get { return string.IsNullOrEmpty(PersonCustomerIdBind.Value) ? 0 : int.Parse(PersonCustomerIdBind.Value); }
            set { PersonCustomerIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодЛицаЗаказчика
        /// </summary>
        public BinderValue PersonCustomerIdBind = new BinderValue();

        /// <summary>
        ///     КодСотрудника
        /// </summary>
        [DBField("КодСотрудника")]
        public int? EmployeeId
        {
            get { return string.IsNullOrEmpty(EmployeeIdIdBind.Value) ? (int?)null : int.Parse(EmployeeIdIdBind.Value); }
            set { EmployeeIdIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодСотрудника
        /// </summary>
        public BinderValue EmployeeIdIdBind = new BinderValue();

        /// <summary>
        ///     КодРозетки
        /// </summary>
        [DBField("КодРозетки")]
        public int? SocketId
        {
            get { return string.IsNullOrEmpty(SocketIdBind.Value) ? (int?)null : int.Parse(SocketIdBind.Value); }
            set { SocketIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодРозетки
        /// </summary>
        public BinderValue SocketIdBind = new BinderValue();

        /// <summary>
        ///     Подключено
        /// </summary>
        [DBField("Подключено")]
        public string Connected
        {
            get { return ConnectedBind.Value; }
            set { ConnectedBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Подключено
        /// </summary>
        public BinderValue ConnectedBind = new BinderValue();

        /// <summary>
        ///     Примечания
        /// </summary>
        [DBField("Примечания")]
        public string Description
        {
            get { return DescriptionBind.Value; }
            set { DescriptionBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Примечания
        /// </summary>
        public BinderValue DescriptionBind = new BinderValue();

        /// <summary>
        ///     СрокВозврата
        /// </summary>
        [DBField("СрокВозврата")]
        public DateTime? ReturnDate
        {
            get { return Convert.ToDateTime(ReturnDateBind.Value); }
            set { ReturnDateBind.Value = value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля СрокВозврата
        /// </summary>
        public BinderValue ReturnDateBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        public int ChangedId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        public DateTime ChangedDate { get; set; }

        #endregion
    }
}