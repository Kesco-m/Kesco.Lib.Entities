using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     Бизнес-объект - Расположения
    /// </summary>
    [Serializable]
    [DBSource("vwСертификаты")]
    public class Certificate : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Владелец
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        ///     Компания
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        ///     Сертификат
        /// </summary>
        public byte[] CertificateFile { get; set; }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID расположения</param>
        public Certificate(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Certificate()
        {
            LoadedExternalProperties = new Dictionary<string, DateTime>();
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
        ///     Получение расположений ключей
        /// </summary>
        public static List<LocationKey> GetLocationKeys(string certificateId)
        {
                var locationKeys = new List<LocationKey>();
                var sqlParams = new Dictionary<string, object> {{"@id", int.Parse(certificateId) }};
                using (var dbReader = new DBReader(SQLQueries.SELECT_ID_РасположенияКлючей, CommandType.Text, Config.DS_user, sqlParams))
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
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");
                        #endregion

                        while (dbReader.Read())
                        {
                            var row = new LocationKey();
                            row.Unavailable = false;
                            row.Id = dbReader.GetInt32(colКодСертификатаРасположенияКлюча).ToString();

                            if (!dbReader.IsDBNull(colКодСертификата)) row.CertificateId = dbReader.GetInt32(colКодСертификата);
                            if (!dbReader.IsDBNull(colКодОборудования)) row.EquipmentId = dbReader.GetInt32(colКодОборудования);
                            if (!dbReader.IsDBNull(colСетевыеИмена)) row.NetName = dbReader.GetString(colСетевыеИмена);
                            if (!dbReader.IsDBNull(colКодСотрудникаПользователя)) row.UserEmployeeId = dbReader.GetInt32(colКодСотрудникаПользователя);
                            if (!dbReader.IsDBNull(colИмяКонтейнераКриптоПро)) row.CryptoProContainerName = dbReader.GetString(colИмяКонтейнераКриптоПро);
                            if (!dbReader.IsDBNull(colИзменено)) row.Changed = dbReader.GetDateTime(colИзменено);
                            if (!dbReader.IsDBNull(colИзменил)) row.ChangedId = dbReader.GetInt32(colИзменил);

                            locationKeys.Add(row);
                        }
                    }
                }

                return locationKeys;
        }

        /// <summary>
        ///     Инициализация сущности на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодСертификата"].ToString();
                Stamp = dt.Rows[0]["ОтпечатокСертификата"].ToString();
                OwnerPersonId = dt.Rows[0]["КодЛицаВладельцаСертификата"] == DBNull.Value? (int?)null: Convert.ToInt32(dt.Rows[0]["КодЛицаВладельцаСертификата"]);
                CompanyPersonId = dt.Rows[0]["КодЛицаКомпании"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодЛицаКомпании"]);
                ContractId = dt.Rows[0]["КодДокументаДоговора"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодДокументаДоговора"]);
                Publisher = dt.Rows[0]["КемВыдан"].ToString();
                Nomination = dt.Rows[0]["НазначениеСертификата"].ToString();
                FromDate = dt.Rows[0]["ДействителенС"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(dt.Rows[0]["ДействителенС"]);
                ToDate = dt.Rows[0]["ДействителенДо"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(dt.Rows[0]["ДействителенДо"]);
                Subject = dt.Rows[0]["Субъект"].ToString();
                SubjectAddName = dt.Rows[0]["ДопИмяСубъекта"].ToString();
                Close = Convert.ToInt32(dt.Rows[0]["Закрыт"]);
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
                ReReleasedId = dt.Rows[0]["КодПеревыпущенногоСертификата"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодПеревыпущенногоСертификата"]);
                HasCertificateFile = Convert.ToInt32(dt.Rows[0]["ЕстьФайлСертификата"]);
                HasClosedKey = Convert.ToInt32(dt.Rows[0]["ЕстьЗакрытыйКлюч"]);
                ResponsibleId = Convert.ToInt32(dt.Rows[0]["КодОтветственногоСотрудника"]);
                PrivateKey = dt.Rows[0]["ЗакрытыйКлюч"].ToString();
                PrivateKeyAccess = Convert.ToInt32(dt.Rows[0]["ДоступКЗакрытомуКлючу"]);
                
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Расположение"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_СертификатПоID, CN, CommandType.Text, sqlParams));
        }

        #region Поля сущности "Сертификаты"

            /// <summary>
            ///     ОтпечатокСертификата
            /// </summary>
        [DBField("ОтпечатокСертификата")]
        public string Stamp
        {
            get { return StampBind.Value; }
            set { StampBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля ОтпечатокСертификата
        /// </summary>
        public BinderValue StampBind = new BinderValue();

        /// <summary>
        ///     КодЛицаВладельцаСертификата
        /// </summary>
        [DBField("КодЛицаВладельцаСертификата")]
        public int? OwnerPersonId
        {
            get { return string.IsNullOrEmpty(OwnerPersonIdBind.Value) ? (int?)null : int.Parse(OwnerPersonIdBind.Value); }
            set { OwnerPersonIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля КодЛицаВладельцаСертификата
        /// </summary>
        public BinderValue OwnerPersonIdBind = new BinderValue();

        /// <summary>
        ///     Binder для поля ВладелецСертификата
        /// </summary>
        public BinderValue OwnerNameBind = new BinderValue();

        /// <summary>
        ///     КодЛицаКомпании
        /// </summary>
        [DBField("КодЛицаКомпании")]
        public int? CompanyPersonId
        {
            get { return string.IsNullOrEmpty(CompanyPersonIdBind.Value) ? (int?)null : int.Parse(CompanyPersonIdBind.Value); }
            set { CompanyPersonIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля КодЛицаКомпании
        /// </summary>
        public BinderValue CompanyPersonIdBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Компания
        /// </summary>
        public BinderValue CompanyNameBind = new BinderValue();

        /// <summary>
        ///     КодДокументаДоговора
        /// </summary>
        [DBField("КодДокументаДоговора")]
        public int? ContractId
        {
            get { return string.IsNullOrEmpty(ContractIdBind.Value) ? (int?)null : int.Parse(ContractIdBind.Value); }
            set { ContractIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля КодДокументаДоговора
        /// </summary>
        public BinderValue ContractIdBind = new BinderValue();

        /// <summary>
        ///     КемВыдан
        /// </summary>
        [DBField("КемВыдан")]
        public string Publisher
        {
            get { return PublisherBind.Value; }
            set { PublisherBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля КемВыдан
        /// </summary>
        public BinderValue PublisherBind = new BinderValue();

        /// <summary>
        ///     НазначениеСертификата
        /// </summary>
        [DBField("НазначениеСертификата")]
        public string Nomination
        {
            get { return NominationBind.Value; }
            set { NominationBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля НазначениеСертификата
        /// </summary>
        public BinderValue NominationBind = new BinderValue();

        /// <summary>
        ///     ЗакрытыйКлюч
        /// </summary>
        [DBField("ЗакрытыйКлюч")]
        public string PrivateKey
        {
            get { return PrivateKeyBind.Value; }
            set { PrivateKeyBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля ЗакрытыйКлюч
        /// </summary>
        public BinderValue PrivateKeyBind = new BinderValue();

        /// <summary>
        ///     ДействителенС
        /// </summary>
        [DBField("ДействителенС")]
        public DateTime FromDate
        {
            get { return string.IsNullOrEmpty(FromDateBind.Value) ? DateTime.MinValue : Convert.ToDateTime(FromDateBind.Value); }
            set { FromDateBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля ДействителенС
        /// </summary>
        public BinderValue FromDateBind = new BinderValue();

        /// <summary>
        ///     ДействителенДо
        /// </summary>
        [DBField("ДействителенДо")]
        public DateTime ToDate
        {
            get { return string.IsNullOrEmpty(ToDateBind.Value) ? DateTime.MinValue : Convert.ToDateTime(ToDateBind.Value); }
            set { ToDateBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля ДействителенДо
        /// </summary>
        public BinderValue ToDateBind = new BinderValue();

        /// <summary>
        ///     Закрыт
        /// </summary>
        [DBField("Закрыт")]
        public int Close
        {
            get { return string.IsNullOrEmpty(CloseBind.Value) ? 0 : int.Parse(CloseBind.Value); }
            set { CloseBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Закрыт
        /// </summary>
        public BinderValue CloseBind = new BinderValue();

        /// <summary>
        ///     Субъект
        /// </summary>
        [DBField("Субъект")]
        public string Subject
        {
            get { return SubjectBind.Value; }
            set { SubjectBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Субъект
        /// </summary>
        public BinderValue SubjectBind = new BinderValue();

        /// <summary>
        ///     Доп имя субъекта
        /// </summary>
        [DBField("ДопИмяСубъекта")]
        public string SubjectAddName
        {
            get { return SubjectAddNameBind.Value; }
            set { SubjectAddNameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ДопИмяСубъекта
        /// </summary>
        public BinderValue SubjectAddNameBind = new BinderValue();

        /// <summary>
        ///     Есть сертификат
        /// </summary>
        [DBField("ЕстьФайлСертификата")]
        public int HasCertificateFile
        {
            get { return string.IsNullOrEmpty(HasCertificateFileBind.Value) ? 0 : int.Parse(HasCertificateFileBind.Value); }
            set { HasCertificateFileBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Есть закрытый ключ
        /// </summary>
        public int HasClosedKey { get; set; }

        /// <summary>
        ///     Binder для поля Есть Сертификат
        /// </summary>
        public BinderValue HasCertificateFileBind = new BinderValue();

        /// <summary>
        ///     КодПеревыпущенногоСертификата
        /// </summary>
        [DBField("КодПеревыпущенногоСертификата")]
        public int? ReReleasedId
        {
            get { return string.IsNullOrEmpty(ReReleasedIdBind.Value) ? (int?)null : int.Parse(ReReleasedIdBind.Value); }
            set { ReReleasedIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодПеревыпущенногоСертификата
        /// </summary>
        public BinderValue ReReleasedIdBind = new BinderValue();
        
        /// <summary>
        ///     Ответственный
        /// </summary>
        [DBField("КодОтветственногоСотрудника")]
        public int ResponsibleId
        {
            get { return string.IsNullOrEmpty(ResponsibleIdBind.Value) ? 0 : int.Parse(ResponsibleIdBind.Value); }
            set { ResponsibleIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля КодОтветственногоСотрудника
        /// </summary>
        public BinderValue ResponsibleIdBind = new BinderValue();

        /// <summary>
        ///     ДоступКЗакрытомуКлючу
        /// </summary>
        [DBField("ДоступКЗакрытомуКлючу")]
        public int PrivateKeyAccess
        {
            get { return string.IsNullOrEmpty(PrivateKeyAccessBind.Value) ? 0 : int.Parse(PrivateKeyAccessBind.Value); }
            set { PrivateKeyAccessBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
        /// <summary>
        ///     Binder для поля ДоступКЗакрытомуКлючу
        /// </summary>
        public BinderValue PrivateKeyAccessBind = new BinderValue();

        /// <summary>
        ///     Изменил
        /// </summary>
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public DateTime ChangedTime { get; set; }

        /// <summary>
        ///     Закрыт
        /// </summary>
        public bool IsClosed => Close == 1;

        /// <summary>
        ///     Закрыт
        /// </summary>
        public bool IsActual => Close == 0 && ToDate >= DateTime.Today && FromDate <= DateTime.Today;

        /// <summary>
        ///     Перевыпущен
        /// </summary>
        public bool IsReReleased => ReReleasedId.ToString().Length > 0;

        /// <summary>
        ///     Есть Файл Сертификата
        /// </summary>
        public bool IsHasCertificateFile => HasCertificateFile == 1;

        /// <summary>
        ///     Есть Доступ к закрытому ключу
        /// </summary>
        public bool IsHasPrivateKeyAccess => PrivateKeyAccess == 1;

        #endregion
    }
}