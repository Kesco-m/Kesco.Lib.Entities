using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Resources;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Corporate.Phones;
using Kesco.Lib.Entities.Persons.Contacts;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс документа Указание на организацию рабочего места
    /// </summary>
    [Serializable]
    public class Direction : Document, IDocumentWithPositions
    {
       
        private List<AdvancedGrant> _advancedGrants;
        private List<AdvancedGrant> _advancedGrantsAvailable;
        private List<CommonFolder> _commonFolders;
        private List<DomainName> _domainNames;


        private List<Language> _languages;
        private Location _locationWorkPlace;
        private Location _locationWorkPlaceTo;

        private Employee _sotrudnik;
        private Employee _sotrudnikParent;
        
        private DataTable _supervisorData;

        private DataTable _groupInconsistencies;

        /// <summary>
        /// Акксессор к ресурсам, относящимся к данному объекту
        /// </summary>
        public ResourceManager Resx = Localization.Resources.ResxDocsDirection;

        /// <summary>
        /// Признак изменялся ли доступ к корпоративной сети
        /// </summary>
        public bool AccessEthernetChange { get; set; } = false;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Direction()
        {
            Type = DocTypeEnum.УказаниеВОтделИтНаОрганизациюРабочегоМеста;

            SotrudnikField = GetDocField("1390");
            
            RedirectNumField = GetDocField("1397");
            
            WorkPlaceTypeField = GetDocField("1809");
            WorkPlaceField = GetDocField("1394");
            WorkPlaceToField = GetDocField("1822");
            
            PhoneEquipField = GetDocField("1395");
            PhoneLinkField = GetDocField("1396");

            CompTypeField = GetDocField("1399");
            AdvEquipField = GetDocField("1400");
            AccessEthernetField = GetDocField("1402");
            LoginField = GetDocField("1403");

            AdvInfoField = GetDocField("1410");
            SotrudnikParentCheckField = GetDocField("1412");
            SotrudnikParentField = GetDocField("1413");
            MailNameField = GetDocField("1418");
            DomainField = GetDocField("1419");
            
            SotrudnikLanguageField = GetDocField("1426");

            SotrudnikPersonField = GetDocField("1470");
            PersonZakazchikField = GetDocField("1788");
            PersonEmployerField = GetDocField("1789");
            PersonEmployerHeadField = GetDocField("1790");

            OsnovanieLinks = new List<DocLink>();

            PositionCommonFoldersField = GetDocField("1404");
            PositionAdvancedGrantsField = GetDocField("1808");
            PositionRolesField = GetDocField("1405");
            PositionTypesField = GetDocField("1406");
        }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        public DocField SotrudnikField { get; }
        
        
        /// <summary>
        ///     Мобильный телефон сотрудника
        /// </summary>
        public DocField RedirectNumField { get; }

        /// <summary>
        ///     Организовать сотруднику
        /// </summary>
        public DocField WorkPlaceTypeField { get; }

        /// <summary>
        ///     Организовать рабочее место
        /// </summary>
        public DocField WorkPlaceField { get; }

        /// <summary>
        ///     Организовать переезд на рабочее место
        /// </summary>
        public DocField WorkPlaceToField { get; }

        /// <summary>
        ///     Телефон
        /// </summary>
        public DocField PhoneEquipField { get; }

        /// <summary>
        ///     Телефонная связь
        /// </summary>
        public DocField PhoneLinkField { get; }

        /// <summary>
        ///     Компьютер
        /// </summary>
        public DocField CompTypeField { get; }

        /// <summary>
        ///     Дополнительное оборудование на рабочем месте
        /// </summary>
        public DocField AdvEquipField { get; }

        /// <summary>
        ///     Доступ к корпоративной сети
        /// </summary>
        public DocField AccessEthernetField { get; }

        /// <summary>
        ///     Логин
        /// </summary>
        public DocField LoginField { get; }

        /// <summary>
        ///     Доп. информация по организации рабочего места
        /// </summary>
        public DocField AdvInfoField { get; }

        /// <summary>
        ///     Вид доступа к данным
        /// </summary>
        public DocField SotrudnikParentCheckField { get; }

        /// <summary>
        ///     Вместо, как у сотрудника
        /// </summary>
        public DocField SotrudnikParentField { get; }

        /// <summary>
        ///     E-mail
        /// </summary>
        public DocField MailNameField { get; }

        /// <summary>
        ///     Domain
        /// </summary>
        public DocField DomainField { get; }

        
        /// <summary>
        ///     Предпочитаемый язык
        /// </summary>
        public DocField SotrudnikLanguageField { get; }

        /// <summary>
        ///     Код лица сотрудника
        /// </summary>
        public DocField SotrudnikPersonField { get; }

        /// <summary>
        ///     Код лица заказчика
        /// </summary>
        public DocField PersonZakazchikField { get; }

        /// <summary>
        ///     Код лица работодателя
        /// </summary>
        public DocField PersonEmployerField { get; }

        /// <summary>
        ///     Код лица работодателя руководителя
        /// </summary>
        public DocField PersonEmployerHeadField { get; }

       
        /// <summary>
        ///     Доступ к общим папкам
        /// </summary>
        public DocField PositionCommonFoldersField { get; }

        /// <summary>
        ///     Дополнительные права
        /// </summary>
        public DocField PositionAdvancedGrantsField { get; }

        /// <summary>
        ///     Выполняемые роли
        /// </summary>
        public DocField PositionRolesField { get; }

        /// <summary>
        ///     Доступ к типам лиц
        /// </summary>
        public DocField PositionTypesField { get; }

        
        /// <summary>
        ///     Список документов
        /// </summary>
        public List<DocLink> OsnovanieLinks { get; set; }


        /// <summary>
        ///     Возвращает объект User по значению поля SotrudnikField
        /// </summary>
        public Employee Sotrudnik
        {
            get
            {
                if (SotrudnikField.ValueString.Length == 0)
                {
                    _sotrudnik = null;
                }
                else
                {
                    if (_sotrudnik == null || _sotrudnik.Unavailable ||
                        !_sotrudnik.Id.Equals(SotrudnikField.ValueString) ||
                        !LoadedExternalProperties.ContainsKey(CacheKey_Sotrudnik))
                    {
                        _sotrudnik = new Employee(SotrudnikField.ValueString);
                        AddLoadedExternalProperties(CacheKey_Sotrudnik);
                    }
                }

                return _sotrudnik;
            }
        }

        /// <summary>
        ///     Возвращает объект User по значению поля SotrudnikParentField
        /// </summary>
        public Employee SotrudnikParent
        {
            get
            {
                if (SotrudnikParentField.ValueString.Length == 0)
                {
                    _sotrudnikParent = null;
                }
                else
                {
                    if (_sotrudnikParent == null || _sotrudnikParent.Unavailable ||
                        !_sotrudnikParent.Id.Equals(SotrudnikParentField.ValueString) ||
                        !LoadedExternalProperties.ContainsKey(CacheKey_SotrudnikParent))
                    {
                        _sotrudnikParent = new Employee(SotrudnikParentField.ValueString);
                        AddLoadedExternalProperties(CacheKey_SotrudnikParent);
                    }
                }

                return _sotrudnikParent;
            }
        }

       

        /// <summary>
        ///     Данные руководителя сотрудника
        /// </summary>
        public DataTable SupervisorData
        {
            get
            {
                if (SotrudnikField.ValueString.Length == 0)
                {
                    _supervisorData = null;
                }

                else
                {
                    if (_supervisorData != null && 
                        LoadedExternalProperties.ContainsKey(CacheKey_SupervisorData))
                        return _supervisorData;

                    _supervisorData = Sotrudnik.SupervisorsData();
                    AddLoadedExternalProperties(CacheKey_SupervisorData);
                }

                return _supervisorData;
            }
        }

        /// <summary>
        ///     Возвращает объект Location по значению поля LocationWorkPlaceField
        /// </summary>
        public Location LocationWorkPlace
        {
            get
            {
                if (WorkPlaceField.ValueString.Length == 0)
                {
                    _locationWorkPlace = null;
                }
                else
                {
                    if (_locationWorkPlace == null || _locationWorkPlace.Unavailable ||
                        !_locationWorkPlace.Id.Equals(WorkPlaceField.ValueString) ||
                        !LoadedExternalProperties.ContainsKey(CacheKey_LocationWorkPlace))
                    {
                        _locationWorkPlace = new Location(WorkPlaceField.ValueString);
                        AddLoadedExternalProperties(CacheKey_LocationWorkPlace);
                    }
                }

                return _locationWorkPlace;
            }
        }

        /// <summary>
        ///     Возвращает объект Location по значению поля LocationWorkPlaceField
        /// </summary>
        public Location LocationWorkPlaceTo
        {
            get
            {
                if (WorkPlaceToField.ValueString.Length == 0)
                {
                    _locationWorkPlaceTo = null;
                }
                else
                {
                    if (_locationWorkPlaceTo == null || _locationWorkPlaceTo.Unavailable ||
                        !_locationWorkPlaceTo.Id.Equals(WorkPlaceToField.ValueString) ||
                        !LoadedExternalProperties.ContainsKey(CacheKey_LocationWorkPlaceTo))
                    {
                        _locationWorkPlaceTo = new Location(WorkPlaceToField.ValueString);
                        AddLoadedExternalProperties(CacheKey_LocationWorkPlaceTo);
                    }
                }

                return _locationWorkPlaceTo;
            }
        }

        /// <summary>
        ///     Получение списка языков
        /// </summary>
        /// <returns></returns>
        public List<Language> Languages
        {
            get
            {
                if (_languages != null && LoadedExternalProperties.ContainsKey(CacheKey_Languages))
                    return _languages;

                _languages = new List<Language>();
                using (var dbReader = new DBReader(SQLQueries.SELECT_Языки, CommandType.Text, Config.DS_user))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var lang = new Language();
                            lang.LoadFromDbReader(dbReader);
                            _languages.Add(lang);
                        }
                }

                AddLoadedExternalProperties(CacheKey_Languages);
                return _languages;
            }
        }

        /// <summary>
        ///     Получение доменных имен
        /// </summary>
        /// <returns></returns>
        public List<DomainName> DomainNames
        {
            get
            {
                if (_domainNames != null && LoadedExternalProperties.ContainsKey(CacheKey_DomainNames))
                    return _domainNames;

                _domainNames = new List<DomainName>();
                using (var dbReader = new DBReader(SQLQueries.SELECT_DomainNames, CommandType.Text, Config.DS_user))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var dname = new DomainName();
                            dname.LoadFromDbReader(dbReader);
                            _domainNames.Add(dname);
                        }
                }

                AddLoadedExternalProperties(CacheKey_DomainNames);
                return _domainNames;
            }
        }

        /// <summary>
        ///     Получение списка общих папок
        /// </summary>
        /// <returns></returns>
        public List<CommonFolder> CommonFolders
        {
            get
            {
                if (_commonFolders != null && LoadedExternalProperties.ContainsKey(CacheKey_CommonFolders))
                    return _commonFolders;

                _commonFolders = new List<CommonFolder>();
                using (var dbReader = new DBReader(SQLQueries.SELECT_CommonFolders, CommandType.Text, Config.DS_user))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var cf = new CommonFolder();
                            cf.LoadFromDbReader(dbReader);
                            _commonFolders.Add(cf);
                        }
                }

                AddLoadedExternalProperties(CacheKey_CommonFolders);
                return _commonFolders;
            }
        }

        /// <summary>
        ///     Получение дополнительных прав
        /// </summary>
        /// <returns></returns>
        public List<AdvancedGrant> AdvancedGrants
        {
            get
            {
                if (_advancedGrants != null && LoadedExternalProperties.ContainsKey(CacheKey_AdvancedGrants))
                    return _advancedGrants;

                _advancedGrants = new List<AdvancedGrant>();
                using (var dbReader = new DBReader(SQLQueries.SELECT_AdvancedGrants, CommandType.Text, Config.DS_user))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var cf = new AdvancedGrant();
                            cf.LoadFromDbReader(dbReader);
                            _advancedGrants.Add(cf);
                        }
                }

                AddLoadedExternalProperties(CacheKey_AdvancedGrants);
                return _advancedGrants;
            }
        }

        /// <summary>
        ///     Список дополнительных прав для вывода на форме
        /// </summary>
        /// <returns>Список объектов</returns>
        public List<AdvancedGrant> AdvancedGrantsAvailable
        {
            get
            {
                if (_advancedGrantsAvailable != null &&
                    LoadedExternalProperties.ContainsKey(CacheKey_AdvancedGrantsAvailable))
                    return _advancedGrantsAvailable;

                _advancedGrantsAvailable = new List<AdvancedGrant>();

                PositionAdvancedGrants.ForEach(delegate(PositionAdvancedGrant p)
                {
                    _advancedGrantsAvailable.Add(new AdvancedGrant
                        {Id = p.GrantId.ToString(), Name = p.GrantDescription, NameEn = p.GrantDescriptionEn});
                });

                AdvancedGrants.ForEach(delegate(AdvancedGrant grant)
                {
                    var g = _advancedGrantsAvailable.FirstOrDefault(x => x.Id == grant.Id);
                    if (g != null)
                    {
                        g.TaskChoose = grant.TaskChoose;
                        g.TaskDefault = grant.TaskDefault;
                        g.OrderOutput = grant.OrderOutput;
                        g.RefersTo = grant.RefersTo;
                    }
                    else if (grant.TaskChoose>0 && WorkPlaceTypeField.ValueInt > 0)
                    {
                        var bit = (int)Math.Pow(2, WorkPlaceTypeField.ValueInt - 1);
                        if ((grant.TaskChoose & bit) == bit) 
                            _advancedGrantsAvailable.Add(grant);
                    }
                });

                AddLoadedExternalProperties(CacheKey_AdvancedGrantsAvailable);
                return _advancedGrantsAvailable;
            }
        }
                        /// <summary>
        ///     Сохранение позиций документа
        /// </summary>
        /// <param name="reloadPostions"></param>
        /// <param name="cmds"></param>
        public void SaveDocumentPositions(bool reloadPostions, List<DBCommand> cmds = null)
        {
            if (cmds == null && Id.IsNullEmptyOrZero())
                throw new LogicalException(
                    "Ошибка вызова процедур сохранения позиций документа, т.к. документ еще не сохранен!", "",
                    Assembly.GetExecutingAssembly().GetName(),
                    MethodBase.GetCurrentMethod().Name, Priority.Error);
            SavePositionsCommonFolders(reloadPostions, cmds);
            SavePositionsRoles(reloadPostions, cmds);
            SavePositionsTypes(reloadPostions, cmds);
            SavePositionsAdvancedGrants(reloadPostions, cmds);
        }

        /// <summary>
        ///     Загрузка из БД позиций документа
        /// </summary>
        public void LoadDocumentPositions()
        {
            LoadPositionCommonFolders();
            LoadPositionRoles();
            LoadPositionTypes();
            LoadPositionAdvancedGrants();
        }

        /// <summary>
        ///     Получение сотрудников кроме указанного, имееющих рабочее место на указанном расположении
        /// </summary>
        public List<Employee> GetOtherUsersOnLocation()
        {
            var userList = new List<Employee>();
            if (SotrudnikField.ValueString.Length == 0 || WorkPlaceField.ValueString.Length == 0)
                return userList;

            var sqlParams = new Dictionary<string, object>
            {
                {"@КодСотрудника", SotrudnikField.ValueInt},
                {"@Кодрасположения", WorkPlaceField.ValueInt}
            };
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ДругиеСотрудникиНаРасположении, CommandType.Text,
                    Config.DS_user, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var user = new Employee(false);
                        user.LoadFromDbReader(dbReader, true);
                        userList.Add(user);
                    }
            }

            return userList;
        }

        /// <summary>
        ///     Форматирование номера мобильно телефона
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <returns>Направление</returns>
        public string FormatingMobilNumber(ref string phoneNumber)
        {
            if (phoneNumber.Length == 0) return "";
           
            var area = new AreaPhoneInfo();
            Phone.AdjustPhoneNumber(ref area, ref phoneNumber);

            if (area == null) return "";

            phoneNumber = Contact.FormatingContact(22, area.CountryPhoneCode, area.PhoneCodeInCountry, phoneNumber, "");

            return area.Direction;
        }

        /// <summary>
        ///     Получение списка оборудования по данным текущего указания
        /// </summary>
        /// <returns>DataTable с оборудованием</returns>
        public DataTable GetDirectionEquipment()
        {
            var dt = new DataTable("Equipments");
            if (IsNew) return dt;
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодДокумента", int.Parse(Id)}
            };
            dt = DBManager.GetData(SQLQueries.SP_ВыданноеПоУказаниюОборудование, Config.DS_user,
                CommandType.StoredProcedure, sqlParams);

            return dt;
        }

        /// <summary>
        /// Получение несоответствий в правах в группе посмненной работы
        /// </summary>
        public DataTable GetAccessGroupInconsistencies
        {
            get
            {

                if (LoadedExternalProperties.ContainsKey("direction._groupInconsistencies"))
                    return _groupInconsistencies;
                _groupInconsistencies = new DataTable();

                var sqlParams = new Dictionary<string, object>();
                sqlParams.Add("@КодСотрудника", SotrudnikField.Value);
                sqlParams.Add("@КодДокумента", int.Parse(Id));

                var personServer = new SqlConnection(Config.DS_person).DataSource;

                _groupInconsistencies = DBManager.GetData(string.Format(SQLQueries.SELECT_НесоотвествиеПравСотрудниковВГруппе,personServer), Config.DS_user,
                    CommandType.Text, sqlParams);

                LoadedExternalProperties.Add("direction._groupInconsistencies", DateTime.UtcNow);
                return _groupInconsistencies;
            }
        }

        #region Cache Keys

        /// <summary>
        ///     Ключ кэширования объекта Сотрудник
        /// </summary>
        public const string CacheKey_GroupInconsistencies = "direction._groupInconsistencies";

        /// <summary>
        ///     Ключ кэширования объекта Сотрудник
        /// </summary>
        public const string CacheKey_Sotrudnik = "direction._sotrudnik";

        /// <summary>
        ///     Ключ кэширования объекта Сотрудник вместо/как
        /// </summary>
        public const string CacheKey_SotrudnikParent = "direction._sotrudnikParent";

        /// <summary>
        ///     Ключ кэширования объекта Руководитель
        /// </summary>
        public const string CacheKey_Supervisor = "direction._supervisor";

        /// <summary>
        ///     Ключ кэширования объекта Данные руководителя
        /// </summary>
        public const string CacheKey_SupervisorData = "direction._supervisorData";

        /// <summary>
        ///     Ключ кэширования объекта Рабочее место
        /// </summary>
        public const string CacheKey_LocationWorkPlace = "direction._locationWorkPlace";

        /// <summary>
        ///     Ключ кэширования объекта Рабочее место на
        /// </summary>
        public const string CacheKey_LocationWorkPlaceTo = "direction._locationWorkPlaceTo";

        /// <summary>
        ///     Ключ кэширования объекта Языки
        /// </summary>
        public const string CacheKey_Languages = "direction._languages";

        /// <summary>
        ///     Ключ кэширования объекта Общие папки
        /// </summary>
        public const string CacheKey_CommonFolders = "direction._commonFolders";

        /// <summary>
        ///     Ключ кэширования объекта Дополнительные права
        /// </summary>
        public const string CacheKey_AdvancedGrants = "direction._advancedGrants";

        /// <summary>
        ///     Ключ кэширования объекта Доступные дополнительные права
        /// </summary>
        public const string CacheKey_AdvancedGrantsAvailable = "direction._advancedGrantsAvailable";

        /// <summary>
        ///     Ключ кэширования объекта DomainNames
        /// </summary>
        public const string CacheKey_DomainNames = "direction._domainNames";

        #endregion

        #region Позиции

        /// <summary>
        ///     Позиции документа: ПозицииУказанийИТРоли
        /// </summary>
        public List<PositionRole> PositionRoles { get; set; }

        /// <summary>
        ///     Позиции документа: ПозицииУказанийИТПапки
        /// </summary>
        public List<PositionCommonFolder> PositionCommonFolders { get; set; }

        /// <summary>
        ///     Позиции документа: ПозицииУказанийИТПрава
        /// </summary>
        public List<PositionAdvancedGrant> PositionAdvancedGrants { get; set; }

        /// <summary>
        ///     Позиции документа: ПозицииУказанийИТТипыЛиц
        /// </summary>
        public List<PositionType> PositionTypes { get; set; }

        #region Папки

        /// <summary>
        ///     Инициализация списка PositionCommonFolders и загрузка позиций ПозицииУказанийИТПапки из БД
        /// </summary>
        public void LoadPositionCommonFolders()
        {
            if (Id.IsNullEmptyOrZero())
                PositionCommonFolders = new List<PositionCommonFolder>();
            else
                PositionCommonFolders = DocumentPosition<PositionCommonFolder>.LoadByDocId(int.Parse(Id));
        }


        /// <summary>
        ///     Сохранение позици ПозицииУказанийИТПапки
        /// </summary>
        /// <param name="folders">Список выбранных папок</param>
        /// <param name="reloadPostions">Перезапросить данные о позициях</param>
        public void SavePositionsCommonFoldersByDictionary(Dictionary<string, string> folders, bool reloadPostions)
        {
            //Удаляем те позиции, которых нет в списке
            var positionForClear = new List<PositionCommonFolder>();
            foreach (var p in from p in PositionCommonFolders
                let f = folders.Where(x => x.Key == p.CommonFolderId.ToString())
                    .Select(x => (KeyValuePair<string, string>?) x)
                    .FirstOrDefault()
                where f == null
                select p)
                if (string.IsNullOrEmpty(p.Id))
                    positionForClear.Add(p);
                else
                    p.Delete(false);

            positionForClear.ForEach(
                delegate(PositionCommonFolder p) { PositionCommonFolders.RemoveAll(x => x.GuidId == p.GuidId); });

            //получаем сохраненные позиции
            var cfSaved = DocumentPosition<PositionCommonFolder>.LoadByDocId(int.Parse(Id));


            //сохраняем те выбранные элементы, для которых уже были созданы объекты PositionCommonFolders
            PositionCommonFolders.Where(x => string.IsNullOrEmpty(x.Id)).ToList().ForEach(
                delegate(PositionCommonFolder p)
                {
                    if (cfSaved.Count > 0)
                    {
                        var cf = cfSaved.FirstOrDefault(x => x.CommonFolderId == p.CommonFolderId);
                        if (cf != null) return;
                    }

                    p.DocumentId = int.Parse(Id);
                    p.Save(false);
                });

            //сохраняем те выбранные элементы, для которых не созданы объекты PositionCommonFolders
            foreach (var p in from f in folders
                let p = PositionCommonFolders.FirstOrDefault(x => x.CommonFolderId.ToString() == f.Key)
                where p == null
                select new PositionCommonFolder
                {
                    DocumentId = int.Parse(Id),
                    CommonFolderId = int.Parse(f.Key),
                    CommonFolderName = f.Value
                })
            {
                if (cfSaved.Count > 0)
                {
                    var cf = cfSaved.FirstOrDefault(x => x.CommonFolderId == p.CommonFolderId);
                    if (cf != null) continue;
                }

                p.Save(false);
                PositionCommonFolders.Add(p);
            }

            //удаляем те позиции, которых нет в PositionCommonFolders
            cfSaved.ForEach(delegate(PositionCommonFolder p)
            {
                var delP = PositionCommonFolders.FirstOrDefault(x => x.CommonFolderId == p.CommonFolderId);
                if (delP == null)
                    p.Delete(false);
            });

            if (reloadPostions) LoadPositionCommonFolders();
        }

        /// <summary>
        ///     Метод, вызываемый вместе с сохранением документа
        /// </summary>
        private void SavePositionsCommonFolders(bool reloadPostions, List<DBCommand> cmds)
        {
            var positionCommonFolders0 = DocumentPosition<PositionCommonFolder>.LoadByDocId(int.Parse(Id));

            positionCommonFolders0.ForEach(delegate(PositionCommonFolder p0)
            {
                var p = PositionCommonFolders.FirstOrDefault(x => x.Id == p0.Id);
                if (p == null)
                    p0.Delete(false);
            });

            PositionCommonFolders.ForEach(delegate(PositionCommonFolder p)
            {
                if (string.IsNullOrEmpty(p.Id))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }

                var p0 = positionCommonFolders0.FirstOrDefault(x => x.Id == p.Id && x.ChangedTime != p.ChangedTime);
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        #endregion

        #region Права

        /// <summary>
        ///     Инициализация списка PositionAdvancedGrants и загрузка позиций ПозицииУказанийИТПрава из БД
        /// </summary>
        public void LoadPositionAdvancedGrants()
        {
            if (Id.IsNullEmptyOrZero())
                PositionAdvancedGrants = new List<PositionAdvancedGrant>();
            else
                PositionAdvancedGrants = DocumentPosition<PositionAdvancedGrant>.LoadByDocId(int.Parse(Id));
        }

        /// <summary>
        ///     Сохранение позици ПозицииУказанийИТПрава
        /// </summary>
        /// <param name="grants">Список выбранных прав</param>
        /// <param name="reloadPostions">Перезапросить данные о позициях</param>
        public void SavePositionsAdvancedGrantsByDictionary(Dictionary<string, string> grants, bool reloadPostions)
        {
            foreach (var p in from p in PositionAdvancedGrants
                let f = grants.Where(x => x.Key == p.GrantId.ToString())
                    .Select(x => (KeyValuePair<string, string>?) x)
                    .FirstOrDefault()
                where f == null
                select p)
                p.Delete(false);

            foreach (var p in from f in grants
                let p = PositionAdvancedGrants.FirstOrDefault(x => x.GrantId.ToString() == f.Key)
                where p == null
                select new PositionAdvancedGrant
                {
                    DocumentId = int.Parse(Id),
                    GrantId = int.Parse(f.Key),
                    GrantDescription = f.Value
                })
            {
                var descrs = p.GrantDescription.Split('#');
                p.GrantDescription = descrs[0];
                p.GrantDescriptionEn = descrs[1];
                p.Save(false);
            }

            if (reloadPostions) LoadPositionAdvancedGrants();
        }

        /// <summary>
        ///     Метод, вызываемый вместе с сохранением документа
        /// </summary>
        private void SavePositionsAdvancedGrants(bool reloadPostions, List<DBCommand> cmds = null)
        {
            var positionAG0 = DocumentPosition<PositionAdvancedGrant>.LoadByDocId(int.Parse(Id));

            positionAG0.ForEach(delegate(PositionAdvancedGrant p0)
            {
                var p = PositionAdvancedGrants.FirstOrDefault(x => x.Id == p0.Id);
                if (p == null)
                    p0.Delete(false);
            });

            PositionAdvancedGrants.ForEach(delegate(PositionAdvancedGrant p)
            {
                if (string.IsNullOrEmpty(p.Id))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }

                var p0 = positionAG0.FirstOrDefault(x => x.Id == p.Id && x.ChangedTime != p.ChangedTime);
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        #endregion

        #region Роли

        /// <summary>
        ///     Инициализация списка PositionRoles и загрузка позиций ПозицииУказанийИТРоли из БД
        /// </summary>
        public void LoadPositionRoles()
        {
            if (Id.IsNullEmptyOrZero())
                PositionRoles = new List<PositionRole>();
            else
                PositionRoles = DocumentPosition<PositionRole>.LoadByDocId(int.Parse(Id));
        }

        /// <summary>
        ///     Метод, вызываемый вместе с сохранением документа
        /// </summary>
        private void SavePositionsRoles(bool reloadPostions, List<DBCommand> cmds = null)
        {
            var positionRoles0 = DocumentPosition<PositionRole>.LoadByDocId(int.Parse(Id));

            positionRoles0.ForEach(delegate(PositionRole p0)
            {
                var p = PositionRoles.FirstOrDefault(x => x.Id == p0.Id);
                if (p == null)
                    p0.Delete(false);
            });

            PositionRoles.ForEach(delegate(PositionRole p)
            {
                if (string.IsNullOrEmpty(p.Id))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }

                var p0 =
                    positionRoles0.FirstOrDefault(
                        x => x.Id == p.Id && (x.RoleId != p.RoleId || x.PersonId != p.PersonId));
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        #endregion

        #region ТипыЛиц

        /// <summary>
        ///     Инициализация списка PositionTypes и загрузка позиций ПозицииУказанийИТТипыЛиц из БД
        /// </summary>
        public void LoadPositionTypes()
        {
            if (Id.IsNullEmptyOrZero())
                PositionTypes = new List<PositionType>();
            else
                PositionTypes = DocumentPosition<PositionType>.LoadByDocId(int.Parse(Id));
        }

        /// <summary>
        ///     Метод, вызываемый вместе с сохранением документа
        /// </summary>
        private void SavePositionsTypes(bool reloadPostions, List<DBCommand> cmds = null)
        {
            var positionTypes0 = DocumentPosition<PositionType>.LoadByDocId(int.Parse(Id));

            positionTypes0.ForEach(delegate(PositionType p0)
            {
                var p = PositionTypes.FirstOrDefault(x => x.Id == p0.Id);
                if (p == null)
                    p0.Delete(false);
            });

            PositionTypes.ForEach(delegate(PositionType p)
            {
                if (string.IsNullOrEmpty(p.Id))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }

                var p0 = positionTypes0.FirstOrDefault(x => x.Id == p.Id && x.ChangedTime != p.ChangedTime);
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        #endregion

        #endregion
    }
}