using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "LocationKey"
    /// </summary>
    [Serializable]
    [DBSource("vwСертификатыРасположениеКлючей")]
    public class LocationKey : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Код расположения ключа</param>
        public LocationKey(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки 
        /// </summary>
        public LocationKey()
        {
            LocationKeyId = 0;
            CertificateId = 0;
            PlaceKeyStorage = 0;
            UserEmployeeId = null;
            NetName = "";
            CryptoProContainerName = "";
            LocationPlace = "";
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
        ///     Подготовка связывания поля объекта КодРасположенияКлюча с контролом
        /// </summary>
        public BinderValue LocationKeyIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта КодСертификата с контролом
        /// </summary>
        public BinderValue CertificateIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Оборудование с контролом
        /// </summary>
        public BinderValue EquipmentIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта СетевыеИмена с контролом
        /// </summary>
        public BinderValue NetNameBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Сотрудник с контролом
        /// </summary>
        public BinderValue UserEmployeeIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта ИмяКонтейнераКриптоПро с контролом
        /// </summary>
        public BinderValue CryptoProContainerNameBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта ТипРасположенияКлюча с контролом
        /// </summary>
        public BinderValue PlaceKeyStorageBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта ПутьРасположенияКлюча с контролом
        /// </summary>
        public BinderValue LocationPlaceBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Выполнено с контролом
        /// </summary>
        public BinderValue ExecutedBind = new BinderValue();

        /// <summary>
        ///     КодСертификатаРасположенияКлюча (int, not null)
        /// </summary>
        [DBField("КодСертификатаРасположенияКлюча", 0)]
        public int LocationKeyId
        {
            get { return string.IsNullOrEmpty(LocationKeyIdBind.Value) ? 0 : int.Parse(LocationKeyIdBind.Value); }
            set { LocationKeyIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     КодСертификата (int, not null)
        /// </summary>
        [DBField("КодСертификата", 0)]
        public int CertificateId
        {
            get { return string.IsNullOrEmpty(CertificateIdBind.Value) ? 0 : int.Parse(CertificateIdBind.Value); }
            set { CertificateIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     КодОборудования (int, not null)
        /// </summary>
        [DBField("КодОборудования", 0)]
        public int? EquipmentId
        {
            get { return string.IsNullOrEmpty(EquipmentIdBind.Value) ? (int?)null : int.Parse(EquipmentIdBind.Value); }
            set { EquipmentIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СетевыеИмена
        /// </value>
        [DBField("СетевыеИмена")]
        public string NetName
        {
            get { return string.IsNullOrEmpty(NetNameBind.Value) ? "" : NetNameBind.Value; }
            set { NetNameBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     КодСотрудникаПользователя (int, null)
        /// </summary>
        [DBField("КодСотрудникаПользователя", 0)]
        public int? UserEmployeeId
        {
            get { return string.IsNullOrEmpty(UserEmployeeIdBind.Value) ? (int?)null : int.Parse(UserEmployeeIdBind.Value); }
            set { UserEmployeeIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Имя Контейнера Крипто-Про
        /// </value>
        [DBField("CryptoProContainerName")]
        public string CryptoProContainerName
        {
            get { return string.IsNullOrEmpty(CryptoProContainerNameBind.Value) ? "" : CryptoProContainerNameBind.Value; }
            set { CryptoProContainerNameBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Место Хранения Ключа
        /// </value>
        [DBField("ТипРасположенияКлюча")]
        public int PlaceKeyStorage
        {
            get { return string.IsNullOrEmpty(PlaceKeyStorageBind.Value) ? 0 : int.Parse(PlaceKeyStorageBind.Value); }
            set { PlaceKeyStorageBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Место Расположения Ключа
        /// </value>
        [DBField("ПутьРасположенияКлюча")]
        public string LocationPlace
        {
            get { return string.IsNullOrEmpty(LocationPlaceBind.Value) ? "" : LocationPlaceBind.Value; }
            set { LocationPlaceBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Изменил
        /// </summary>
        [DBField("Изменил", "", false)]
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        [DBField("Изменено", "", false)]
        public DateTime ChangedTime { get; set; }

        /// <summary>
        ///     Выполнено
        /// </summary>
        [DBField("Выполнено")]
        public int Executed
        {
            get { return string.IsNullOrEmpty(ExecutedBind.Value) ? 0 : int.Parse(ExecutedBind.Value); }
            set { ExecutedBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Тип личного кабинета"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", Id}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_СертификатРасположенияКлюча, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСертификатаРасположенияКлюча = dbReader.GetOrdinal("КодСертификатаРасположенияКлюча");
                    var colКодСертификата = dbReader.GetOrdinal("КодСертификата");
                    var colКодОборудования = dbReader.GetOrdinal("КодОборудования");
                    var colСетевыеИмена = dbReader.GetOrdinal("СетевыеИмена");
                    var colКодСотрудникаПользователя = dbReader.GetOrdinal("КодСотрудникаПользователя");
                    var colИмяКонтейнераКриптоПро = dbReader.GetOrdinal("ИмяКонтейнераКриптоПро");
                    var colТипРасположенияКлюча = dbReader.GetOrdinal("ТипРасположенияКлюча");
                    var colПутьРасположенияКлюча = dbReader.GetOrdinal("ПутьРасположенияКлюча");
                    var colВыполнено = dbReader.GetOrdinal("Выполнено");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;


                        if (!dbReader.IsDBNull(colКодСертификатаРасположенияКлюча)) LocationKeyId = dbReader.GetInt32(LocationKeyId);
                        Id = LocationKeyId.ToString();
                        if (!dbReader.IsDBNull(colКодСертификата)) CertificateId = dbReader.GetInt32(colКодСертификата);
                        if (!dbReader.IsDBNull(colКодОборудования)) EquipmentId = dbReader.GetInt32(colКодОборудования);
                        if (!dbReader.IsDBNull(colСетевыеИмена)) NetName = dbReader.GetString(colСетевыеИмена);
                        if (!dbReader.IsDBNull(colИмяКонтейнераКриптоПро)) CryptoProContainerName = dbReader.GetString(colИмяКонтейнераКриптоПро);
                        if (!dbReader.IsDBNull(colТипРасположенияКлюча)) PlaceKeyStorage = dbReader.GetInt32(colТипРасположенияКлюча);
                        if (!dbReader.IsDBNull(colПутьРасположенияКлюча)) LocationPlace = dbReader.GetString(colПутьРасположенияКлюча);
                        if (!dbReader.IsDBNull(colКодСотрудникаПользователя)) UserEmployeeId = dbReader.GetInt32(colКодСотрудникаПользователя);
                        if (!dbReader.IsDBNull(colВыполнено)) Executed = dbReader.GetInt32(colВыполнено);
                        ChangedId = dbReader.GetInt32(colИзменил);
                        ChangedTime = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                LocationKeyId = Convert.ToInt32(dt.Rows[0]["КодСертификатаРасположенияКлюча"]);
                Id = LocationKeyId.ToString();
                CertificateId = Convert.ToInt32(dt.Rows[0]["КодСертификата"]);
                EquipmentId = Convert.ToInt32(dt.Rows[0]["КодОборудования"]);
                NetName = dt.Rows[0]["СетевыеИмена"].ToString();
                CryptoProContainerName = dt.Rows[0]["ИмяКонтейнераКриптоПро"].ToString();
                UserEmployeeId = dt.Rows[0]["КодСотрудникаПользователя"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодСотрудникаПользователя"]);
                PlaceKeyStorage = Convert.ToInt32(dt.Rows[0]["ТипРасположенияКлюча"]);
                LocationPlace = dt.Rows[0]["ПутьРасположенияКлюча"].ToString();
                Executed = Convert.ToInt32(dt.Rows[0]["Выполнено"]);

                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

    }
}