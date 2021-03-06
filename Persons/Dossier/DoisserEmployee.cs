﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.Dossier
{
    /// <summary>
    ///     Класс сущности досье сотрудника
    /// </summary>
    [Serializable]
    public class DoisserEmployee : Entity
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public DoisserEmployee()
        {
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public DoisserEmployee(int userID)
        {
            if (userID == 0) return;
            Unavailable = false;
            EmployeeID = userID;
            Contacts = new List<EmployeeContact>();
            Positions = new List<EmployeePosition>();
            WorkPlaces = new List<Location>();
            CoWorkers = new List<EmployeeCoWorker>();
            Roles = new List<EmployeeRole>();

            Replaced = false;

            Employee = new Employee(userID.ToString());

            if (Employee == null || Employee.Unavailable)
            {
                Unavailable = true;
                return;
            }

            /*Общие сотрудники */
            if (string.IsNullOrEmpty(Employee.CommonEmployeeID) && Employee.Status == 0 &&
                Employee.PersonEmployeeId == null)
                CommonEmployees = GetListCommonEmployeesByIds(Employee.Id);
            else
                CommonEmployees = new List<Employee>();

            EmployeeReplacements = GetEmployeeReplacementById(Employee.Id);
            EmployeeRepresentatives = GetEmployeeReplacementiveById(Employee.Id);
            Replaced = EmployeeReplacements.Count != 0;
            EmployeeStatus = Employee.GetEmployeeStatus(Employee.Status);
            LastPassage = new EmployeePassage(Employee.Id);
            IsPersonAdministrator = BelongsToPersonAdministators(Employee.Id);

            if (Employee.Status != 3)
            {
                Contacts = Employee.GetEmployeeContacts();

                if (Employee.OrganizationId.HasValue) EmployeeCustomer = new Corporate.PersonCustomer(Employee.OrganizationId.Value.ToString());

                Roles = GetAllEmployeRoles(Employee.Id);
                Positions = GetEmployeePositions(Employee.Id);

                WorkPlaces = GetEmployeeWorkPlaces(Employee.Id);

            }

            ContactGroups = Contacts
                .Where(c => c.ContactType == -1)
                .GroupBy(c => c.ContactType)
                .Select(g => new ContactTypeGroup
                {
                    ContactTypeID = g.Key,
                    Items = g.Select(c => new UserInfoContact
                        {
                            Contact = c.Contact,
                            ContactType = c.ContactType,
                            Icon = string.IsNullOrEmpty(c.Icon) ? null : c.Icon,
                            PhoneNumber = c.ContactType == -1 ? c.Contact : string.Empty,
                            Note = c.Note,
                            Type = c.ContactTypeDesc,
                            Order = c.Order,
                            InDictionary = c.InDictionary
                        })
                        .OrderBy(c => c.Order).ToList()
                }).ToList();

            CanChangeWorkPlace = BelongsToPersonAdministators(Employee.Id);
        }
        
        private List<Location> GetEmployeeWorkPlaces(string userID)
        {
            var userUserWorkPlacesList = new List<Location>();
            var sqlParams = new Dictionary<string, object> {{"@Id", userID}};
            using (var dbReader =
                new DBReader(SQLQueries.SELECT_РабочиеМестаСотрудника, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserWorkPlace = new Location();
                        tempUserWorkPlace.LoadFromDbReader(dbReader);
                        userUserWorkPlacesList.Add(tempUserWorkPlace);
                    }
            }

            return userUserWorkPlacesList;
        }

        private List<EmployeePosition> GetEmployeePositions(string userID)
        {
            var userPositionsList = new List<EmployeePosition>();
            var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", userID}, {"@Совместитель ", -1}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_ДолжностиСотрудника, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserPosition = new EmployeePosition();
                        tempUserPosition.LoadFromDbReader(dbReader);
                        userPositionsList.Add(tempUserPosition);
                    }
            }

            return userPositionsList;
        }

        private List<EmployeeRole> GetAllEmployeRoles(string userID)
        {
            var userRole = new List<EmployeeRole>();
            var sqlParams = new Dictionary<string, object> {{"@КодСотрудника", userID}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_РолиСотрудника, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserRole = new EmployeeRole();
                        tempUserRole.LoadFromDbReader(dbReader);
                        userRole.Add(tempUserRole);
                    }
            }

            return userRole;
        }

        private List<EmployeeContact> GetEmployeeContacts(string userID)
        {
            var userContactList = new List<EmployeeContact>();
            var sqlParams = new Dictionary<string, object>
                {{"@КодСотрудника", userID}, {"@КодТелефоннойСтанцииЗвонящего", ""}};
            using (var dbReader = new DBReader(SQLQueries.SP_Сотрудники_Контакты, CommandType.StoredProcedure, CN,
                sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserContact = new EmployeeContact();
                        tempUserContact.LoadFromDbReader(dbReader);
                        userContactList.Add(tempUserContact);
                    }
            }

            return userContactList;
        }

        private bool BelongsToPersonAdministators(string userID)
        {
            var isAdmin = false;
            var sqlParams = new Dictionary<string, object> {{"@Id", userID}};
            using (var dbReader =
                new DBReader(SQLQueries.SELECT_АдминистраторСотрудника, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    var colЯвляетсяАдминистраторомЛица = dbReader.GetOrdinal("ЯвляетсяАдминистраторомСотрудника");
                    if (dbReader.Read())
                        if (!dbReader.IsDBNull(colЯвляетсяАдминистраторомЛица))
                            isAdmin = dbReader.GetBoolean(colЯвляетсяАдминистраторомЛица);
                }
            }

            return isAdmin;
        }

        private List<EmployeeReplacement> GetEmployeeReplacementById(string userID)
        {
            var userReplacementList = new List<EmployeeReplacement>();
            var sqlParams = new Dictionary<string, object> {{"@Id", userID}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_ЗамещенияСотрудника_ПоЗамещаемому, CommandType.Text,
                CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserReplacement = new EmployeeReplacement();
                        tempUserReplacement.LoadFromDbReader(dbReader);
                        userReplacementList.Add(tempUserReplacement);
                    }
            }

            return userReplacementList;
        }

        private List<EmployeeReplacement> GetEmployeeReplacementiveById(string userID)
        {
            var userReplacementList = new List<EmployeeReplacement>();
            var sqlParams = new Dictionary<string, object> {{"@Id", userID}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_ЗамещенияСотрудника_ПоЗамещающему, CommandType.Text,
                CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUserReplacement = new EmployeeReplacement();
                        tempUserReplacement.LoadFromDbReader(dbReader);
                        userReplacementList.Add(tempUserReplacement);
                    }
            }

            return userReplacementList;
        }

        private List<Employee> GetListCommonEmployeesByIds(string userID)
        {
            var userList = new List<Employee>();
            var sqlParams = new Dictionary<string, object> {{"@Id", userID}};
            using (var dbReader = new DBReader(SQLQueries.SELECT_ОбщихСотрудников, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var tempUser = new Employee(false);
                        tempUser.LoadFromDbReader(dbReader, true);
                        userList.Add(tempUser);
                    }
            }

            return userList;
        }

        /// <summary>
        ///     Объединение контактов по их типу
        /// </summary>
        public class ContactTypeGroup
        {
            #region Свойтва

            /// <summary>
            ///     название типа
            /// </summary>
            public string ContactType { get; set; }

            /// <summary>
            ///     Id типа контактов
            /// </summary>
            public int ContactTypeID { get; set; }

            /// <summary>
            ///     список контактов такого типа
            /// </summary>
            public List<UserInfoContact> Items { get; set; }

            #endregion
        }

        /// <summary>
        ///     Информация о контакте сотрудника
        /// </summary>
        public class UserInfoContact
        {
            /// <summary>
            /// </summary>
            public string Contact { get; set; }

            /// <summary>
            /// </summary>
            public int ContactType { get; set; }

            /// <summary>
            /// </summary>
            public string Icon { get; set; }

            /// <summary>
            /// </summary>
            public string PhoneNumber { get; set; }

            /// <summary>
            /// </summary>
            public string Note { get; set; }

            /// <summary>
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// </summary>
            public int Order { get; set; }

            /// <summary>
            /// </summary>
            public bool InDictionary { get; set; }
        }

        #region Свойтва

        /// <summary>
        ///     Gets or sets the employee ID.
        /// </summary>
        /// <value>
        ///     The employee ID.
        /// </value>
        public int EmployeeID { get; set; }

        /// <summary>
        ///     Отсутствует
        /// </summary>
        public bool Replaced { get; set; }

        /// <summary>
        ///     Gets or sets the employee.
        /// </summary>
        /// <value>
        ///     The employee.
        /// </value>
        public Employee Employee { get; protected set; }

        /// <summary>
        ///     Gets or sets the person.
        /// </summary>
        /// <value>
        ///     The person.
        /// </value>
        public Corporate.PersonCustomer EmployeeCustomer { get; protected set; }

        /// <summary>
        ///     Gets or sets the person who change.
        /// </summary>
        /// <value>
        ///     The person.
        /// </value>
        public Corporate.PersonCustomer EmployeeChangedBy { get; protected set; }

        /// <summary>
        ///     Последний проход
        /// </summary>
        public EmployeePassage LastPassage { get; protected set; }

        /// <summary>
        ///     Статус сотрудника
        /// </summary>
        public string EmployeeStatus { get; protected set; }

        /// <summary>
        ///     Текущий пользователь может изменять месторасположение сотрудника
        /// </summary>
        public bool CanChangeWorkPlace { get; protected set; }

        /// <summary>
        ///     Замещает
        /// </summary>
        public List<EmployeeReplacement> EmployeeRepresentatives { get; set; }

        /// <summary>
        ///     Замещающие
        /// </summary>
        public List<EmployeeReplacement> EmployeeReplacements { get; set; }

        /// <summary>
        ///     Контакты
        /// </summary>
        public List<EmployeeContact> Contacts { get; set; }

        /// <summary>
        ///     Gets or sets all contacts.
        /// </summary>
        /// <value>
        ///     All contacts.
        /// </value>
        public List<ContactTypeGroup> ContactGroups { get; set; }

        /// <summary>
        ///     Gets or sets the positions.
        /// </summary>
        /// <value>
        ///     The positions.
        /// </value>
        public List<EmployeePosition> Positions { get; set; }

        /// <summary>
        /// </summary>
        public List<Location> WorkPlaces { get; set; }

        /// <summary>
        /// </summary>
        public List<EmployeeCoWorker> CoWorkers { get; set; }

        /// <summary>
        /// </summary>
        public List<EmployeeRole> Roles { get; set; }

        /// <summary>
        /// </summary>
        public bool IsPersonAdministrator { get; set; }

        /// <summary>
        /// </summary>
        public List<Employee> CommonEmployees { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        #endregion
    }
}