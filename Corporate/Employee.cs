using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
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
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        private List<CommonFolder> _commonFolders;
        private List<Employee> _employeesOnWorkPlace;
        private PersonOld _employer;
        private PersonOld _organization;
        private PersonOld _personEmployee;
        private List<EmployeePhoto> _photos;
        private List<EmployeeRole> _roles;
        private List<EmployeePersonType> _types;
        private List<EmployeeWorkPlace> _workPlaces;
        private DataTable supervisorsData;
        private bool? _simRequired;
        private bool? _simGprsPackage;
        private string _simPost="";
        

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id"></param>
        public Employee(string id)
            : base(id)
        {
            Id = id;
            Load();
        }

        /// <summary>
        /// Конструктор по-умолчанию. Если необходимо получить текущего создайте объект с fillCurrentUser = true 
        /// </summary>
        /// <param name="fillCurrentUser">Заполнить объект данными текущего сотрудника</param>
        public Employee(bool fillCurrentUser)
        {
            if (fillCurrentUser) GetCurrentUser();
        }
        

        /// <summary>
        ///     ID. Поле КодСотрудника
        /// </summary>
        /// <remarks>
        ///     Типизированный псевданим для ID
        /// </remarks>
        public int EmployeeId
        {
            get { return Id.ToInt(); }
        }

        /// <summary>
        ///     Имя сотудника
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Код лица
        /// </summary>
        public int? PersonEmployeeId { get; set; }

        /// <summary>
        ///     Код лица заказчика
        /// </summary>
        public int? OrganizationId { get; set; }

  
        /// <summary>
        ///     ФИО сотрудника
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        ///     ФИО сотрудника на английском
        /// </summary>
        public string FIOEn { get; set; }

        /// <summary>
        ///     Язык
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Хост
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public DateTime? Changed { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>
        ///     The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>
        ///     The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the middle.
        /// </summary>
        /// <value>
        ///     The name of the middle.
        /// </value>
        public string MiddleName { get; set; }

        /// <summary>
        ///     Gets or sets the last name en.
        /// </summary>
        /// <value>
        ///     The last name en.
        /// </value>
        public string LastNameEn { get; set; }

        /// <summary>
        ///     Gets or sets the first name en.
        /// </summary>
        /// <value>
        ///     The first name en.
        /// </value>
        public string FirstNameEn { get; set; }

        /// <summary>
        ///     Gets or sets the middle name en.
        /// </summary>
        /// <value>
        ///     The middle name en.
        /// </value>
        public string MiddleNameEn { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the initials with.
        /// </summary>
        /// <value>
        ///     The last name of the initials with.
        /// </value>
        public string InitialsWithLastName { get; set; }

        /// <summary>
        ///     Gets or sets the full name en.
        /// </summary>
        /// <value>
        ///     The full name en.
        /// </value>
        public string FullNameEn { get; set; }

        /// <summary>
        ///     Gets or sets the initials with last name en.
        /// </summary>
        /// <value>
        ///     The initials with last name en.
        /// </value>
        public string InitialsWithLastNameEn { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        public int Status { get; set; }

        /// <summary>
        ///     Gets or sets the GUID.
        /// </summary>
        /// <value>
        ///     The GUID.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        ///     Gets or sets the login.
        /// </summary>
        /// <value>
        ///     The login.
        /// </value>
        public string Login { get; set; }

        /// <summary>
        ///     Gets or sets the login.
        /// </summary>
        /// <value>
        ///     The LoginFull.
        /// </value>
        public string LoginFull {
            get { return Name; }
            set { Name = value; }
        }


        /// <summary>
        ///     Gets or sets the account disabled.
        /// </summary>
        /// <value>
        ///     The account disabled.
        /// </value>
        public int? AccountDisabled { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the personal folder.
        /// </summary>
        /// <value>
        ///     The personal folder.
        /// </value>
        public string PersonalFolder { get; set; }

        /// <summary>
        ///     Gets or sets the access.
        /// </summary>
        /// <value>
        ///     The access.
        /// </value>
        public string Access { get; set; }

        /// <summary>
        ///     Gets or sets the gender.
        /// </summary>
        /// <value>
        ///     The gender.
        /// </value>
        public string Gender { get; set; }

        /// <summary>
        ///     Gets or sets the comments.
        /// </summary>
        /// <value>
        ///     The comments.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        ///     Gets or sets the last name RL.
        /// </summary>
        /// <value>
        ///     The last name RL.
        /// </value>
        public string LastNameRL { get; set; }

        /// <summary>
        ///     Gets or sets the first name RL.
        /// </summary>
        /// <value>
        ///     The first name RL.
        /// </value>
        public string FirstNameRL { get; set; }

        /// <summary>
        ///     Gets or sets the middle name RL.
        /// </summary>
        /// <value>
        ///     The middle name RL.
        /// </value>
        public string MiddleNameRL { get; set; }

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
        /// Сотруднику по должности положена Sim-карта
        /// </summary>
        public bool SimRequired {
            get
            {
                SetSimInfo();
                return _simRequired != null && _simRequired.Value;
            } 
          }
        /// <summary>
        /// К Sim-карте сотрудника подключен GPRS-пакет
        /// </summary>
        public bool SimGprsPackage {
            get
            {
                SetSimInfo();
                return _simGprsPackage != null && _simGprsPackage.Value;
            }
        }

        /// <summary>
        /// По какой должности сотруднику положена Sim-карта
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
        /// Получение информации из должностей, о наличии у должности Sim-карты
        /// </summary>
        private void SetSimInfo()
        {
            if (RequiredRefreshInfo || !_simGprsPackage.HasValue || !_simRequired.HasValue)
            {
                var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", Id}};
                var dt = DBManager.GetData(SQLQueries.SELECT_СотрудникуПоДолжностиПоложенаSIM, ConnString,
                    CommandType.Text, sqlParams);

                _simGprsPackage = false;
                _simRequired = false;
                _simPost = "";
                if (dt.Rows.Count == 1)
                {
                    _simRequired = (dt.Rows[0]["НеобходимаSIMКарта"].ToString() != "0");
                    _simGprsPackage = (dt.Rows[0]["ПодключитьGPRSПакет"].ToString() != "0");
                    _simPost = dt.Rows[0]["Должность"].ToString();
                }

            }
        }


        /// <summary>
        ///     Доступ к сети через VPN -- ЗНАЧЕНИЕ УСТАНАВЛИВАЕТСЯ ЧЕРЕЗ вызов CommonFolders
        /// </summary>
        public bool IsVPNGroup { get; set; }

        /// <summary>
        ///     Доступ в интернет из офиса -- ЗНАЧЕНИЕ УСТАНАВЛИВАЕТСЯ ЧЕРЕЗ вызов CommonFolders
        /// </summary>
        public bool IsInternetGroup { get; set; }

        /// <summary>
        ///     Доступ через модем -- ЗНАЧЕНИЕ УСТАНАВЛИВАЕТСЯ ЧЕРЕЗ вызов CommonFolders
        /// </summary>
        public bool IsDialUpGroup { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get
            {
                return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NationalAndInternationalAreTheSame
        {
            get
            {
                return
                    ((LastName ?? String.Empty).Trim() == (LastNameEn ?? String.Empty).Trim())
                    || ((FirstName ?? String.Empty).Trim() == (FirstNameEn ?? String.Empty).Trim())
                    || ((MiddleName ?? String.Empty).Trim() == (MiddleNameEn ?? String.Empty).Trim());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullNameCorrected
        {
            get
            {
                var sb = new StringBuilder();

                if (!String.IsNullOrEmpty(LastName))
                {
                    sb.Append(LastName);
                }


                if (!String.IsNullOrEmpty(FirstName))
                {
                    sb.AppendFormat(sb.Length > 0 ? " {0}" : "{0}", FirstName);
                }

                if (!String.IsNullOrEmpty(MiddleName))
                {
                    sb.AppendFormat(sb.Length > 0 ? " {0}" : "{0}", MiddleName);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullNameEnCorrected
        {
            get
            {
                var sb = new StringBuilder();

                if (!String.IsNullOrEmpty(LastNameEn))
                {
                    sb.Append(LastNameEn);
                }

                if (!String.IsNullOrEmpty(FirstNameEn))
                {
                    sb.AppendFormat((sb.Length > 0) ? " {0}" : "{0}", FirstNameEn);
                }

                if (!String.IsNullOrEmpty(MiddleNameEn))
                {
                    sb.AppendFormat(sb.Length > 0 ? " {0}" : "{0}", MiddleNameEn);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     Аксессор к лицу заказчику сотрудника
        /// </summary>
        public PersonOld Organization
        {
            get
            {
                if (!OrganizationId.HasValue)
                    _organization = null;
                else
                {
                    if (RequiredRefreshInfo || _organization == null || _organization.Unavailable ||
                        !_organization.Id.Equals(OrganizationId.ToString()))
                        _organization = new PersonOld(OrganizationId.ToString());
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
                    _personEmployee = null;
                else
                {
                    if (RequiredRefreshInfo || _personEmployee == null || _personEmployee.Unavailable ||
                        !_personEmployee.Id.Equals(PersonEmployeeId.ToString()))
                        _personEmployee = new PersonOld(PersonEmployeeId.ToString());
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
                if (RequiredRefreshInfo || _employer == null || _employer.Unavailable)
                {
                    var dt = GetUserPositions(CombineType.ОсновноеМестоРаботы);
                    if (dt.Rows.Count == 1)
                        _employer = new PersonOld(dt.Rows[0]["КодЛица"].ToString());
                    else
                        _employer = null;
                }
                return _employer;
            }
        }

        /// <summary>
        ///     Список рабочих мест сотрудника
        /// </summary>
        public List<EmployeeWorkPlace> Workplaces
        {
            get
            {
                if (RequiredRefreshInfo || _workPlaces == null)
                {
                    _workPlaces = new List<EmployeeWorkPlace>();
                    var sqlParams = new Dictionary<string, object> {{"@Id", int.Parse(Id)}};
                    using (
                        var dbReader = new DBReader(SQLQueries.SELECT_РабочиеМестаСотрудника, CommandType.Text, CN,
                            sqlParams))
                    {
                        if (dbReader.HasRows)
                        {
                            while (dbReader.Read())
                            {
                                var wp = new EmployeeWorkPlace();
                                wp.LoadFromDbReader(dbReader);
                                _workPlaces.Add(wp);
                            }
                        }
                    }
                }
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
                if (RequiredRefreshInfo || _photos == null)
                {
                    _photos = new List<EmployeePhoto>();
                    var sqlParams = new Dictionary<string, object> {{"@Id", int.Parse(Id)}};
                    using (
                        var dbReader = new DBReader(SQLQueries.SELECT_ФотографииСотрудника, CommandType.Text, CN,
                            sqlParams))
                    {
                        if (dbReader.HasRows)
                        {
                            while (dbReader.Read())
                            {
                                var ph = new EmployeePhoto();
                                ph.LoadFromDbReader(dbReader);
                                _photos.Add(ph);
                            }
                        }
                    }
                }
                return _photos;
            }
        }

        /// <summary>
        ///     Общие папки сотрудника
        /// </summary>
        public List<CommonFolder> CommonFolders
        {
            get
            {
                if (RequiredRefreshInfo || _commonFolders == null)
                {
                    _commonFolders = new List<CommonFolder>();
                    var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", int.Parse(Id)}};
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

                            //Кривая структура процедуры, которая возвращает два несвязанных между собой рекордсета!!!
                            if (dbReader.NextResult())
                            {
                                if (dbReader.HasRows)
                                {
                                    var colVPN = dbReader.GetOrdinal("VPN");
                                    var colInternet = dbReader.GetOrdinal("Internet");
                                    var colDialUp = dbReader.GetOrdinal("DialUp");

                                    while (dbReader.Read())
                                    {
                                        if (!dbReader.IsDBNull(colVPN))
                                        {
                                            IsVPNGroup = dbReader.GetByte(colVPN).Equals(1);
                                        }
                                        if (!dbReader.IsDBNull(colInternet))
                                        {
                                            IsInternetGroup = dbReader.GetByte(colInternet).Equals(1);
                                        }
                                        if (!dbReader.IsDBNull(colDialUp))
                                        {
                                            IsDialUpGroup = dbReader.GetByte(colDialUp).Equals(1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return _commonFolders;
            }
        }

        /// <summary>
        ///     Роли сотрудника
        /// </summary>
        public List<EmployeeRole> Roles
        {
            get
            {
                if (!(RequiredRefreshInfo || _roles == null)) return _roles;

                _roles = new List<EmployeeRole>();
                var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", int.Parse(Id)}};
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_РолиСотрудника, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                    {
                        while (dbReader.Read())
                        {
                            var tempUserRole = new EmployeeRole();
                            tempUserRole.LoadFromDbReader(dbReader);
                            _roles.Add(tempUserRole);
                        }
                    }
                }

                return _roles;
            }
        }

        /// <summary>
        ///     Типы сотрудника
        /// </summary>
        public List<EmployeePersonType> Types
        {
            get
            {
                if (!(RequiredRefreshInfo || _types == null)) return _types;

                _types = new List<EmployeePersonType>();
                var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", int.Parse(Id)}};
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ПраваТипыЛицСотрудника, CommandType.Text, Config.DS_person,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                    {
                        while (dbReader.Read())
                        {
                            var tempPersonType = new EmployeePersonType();
                            tempPersonType.LoadFromDbReader(dbReader);
                            _types.Add(tempPersonType);
                        }
                    }
                }

                return _types;
            }
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

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return (from DataRow row in dt.Rows select (int) row["КодСотрудника"]).ToList();
        }
        
        /// <summary>
        /// Загрузка информации о сотруднике
        /// </summary>
        public override void Load()
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Сотрудник, EmployeeId, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                    var colФИО = dbReader.GetOrdinal("ФИО");
                    var colFIO = dbReader.GetOrdinal("FIO");
                    var colКодЛица = dbReader.GetOrdinal("КодЛица");
                    var colКодЛицаЗаказчика = dbReader.GetOrdinal("КодЛицаЗаказчика");
                    
                    var colЯзык = dbReader.GetOrdinal("Язык");
                    var colLastName = dbReader.GetOrdinal("Фамилия");
                    var colFirstName = dbReader.GetOrdinal("Имя");
                    var colMiddleName = dbReader.GetOrdinal("Отчество");
                    var colLastNameEn = dbReader.GetOrdinal("LastName");
                    var colFirstNameEn = dbReader.GetOrdinal("FirstName");
                    var colMiddleNameEn = dbReader.GetOrdinal("MiddleName");
                    var colFullName = dbReader.GetOrdinal("Сотрудник");
                    var colInitialsWithLastName = dbReader.GetOrdinal("ИОФ");
                    var colFullNameEn = dbReader.GetOrdinal("Employee");
                    var colInitialsWithLastNameEn = dbReader.GetOrdinal("IOF");
                    var colStatus = dbReader.GetOrdinal("Состояние");
                    var colGuid = dbReader.GetOrdinal("GUID");
                    var colLogin = dbReader.GetOrdinal("Login");
                    var colLoginFull = dbReader.GetOrdinal("LoginFull");
                    var colAccountDisabled = dbReader.GetOrdinal("AccountDisabled");
                    var colDisplayName = dbReader.GetOrdinal("DisplayName");
                    var colEmail = dbReader.GetOrdinal("Email");
                    var colPersonalFolder = dbReader.GetOrdinal("ЛичнаяПапка");
                    var colAccess = dbReader.GetOrdinal("Access");
                    var colGender = dbReader.GetOrdinal("Пол");
                    var colComments = dbReader.GetOrdinal("Примечания");
                    var colLastNameRL = dbReader.GetOrdinal("ФамилияRL");
                    var colFirstNameRL = dbReader.GetOrdinal("ИмяRL");
                    var colMiddleNameRL = dbReader.GetOrdinal("ОтчествоRL");
                    var colCommonEmployeeID = dbReader.GetOrdinal("КодОбщегоСотрудника");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();
                        FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colФИО))
                        {
                            FIO = dbReader.GetString(colФИО);
                        }
                        if (!dbReader.IsDBNull(colFIO))
                        {
                            FIOEn = dbReader.GetString(colFIO);
                        }
                        if (!dbReader.IsDBNull(colКодЛица))
                        {
                            PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        }
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                        {
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);
                        }
                       
                        Name = dbReader.GetString(colLoginFull);
                        Language = dbReader.GetString(colЯзык);
                        if (!dbReader.IsDBNull(colИзменено)) Changed = dbReader.GetDateTime(colИзменено);
                        if (!dbReader.IsDBNull(colИзменил)) ChangedBy = dbReader.GetInt32(colИзменил);

                        if (!dbReader.IsDBNull(colLastName)) LastName = dbReader.GetString(colLastName);
                        if (!dbReader.IsDBNull(colFirstName)) FirstName = dbReader.GetString(colFirstName);
                        if (!dbReader.IsDBNull(colMiddleName)) MiddleName = dbReader.GetString(colMiddleName);
                        if (!dbReader.IsDBNull(colLastNameEn)) LastNameEn = dbReader.GetString(colLastNameEn);
                        if (!dbReader.IsDBNull(colFirstNameEn)) FirstNameEn = dbReader.GetString(colFirstNameEn);
                        if (!dbReader.IsDBNull(colMiddleNameEn)) MiddleNameEn = dbReader.GetString(colMiddleNameEn);
                        if (!dbReader.IsDBNull(colFullName)) FullName = dbReader.GetString(colFullName);
                        if (!dbReader.IsDBNull(colInitialsWithLastName))
                            InitialsWithLastName = dbReader.GetString(colInitialsWithLastName);
                        if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);
                        if (!dbReader.IsDBNull(colInitialsWithLastNameEn))
                            InitialsWithLastNameEn = dbReader.GetString(colInitialsWithLastNameEn);
                        if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);
                        if (!dbReader.IsDBNull(colGuid)) Guid = dbReader.GetGuid(colGuid);
                        if (!dbReader.IsDBNull(colLogin)) Login = dbReader.GetString(colLogin);
                        if (!dbReader.IsDBNull(colAccountDisabled))
                            AccountDisabled = dbReader.GetByte(colAccountDisabled);
                        if (!dbReader.IsDBNull(colDisplayName)) DisplayName = dbReader.GetString(colDisplayName);
                        if (!dbReader.IsDBNull(colEmail)) Email = dbReader.GetString(colEmail);
                        if (!dbReader.IsDBNull(colPersonalFolder))
                            PersonalFolder = dbReader.GetString(colPersonalFolder);
                        if (!dbReader.IsDBNull(colAccess)) Access = dbReader.GetString(colAccess);
                        if (!dbReader.IsDBNull(colGender)) Gender = dbReader.GetString(colGender);
                        if (!dbReader.IsDBNull(colComments)) Comments = dbReader.GetString(colComments);
                        if (!dbReader.IsDBNull(colLastNameRL)) LastNameRL = dbReader.GetString(colLastNameRL);
                        if (!dbReader.IsDBNull(colFirstNameRL)) FirstNameRL = dbReader.GetString(colFirstNameRL);
                        if (!dbReader.IsDBNull(colMiddleNameRL)) MiddleNameRL = dbReader.GetString(colMiddleNameRL);
                        if (!dbReader.IsDBNull(colCommonEmployeeID))
                            CommonEmployeeID = dbReader.GetInt32(colCommonEmployeeID).ToString();
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// Заполнение полей объекта через DBReader
       /// </summary>
        /// <param name="dbReader">V4.DBReader</param>
       /// <param name="loadFromOutSource">Заполнять минимальное количество полей</param>
        public void LoadFromDbReader(DBReader dbReader, bool loadFromOutSource = false)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца

                var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                var colСотрудник = dbReader.GetOrdinal("Сотрудник");
                var colФИО = dbReader.GetOrdinal("ФИО");
                var colFIO = dbReader.GetOrdinal("FIO");
                var colКодЛица = dbReader.GetOrdinal("КодЛица");
                var colЯзык = dbReader.GetOrdinal("Язык");
                var colLastName = dbReader.GetOrdinal("Фамилия");
                var colFirstName = dbReader.GetOrdinal("Имя");
                var colMiddleName = dbReader.GetOrdinal("Отчество");
                var colLastNameEn = dbReader.GetOrdinal("LastName");
                var colFirstNameEn = dbReader.GetOrdinal("FirstName");
                var colMiddleNameEn = dbReader.GetOrdinal("MiddleName");
                var colFullName = dbReader.GetOrdinal("Сотрудник");
                var colInitialsWithLastName = dbReader.GetOrdinal("ИОФ");
                var colFullNameEn = dbReader.GetOrdinal("Employee");
                var colInitialsWithLastNameEn = dbReader.GetOrdinal("IOF");
                var colStatus = dbReader.GetOrdinal("Состояние");
                var colGuid = dbReader.GetOrdinal("GUID");
                var colLogin = dbReader.GetOrdinal("Login");
                var colLoginFull = dbReader.GetOrdinal("LoginFull");
                var colAccountDisabled = dbReader.GetOrdinal("AccountDisabled");
                var colDisplayName = dbReader.GetOrdinal("DisplayName");
                var colEmail = dbReader.GetOrdinal("Email");
                var colPersonalFolder = dbReader.GetOrdinal("ЛичнаяПапка");
                var colAccess = dbReader.GetOrdinal("Access");
                var colGender = dbReader.GetOrdinal("Пол");
                var colComments = dbReader.GetOrdinal("Примечания");
                var colLastNameRL = dbReader.GetOrdinal("ФамилияRL");
                var colFirstNameRL = dbReader.GetOrdinal("ИмяRL");
                var colMiddleNameRL = dbReader.GetOrdinal("ОтчествоRL");
                var colCommonEmployeeID = dbReader.GetOrdinal("КодОбщегоСотрудника");
                var colEmployerID = dbReader.GetOrdinal("КодЛицаЗаказчика");
                var colИзменил = dbReader.GetOrdinal("Изменил");
                var colИзменено = dbReader.GetOrdinal("Изменено");

                #endregion

                if (!loadFromOutSource)
                {
                    if (dbReader.Read())
                    {
                        Unavailable = false;

                        Id = dbReader.GetInt32(colКодСотрудника).ToString();
                        FullName = dbReader.GetString(colСотрудник);
                        if (!dbReader.IsDBNull(colФИО))
                        {
                            FIO = dbReader.GetString(colФИО);
                        }
                        if (!dbReader.IsDBNull(colFIO))
                        {
                            FIOEn = dbReader.GetString(colFIO);
                        }
                        if (!dbReader.IsDBNull(colКодЛица))
                        {
                            PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        }
                        Name = dbReader.GetString(colLoginFull);
                        Language = dbReader.GetString(colЯзык);
                    }
                }
                else if (loadFromOutSource)
                {
                    Unavailable = false;

                    Id = dbReader.GetInt32(colКодСотрудника).ToString();
                    FullName = dbReader.GetString(colСотрудник);
                    if (!dbReader.IsDBNull(colФИО))
                    {
                        FIO = dbReader.GetString(colФИО);
                    }
                    if (!dbReader.IsDBNull(colFIO))
                    {
                        FIOEn = dbReader.GetString(colFIO);
                    }
                    if (!dbReader.IsDBNull(colКодЛица))
                    {
                        PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                    }
                    Name = dbReader.GetString(colLoginFull);
                    Language = dbReader.GetString(colЯзык);
                    if (!dbReader.IsDBNull(colИзменено)) Changed = dbReader.GetDateTime(colИзменено);
                    if (!dbReader.IsDBNull(colИзменил)) ChangedBy = dbReader.GetInt32(colИзменил);

                    if (!dbReader.IsDBNull(colLastName)) LastName = dbReader.GetString(colLastName);
                    if (!dbReader.IsDBNull(colFirstName)) FirstName = dbReader.GetString(colFirstName);
                    if (!dbReader.IsDBNull(colMiddleName)) MiddleName = dbReader.GetString(colMiddleName);
                    if (!dbReader.IsDBNull(colLastNameEn)) LastNameEn = dbReader.GetString(colLastNameEn);
                    if (!dbReader.IsDBNull(colFirstNameEn)) FirstNameEn = dbReader.GetString(colFirstNameEn);
                    if (!dbReader.IsDBNull(colMiddleNameEn)) MiddleNameEn = dbReader.GetString(colMiddleNameEn);
                    if (!dbReader.IsDBNull(colFullName)) FullName = dbReader.GetString(colFullName);
                    if (!dbReader.IsDBNull(colInitialsWithLastName))
                        InitialsWithLastName = dbReader.GetString(colInitialsWithLastName);
                    if (!dbReader.IsDBNull(colFullNameEn)) FullNameEn = dbReader.GetString(colFullNameEn);
                    if (!dbReader.IsDBNull(colInitialsWithLastNameEn))
                        InitialsWithLastNameEn = dbReader.GetString(colInitialsWithLastNameEn);
                    if (!dbReader.IsDBNull(colStatus)) Status = dbReader.GetByte(colStatus);
                    if (!dbReader.IsDBNull(colGuid)) Guid = dbReader.GetGuid(colGuid);
                    if (!dbReader.IsDBNull(colLogin)) Login = dbReader.GetString(colLogin);
                    if (!dbReader.IsDBNull(colAccountDisabled)) AccountDisabled = dbReader.GetByte(colAccountDisabled);
                    if (!dbReader.IsDBNull(colDisplayName)) DisplayName = dbReader.GetString(colDisplayName);
                    if (!dbReader.IsDBNull(colEmail)) Email = dbReader.GetString(colEmail);
                    if (!dbReader.IsDBNull(colPersonalFolder)) PersonalFolder = dbReader.GetString(colPersonalFolder);
                    if (!dbReader.IsDBNull(colAccess)) Access = dbReader.GetString(colAccess);
                    if (!dbReader.IsDBNull(colGender)) Gender = dbReader.GetString(colGender);
                    if (!dbReader.IsDBNull(colComments)) Comments = dbReader.GetString(colComments);
                    if (!dbReader.IsDBNull(colLastNameRL)) LastNameRL = dbReader.GetString(colLastNameRL);
                    if (!dbReader.IsDBNull(colFirstNameRL)) FirstNameRL = dbReader.GetString(colFirstNameRL);
                    if (!dbReader.IsDBNull(colMiddleNameRL)) MiddleNameRL = dbReader.GetString(colMiddleNameRL);
                    if (!dbReader.IsDBNull(colCommonEmployeeID))
                        CommonEmployeeID = dbReader.GetString(colCommonEmployeeID);
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
                PersonEmployeeId = dt.Rows[0]["КодЛица"] == DBNull.Value ? null : (int?) dt.Rows[0]["КодЛица"];
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
                        if (!dbReader.IsDBNull(colФИО))
                        {
                            FIO = dbReader.GetString(colФИО);
                        }
                        if (!dbReader.IsDBNull(colEmployee))
                        {
                            FullNameEn = dbReader.GetString(colEmployee);
                        }
                        if (!dbReader.IsDBNull(colFIO))
                        {
                            FIOEn = dbReader.GetString(colFIO);
                        }
                        if (!dbReader.IsDBNull(colКодЛица))
                        {
                            PersonEmployeeId = dbReader.GetInt32(colКодЛица);
                        }
                        if (!dbReader.IsDBNull(colКодЛицаЗаказчика))
                        {
                            OrganizationId = dbReader.GetInt32(colКодЛицаЗаказчика);
                        }
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
                    return String.Empty;
            }
        }

        /// <summary>
        ///     Получение информации о руководителе сотрудника в разрезе указанной компании
        /// </summary>
        /// <returns>
        ///     DataTable[КодОрганизацииСотрудника, КодСотрудника, КодЛицаСотрудника, Сотрудник, ДолжностьСотрудника,
        ///     КодСотрудникаРуководителя, КодЛицаРуководителя, Руководитель, ДолжностьРуководителя]
        /// </returns>
        public DataTable GetSupervisorsData()
        {
            if (supervisorsData != null && !RequiredRefreshInfo) return supervisorsData;

            supervisorsData = new DataTable("SupervisorsData");

            var dr = supervisorsData.NewRow();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСотрудника", int.Parse(Id));


            supervisorsData = DBManager.GetData(SQLQueries.SELECT_НепосредственныйРуководитель, CN, CommandType.Text,
                sqlParams);

            return supervisorsData;
        }

        /// <summary>
        ///     Получение информации о должностях сотрудника
        /// </summary>
        /// <param name="combine">0-по основному месту работы, 1 - там, где сотрудник является совместителем</param>
        /// <returns>DataTable[КодДолжности, Должность, КодСотрудника, КодЛица]</returns>
        public DataTable GetUserPositions(CombineType combine)
        {
            var dt = new DataTable("UserPositions");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСотрудника", new object[] {Id, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@Совместитель", new object[] {(int) combine, DBManager.ParameterTypes.Int32});

            dt = DBManager.GetData(SQLQueries.SELECT_ДолжностиСотрудника, CN, CommandType.Text, sqlParams);

            return dt;
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
            sqlParams.Add("@КодСотрудника", new object[] {subordinateUserId, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@КодРуководителя", new object[] {int.Parse(Id), DBManager.ParameterTypes.Int32});

            dt = DBManager.GetData(SQLQueries.SELECT_ПроверкаПодчиненияСотрудника, CN, CommandType.Text, sqlParams);
            return dt.Rows.Count > 0;
        }

        /// <summary>
        ///     Получение списка сотрудников, которые работатют на указанном рабочем месте
        /// </summary>
        /// <param name="wp">КодРасположения</param>
        /// <returns>Список сотрудников</returns>
        public List<Employee> EmployeesOnWorkPlace(int wp)
        {
            _employeesOnWorkPlace = new List<Employee>();
            var sqlParams = new Dictionary<string, object> {{"@Id", wp}, {"@idEmpl", int.Parse(Id)}, {"@state", 0}};
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_СотрудникиНаРабочемМесте, CommandType.Text, CN,
                    sqlParams))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var empl = new Employee(false);
                        empl.LoadFromDbReader(dbReader, true);
                        _employeesOnWorkPlace.Add(empl);
                    }
                }
            }

            return _employeesOnWorkPlace;
        }
    }
}