using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Лицо
    /// </summary>
    [Serializable]
    public class Person : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;
        private List<PersonLogo> _logos;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public Person(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Person()
        {
        }

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
        /// </summary>
        public bool HasBProject => BusinessProjectID != 0;

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
                IsChecked = Convert.ToBoolean(dt.Rows[0]["Проверено"]);
                NickRL = dt.Rows[0]["КличкаRL"].ToString();
                NameRL = dt.Rows[0]["НазваниеRL"].ToString();
                RegionID = dt.Rows[0]["КодТерритории"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(dt.Rows[0]["КодТерритории"]);
                IsStateCompany = Convert.ToBoolean(dt.Rows[0]["ГосОрганизация"]);
                INN = dt.Rows[0]["ИНН"] == DBNull.Value ? string.Empty : dt.Rows[0]["ИНН"].ToString();
                OGRN = dt.Rows[0]["ОГРН"] == DBNull.Value ? string.Empty : dt.Rows[0]["ОГРН"].ToString();
                OKPO = dt.Rows[0]["ОКПО"] == DBNull.Value ? string.Empty : dt.Rows[0]["ОКПО"].ToString();
                BIK = dt.Rows[0]["БИК"] == DBNull.Value ? string.Empty : dt.Rows[0]["БИК"].ToString();
                CorrAccount = dt.Rows[0]["КорСчет"] == DBNull.Value ? string.Empty : dt.Rows[0]["КорСчет"].ToString();
                BIKRKC = dt.Rows[0]["БИКРКЦ"] == DBNull.Value ? string.Empty : dt.Rows[0]["БИКРКЦ"].ToString();
                SWIFT = dt.Rows[0]["SWIFT"] == DBNull.Value ? string.Empty : dt.Rows[0]["SWIFT"].ToString();
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
        public static List<Person> GetPersonsList(DataTable dt)
        {
            var personsList = new List<Person>();

            for (var i = 0; i < dt.Rows.Count; i++)
                personsList.Add(
                    new Person
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодЛица"].ToString(),
                        Name = dt.Rows[i]["Кличка"].ToString(),
                        Type = Convert.ToInt32(dt.Rows[i]["ТипЛица"]),
                        BusinessProjectID = dt.Rows[i]["КодБизнесПроекта"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dt.Rows[i]["КодБизнесПроекта"]),
                        IsChecked = Convert.ToBoolean(dt.Rows[i]["Проверено"]),
                        NickRL = dt.Rows[i]["КличкаRL"].ToString(),
                        NameRL = dt.Rows[i]["НазваниеRL"].ToString(),
                        RegionID = Convert.ToInt32(dt.Rows[i]["КодТерритории"]),
                        IsStateCompany = Convert.ToBoolean(dt.Rows[i]["ГосОрганизация"]),
                        INN = dt.Rows[i]["ИНН"] == DBNull.Value ? string.Empty : dt.Rows[i]["ИНН"].ToString(),
                        OGRN = dt.Rows[i]["ОГРН"] == DBNull.Value ? string.Empty : dt.Rows[i]["ОГРН"].ToString(),
                        OKPO = dt.Rows[i]["ОКПО"] == DBNull.Value ? string.Empty : dt.Rows[i]["ОКПО"].ToString(),
                        BIK = dt.Rows[i]["БИК"] == DBNull.Value ? string.Empty : dt.Rows[i]["БИК"].ToString(),
                        CorrAccount = dt.Rows[i]["КорСчет"] == DBNull.Value
                            ? string.Empty
                            : dt.Rows[i]["КорСчет"].ToString(),
                        BIKRKC = dt.Rows[i]["БИКРКЦ"] == DBNull.Value ? string.Empty : dt.Rows[i]["БИКРКЦ"].ToString(),
                        SWIFT = dt.Rows[i]["SWIFT"] == DBNull.Value ? string.Empty : dt.Rows[i]["SWIFT"].ToString(),
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
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Лицо, CN, CommandType.Text, sqlParams));
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
        ///     Проверено
        /// </summary>
        public bool IsChecked { get; set; }

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
        ///     ИНН
        /// </summary>
        public string INN { get; set; }

        /// <summary>
        ///     ОГРН
        /// </summary>
        public string OGRN { get; set; }

        /// <summary>
        ///     ОКПО
        /// </summary>
        public string OKPO { get; set; }

        /// <summary>
        ///     БИК
        /// </summary>
        public string BIK { get; set; }

        /// <summary>
        ///     Корреспондентский счет
        /// </summary>
        public string CorrAccount { get; set; }

        /// <summary>
        ///     БИКРКЦ
        /// </summary>
        public string BIKRKC { get; set; }

        /// <summary>
        ///     SWIFT
        /// </summary>
        public string SWIFT { get; set; }

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

        #endregion


        /// <summary>
        /// Получение списка контактов лица
        /// </summary>        
        /// <returns>Список контактов</returns>
        public List<PersonContact> GetPersonContacts()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодЛица", int.Parse(Id) } };
            var dt = DALC.DBManager.GetData(SQLQueries.SP_Лица_Контакты, Config.DS_person, CommandType.StoredProcedure, sqlParams);

            var result = dt.AsEnumerable().Select(dr => new PersonContact
            {
                ContactId = dr.Field<int>("КодКонтакта"),
                TypeContactId = dr.Field<int>("КодТипаКонтакта"),
                Icon = dr.Field<string>("icon"),
                TypeContact = dr.Field<string>("ТипКонтакта"),
                Contact = dr.Field<string>("Контакт"),
                Description = dr.Field<string>("Примечание"),
                InternationalNumber = dr.Field<string>("НомерМеждународный"),
                Order = dr.Field<int>("Порядок")
            }).ToList();

            return result;

        }


        /// <summary>
        ///     Список логотипов лица
        /// </summary>
        public List<PersonLogo> Logos
        {
            get
            {
                if (LoadedExternalProperties.ContainsKey("person._logos")) return _logos;

                _logos = new List<PersonLogo>();
                var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) } };
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ЛоготипыЛица, CommandType.Text, CN,
                        sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var ph = new PersonLogo();
                            ph.LoadFromDbReader(dbReader);
                            _logos.Add(ph);
                        }
                }

                LoadedExternalProperties.Add("person._logos", DateTime.UtcNow);

                return _logos;
            }
        }

        /// <summary>
        /// Является ли лицо сотрудником
        /// </summary>
        public int EmployeeId
        {

            get
            {
                var sqlParams = new Dictionary<string, object> { { "@КодЛица", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
                var emplId = DBManager.ExecuteScalar(SQLQueries.SELECT_PERSON_ID_Сотрудник, CommandType.Text, Config.DS_user, sqlParams);
                if (emplId == null || emplId.Equals(System.DBNull.Value))
                    return 0;

                return int.Parse(emplId.ToString());
            }
        }

        /// <summary>
        /// Существуют ли действующие на текущую дату реквизиты
        /// </summary>
        /// <returns></returns>
        public bool ExistsActualData()
        {
            if (IsNew) return false;
            var sqlParams = new Dictionary<string, object>() { { "@КодЛица", int.Parse(Id) }, { "@Дата", DateTime.Now } };
            var ret = false;
            if (Type == 1)
            {
                var sqlQuery = SQLQueries.SELECT_КарточкаЮрЛица_ДействующаяНаДату;

                using (var dr = new DBReader(sqlQuery, CommandType.Text, Config.DS_person, sqlParams))
                {
                    if (dr.HasRows)
                        ret = true;
                }

            }

            return ret;
        }
         
    }

}
