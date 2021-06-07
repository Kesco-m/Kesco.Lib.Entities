using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate.Equipments;
using Kesco.Lib.Entities.Persons.PersonOld;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Пользователь (Сотрудник)
    /// </summary>
    /// <example>
    ///     Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.CorporateTest
    /// </example>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, FullName = {FullName}, Language = {Language}")]
    public class Employee : Entity
    {
        /// <summary>
        ///     Ключ кэширования объекта Сотрудник
        /// </summary>
        public const string CacheKey_EquipmentsAnotherWorkPlaces = "employee.equipmentsAnotherWorkPlaces";
        /// <summary>
        ///     Ключ кэширования объекта UserStatus
        /// </summary>
        public const string CacheKey_UserStatus = "employee.userStatus";

        private DateTime? _adsiAccountExpires = null;
        private int? _adsiAccountDisabled = null;
        private string _adsiAccountPath = null;

        private DataTable _groupInconsistencies;

        private int? _isAccountLocked;
        private DateTime? _pwdExpiries;

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        private Dictionary<int, byte?> _advancedGrants;
        private Dictionary<int, byte?> _advancedGrantsFullInGroup;

        private List<CommonFolder> _commonFolders;
        private List<CommonFolder> _commonFoldersFullInGroup;
        private List<Equipment> _employeeEquipmentsAnotherWorkPlaces;
        private List<Employee> _employeesOnWorkPlace;

        private PersonOld _employer;
        private List<Employee> _groupColleagues;
        private List<Employee> _groupMembers;
        private PersonOld _organization;
        private PersonOld _personEmployee;
        private List<EmployeePhoto> _photos;
        private List<EmployeeRole> _roles;
        private List<EmployeeRole> _rolesFullInGroup;
        private List<EmployeeRole> _rolesСurrentEmployes;
        private bool? _simGprsPackage;
        private string _simNumber = "";
        private string _simPost = "";
        private bool? _simRequired;
        private List<EmployeePersonType> _types;
        private List<EmployeePersonType> _typesFullInGroup;
        private List<Location> _workPlaces;




        /// <summary>
        ///     Binder для поля AccountDisabled
        /// </summary>
        public BinderValue AccountExpiresBind = new BinderValue();

        /// <summary>
        ///     Binder для поля AccountDisabled
        /// </summary>
        public BinderValue AccountDisabledBind = new BinderValue();

        /// <summary>
        ///     Binder для поля DisplayName
        /// </summary>
        public BinderValue DisplayNameBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Email
        /// </summary>
        public BinderValue EmailBind = new BinderValue();

        /// <summary>
        ///     Binder для поля FirstName
        /// </summary>
        public BinderValue FirstNameBind = new BinderValue();

        /// <summary>
        ///     Binder для поля FirstNameEn
        /// </summary>
        public BinderValue FirstNameEnBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Guid
        /// </summary>
        public BinderValue GuidBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Язык
        /// </summary>
        public BinderValue LanguageBind = new BinderValue();

        /// <summary>
        ///     Binder для поля LastName
        /// </summary>
        public BinderValue LastNameBind = new BinderValue();

        /// <summary>
        ///     Binder для поля LastNameEn
        /// </summary>
        public BinderValue LastNameEnBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Login
        /// </summary>
        public BinderValue LoginBind = new BinderValue();

        /// <summary>
        ///     Binder для поля MiddleName
        /// </summary>
        public BinderValue MiddleNameBind = new BinderValue();

        /// <summary>
        ///     Binder для поля MiddleName
        /// </summary>
        public BinderValue MiddleNameEnBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Примечания
        /// </summary>
        public BinderValue NotesBind = new BinderValue();

        /// <summary>
        ///     Binder для поля КодЛицаЗаказчика
        /// </summary>
        public BinderValue OrganizationIdBind = new BinderValue();

        /// <summary>
        ///     Binder для поля ЛичнаяПапка
        /// </summary>
        public BinderValue PersonalFolderBind = new BinderValue();

        /// <summary>
        ///     Binder для поля Пол
        /// </summary>
        public BinderValue GenderBind = new BinderValue();

        /// <summary>
        ///     Binder для поля ДатаРождения
        /// </summary>
        public BinderValue BirthdateBind = new BinderValue();

        /// <summary>
        ///     Binder для поля МестоРождения
        /// </summary>
        public BinderValue BirthplaceBind = new BinderValue();

        /// <summary>
        ///     Binder для поля ИНН
        /// </summary>
        public BinderValue InnBind = new BinderValue();

        /// <summary>
        ///     Binder для поля ПенсСвидетельство
        /// </summary>
        public BinderValue PensionCertBind = new BinderValue();

        private DataTable positionsData;

        /// <summary>
        ///     Binder для поля Status
        /// </summary>
        public BinderValue StatusBind = new BinderValue();

        private DataTable supervisorsData;

        /// <inheritdoc />
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Код сотрудника</param>
        public Employee(string id)
            : base(id)
        {
            Id = id;
            Load();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="guid">GUID сотрудника</param>
        public Employee(Guid guid)
        {
            LoadedExternalProperties = new Dictionary<string, DateTime>();
            LoadByGuid(guid);
        }

        /// <summary>
        ///     Конструктор по-умолчанию. Если необходимо получить текущего создайте объект с fillCurrentUser = true
        /// </summary>
        /// <param name="fillCurrentUser">Заполнить объект данными текущего сотрудника</param>
        public Employee(bool fillCurrentUser)
        {
            LoadedExternalProperties = new Dictionary<string, DateTime>();
            if (fillCurrentUser) GetCurrentUser();
        }

        /// <summary>
        ///     Для заполнения через datareader
        /// </summary>
        public Employee()
        {
            Gender = "";
            Birthplace = "";
            Inn = "";
            PensionCert = "";

            LoadedExternalProperties = new Dictionary<string, DateTime>();
        }


        //TODO: Используется в контолах SELECT из-за невозможности инициировать форму раньше объявленных контролах. Переделать в будущем!!!
        /// <summary>
        ///     Ленивая загрузка значениями CurrentUser
        /// </summary>
        public bool IsLazyLoadingByCurrentUser { get; set; }

        /// <summary>
        /// Дежурства сотрудника
        /// </summary>
        public string DutyDays { get; set; }

        /// <summary>
        ///     ID. Поле КодСотрудника
        /// </summary>
        /// <remarks>
        ///     Типизированный псевданим для ID
        /// </remarks>
        public int EmployeeId => Id.ToInt();

        /// <summary>
        ///     Имя сотудника
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Код лица
        /// </summary>
        public int? PersonEmployeeId { get; set; }

        /// <summary>
        ///     ФИО сотрудника
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        ///     Локализованное ФИО сотрудника
        /// </summary>
        public string NameShort { get; set; }

        /// <summary>
        ///     ФИО сотрудника на английском
        /// </summary>
        public string FIOEn { get; set; }

        /// <summary>
        ///     Хост
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public new DateTime? Changed { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        /// <summary>
        ///     Фамилия
        /// </summary>
        /// <value>
        ///     Фамилия (nvarchar(50), not null)
        /// </value>
        [DBField("Фамилия")]
        public string LastName
        {
            get { return LastNameBind.Value; }
            set { LastNameBind.Value = value; }
        }

        /// <summary>
        ///     Имя
        /// </summary>
        /// <value>
        ///     Имя (nvarchar(50), not null)
        /// </value>
        [DBField("Имя")]
        public string FirstName
        {
            get { return FirstNameBind.Value; }
            set { FirstNameBind.Value = value; }
        }

        /// <summary>
        ///     Отчество
        /// </summary>
        /// <value>
        ///     Отчество (nvarchar(50), not null)
        /// </value>
        [DBField("Отчество")]
        public string MiddleName
        {
            get { return MiddleNameBind.Value; }
            set { MiddleNameBind.Value = value; }
        }

        /// <summary>
        ///     Фамилия на латинице
        /// </summary>
        /// <value>
        ///     LastName (nvarchar(50), not null)
        /// </value>
        [DBField("LastName")]
        public string LastNameEn
        {
            get { return LastNameEnBind.Value; }
            set { LastNameEnBind.Value = value; }
        }

        /// <summary>
        ///     Имя на латинице
        /// </summary>
        /// <value>
        ///     FirstName (nvarchar(50), not null)
        /// </value>
        [DBField("FirstName")]
        public string FirstNameEn
        {
            get { return FirstNameEnBind.Value; }
            set { FirstNameEnBind.Value = value; }
        }

        /// <summary>
        ///     Отчество на латинице
        /// </summary>
        /// <value>
        ///     MiddleName (nvarchar(50), not null)
        /// </value>
        [DBField("MiddleName")]
        public string MiddleNameEn
        {
            get { return MiddleNameEnBind.Value; }
            set { MiddleNameEnBind.Value = value; }
        }

        /// <summary>
        ///     Gets or sets the full name en.
        /// </summary>
        /// <value>
        ///     The full name en.
        /// </value>
        public string FullNameEn { get; set; }

        /// <summary>
        ///     Состояние
        /// </summary>
        /// <value>
        ///     Состояние (tinyint, not null)
        /// </value>
        [DBField("Состояние")]
        public int Status
        {
            get { return string.IsNullOrEmpty(StatusBind.Value) ? 0 : int.Parse(StatusBind.Value); }
            set { StatusBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     GUID
        /// </summary>
        /// <value>
        ///     GUID (uniqueidentifier, null)
        /// </value>
        [DBField("Guid")]
        public Guid? Guid
        {
            get { return string.IsNullOrEmpty(GuidBind.Value) ? (Guid?)null : new Guid(GuidBind.Value); }
            set { GuidBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Логин
        /// </summary>
        /// <value>
        ///     Login (varchar(32), not null)
        /// </value>
        [DBField("Login")]
        public string Login
        {
            get { return LoginBind.Value; }
            set { LoginBind.Value = value; }
        }

        /// <summary>
        ///     AccountDisabled
        /// </summary>
        /// <value>
        ///     AccountDisabled (tinyint, null)
        /// </value>
        [DBField("AccountDisabled")]
        public int? AccountDisabled
        {
            get
            {
                return string.IsNullOrEmpty(AccountDisabledBind.Value)
                    ? (int?)null
                    : int.Parse(AccountDisabledBind.Value);
            }
            set { AccountDisabledBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     AccountExpires
        /// </summary>
        /// <value>
        ///     AccountExpires (datetime, null)
        /// </value>
        [DBField("AccountExpires")]
        public DateTime? AccountExpires
        {
            get
            {
                return string.IsNullOrEmpty(AccountExpiresBind.Value)
                    ? (DateTime?)null
                    : DateTime.Parse(AccountExpiresBind.Value);
            }
            set { AccountExpiresBind.Value = value == null ? "" : value.ToString(); }
        }




        /// <summary>
        ///     Учетная запись отключена
        /// </summary>
        public bool IsAccountDisabled => AccountDisabled == 1;

        /// <summary>
        ///     Срок действия учетной записи истек
        /// </summary>
        public bool IsAccountExpires => AccountExpires.HasValue && AccountExpires.Value < DateTime.UtcNow;

        /// <summary>
        ///     Gets or sets the login.
        /// </summary>
        /// <value>
        ///     The LoginFull.
        /// </value>
        public string LoginFull
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        ///     DisplayName
        /// </summary>
        /// <value>
        ///     DisplayName (varchar(50), not null)
        /// </value>
        [DBField("DisplayName")]
        public string DisplayName
        {
            get { return DisplayNameBind.Value; }
            set { DisplayNameBind.Value = value; }
        }

        /// <summary>
        ///     Email
        /// </summary>
        /// <value>
        ///     Email (varchar(50), not null)
        /// </value>
        [DBField("Email")]
        public string Email
        {
            get { return EmailBind.Value; }
            set { EmailBind.Value = value; }
        }

        /// <summary>
        ///     Язык
        /// </summary>
        /// <value>
        ///     Язык (char(2), not null)
        /// </value>
        [DBField("Язык")]
        public string Language
        {
            get { return LanguageBind.Value; }
            set { LanguageBind.Value = value; }
        }

        /// <summary>
        ///     Личная папка
        /// </summary>
        /// <value>
        ///     ЛичнаяПапка (nvarchar(80), not null)
        /// </value>
        [DBField("ЛичнаяПапка")]
        public string PersonalFolder
        {
            get { return PersonalFolderBind.Value; }
            set { PersonalFolderBind.Value = value; }
        }

        /// <summary>
        ///     Пол
        /// </summary>
        /// <value>
        ///     Пол (char(1), not null)
        /// </value>
        [DBField("Пол")]
        public string Gender
        {
            get { return GenderBind.Value; }
            set { GenderBind.Value = value; }
        }

        /// <summary>
        ///     Дата рождения
        /// </summary>
        /// <value>
        ///     ДатаРождения (smalldatetime, null)
        /// </value>
        [DBField("ДатаРождения")]
        public DateTime? Birthdate
        {
            get { return string.IsNullOrEmpty(BirthdateBind.Value) ? (DateTime?)null : DateTime.Parse(BirthdateBind.Value); }
            set { BirthdateBind.Value = value == null ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Место рождения
        /// </summary>
        /// <value>
        ///     МестоРождения (varchar(300), not null)
        /// </value>
        [DBField("МестоРождения")]
        public string Birthplace
        {
            get { return BirthplaceBind.Value; }
            set { BirthplaceBind.Value = value; }
        }

        /// <summary>
        ///     ИНН
        /// </summary>
        /// <value>
        ///     ИНН (varchar(50), not null)
        /// </value>
        [DBField("ИНН")]
        public string Inn
        {
            get { return InnBind.Value; }
            set { InnBind.Value = value; }
        }

        /// <summary>
        ///     Пенсионное свидетельство
        /// </summary>
        /// <value>
        ///     ПенсСвидетельство (varchar(50), not null)
        /// </value>
        [DBField("ПенсСвидетельство")]
        public string PensionCert
        {
            get { return PensionCertBind.Value; }
            set { PensionCertBind.Value = value; }
        }

        /// <summary>
        ///     Код лица заказчика
        /// </summary>
        /// <value>
        ///     КодЛицаЗаказчика (int, null)
        /// </value>
        [DBField("КодЛицаЗаказчика")]
        public int? OrganizationId
        {
            get
            {
                return string.IsNullOrEmpty(OrganizationIdBind.Value)
                    ? (int?)null
                    : int.Parse(OrganizationIdBind.Value);
            }
            set { OrganizationIdBind.Value = value.ToString().IsNullEmptyOrZero() ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Примечания
        /// </summary>
        /// <value>
        ///     Примечания (nvarchar(300), not null)
        /// </value>
        [DBField("Примечания")]
        public string Notes
        {
            get { return NotesBind.Value; }
            set { NotesBind.Value = value; }
        }

        /// <summary>
        ///     Gets or sets the Addition.
        /// </summary>
        /// <value>
        ///     The Addition.
        /// </value>
        public string Addition
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        ///     Gets or sets the AdditionEn.
        /// </summary>
        /// <value>
        ///     The AdditionEn.
        /// </value>
        public string AdditionEn
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        ///     Gets or sets the common employee.
        /// </summary>
        /// <value>
        ///     The common employee
        /// </value>
        public string CommonEmployeeID { get; set; }

        /// <summary>
        ///     Название сотрудника для отображения в справочниках, по-умолчанию FullName
        /// </summary>
        public string Text
        {
            get
            {
                if (Unavailable) return "#" + Id;
                return FullName;
            }
        }

        /// <summary>
        ///     Сотруднику по должности положена Sim-карта
        /// </summary>
        public bool SimRequired
        {
            get
            {
                SetSimInfo();
                return _simRequired != null && _simRequired.Value;
            }
        }

        /// <summary>
        ///     К Sim-карте сотрудника подключен GPRS-пакет
        /// </summary>
        public bool SimGprsPackage
        {
            get
            {
                SetSimInfo();
                return _simGprsPackage != null && _simGprsPackage.Value;
            }
        }

        /// <summary>
        ///     По какой должности сотруднику положена Sim-карта
        /// </summary>
        public string SimPost
        {
            get
            {
                SetSimInfo();
                return _simPost;
            }
        }

        /// <summary>
        ///     Какой номер телефона будет на выданной Sim-карте
        /// </summary>
        public string SimNumber
        {
            get
            {
                SetSimInfo();
                return _simNumber;
            }
        }

        /// <summary>
        ///     Является группой посменной работы
        /// </summary>
        public bool IsGroupWork => PersonEmployeeId == null && Status == 0;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Аксессор к лицу заказчику сотрудника
        /// </summary>
        public PersonOld Organization
        {
            get
            {
                if (!OrganizationId.HasValue)
                {
                    _organization = null;
                }
                else
                {
                    if (LoadedExternalProperties.ContainsKey("employee._organization") && _organization != null &&
                        !_organization.Unavailable && _organization.Id.Equals(OrganizationId.ToString()))
                        return _organization;

                    _organization = new PersonOld(OrganizationId.ToString());

                    if (!LoadedExternalProperties.ContainsKey("employee._organization"))
                        LoadedExternalProperties.Add("employee._organization", DateTime.UtcNow);
                    else
                        LoadedExternalProperties["employee._organization"] = DateTime.UtcNow;
                }

                return _organization;
            }
        }

        /// <summary>
        ///     Аксессор к лицу сотрудника
        /// </summary>
        public PersonOld PersonEmployee
        {
            get
            {
                if (!PersonEmployeeId.HasValue)
                {
                    _personEmployee = null;
                }
                else
                {
                    if (LoadedExternalProperties.ContainsKey("employee._personEmployee") && _personEmployee != null &&
                        !_personEmployee.Unavailable && _personEmployee.Id.Equals(PersonEmployeeId.ToString()))
                        return _organization;

                    _personEmployee = new PersonOld(PersonEmployeeId.ToString());

                    if (!LoadedExternalProperties.ContainsKey("employee._personEmployee"))
                        LoadedExternalProperties.Add("employee._personEmployee", DateTime.UtcNow);
                    else
                        LoadedExternalProperties["employee._personEmployee"] = DateTime.UtcNow;
                }

                return _personEmployee;
            }
        }

        /// <summary>
        ///     Аксессор к лицу работодателю по основному месту работы
        /// </summary>
        public PersonOld Employer
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._employer")) return _employer;

                var dt = GetUserPositions(CombineType.ОсновноеМестоРаботы);

                if (dt.Rows.Count == 1)
                    _employer = new PersonOld(dt.Rows[0]["КодЛица"].ToString());
                else
                    _employer = null;

                LoadedExternalProperties.Add("employee._employer", DateTime.UtcNow);
                return _employer;
            }
        }

        /// <summary>
        ///     Список рабочих мест сотрудника
        /// </summary>
        public List<Location> Workplaces
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._workPlaces")) return _workPlaces;

                _workPlaces = new List<Location>();
                var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_РабочиеМестаСотрудника, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var wp = new Location();
                            wp.LoadFromDbReader(dbReader);
                            _workPlaces.Add(wp);
                        }
                }

                LoadedExternalProperties.Add("employee._workPlaces", DateTime.UtcNow);

                return _workPlaces;
            }
        }

        /// <summary>
        ///     Список фотографий сотрудника
        /// </summary>
        public List<EmployeePhoto> Photos
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._photos")) return _photos;

                _photos = new List<EmployeePhoto>();
                var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ФотографииСотрудника, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var ph = new EmployeePhoto();
                            ph.LoadFromDbReader(dbReader);
                            _photos.Add(ph);
                        }
                }

                LoadedExternalProperties.Add("employee._photos", DateTime.UtcNow);

                return _photos;
            }
        }

        /// <summary>
        ///     Получение дополнительных прав сотрудника
        /// </summary>
        public Dictionary<int, byte?> AdvancedGrants
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._advancedGrants")) return _advancedGrants;
                FillCommonFoldersAndAdvancedGrants();
                return _advancedGrants;
            }
        }

        /// <summary>
        ///     Получение дополнительных прав сотрудников в группе
        /// </summary>
        public Dictionary<int, byte?> AdvancedGrantsFullInGroup
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._advancedGrantsFullInGroup"))
                    return _advancedGrantsFullInGroup;
                FillCommonFoldersAndAdvancedGrantsFullInGroup();
                return _advancedGrantsFullInGroup;
            }
        }

        /// <summary>
        ///     Общие папки сотрудника
        /// </summary>
        public List<CommonFolder> CommonFolders
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._commonFolders")) return _commonFolders;
                FillCommonFoldersAndAdvancedGrants();
                return _commonFolders;
            }
        }

        /// <summary>
        ///     Общие папки сотрудников в группе
        /// </summary>
        public List<CommonFolder> CommonFoldersFullInGroup
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._commonFoldersFullInGroup"))
                    return _commonFoldersFullInGroup;
                FillCommonFoldersAndAdvancedGrantsFullInGroup();
                return _commonFoldersFullInGroup;
            }
        }

        /// <summary>
        ///     Роли сотрудника
        /// </summary>
        public List<EmployeeRole> Roles
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._roles")) return _roles;

                _roles = new List<EmployeeRole>();
                var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_РолиСотрудника, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempUserRole = new EmployeeRole();
                            tempUserRole.LoadFromDbReader(dbReader);
                            _roles.Add(tempUserRole);
                        }
                }

                LoadedExternalProperties.Add("employee._roles", DateTime.UtcNow);
                return _roles;
            }
        }

        /// <summary>
        ///     Все роли всех сотрудников в группе
        /// </summary>
        public List<EmployeeRole> RolesFullInGroup
        {
            get
            {
                if (string.IsNullOrEmpty(CommonEmployeeID)) return Roles;

                if (LoadedExternalProperties.ContainsKey("employee._rolesFullInGroup")) return _rolesFullInGroup;

                _rolesFullInGroup = new List<EmployeeRole>();
                var sqlParams = new Dictionary<string, object> { { "@КодОбщегоСотрудника", int.Parse(CommonEmployeeID) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_РолиСотрудникаВГруппе, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempUserRole = new EmployeeRole();
                            tempUserRole.LoadFromDbReader(dbReader);
                            _rolesFullInGroup.Add(tempUserRole);
                        }
                }

                LoadedExternalProperties.Add("employee._rolesFullInGroup", DateTime.UtcNow);
                return _rolesFullInGroup;
            }
        }

        /// <summary>
        ///     Роли текущего сотрудника(с замещениями)
        /// </summary>
        public List<EmployeeRole> RolesCurrentEmployes
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._rolesСurrentEmployes"))
                    return _rolesСurrentEmployes;

                _rolesСurrentEmployes = new List<EmployeeRole>();
                var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_РолиТекущегоСотрудника, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempUserRole = new EmployeeRole();
                            tempUserRole.LoadFromDbReader(dbReader);
                            _rolesСurrentEmployes.Add(tempUserRole);
                        }
                }

                LoadedExternalProperties.Add("employee._rolesСurrentEmployes", DateTime.UtcNow);

                return _rolesСurrentEmployes;
            }
        }

        /// <summary>
        ///     Типы сотрудника
        /// </summary>
        public List<EmployeePersonType> Types
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._types")) return _types;

                _types = new List<EmployeePersonType>();
                var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ПраваТипыЛицСотрудника, CommandType.Text,
                        Config.DS_person,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempPersonType = new EmployeePersonType();
                            tempPersonType.LoadFromDbReader(dbReader);
                            _types.Add(tempPersonType);
                        }
                }

                LoadedExternalProperties.Add("employee._types", DateTime.UtcNow);

                return _types;
            }
        }

        /// <summary>
        ///     Все типы всех сотрудников, входящих в группу
        /// </summary>
        public List<EmployeePersonType> TypesFullInGroup
        {
            get
            {
                if (string.IsNullOrEmpty(CommonEmployeeID)) return Types;

                if (LoadedExternalProperties.ContainsKey("employee._typesFullInGroup")) return _typesFullInGroup;

                _typesFullInGroup = new List<EmployeePersonType>();
                var sqlParams = new Dictionary<string, object> { { "@КодОбщегоСотрудника", int.Parse(CommonEmployeeID) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ПраваТипыЛицСотрудникаВГруппе, CommandType.Text,
                        Config.DS_person,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempPersonType = new EmployeePersonType();
                            tempPersonType.LoadFromDbReader(dbReader);
                            _typesFullInGroup.Add(tempPersonType);
                        }
                }

                LoadedExternalProperties.Add("employee._typesFullInGroup", DateTime.UtcNow);

                return _typesFullInGroup;
            }
        }

        /// <summary>
        ///     Список сотрудников коллег по группе
        /// </summary>
        public List<Employee> GroupColleagues
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._groupColleagues")) return _groupColleagues;

                _groupColleagues = new List<Employee>();

                if (string.IsNullOrEmpty(CommonEmployeeID)) return _groupColleagues;

                var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(CommonEmployeeID) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_СотрудникиВГруппе, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var empl = new Employee(false);
                            empl.LoadFromDbReader(dbReader, true);
                            _groupColleagues.Add(empl);
                        }
                }

                LoadedExternalProperties.Add("employee._groupColleagues", DateTime.UtcNow);


                return _groupColleagues;
            }
        }

        /// <summary>
        ///     Список сотрудников в группе
        /// </summary>
        public List<Employee> GroupMembers
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._groupMembers")) return _groupMembers;

                _groupMembers = new List<Employee>();
                var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_СотрудникиВГруппе, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var empl = new Employee(false);
                            empl.LoadFromDbReader(dbReader, true);
                            _groupMembers.Add(empl);
                        }
                }

                LoadedExternalProperties.Add("employee._groupMembers", DateTime.UtcNow);

                return _groupMembers;
            }
        }

        /// <summary>
        ///     Непосредственный руководитель
        /// </summary>
        public Employee Supervisor
        {
            get
            {
                var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };

                var superObj = DBManager.ExecuteScalar(SQLQueries.SELECT_НепосредственныйРуководитель, CommandType.Text,
                    CN, sqlParams);
                if (superObj is int)
                    return new Employee(superObj.ToString());

                return null;
            }
        }

        /// <summary>
        /// Получение списка общих папок и дополнительных прав в группе посменной работы
        /// </summary>
        public void FillCommonFoldersAndAdvancedGrantsFullInGroup()
        {
            _commonFoldersFullInGroup = new List<CommonFolder>();
            _advancedGrantsFullInGroup = new Dictionary<int, byte?>();

            GroupColleagues.ForEach(x =>
            {
                var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(x.Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SP_ОбщиеПапкиСотрудника, CommandType.StoredProcedure, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                    {
                        var colКодОбщейПапки = dbReader.GetOrdinal("КодОбщейПапки");
                        var colОбщаяПапка = dbReader.GetOrdinal("ОбщаяПапка");

                        while (dbReader.Read())
                        {
                            var idCf = dbReader.GetInt32(colКодОбщейПапки);
                            var nameCf = dbReader.GetString(colОбщаяПапка);

                            var cf = _commonFoldersFullInGroup.FirstOrDefault(y => y.Id == idCf.ToString());
                            if (cf == null)
                                _commonFoldersFullInGroup.Add(new CommonFolder { Id = idCf.ToString(), Name = nameCf });
                        }
                    }

                    if (dbReader.NextResult())
                        if (dbReader.HasRows)
                        {
                            var colКодДопПараметраУказанийИТ = dbReader.GetOrdinal("КодДопПараметраУказанийИТ");
                            var colЕстьПраво = dbReader.GetOrdinal("Есть");

                            while (dbReader.Read())
                            {
                                var idParam = dbReader.GetInt32(colКодДопПараметраУказанийИТ);
                                byte? paramIs = null;
                                if (!dbReader.IsDBNull(colЕстьПраво)) paramIs = dbReader.GetByte(colЕстьПраво);

                                if (_advancedGrantsFullInGroup.ContainsKey(idParam))
                                {
                                    var pIs = _advancedGrantsFullInGroup[idParam];
                                    if (paramIs.HasValue && !pIs.HasValue)
                                        _advancedGrantsFullInGroup[idParam] = paramIs;
                                }
                                else
                                {
                                    _advancedGrantsFullInGroup.Add(idParam, paramIs);
                                }
                            }
                        }
                }
            });

            if (LoadedExternalProperties.ContainsKey("employee._commonFoldersFullInGroup"))
                LoadedExternalProperties.Remove("employee._commonFoldersFullInGroup");
            if (LoadedExternalProperties.ContainsKey("employee._advancedGrantsFullInGroup"))
                LoadedExternalProperties.Remove("employee._advancedGrantsFullInGroup");

            LoadedExternalProperties.Add("employee._commonFoldersFullInGroup", DateTime.UtcNow);
            LoadedExternalProperties.Add("employee._advancedGrantsFullInGroup", DateTime.UtcNow);
        }

        /// <summary>
        ///     Получение данных об общих папках и дополнительных правах сотрудника
        ///     Кривая структура процедуры sp_ОбщиеПапкиСотрудника - возвращает два несвязанных между собой рекордсета!!!
        /// </summary>
        private void FillCommonFoldersAndAdvancedGrants()
        {
            _commonFolders = new List<CommonFolder>();
            _advancedGrants = new Dictionary<int, byte?>();

            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };
            using (
                var dbReader = new DBReader(SQLQueries.SP_ОбщиеПапкиСотрудника, CommandType.StoredProcedure, CN,
                    sqlParams))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var f = new CommonFolder();
                        var colКодОбщейПапки = dbReader.GetOrdinal("КодОбщейПапки");
                        var colОбщаяПапка = dbReader.GetOrdinal("ОбщаяПапка");

                        f.Unavailable = false;

                        if (!dbReader.IsDBNull(colКодОбщейПапки))
                            f.Id = dbReader.GetInt32(colКодОбщейПапки).ToString();
                        if (!dbReader.IsDBNull(colОбщаяПапка)) f.Name = dbReader.GetString(colОбщаяПапка);

                        _commonFolders.Add(f);
                    }
                }

                if (dbReader.NextResult())
                    if (dbReader.HasRows)
                    {
                        var colКодДопПараметраУказанийИТ = dbReader.GetOrdinal("КодДопПараметраУказанийИТ");
                        var colЕстьПраво = dbReader.GetOrdinal("Есть");

                        while (dbReader.Read())
                            if (!dbReader.IsDBNull(colЕстьПраво))
                                _advancedGrants.Add(dbReader.GetInt32(colКодДопПараметраУказанийИТ),
                                    dbReader.GetByte(colЕстьПраво));
                            else
                                _advancedGrants.Add(dbReader.GetInt32(colКодДопПараметраУказанийИТ), null);
                    }

            }

            if (LoadedExternalProperties.ContainsKey("employee._commonFolders"))
                LoadedExternalProperties.Remove("employee._commonFolders");
            if (LoadedExternalProperties.ContainsKey("employee._advancedGrants"))
                LoadedExternalProperties.Remove("employee._advancedGrants");

            LoadedExternalProperties.Add("employee._commonFolders", DateTime.UtcNow);
            LoadedExternalProperties.Add("employee._advancedGrants", DateTime.UtcNow);
        }

        /// <summary>
        ///     Получение информации из должностей, о наличии у должности Sim-карты
        /// </summary>
        private void SetSimInfo()
        {
            if (LoadedExternalProperties.ContainsKey("employee._simInfo")) return;

            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", Id } };
            var dt = DBManager.GetData(SQLQueries.SELECT_СотрудникуПоДолжностиПоложенаSIM, ConnString,
                CommandType.Text, sqlParams);

            _simGprsPackage = false;
            _simRequired = false;
            _simPost = "";
            _simNumber = "";
            if (dt.Rows.Count == 1)
            {
                _simRequired = dt.Rows[0]["НеобходимаSIMКарта"].ToString() != "0";
                _simGprsPackage = dt.Rows[0]["ПодключитьGPRSПакет"].ToString() != "0";
                _simPost = dt.Rows[0]["Должность"].ToString();
                _simNumber = dt.Rows[0]["SIMКартаНомер"].ToString();
            }

            LoadedExternalProperties.Add("employee._simInfo", DateTime.UtcNow);
        }

        /// <summary>
        ///     получить локализованное ФИО
        /// </summary>
        /// <returns></returns>
        public string GetLocalizationFio()
        {
            switch (Language)
            {
                case "ru":
                    return FIO;
                case "en":
                    return FIOEn;
                default:
                    return FIOEn;
            }
        }

        /// <summary>
        ///     Список сотрудников
        /// </summary>
        /// <returns></returns>
        public List<int> CurrentEmployees()
        {
            var dt = DBManager.GetData(SQLQueries.SELECT_FN_ТекущийСотрудник, CN, CommandType.Text, null);

            if (dt.Rows.Count == 0) return null;
            return (from DataRow row in dt.Rows select (int)row["КодСотрудника"]).ToList();
        }

        /// <summary>
        ///     Загрузка информации о сотруднике
        /// </summary>
        public override void Load()
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Сотрудник, EmployeeId, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    var colCommonEmployeeID = dbReader.GetOrdinal("КодОбщегоСотрудника");
                    var colLogin = dbReader.GetOrdinal("Login");
                    var colСотрудникLocal = dbReader.GetOrdinal("СотрудникLocal");
                    var colFioLocal = dbReader.GetOrdinal("ФИОLocal");
                    var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                    var colFullNameEn = dbReader.GetOrdinal("Employee");
                    var colФИО = dbReader.GetOrdinal("ФИО");
                    var colFIO = dbReader.GetOrdinal("FIO");

                    var colДополнение = dbReader.GetOrdinal("Дополнение");
                    var colAddition = dbReader.GetOrdinal("Addition");

                    var colЯзык = dbReader.GetOrdinal("Язык");
                    var colStatus = dbReader.GetOrdinal("Состояние");
                    var colGuid = dbReader.GetOrdinal("GUID");

                    var colКодЛица = dbReader.GetOrdinal("КодЛица");
                    var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");

                    var colLastNameEn = dbReader.GetOrdinal("LastName");
                    var colFirstNameEn = dbReader.GetOrdinal("FirstName");
                    var colMiddleNameEn = dbReader.GetOrdinal("MiddleName");

                    var colDisplayName = dbReader.GetOrdinal("DisplayName");
                    var colEmail = dbReader.GetOrdinal("Email");
                    var colPersonalFolder = dbReader.GetOrdinal("ЛичнаяПапка");

                    var colGender = dbReader.GetOrdinal("Пол");
                    var colBirthdate = dbReader.GetOrdinal("ДатаРождения");
                    var colBirthplace = dbReader.GetOrdinal("МестоРождения");
                    var colInn = dbReader.GetOrdinal("ИНН");
                    var colPensionCert = dbReader.GetOrdinal("ПенсСвидетельство");

                    var colFirstName = dbReader.GetOrdinal("Имя");
                    var colLastName = dbReader.GetOrdinal("Фамилия");
                    var colMiddleName = dbReader.GetOrdinal("Отчество");

                    var colAccountDisabled = dbReader.GetOrdinal("AccountDisabled");
                    var colAccountExpires = dbReader.GetOrdinal("AccountExpires");

                    var colПримечания = dbReader.GetOrdinal("Примечания");

                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();

                        if (!dbReader.IsDBNull(colCommonEmployeeID))
                            CommonEmployeeID = dbReader.GetInt32(colCommonEmployeeID).ToString();

                        if (!dbReader.IsDBNull(colСотрудникLocal)) Name = dbReader.GetString(colСотрудникLocal);

                        if (!dbReader.IsDBNull(colСотрудник)) FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);

                        if (!dbReader.IsDBNull(colДополнение)) Addition = dbReader.GetString(colДополнение);
                        if (!dbReader.IsDBNull(colAddition)) AdditionEn = dbReader.GetString(colAddition);

                        if (!dbReader.IsDBNull(colЯзык)) Language = dbReader.GetString(colЯзык);
                        if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);

                        if (!dbReader.IsDBNull(colGuid)) Guid = dbReader.GetGuid(colGuid);
                        if (!dbReader.IsDBNull(colLogin)) Login = dbReader.GetString(colLogin);

                        if (!dbReader.IsDBNull(colFioLocal)) NameShort = dbReader.GetString(colFioLocal);
                        if (!dbReader.IsDBNull(colФИО)) FIO = dbReader.GetString(colФИО);
                        if (!dbReader.IsDBNull(colFIO)) FIOEn = dbReader.GetString(colFIO);

                        if (!dbReader.IsDBNull(colКодЛица)) PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);

                        if (!dbReader.IsDBNull(colLastNameEn)) LastNameEn = dbReader.GetString(colLastNameEn);
                        if (!dbReader.IsDBNull(colFirstNameEn)) FirstNameEn = dbReader.GetString(colFirstNameEn);
                        if (!dbReader.IsDBNull(colMiddleNameEn)) MiddleNameEn = dbReader.GetString(colMiddleNameEn);

                        if (!dbReader.IsDBNull(colDisplayName)) DisplayName = dbReader.GetString(colDisplayName);
                        if (!dbReader.IsDBNull(colEmail)) Email = dbReader.GetString(colEmail);
                        if (!dbReader.IsDBNull(colPersonalFolder))
                            PersonalFolder = dbReader.GetString(colPersonalFolder);

                        if (!dbReader.IsDBNull(colGender)) Gender = dbReader.GetString(colGender);
                        if (!dbReader.IsDBNull(colBirthdate)) Birthdate = dbReader.GetDateTime(colBirthdate);
                        if (!dbReader.IsDBNull(colBirthplace)) Birthplace  = dbReader.GetString(colBirthplace);
                        if (!dbReader.IsDBNull(colInn)) Inn = dbReader.GetString(colInn);
                        if (!dbReader.IsDBNull(colPensionCert)) PensionCert  = dbReader.GetString(colPensionCert);

                        if (!dbReader.IsDBNull(colLastName)) LastName = dbReader.GetString(colLastName);
                        if (!dbReader.IsDBNull(colFirstName)) FirstName = dbReader.GetString(colFirstName);
                        if (!dbReader.IsDBNull(colMiddleName)) MiddleName = dbReader.GetString(colMiddleName);

                        if (!dbReader.IsDBNull(colAccountDisabled))
                            AccountDisabled = dbReader.GetByte(colAccountDisabled);

                        if (!dbReader.IsDBNull(colAccountExpires))
                            AccountExpires = dbReader.GetDateTime(colAccountExpires);

                        if (!dbReader.IsDBNull(colПримечания)) Notes = dbReader.GetString(colПримечания);

                        if (!dbReader.IsDBNull(colИзменено)) Changed = dbReader.GetDateTime(colИзменено);
                        if (!dbReader.IsDBNull(colИзменил)) ChangedBy = dbReader.GetInt32(colИзменил);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Загрузка информации о сотруднике по GUID
        /// </summary>
        public void LoadByGuid(Guid guid)
        {
            var sqlParams = new Dictionary<string, object> { { "@GUID", guid } };
            using (var dbReader = new DBReader(SQLQueries.SELECT_GUID_Сотрудник, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    var colCommonEmployeeID = dbReader.GetOrdinal("КодОбщегоСотрудника");
                    var colLogin = dbReader.GetOrdinal("Login");
                    var colСотрудникLocal = dbReader.GetOrdinal("СотрудникLocal");
                    var colFioLocal = dbReader.GetOrdinal("ФИОLocal");
                    var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                    var colFullNameEn = dbReader.GetOrdinal("Employee");
                    var colФИО = dbReader.GetOrdinal("ФИО");
                    var colFIO = dbReader.GetOrdinal("FIO");

                    var colДополнение = dbReader.GetOrdinal("Дополнение");
                    var colAddition = dbReader.GetOrdinal("Addition");

                    var colЯзык = dbReader.GetOrdinal("Язык");
                    var colStatus = dbReader.GetOrdinal("Состояние");
                    var colGuid = dbReader.GetOrdinal("GUID");

                    var colКодЛица = dbReader.GetOrdinal("КодЛица");
                    var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");

                    var colLastNameEn = dbReader.GetOrdinal("LastName");
                    var colFirstNameEn = dbReader.GetOrdinal("FirstName");
                    var colMiddleNameEn = dbReader.GetOrdinal("MiddleName");

                    var colDisplayName = dbReader.GetOrdinal("DisplayName");
                    var colEmail = dbReader.GetOrdinal("Email");
                    var colPersonalFolder = dbReader.GetOrdinal("ЛичнаяПапка");

                    var colGender = dbReader.GetOrdinal("Пол");

                    var colFirstName = dbReader.GetOrdinal("Имя");
                    var colLastName = dbReader.GetOrdinal("Фамилия");
                    var colMiddleName = dbReader.GetOrdinal("Отчество");

                    var colAccountDisabled = dbReader.GetOrdinal("AccountDisabled");
                    var colAccountExpires = dbReader.GetOrdinal("AccountExpires");

                    var colПримечания = dbReader.GetOrdinal("Примечания");

                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();

                        if (!dbReader.IsDBNull(colCommonEmployeeID))
                            CommonEmployeeID = dbReader.GetInt32(colCommonEmployeeID).ToString();

                        if (!dbReader.IsDBNull(colСотрудникLocal)) Name = dbReader.GetString(colСотрудникLocal);

                        if (!dbReader.IsDBNull(colСотрудник)) FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);

                        if (!dbReader.IsDBNull(colДополнение)) Addition = dbReader.GetString(colДополнение);
                        if (!dbReader.IsDBNull(colAddition)) AdditionEn = dbReader.GetString(colAddition);

                        if (!dbReader.IsDBNull(colЯзык)) Language = dbReader.GetString(colЯзык);
                        if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);

                        if (!dbReader.IsDBNull(colGuid)) Guid = dbReader.GetGuid(colGuid);
                        if (!dbReader.IsDBNull(colLogin)) Login = dbReader.GetString(colLogin);

                        if (!dbReader.IsDBNull(colFioLocal)) NameShort = dbReader.GetString(colFioLocal);
                        if (!dbReader.IsDBNull(colФИО)) FIO = dbReader.GetString(colФИО);
                        if (!dbReader.IsDBNull(colFIO)) FIOEn = dbReader.GetString(colFIO);

                        if (!dbReader.IsDBNull(colКодЛица)) PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);

                        if (!dbReader.IsDBNull(colLastNameEn)) LastNameEn = dbReader.GetString(colLastNameEn);
                        if (!dbReader.IsDBNull(colFirstNameEn)) FirstNameEn = dbReader.GetString(colFirstNameEn);
                        if (!dbReader.IsDBNull(colMiddleNameEn)) MiddleNameEn = dbReader.GetString(colMiddleNameEn);

                        if (!dbReader.IsDBNull(colDisplayName)) DisplayName = dbReader.GetString(colDisplayName);
                        if (!dbReader.IsDBNull(colEmail)) Email = dbReader.GetString(colEmail);
                        if (!dbReader.IsDBNull(colPersonalFolder))
                            PersonalFolder = dbReader.GetString(colPersonalFolder);

                        if (!dbReader.IsDBNull(colGender)) Gender = dbReader.GetString(colGender);

                        if (!dbReader.IsDBNull(colLastName)) LastName = dbReader.GetString(colLastName);
                        if (!dbReader.IsDBNull(colFirstName)) FirstName = dbReader.GetString(colFirstName);
                        if (!dbReader.IsDBNull(colMiddleName)) MiddleName = dbReader.GetString(colMiddleName);

                        if (!dbReader.IsDBNull(colAccountDisabled))
                            AccountDisabled = dbReader.GetByte(colAccountDisabled);

                        if (!dbReader.IsDBNull(colAccountExpires))
                            AccountExpires = dbReader.GetDateTime(colAccountExpires);


                        if (!dbReader.IsDBNull(colПримечания)) Notes = dbReader.GetString(colПримечания);

                        if (!dbReader.IsDBNull(colИзменено)) Changed = dbReader.GetDateTime(colИзменено);
                        if (!dbReader.IsDBNull(colИзменил)) ChangedBy = dbReader.GetInt32(colИзменил);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Заполнение полей объекта через DBReader
        /// </summary>
        /// <param name="dbReader">V4.DBReader</param>
        /// <param name="loadFromOutSource">Заполнять минимальное количество полей</param>
        public void LoadFromDbReader(DBReader dbReader, bool loadFromOutSource = false)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца

                var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                var colCommonEmployeeID = dbReader.GetOrdinal("КодОбщегоСотрудника");
                var colLogin = dbReader.GetOrdinal("Login");
                var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                var colFullNameEn = dbReader.GetOrdinal("Employee");
                var colЯзык = dbReader.GetOrdinal("Язык");
                var colStatus = dbReader.GetOrdinal("Состояние");
                var colGuid = dbReader.GetOrdinal("GUID");

                var colФИО = dbReader.GetOrdinal("ФИО");
                var colFIO = dbReader.GetOrdinal("FIO");

                var colДополнение = dbReader.GetOrdinal("Дополнение");
                var colAddition = dbReader.GetOrdinal("Addition");

                var colКодЛица = dbReader.GetOrdinal("КодЛица");
                var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");

                var colLastNameEn = dbReader.GetOrdinal("LastName");
                var colFirstNameEn = dbReader.GetOrdinal("FirstName");

                var colDisplayName = dbReader.GetOrdinal("DisplayName");
                var colEmail = dbReader.GetOrdinal("Email");
                var colPersonalFolder = dbReader.GetOrdinal("ЛичнаяПапка");

                var colGender = dbReader.GetOrdinal("Пол");

                var colИзменил = dbReader.GetOrdinal("Изменил");
                var colИзменено = dbReader.GetOrdinal("Изменено");

                #endregion

                if (!loadFromOutSource)
                {
                    if (!dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();

                        if (!dbReader.IsDBNull(colCommonEmployeeID))
                            CommonEmployeeID = dbReader.GetInt32(colCommonEmployeeID).ToString();

                        if (!dbReader.IsDBNull(colLogin)) Name = dbReader.GetString(colLogin);
                        if (!dbReader.IsDBNull(colСотрудник)) FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);

                        if (!dbReader.IsDBNull(colФИО)) FIO = dbReader.GetString(colФИО);
                        if (!dbReader.IsDBNull(colFIO)) FIOEn = dbReader.GetString(colFIO);

                        if (!dbReader.IsDBNull(colДополнение)) Addition = dbReader.GetString(colДополнение);
                        if (!dbReader.IsDBNull(colAddition)) AdditionEn = dbReader.GetString(colAddition);

                        if (!dbReader.IsDBNull(colЯзык)) Language = dbReader.GetString(colЯзык);
                        if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);

                        if (!dbReader.IsDBNull(colКодЛица)) PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);
                    }
                }
                else
                {
                    Unavailable = false;

                    Id = dbReader.GetInt32(colКодСотрудника).ToString();

                    if (!dbReader.IsDBNull(colCommonEmployeeID))
                        CommonEmployeeID = dbReader.GetInt32(colCommonEmployeeID).ToString();
                    if (!dbReader.IsDBNull(colLogin)) Name = dbReader.GetString(colLogin);

                    if (!dbReader.IsDBNull(colСотрудник)) FullName = dbReader.GetString(colСотрудник);
                    if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);

                    if (!dbReader.IsDBNull(colФИО)) FIO = dbReader.GetString(colФИО);
                    if (!dbReader.IsDBNull(colFIO)) FIOEn = dbReader.GetString(colFIO);

                    if (!dbReader.IsDBNull(colДополнение)) Addition = dbReader.GetString(colДополнение);
                    if (!dbReader.IsDBNull(colAddition)) AdditionEn = dbReader.GetString(colAddition);

                    if (!dbReader.IsDBNull(colЯзык)) Language = dbReader.GetString(colЯзык);
                    if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);

                    if (!dbReader.IsDBNull(colGuid)) Guid = dbReader.GetGuid(colGuid);
                    if (!dbReader.IsDBNull(colLogin)) Login = dbReader.GetString(colLogin);

                    if (!dbReader.IsDBNull(colКодЛица)) PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                    if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                        OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);

                    if (!dbReader.IsDBNull(colLastNameEn)) LastNameEn = dbReader.GetString(colLastNameEn);
                    if (!dbReader.IsDBNull(colFirstNameEn)) FirstNameEn = dbReader.GetString(colFirstNameEn);

                    if (!dbReader.IsDBNull(colDisplayName)) DisplayName = dbReader.GetString(colDisplayName);
                    if (!dbReader.IsDBNull(colEmail)) Email = dbReader.GetString(colEmail);
                    if (!dbReader.IsDBNull(colPersonalFolder)) PersonalFolder = dbReader.GetString(colPersonalFolder);

                    if (!dbReader.IsDBNull(colGender)) Gender = dbReader.GetString(colGender);

                    if (!dbReader.IsDBNull(colИзменено)) Changed = dbReader.GetDateTime(colИзменено);
                    if (!dbReader.IsDBNull(colИзменил)) ChangedBy = dbReader.GetInt32(colИзменил);
                }
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получить подразделение в котором работает сотрудник
        /// </summary>
        public static string GetUserDivision(int EmployeeId)
        {
            var division = DBManager.ExecuteScalar(SQLQueries.SELECT_ПодразделениеСотрудника, EmployeeId,
                CommandType.Text, ConnString);

            if (division is string)
                return division.ToString();

            return string.Empty;
        }

        /// <summary>
        ///     Подготовка данных для заполни объекта User
        /// </summary>
        /// <param name="dt">Заполненная Datatable c информацией о сотруднике</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодСотрудника"].ToString();
                FullName = dt.Rows[0]["Сотрудник"].ToString();
                FullNameEn = dt.Rows[0]["Employee"].ToString();
                PersonEmployeeId = dt.Rows[0]["КодЛица"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодЛица"];
                Name = dt.Rows[0]["LoginFull"].ToString();
                FIO = dt.Rows[0]["ФИО"].ToString();
                FIOEn = dt.Rows[0]["FIO"].ToString();
                Language = dt.Rows[0]["Язык"].ToString();
            }
        }

        /// <summary>
        ///     Получение информации о текущем сотруднике по SID
        /// </summary>
        protected void GetCurrentUser()
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_SID_ТекущийСотрудник, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                    var colФИО = dbReader.GetOrdinal("ФИО");
                    var colEmployee = dbReader.GetOrdinal("Employee");
                    var colFIO = dbReader.GetOrdinal("FIO");
                    var colКодЛица = dbReader.GetOrdinal("КодЛица");
                    var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");
                    var colLoginFull = dbReader.GetOrdinal("LoginFull");
                    var colЯзык = dbReader.GetOrdinal("Язык");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();
                        FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colФИО)) FIO = dbReader.GetString(colФИО);
                        if (!dbReader.IsDBNull(colEmployee)) FullNameEn = dbReader.GetString(colEmployee);
                        if (!dbReader.IsDBNull(colFIO)) FIOEn = dbReader.GetString(colFIO);
                        if (!dbReader.IsDBNull(colКодЛица)) PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);
                        Name = dbReader.GetString(colLoginFull);
                        Language = dbReader.GetString(colЯзык);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Получение описание состония сотрудника по коду состония
        /// </summary>
        /// <param name="status">Код состони</param>
        /// <returns>Описание состония</returns>
        public string GetEmployeeStatus(int status)
        {
            var Resx = Localization.Resources.Resx;


            switch (status)
            {
                case 1:
                    return Resx.GetString("EmployeeStatus_MaternityLeave");
                case 2:
                    return Resx.GetString("EmployeeStatus_PersonalGuest");
                case 3:
                    return Resx.GetString("EmployeeStatus_Fired");
                case 4:
                    return Resx.GetString("EmployeeStatus_Guest");
                case 5:
                    return Resx.GetString("EmployeeStatus_Outlier");

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        ///     Получение информации о руководителе сотрудника в разрезе указанной компании
        /// </summary>
        /// <returns>
        ///     DataTable[КодОрганизацииСотрудника, КодСотрудника, КодЛицаСотрудника, Сотрудник, ДолжностьСотрудника,
        ///     КодСотрудникаРуководителя, КодЛицаРуководителя, Руководитель, ДолжностьРуководителя]
        /// </returns>
        public DataTable SupervisorsData()
        {
            if (LoadedExternalProperties.ContainsKey("employee.supervisorsData")) return supervisorsData;

            supervisorsData = new DataTable("SupervisorsData");

            var dr = supervisorsData.NewRow();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСотрудника", int.Parse(Id));


            supervisorsData = DBManager.GetData(SQLQueries.SELECT_НепосредственныйРуководитель_Данные, CN,
                CommandType.Text,
                sqlParams);

            LoadedExternalProperties.Add("employee.supervisorsData", DateTime.UtcNow);

            return supervisorsData;
        }

        /// <summary>
        ///     Получение информации о должностях сотрудника
        /// </summary>
        /// <param name="combine">0-по основному месту работы, 1 - там, где сотрудник является совместителем</param>
        /// <returns>DataTable[КодДолжности, Должность, КодСотрудника, КодЛица]</returns>
        public DataTable GetUserPositions(CombineType combine)
        {

            if (LoadedExternalProperties.ContainsKey("employee.positionsData")) return UserPositions(combine);

            positionsData = new DataTable("UserPositions");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСотрудника", new object[] { Id, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@Совместитель", new object[] { (int)combine, DBManager.ParameterTypes.Int32 });

            positionsData = DBManager.GetData(SQLQueries.SELECT_ДолжностиСотрудника, CN, CommandType.Text, sqlParams);

            LoadedExternalProperties.Add("employee.positionsData", DateTime.UtcNow);
            return UserPositions(combine);
        }


        /// <summary>
        /// Вспомогательная функиция для получения определенного вида должностей
        /// </summary>
        /// <param name="combine">Какие нужны должности</param>
        /// <returns></returns>
        private DataTable UserPositions(CombineType combine)
        {
            var positionsDataClone = positionsData.Clone();

            DataRow[] drs = null;
            switch (combine)
            {

                case CombineType.Неважно: return positionsData;
                case CombineType.ОсновноеМестоРаботы:
                    drs = positionsDataClone.Select("Совместитель = 1");
                    break;
                case CombineType.Совместитель:
                    drs = positionsDataClone.Select("Совместитель = 0");
                    break;
            }

            foreach (var row in drs)
                row.Delete();
            positionsDataClone.AcceptChanges();
            return positionsDataClone;
        }

        /// <summary>
        ///     Получение мобильного телефонона сотудника из конткатов его лица
        /// </summary>
        /// <returns>Мобильный телефон</returns>
        public string GetMobilePhone()
        {
            var p = PersonEmployee;

            if (p == null || p.Unavailable) return "";

            var contacts = p.GetContacts(ContactTypeEnum.ТелефонМобильный);
            if (contacts == null || contacts.Count != 1) return "";

            return contacts[0].ContactName;
        }

        /// <summary>
        ///     Проверка подчиненности текущему сотруднику
        /// </summary>
        /// <param name="subordinateUserId">Код подчиненного сотрудника</param>
        /// <returns>true - подчинен, false - не подчинен</returns>
        public bool CheckSubordination(int subordinateUserId)
        {
            var dt = new DataTable("UserPositions");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСотрудника", new object[] { subordinateUserId, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@КодРуководителя", new object[] { int.Parse(Id), DBManager.ParameterTypes.Int32 });

            dt = DBManager.GetData(SQLQueries.SELECT_ПроверкаПодчиненияСотрудника, CN, CommandType.Text, sqlParams);
            return dt.Rows.Count > 0;
        }

        /// <summary>
        ///     Получение списка сотрудников, которые работатют на указанном рабочем месте
        /// </summary>
        /// <param name="wp">КодРасположения</param>
        /// <param name="maxState">Максимальное состояние сотрудника</param>
        /// <returns>Список сотрудников на рабочем месте</returns>
        public List<Employee> EmployeesOnWorkPlace(int wp, int maxState = 3)
        {
            _employeesOnWorkPlace = new List<Employee>();
            var sqlParams = new Dictionary<string, object>
                {{"@Id", wp}, {"@idEmpl", int.Parse(Id)}, {"@state", maxState}};
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_СотрудникиНаРабочемМесте, CommandType.Text, CN,
                    sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var empl = new Employee(false);
                        empl.LoadFromDbReader(dbReader, true);
                        _employeesOnWorkPlace.Add(empl);
                    }
            }

            return _employeesOnWorkPlace;
        }

        /// <summary>
        ///     Получение списка обордования сотрудника на рабочих местах, где не работает указанный сотрудник
        /// </summary>
        /// <param name="excludeWpId"></param>
        /// <returns></returns>
        public List<Equipment> EmployeeEquipmentsAnotherWorkPlaces(string excludeWpId = "")
        {
            if (LoadedExternalProperties.ContainsKey(CacheKey_EquipmentsAnotherWorkPlaces))
                return _employeeEquipmentsAnotherWorkPlaces;

            _employeeEquipmentsAnotherWorkPlaces = new List<Equipment>();
            var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) }, { "@IT", 1 }, { "@WPType", 1 } };
            if (!string.IsNullOrEmpty(excludeWpId))
                sqlParams.Add("@ExcludeId", int.Parse(excludeWpId));
            else
                sqlParams.Add("@ExcludeId", 0);

            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ID_ОборудованиеСотрудникаНаДругихРабочихМестах,
                    CommandType.Text, CN, sqlParams))
            {
                _employeeEquipmentsAnotherWorkPlaces = Equipment.GetEquipmentList(dbReader);
            }

            LoadedExternalProperties.Add(CacheKey_EquipmentsAnotherWorkPlaces, DateTime.UtcNow);

            return _employeeEquipmentsAnotherWorkPlaces;
        }


        /// <summary>
        /// Возвращает список лиц сотрудника
        /// </summary>
        /// <param name="bitMask">
        /// 1 - компания ответственная за фин. регулирование; 
        /// 2 - лицо сотрудника;
        /// 4 - работодатели;
        /// </param>
        /// <returns>Список лиц</returns>
        public List<int> PersonsIds(int bitMask)
        {
            var persons = new List<int>();

            if ((bitMask & 1) == 1 && OrganizationId.HasValue)
            {
                if (!persons.Contains(OrganizationId.Value))
                    persons.Add(OrganizationId.Value);
            }

            if ((bitMask & 2) == 2 && PersonEmployeeId.HasValue)
            {
                if (!persons.Contains(PersonEmployeeId.Value))
                    persons.Add(PersonEmployeeId.Value);
            }

            if ((bitMask & 4) == 4)
            {
                var posts = GetUserPositions(CombineType.Неважно);
                foreach (DataRow row in posts.Rows)
                {
                    int personId = int.Parse(row["КодЛица"].ToString());
                    if (persons.Contains(personId)) break;
                    persons.Add(personId);
                }
            }


            return persons;

        }

        /// <summary>
        /// Проверка соответствия доменного имении, организациям сотрудника
        /// </summary>
        /// <param name="domain">Доменное имя</param>
        /// <param name="domains">Список доменных имен</param>
        /// <returns>Соответствует или нет</returns>
        public bool CheckEmailDomainName(string domain, List<DomainName> domains)
        {

            var persons = PersonsIds(7);

            if (persons.Count == 0) return false;

            var dn = domains.FirstOrDefault(x => x.Name.ToLower() == domain.ToLower());
            if (dn == null) return false;

            if (string.IsNullOrEmpty(dn.PersonIds)) return true;

            var pIds = dn.PersonIds.Split(',');
            for (var i = 0; i < pIds.Length; i++)
            {
                if (persons.Contains(int.Parse(pIds[i].Trim()))) return true;

            }

            return false;
        }

        /// <summary>
        /// Проверка на соответсвие логина имени сотрудника
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Соответствует или нет</returns>
        public bool CheckLoginName(string login) {

            if (string.IsNullOrEmpty(login)) return true;
            if (login.Equals(FirstNameEn, StringComparison.InvariantCultureIgnoreCase)) return true;
            if (login.Equals(LastNameEn, StringComparison.InvariantCultureIgnoreCase)) return true;
            if (login.Equals(FirstNameEn.Left(1) + LastNameEn, StringComparison.InvariantCultureIgnoreCase)) return true;
            if (login.Equals(LastNameEn.Left(1) + FirstNameEn, StringComparison.InvariantCultureIgnoreCase)) return true;
            if (login.Equals(FirstNameEn.Left(1) + MiddleNameEn.Left(1) + LastNameEn, StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        ///     Определить имеет ли сотрудник роль
        /// </summary>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Возвращает значение, указывающее, имеет ли сотрудник роль</returns>
        public bool HasRole(int roleId)
        {
            return HasRole(roleId, -1);
        }

        /// <summary>
        ///     Определить имеет ли сотрудник роль в компании
        /// </summary>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="personId">Идентификатор лица</param>
        /// <returns>Возвращает значение, указывающее, имеет ли сотрудник роль в указанной компании</returns>
        public bool HasRole(int roleId, int personId)
        {
            if (personId == -1)
                return RolesCurrentEmployes.Exists(x => x.RoleId.Equals(roleId));

            return RolesCurrentEmployes.Exists(x =>
                x.RoleId.Equals(roleId) && (personId == x.PersonId || x.PersonId == 0));
        }

        /// <summary>
        /// Получение несоответствий в правах в группе посмненной работы
        /// </summary>
        public DataTable GetAccessGroupInconsistencies
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("employee._groupInconsistencies")) return _groupInconsistencies;
                _groupInconsistencies = new DataTable();

                var sqlParams = new Dictionary<string, object>();
                sqlParams.Add("@КодСотрудника", int.Parse(Id));
                sqlParams.Add("@КодДокумента", DBNull.Value);

                var personServer = new SqlConnection(Config.DS_person).DataSource;
                _groupInconsistencies = DBManager.GetData(string.Format(SQLQueries.SELECT_НесоотвествиеПравСотрудниковВГруппе, personServer), CN, CommandType.Text, sqlParams);

                LoadedExternalProperties.Add("employee._groupInconsistencies", DateTime.UtcNow);
                return _groupInconsistencies;
            }
        }

        /// <summary>
        /// Свойство: Сотрудник имеет учетную запись
        /// </summary>
        public bool HasAccount_ => AccountDisabled.HasValue;

        /// <summary>
        /// Свойство: Сотрудник имеет действующую учетную запись. т.е. может работать
        /// т.е сотрудник имеет учетную запись и его учетная запись не отключена и срок действия учетной записи не истек
        /// </summary>
        public bool HasAccountValid => HasAccount_ && !IsAccountDisabled && !IsAccountExpires;

        /// <summary>
        /// Свойство: Учетная запись заблокирована
        /// </summary>
        public int? IsAccountLocked
        {

            get
            {
                GetUserStatus();
                return _isAccountLocked;
            }
        }

        /// <summary>
        /// Свойство: Пароль пользователя истекает
        /// </summary>
        public DateTime? PwdExpiries
        {

            get
            {
                GetUserStatus();
                return _pwdExpiries;
            }

        }

        /// <summary>
        /// Путь в ad
        /// </summary>
        /// <param name="currentUser">Кто запрашивает информацию</param>
        /// <returns></returns>
        public string ADSIAccountPath(Employee currentUser)
        {
            if (LoadedExternalProperties.ContainsKey("employee._adsiAccountPath")) return _adsiAccountPath;
            ADSIAccountInfo(currentUser);
            return _adsiAccountPath;
        }

        /// <summary>
        /// Отключена ли учентная запись
        /// </summary>
        /// <param name="currentUser">Кто запрашивает информацию</param>
        /// <returns></returns>
        public int? ADSIAccountDisabled(Employee currentUser)
        {
            if (LoadedExternalProperties.ContainsKey("employee._adsiAccountDisabled")) return _adsiAccountDisabled;
            ADSIAccountInfo(currentUser);
            return _adsiAccountDisabled;
        }

        /// <summary>
        ///  Получение информации из vwADSI, только для Анисимова В.Л.
        /// </summary>
        public void ADSIAccountInfo(Employee currentUser)
        {
            if (LoadedExternalProperties.ContainsKey("employee._adsiAccountExpires")) return;

            if (int.Parse(currentUser.Id) != (int)BaseExtention.Enums.Corporate.КодыСотрудников.Анисимов || string.IsNullOrEmpty(Login)) return;

            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@Login", Login);

            var dt = DBManager.GetData(SQLQueries.SELECT_ADSI_ПоЛогину, CN, CommandType.Text, sqlParams);

            if (dt.Rows.Count != 1)
            {
                _adsiAccountExpires = null;
                _adsiAccountDisabled = null;
                _adsiAccountPath = null;
            }
            else
            {
                if (dt.Rows[0]["accountExpires"].Equals(DBNull.Value))
                    _adsiAccountExpires = null;
                else
                    _adsiAccountExpires = (DateTime)dt.Rows[0]["accountExpires"];

                if (dt.Rows[0]["Disabled"].Equals(DBNull.Value))
                    _adsiAccountDisabled = null;
                else
                    _adsiAccountDisabled = int.Parse(dt.Rows[0]["Disabled"].ToString());


                if (dt.Rows[0]["Path"].Equals(DBNull.Value))
                    _adsiAccountPath = null;
                else
                    _adsiAccountPath = dt.Rows[0]["Path"].ToString();
            }

            LoadedExternalProperties.Add("employee._adsiAccountExpires", DateTime.UtcNow);
            LoadedExternalProperties.Add("employee._adsiAccountDisabled", DateTime.UtcNow);
            LoadedExternalProperties.Add("employee._adsiAccountPath", DateTime.UtcNow);
            
        }

        /// <summary>
        /// Получение списка контактов сотрудника
        /// </summary>
        /// <returns>Список контактов</returns>
        public List<EmployeeContact> GetEmployeeContacts()
        {
            var contactList = new List<EmployeeContact>();
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) }, { "@КодТелефоннойСтанцииЗвонящего", "" } };
            using (var dbReader = new DBReader(SQLQueries.SP_Сотрудники_Контакты, CommandType.StoredProcedure, CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserContact = new EmployeeContact();
                        tempUserContact.LoadFromDbReader(dbReader);
                        contactList.Add(tempUserContact);
                    }
            }

            return contactList;
        }

        /// <summary>
        /// Существуют выполненные указания на сотрудника, дата сохранения которых больше дат изменения его должностей
        /// </summary>
        /// <returns>Существую или нет</returns>
        public bool ExistDirectionsAfterChangePost(int docId = 0)
        {
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) }, { "@КодДокумента", docId } };
            var dt = DBManager.GetData(SQLQueries.SELECT_2356_ДатаСохраненияПоследнегоУказанияНаСотрудника, Config.DS_document, CommandType.Text, sqlParams);
            if (dt.Rows.Count != 1) return false;

            var svDateObj = dt.Rows[0][0];
            if (svDateObj.Equals(DBNull.Value)) return false;
            var posts = GetUserPositions(CombineType.Неважно);

            try
            {

                var svDate = Convert.ToDateTime(svDateObj);

                foreach (DataRow row in posts.Rows)
                {
                    var chDate = Convert.ToDateTime(row["Изменено"]);
                    if (svDate > chDate) return true;
                }
            }
            catch { return false; }

            return false;
        }

        private void GetUserStatus()
        {
            if (LoadedExternalProperties.ContainsKey(CacheKey_UserStatus)) return;

            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", int.Parse(Id) } };

            var dt = DBManager.GetData(SQLQueries.SP_UserGetStatus, Config.DS_user, CommandType.StoredProcedure, sqlParams);
           
            if (dt.Rows.Count != 1)
            {
                _isAccountLocked = null;
                _pwdExpiries = null;
            };

            try
            {
                
                if (dt.Rows[0]["IsAccountLocked"].Equals(DBNull.Value))
                    _isAccountLocked = null;
                else
                    _isAccountLocked = Convert.ToInt32(dt.Rows[0]["IsAccountLocked"]);

                if (dt.Rows[0]["PwdExpires"].Equals(DBNull.Value))
                    _pwdExpiries = null;
                else
                    _pwdExpiries = Convert.ToDateTime(dt.Rows[0]["PwdExpires"]);

                LoadedExternalProperties.Add(CacheKey_UserStatus, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Log.Logger.WriteEx(new Log.DetailedException("Ошибка получения UserStatus", ex));
            }
        }

        /// <summary>
        /// Проверка, что дата является рабочим днем сотрудника
        /// </summary>
        /// <param name="emplId">КодСотрудника</param>
        /// <param name="date">Дата</param>
        /// <returns>1-Рабочий день; 0-Выходной день</returns>
        public static int CheckWorkDay(int? emplId, DateTime date) {
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", emplId }, { "@Дата", date } };
            var dt = DBManager.GetData(SQLQueries.SELECT_Сотрудник_РабочийДень, Config.DS_user, CommandType.Text, sqlParams);

            return Convert.ToInt32(dt.Rows[0][0]);
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@Фамилия", LastName },
                { "@Имя", FirstName },
                { "@Отчество", MiddleName },
                { "@LastName", LastNameEn },
                { "@FirstName", FirstNameEn },
                { "@MiddleName", MiddleNameEn },
                { "@Пол", Gender },
                { "@ДатаРождения", Birthdate },
                { "@МестоРождения", Birthplace },
                { "@КодЛицаЗаказчика", OrganizationId },
                { "@ИНН", Inn },
                { "@ПенсСвидетельство", PensionCert }
            };

            if (IsNew)
            {
                var empId = DBManager.ExecuteScalar(SQLQueries.INSERT_Сотрудники,
                    CommandType.Text, Config.DS_user, sqlParams);

                if (empId != null)
                    Id = empId.ToString();
            }
            else
            {
                sqlParams.Add("@КодСотрудника", Id);

                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Сотрудники, CommandType.Text,
                    Config.DS_user, sqlParams, timeout: 120);
            }
        }
    }
}