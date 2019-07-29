using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Persons.Contacts;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;
using Attribute = Kesco.Lib.Entities.Persons.Attributes.Attribute;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Лицо
    /// </summary>
    [Serializable]
    public class PersonBase : Entity
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PersonBase(string id)
            : base(id)
        {
            PersonAttributes = new List<Attribute>();
            ResponsibleEmployes = new List<Employee>();
            PersonTypes = new List<PersonType>();
            PersonContacts = new List<Contact>();
            PersonLinks = new List<Link.Link>();
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonBase()

        {
            PersonAttributes = new List<Attribute>();
            ResponsibleEmployes = new List<Employee>();
            PersonTypes = new List<PersonType>();
            PersonContacts = new List<Contact>();
            PersonLinks = new List<Link.Link>();
        }

        /// <summary>
        ///     Метод вызывающий хранимую процедуру Удаления/Создания/Изменения связей лиц
        /// </summary>
        public object AddPersonEmployes(int? empoyeePersonID, int personID)
        {
            if (empoyeePersonID == null || personID == 0) return null;

            try
            {
                var sqlParams = new Dictionary<string, object>(100)
                {
                    {"@WhatDo", 0},
                    {"@КодЛицаРодителя", empoyeePersonID},
                    {"@КодЛицаПотомка", personID},
                    {"@КодТипаСвязиЛиц", 1},
                    {"@Параметр", 0}
                };


                var outputParams = new Dictionary<string, object>();
                DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_InsUpdDel_СвязиЛиц, CommandType.StoredProcedure, CN,
                    sqlParams, outputParams);
                return outputParams["@RETURN_VALUE"].ToString();
            }
            catch (Exception ex)
            {
                Logger.WriteEx(new DetailedException("Ошибка при создании ответвенных", ex));
                throw ex;
            }
        }

        /// <summary>
        ///     Метод вызова функции транслитерации с Русского на латиницу в базе
        /// </summary>
        /// <param name="rusValue">Значение на Русском языке</param>
        protected string TranslateRusToEng(string rusValue)
        {
            var sql = string.Format(@"SELECT Инвентаризация.dbo.fn_TransLit('{0}')", rusValue);
            var resultTable = DBManager.GetData(sql, Config.DS_person);

            var result = "";

            if (resultTable.Rows.Count > 0) result = resultTable.Rows[0][0].ToString();
            return result;
        }

        /// <summary>
        ///     Инициализация сущности Лицо на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодЛица"].ToString();
                Name = dt.Rows[0]["Кличка"].ToString();
                Type = Convert.ToInt32(dt.Rows[0]["ТипЛица"]);
                BusinessProjectID = dt.Rows[0]["КодБизнесПроекта"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодБизнесПроекта"]);
                //Cheching = Convert.ToBoolean(dt.Rows[0]["ВыполнятьПроверкуНаПохожиеЛица"]);
                NickRL = dt.Rows[0]["КличкаRL"].ToString();
                NameRL = dt.Rows[0]["НазваниеRL"].ToString();
                RegionID = dt.Rows[0]["КодТерритории"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодТерритории"]);
                IsStateCompany = Convert.ToBoolean(dt.Rows[0]["ГосОрганизация"]);
                BirthDate = dt.Rows[0]["ДатаРождения"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(dt.Rows[0]["ДатаРождения"]);
                EndDate = dt.Rows[0]["ДатаКонца"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(dt.Rows[0]["ДатаКонца"]);
                Note = dt.Rows[0]["Примечание"] == DBNull.Value ? string.Empty : dt.Rows[0]["Примечание"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получение списка лиц из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных лиц</param>
        public static List<PersonBase> GetPersonsList(DataTable dt)
        {
            var personsList = new List<PersonBase>();

            for (var i = 0; i < dt.Rows.Count; i++)
                personsList.Add(
                    new PersonBase
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодЛица"].ToString(),
                        Name = dt.Rows[i]["Кличка"].ToString(),
                        Type = Convert.ToInt32(dt.Rows[i]["ТипЛица"]),
                        BusinessProjectID = dt.Rows[i]["КодБизнесПроекта"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dt.Rows[i]["КодБизнесПроекта"]),
                        Cheching = Convert.ToBoolean(dt.Rows[i]["ВыполнятьПроверкуНаПохожиеЛица"]),
                        NickRL = dt.Rows[i]["КличкаRL"].ToString(),
                        NameRL = dt.Rows[i]["НазваниеRL"].ToString(),
                        RegionID = Convert.ToInt32(dt.Rows[i]["КодТерритории"]),
                        IsStateCompany = Convert.ToBoolean(dt.Rows[i]["ГосОрганизация"]),
                        BirthDate = dt.Rows[i]["ДатаРождения"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(dt.Rows[i]["ДатаРождения"]),
                        EndDate = dt.Rows[i]["ДатаКонца"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(dt.Rows[i]["ДатаКонца"]),
                        Note = dt.Rows[i]["Примечание"] == DBNull.Value
                            ? string.Empty
                            : dt.Rows[i]["Примечание"].ToString()
                    }
                );
            return personsList;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Лицо, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Метод создания сущности лицо
        /// </summary>
        public virtual object Create()
        {
            return null;
        }

        #region Поля сущности "Лицо"

        /// <summary>
        ///     Тип лица
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     Код бизнес-проекта
        /// </summary>
        public int BusinessProjectID { get; set; }

        /// <summary>
        ///     ВыполнятьПроверкуНаПохожиеЛица
        /// </summary>
        public bool Cheching { get; set; }

        /// <summary>
        ///     Кличка RL
        /// </summary>
        public string NickRL { get; set; }

        /// <summary>
        ///     Название RL
        /// </summary>
        public string NameRL { get; set; }

        /// <summary>
        ///     Код территории
        /// </summary>
        public int RegionID { get; set; }

        /// <summary>
        ///     ГосОрганизация
        /// </summary>
        public bool IsStateCompany { get; set; }

        /// <summary>
        ///     Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        ///     Дата окончания
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///     Пол
        /// </summary>
        public char Sex { get; set; }

        /// <summary>
        ///     Атрибуты лица
        /// </summary>
        public List<Attribute> PersonAttributes { get; set; }

        /// <summary>
        ///     Отвественные сотрудники
        /// </summary>
        public List<Employee> ResponsibleEmployes { get; set; }

        /// <summary>
        ///     Типы лица
        /// </summary>
        public List<PersonType> PersonTypes { get; set; }

        /// <summary>
        ///     Контакты лица
        /// </summary>
        public List<Contact> PersonContacts { get; set; }

        /// <summary>
        ///     Связи лица
        /// </summary>
        public List<Link.Link> PersonLinks { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_person
            : _connectionString;

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        #endregion
    }
}