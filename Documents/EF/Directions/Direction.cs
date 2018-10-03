using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
    public class Direction : Document, IDocumentWithPositions
    {
        private List<AdvancedGrant> _advancedGrants;
        private List<CommonFolder> _commonFolders;
        private List<DomainName> _domainNames;
        private List<Language> _languages;
        private Location _locationWorkPlace;
        private Employee _sotrudnik;
        private Employee _sotrudnikParent;
        private Employee _supervisor;
        private DataTable _supervisorData;

        public Direction()
        {
            Type = DocTypeEnum.УказаниеВОтделИтНаОрганизациюРабочегоМеста;

            SotrudnikField = GetDocField("1390");
            SotrudnikPostField = GetDocField("1391");
            SupervisorField = GetDocField("1392");

            RedirectNumField = GetDocField("1397");

            OsnovanieField = GetDocField("1407");
            OsnovanieBind = new BaseDocFacade(this, OsnovanieField);

            WorkPlaceTypeField = GetDocField("1809");
            WorkPlaceField = GetDocField("1394");


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
            DisplayNameField = GetDocField("1420");
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

        public DocField SotrudnikField { get; private set; }
        public DocField SotrudnikPostField { get; private set; }
        public DocField SupervisorField { get; private set; }
        public DocField RedirectNumField { get; private set; }
        public DocField WorkPlaceTypeField { get; private set; }
        public DocField WorkPlaceField { get; private set; }
        public DocField PhoneEquipField { get; private set; }
        public DocField PhoneLinkField { get; private set; }
        public DocField CompTypeField { get; private set; }
        public DocField AdvEquipField { get; private set; }
        public DocField AccessEthernetField { get; private set; }
        public DocField LoginField { get; private set; }
        public DocField AdvInfoField { get; private set; }
        public DocField SotrudnikParentCheckField { get; private set; }
        public DocField SotrudnikParentField { get; private set; }
        public DocField MailNameField { get; private set; }
        public DocField DomainField { get; private set; }
        public DocField DisplayNameField { get; private set; }
        public DocField SotrudnikLanguageField { get; private set; }
        public DocField SotrudnikPersonField { get; private set; }
        public DocField PersonZakazchikField { get; private set; }
        public DocField PersonEmployerField { get; private set; }
        public DocField PersonEmployerHeadField { get; private set; }
        public DocField OsnovanieField { get; private set; }

        public DocField PositionCommonFoldersField { get; private set; }
        public DocField PositionAdvancedGrantsField { get; private set; }
        public DocField PositionRolesField { get; private set; }
        public DocField PositionTypesField { get; private set; }

        public BaseDocFacade OsnovanieBind { get; private set; }
        public List<DocLink> OsnovanieLinks { get; set; }

       
        /// <summary>
        ///     Возвращает объект User по значению поля SotrudnikField
        /// </summary>
        public Employee Sotrudnik
        {
            get
            {
                if (SotrudnikField.ValueString.Length == 0)
                    _sotrudnik = null;
                else
                {
                    if (_sotrudnik == null || _sotrudnik.Unavailable ||
                        !_sotrudnik.Id.Equals(SotrudnikField.ValueString))
                        _sotrudnik = new Employee(SotrudnikField.ValueString);
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
                    _sotrudnikParent = null;
                else
                {
                    if (_sotrudnikParent == null || _sotrudnikParent.Unavailable ||
                        !_sotrudnikParent.Id.Equals(SotrudnikParentField.ValueString))
                        _sotrudnikParent = new Employee(SotrudnikParentField.ValueString);
                }

                return _sotrudnikParent;
            }
        }

        /// <summary>
        ///     Возвращает объект User по значению поля SupervisorField
        /// </summary>
        public Employee Supervisor
        {
            get
            {
                if (SupervisorField.ValueString.Length == 0)
                    _supervisor = null;
                else
                {
                    if (_supervisor == null || _supervisor.Unavailable ||
                        !_supervisor.Id.Equals(SupervisorField.ValueString))
                        _supervisor = new Employee(SupervisorField.ValueString);
                }

                return _supervisor;
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
                    return _supervisorData;
                }

                if (_supervisorData != null && SupervisorField.ValueString.Length > 0 && !RequiredRefreshInfo)
                    return _supervisorData;

                _supervisorData = Sotrudnik.GetSupervisorsData();

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
                    _locationWorkPlace = null;
                else
                {
                    if (_locationWorkPlace == null || _locationWorkPlace.Unavailable ||
                        !_locationWorkPlace.Id.Equals(WorkPlaceField.ValueString))
                        _locationWorkPlace = new Location(WorkPlaceField.ValueString);
                }

                return _locationWorkPlace;
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
            {
                throw new LogicalException(
                    "Ошибка вызова процедур сохранения позиций документа, т.к. документ еще не сохранен!", "",
                    Assembly.GetExecutingAssembly().GetName(),
                    MethodBase.GetCurrentMethod().Name, Priority.Error);
                return;
            }
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
                {
                    while (dbReader.Read())
                    {
                        var user = new Employee();
                        user.LoadFromDbReader(dbReader, true);
                        userList.Add(user);
                    }
                }
            }
            return userList;
        }

        /// <summary>
        ///     Получение списка языков
        /// </summary>
        /// <returns></returns>
        public List<Language> Languages()
        {
            if (_languages != null && !RequiredRefreshInfo) return _languages;
            _languages = new List<Language>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_Языки, CommandType.Text, Config.DS_user))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var lang = new Language();
                        lang.LoadFromDbReader(dbReader);
                        _languages.Add(lang);
                    }
                }
            }
            return _languages;
        }

        /// <summary>
        ///     Получение доменных имен
        /// </summary>
        /// <returns></returns>
        public List<DomainName> DomainNames()
        {
            if (_domainNames != null && !RequiredRefreshInfo) return _domainNames;
            _domainNames = new List<DomainName>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_DomainNames, CommandType.Text, Config.DS_user))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var dname = new DomainName();
                        dname.LoadFromDbReader(dbReader);
                        _domainNames.Add(dname);
                    }
                }
            }
            return _domainNames;
        }

        /// <summary>
        ///     Получение списка общих папок
        /// </summary>
        /// <returns></returns>
        public List<CommonFolder> CommonFolders()
        {
            if (_commonFolders != null && !RequiredRefreshInfo) return _commonFolders;
            _commonFolders = new List<CommonFolder>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_CommonFolders, CommandType.Text, Config.DS_user))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var cf = new CommonFolder();
                        cf.LoadFromDbReader(dbReader);
                        _commonFolders.Add(cf);
                    }
                }
            }
            return _commonFolders;
        }

        /// <summary>
        ///     Получение дополнительных прав
        /// </summary>
        /// <returns></returns>
        public List<AdvancedGrant> AdvancedGrants()
        {
            if (_advancedGrants != null && !RequiredRefreshInfo) return _advancedGrants;
            _advancedGrants = new List<AdvancedGrant>();
            using (var dbReader = new DBReader(SQLQueries.SELECT_AdvancedGrants, CommandType.Text, Config.DS_user))
            {
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        var cf = new AdvancedGrant();
                        cf.LoadFromDbReader(dbReader);
                        _advancedGrants.Add(cf);
                    }
                }
            }
            return _advancedGrants;
        }


        /// <summary>
        /// Форматирование номера мобильно телефона
        /// </summary>
        /// <param name="phoneNum">Номер телефона</param>
        /// <returns>Направление</returns>
        public string FormatingMobilNumber(ref string phoneNum)
        {
            if (phoneNum.Length == 0) return "";
            var ret = "";
            var areaName = "";
            var telCodeCountry = "";
            var telCodeInCountry = "";
            var direction = "";
            var phone = "";

            Phone.GetPhoneNumberInfo(phoneNum, ref areaName, ref telCodeCountry,
                ref telCodeInCountry, ref direction, ref phone);
            phoneNum = Contact.FormatingContact(22, telCodeCountry, telCodeInCountry, phone, "");

            return direction;
        }

        /// <summary>
        ///     Получение списка оборудования по данным текущего указания
        /// </summary>
        /// <returns>DataTable с оборудованием</returns>
        public DataTable GetDirectionEquipment()
        {
            var dt = new DataTable("Equipments");
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодДокумента", int.Parse(Id)}
            };
            dt = DBManager.GetData(SQLQueries.SP_ВыданноеПоУказаниюОборудование, Config.DS_user,
                CommandType.StoredProcedure, sqlParams);

            return dt;
        }

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
            {
                if (string.IsNullOrEmpty(p.Id))
                    positionForClear.Add(p);
                else
                    p.Delete(false);
            }

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
                if (delP==null)
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
        public void SavePositionsAdvancedGrantsByDictionary(Dictionary<string, string> grants, bool reloadPostions)
        {
            foreach (var p in from p in PositionAdvancedGrants
                let f = grants.Where(x => x.Key == p.GrantId.ToString())
                    .Select(x => (KeyValuePair<string, string>?) x)
                    .FirstOrDefault()
                where f == null
                select p)
            {
                p.Delete(false);
            }

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