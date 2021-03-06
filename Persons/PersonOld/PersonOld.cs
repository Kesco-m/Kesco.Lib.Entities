﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons.Contacts;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.PersonOld
{
    /// <summary>
    ///     Класс сущности Лицо для совместимости с V3
    /// </summary>
    /// <remarks>
    ///     В V4 отсутствует старые person и Card
    /// </remarks>
    [Serializable]
    public class PersonOld : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PersonOld(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonOld()
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
        ///     Проверка наличия ФорматНомеровРегистрацииЛиц по Коду Территории
        /// </summary>
        public bool CheckTemplateData
        {
            get
            {
                if (RegionID == 0) return false;
                var query =
                    "SELECT COUNT(*) FROM Справочники.dbo.ФорматНомеровРегистрацииЛиц ФНРЛ WHERE КодТерритории=" +
                    RegionID;
                var cntObj = DBManager.ExecuteScalar(query, CommandType.Text, ConnString);

                var cnt = cntObj as int?;
                return cnt == 1;
            }
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
                Changed = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
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
        public static List<PersonOld> GetPersonsList(DataTable dt)
        {
            var personsList = new List<PersonOld>();

            for (var i = 0; i < dt.Rows.Count; i++)
                personsList.Add(
                    new PersonOld
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
                            : dt.Rows[i]["Примечание"].ToString(),
                        Changed = Convert.ToDateTime(dt.Rows[0]["Изменено"])
                    }
                );
            return personsList;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override void Load()
        {
            if (Id.IsNullEmptyOrZero()) return;
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Лицо, CN, CommandType.Text, sqlParams));
        }


        /// <summary>
        ///     Полчение списка контактов по лицу
        /// </summary>
        /// <param name="contactType">Код типа контакта</param>
        /// <returns>Список контактов</returns>
        public List<Contact> GetContacts(ContactTypeEnum contactType)
        {
            List<Contact> list = null;
            var sqlParams = new Dictionary<string, object>
                {{"@Id", int.Parse(Id)}, {"@КодТипаКонтакта", (int) contactType}};
            using (var dbReader =
                new DBReader(SQLQueries.SELECT_Контакты_ПоЛицу, CommandType.Text, ConnString, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    list = new List<Contact>();

                    #region Получение порядкового номера столбца

                    var colКодКонтакта = dbReader.GetOrdinal("КодКонтакта");
                    var colКодЛица = dbReader.GetOrdinal("КодЛица");
                    var colКонтакт = dbReader.GetOrdinal("Контакт");
                    var colКонтактRL = dbReader.GetOrdinal("КонтактRL");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new Contact();
                        row.Unavailable = false;
                        if (!dbReader.IsDBNull(colКодКонтакта)) row.Id = dbReader.GetInt32(colКодКонтакта).ToString();
                        if (!dbReader.IsDBNull(colКодЛица)) row.PersonID = dbReader.GetInt32(colКодЛица);
                        row.Name = dbReader.GetString(colКонтакт);
                        row.ContactName = dbReader.GetString(colКонтакт);
                        row.ContactNameRL = dbReader.GetString(colКонтактRL);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Получение краткого названия лица на дату
        /// </summary>
        /// <param name="d">На какую дату получать имя</param>
        /// <param name="isRusLocal">Какое название возвращать</param>
        /// <returns>Краткое название лица</returns>
        public string GetName(DateTime d, bool isRusLocal= true)
        {
            var name = Name;

            var crd = GetCard(d == DateTime.MinValue ? DateTime.Today : d);
            if (crd != null)
                name = crd.NameRus.Length == 0 ? crd.NameLat : crd.NameRus;

            return name;
        }


        //Карточки лиц

        /// <summary>
        ///     Получить списк карточек
        /// </summary>
        /// <returns></returns>
        public List<Card> GetCards()
        {
            switch (Type)
            {
                case 1:
                    return GetCardsJ();
                case 2:
                    return GetCardsN();
            }

            return new List<Card>();
        }


        /// <summary>
        ///     Получить карточку юридического лица
        /// </summary>
        public List<Card> GetCardsJ()
        {
            return CardJ.GetCardJList(SQLQueries.SELECT_ID_КарточкиЮрЛица, int.Parse(Id));
        }

        /// <summary>
        ///     Получить карточку физического лица
        /// </summary>
        public List<Card> GetCardsN()
        {
            return CardN.GetCardNList(SQLQueries.SELECT_ID_КарточкиФизЛица, int.Parse(Id));
        }

        /// <summary>
        ///     Получить карточку лица
        /// </summary>
        public Card GetCard(DateTime d)
        {
            var arr = GetCards();
            return arr == null ? null : arr.FirstOrDefault(t => t.От <= d && t.До > d);
        }

        /// <summary>
        ///     ФИО в дательном падеже
        /// </summary>
        /// <param name="d">Дата</param>
        /// <param name="prefix">пол (м/ж)</param>
        /// <returns>ФИО</returns>
        public string PersonNP_GetFIODadelPareg(DateTime d, bool prefix = false)
        {
            var crd = (CardN) GetCard(d);

            if (crd == null) return Name;
            if (crd.ФамилияРус.Length == 0) return Name;

            var pref = "";

            if (prefix) pref = crd.Пол.ToLower().Equals("ж") ? "госпоже " : "господину ";
            var ret = "";

            var param = new Dictionary<string, object> {{"@ФамилияИменительный", crd.ФамилияРус}};
            var dt = DBManager.GetData(@"
                DECLARE @ФамилияДательный nvarchar(300)
                EXEC sp_ФамилияВДательномПадеже  @ФамилияИменительный,1, @ФамилияДательный OUT
                SELECT @ФамилияДательный Фамилия", Config.DS_person, CommandType.Text, param);

            ret = dt.Rows[0][0].ToString();
            ret += ret.Length > 0 ? " " : "";
            ret += crd.ИмяРус.Length > 0 ? crd.ИмяРус.Substring(0, 1).ToUpper() + "." : "";
            ret += crd.ОтчествоРус.Length > 0 ? crd.ОтчествоРус.Substring(0, 1).ToUpper() + "." : "";

            return pref + ret;
        }

        /// <summary>
        ///     Описание cвязиЛиц
        /// </summary>
        /// <param name="_parent">КодЛицаРодителя</param>
        /// <param name="_date">Дата</param>
        /// <returns></returns>
        public string PersonNP_GetPostDatelPadegHead(string _parent, string _date)
        {
            var ret = "";
            var param = new Dictionary<string, object>
            {
                {"@Child", Id},
                {"@Parent", _parent},
                {"@Date", _date}
            };
            var dt = DBManager.GetData(@"
                SELECT Описание
                FROM vwСвязиЛиц
                WHERE Параметр=1 AND КодЛицаРодителя=@Parent
	                AND КодЛицаПотомка =@Child
	                AND @Date BETWEEN От AND До", Config.DS_person, CommandType.Text, param);

            if (dt.Rows.Count != 1) return "";

            param = new Dictionary<string, object>
            {
                {"@ДолжностьИменительный", dt.Rows[0][0].ToString()},
                {"@Parent", _parent},
                {"@Date", _date}
            };
            dt = DBManager.GetData(@"
                DECLARE @ДолжностьДательный nvarchar(300)
                EXEC sp_ДолжностьВДательномПадеже  @ДолжностьИменительный, @ДолжностьДательный OUT
                SELECT @ДолжностьДательный Доложность", Config.DS_person, CommandType.Text, param);
            ret = dt.Rows[0][0].ToString();

            return ret;
        }

        /// <summary>
        ///     Метод получения даты последнего изменения
        /// </summary>
        public override DateTime GetLastChanged(string id)
        {
            var param = new Dictionary<string, object> {{"@Id", id}};
            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_Лицо_LastChanged, CommandType.Text, CN, param);

            if (res is DateTime)
                return (DateTime) res;

            return DateTime.MinValue;
        }

        /// <summary>
        ///     Класс карточки клиента
        /// </summary>
        public abstract class Card : Entity
        {
            /// <summary>
            ///     Регулярное выражение для удаления пробелов
            /// </summary>
            protected static Regex spaceRemover = new Regex("^[ ]+|[ ]+$");

            /// <summary>
            ///     Карточка клиента
            /// </summary>
            public PersonOld Person => КодЛица == 0 ? null : new PersonOld(КодЛица.ToString());

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодЛица (int, null)
            /// </value>
            public int КодЛица { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     От (datetime, not null)
            /// </value>
            public DateTime От { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     До (datetime, not null)
            /// </value>
            public DateTime До { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КПП (varchar(20), not null)
            /// </value>
            public string КПП { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     АдресЮридический (nvarchar(300), not null)
            /// </value>
            public string АдресЮридический { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     АдресЮридическийЛат (varchar(300), not null)
            /// </value>
            public string АдресЮридическийЛат { get; set; }

            /// <summary>
            ///     Русское наименование
            /// </summary>
            public abstract string NameRus { get; }

            /// <summary>
            ///     Английское наименование
            /// </summary>
            public abstract string NameLat { get; }
        }

        /// <summary>
        ///     Класс карточки физического лица
        /// </summary>
        public class CardN : Card
        {
            /// <summary>
            ///     Получение полного наименования лица
            ///     Выводится ИП для физ. лиц, при такой ОП форме
            /// </summary>
            public override string NameRus
            {
                get
                {
                    if (ФамилияРус.Length + ИмяРус.Length + ОтчествоРус.Length == 0) return "";

                    //string _form = (_Form.Equals(V2.PersonOrgPrForma._IChP) && !Form.Unavailable) ? Form._Name + " " : "";
                    return /*_form + */spaceRemover.Replace(ФамилияРус + " " + ИмяРус + " " + ОтчествоРус, "");
                }
            }

            /// <summary>
            ///     Английское наименование
            /// </summary>
            public override string NameLat => spaceRemover.Replace(ФамилияРус + " " + ИмяРус + " " + ОтчествоРус, "");

            /// <summary>
            ///     Заполнить данные по ID
            /// </summary>
            /// <param name="id"></param>
            public void FillData(string id)
            {
                using (var dbReader = new DBReader(SQLQueries.SELECT_ID_КарточкаФизЛица, КодКарточкиФизЛица,
                    CommandType.Text, ConnString))
                {
                    if (dbReader.HasRows)
                    {
                        #region Получение порядкового номера столбца

                        var colКодКарточкиФизЛица = dbReader.GetOrdinal("КодКарточкиФизЛица");
                        var colКодЛица = dbReader.GetOrdinal("КодЛица");
                        var colОт = dbReader.GetOrdinal("От");
                        var colДо = dbReader.GetOrdinal("До");
                        var colКодОргПравФормы = dbReader.GetOrdinal("КодОргПравФормы");
                        var colФамилияРус = dbReader.GetOrdinal("ФамилияРус");
                        var colИмяРус = dbReader.GetOrdinal("ИмяРус");
                        var colОтчествоРус = dbReader.GetOrdinal("ОтчествоРус");
                        var colФИОРус = dbReader.GetOrdinal("ФИОРус");
                        var colИОФРус = dbReader.GetOrdinal("ИОФРус");
                        var colФамилияЛат = dbReader.GetOrdinal("ФамилияЛат");
                        var colИмяЛат = dbReader.GetOrdinal("ИмяЛат");
                        var colОтчествоЛат = dbReader.GetOrdinal("ОтчествоЛат");
                        var colПол = dbReader.GetOrdinal("Пол");
                        var colОКОНХ = dbReader.GetOrdinal("ОКОНХ");
                        var colОКВЭД = dbReader.GetOrdinal("ОКВЭД");
                        var colКПП = dbReader.GetOrdinal("КПП");
                        var colКодЖД = dbReader.GetOrdinal("КодЖД");
                        var colАдресЮридический = dbReader.GetOrdinal("АдресЮридический");
                        var colАдресЮридическийЛат = dbReader.GetOrdinal("АдресЮридическийЛат");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        if (dbReader.Read())
                        {
                            Unavailable = false;
                            if (!dbReader.IsDBNull(colКодКарточкиФизЛица))
                                КодКарточкиФизЛица = dbReader.GetInt32(colКодКарточкиФизЛица);
                            if (!dbReader.IsDBNull(colКодЛица)) КодЛица = dbReader.GetInt32(colКодЛица);
                            От = dbReader.GetDateTime(colОт);
                            До = dbReader.GetDateTime(colДо);
                            if (!dbReader.IsDBNull(colКодОргПравФормы))
                                КодОргПравФормы = dbReader.GetInt32(colКодОргПравФормы);
                            ФамилияРус = dbReader.GetString(colФамилияРус);
                            ИмяРус = dbReader.GetString(colИмяРус);
                            ОтчествоРус = dbReader.GetString(colОтчествоРус);
                            if (!dbReader.IsDBNull(colФИОРус)) ФИОРус = dbReader.GetString(colФИОРус);
                            if (!dbReader.IsDBNull(colИОФРус)) ИОФРус = dbReader.GetString(colИОФРус);
                            ФамилияЛат = dbReader.GetString(colФамилияЛат);
                            ИмяЛат = dbReader.GetString(colИмяЛат);
                            ОтчествоЛат = dbReader.GetString(colОтчествоЛат);
                            Пол = dbReader.GetString(colПол);
                            ОКОНХ = dbReader.GetString(colОКОНХ);
                            ОКВЭД = dbReader.GetString(colОКВЭД);
                            КПП = dbReader.GetString(colКПП);
                            КодЖД = dbReader.GetString(colКодЖД);
                            АдресЮридический = dbReader.GetString(colАдресЮридический);
                            АдресЮридическийЛат = dbReader.GetString(colАдресЮридическийЛат);
                            Изменил = dbReader.GetInt32(colИзменил);
                            Changed = dbReader.GetDateTime(colИзменено);
                        }
                    }
                    else
                    {
                        Unavailable = true;
                    }
                }
            }

            /// <summary>
            ///     Получить список
            /// </summary>
            public static List<Card> GetCardNList(string query, int id)
            {
                List<Card> list = null;
                using (var dbReader = new DBReader(query, id, CommandType.Text, ConnString))
                {
                    if (dbReader.HasRows)
                    {
                        list = new List<Card>();

                        #region Получение порядкового номера столбца

                        var colКодКарточкиФизЛица = dbReader.GetOrdinal("КодКарточкиФизЛица");
                        var colКодЛица = dbReader.GetOrdinal("КодЛица");
                        var colОт = dbReader.GetOrdinal("От");
                        var colДо = dbReader.GetOrdinal("До");
                        var colКодОргПравФормы = dbReader.GetOrdinal("КодОргПравФормы");
                        var colФамилияРус = dbReader.GetOrdinal("ФамилияРус");
                        var colИмяРус = dbReader.GetOrdinal("ИмяРус");
                        var colОтчествоРус = dbReader.GetOrdinal("ОтчествоРус");
                        var colФИОРус = dbReader.GetOrdinal("ФИОРус");
                        var colИОФРус = dbReader.GetOrdinal("ИОФРус");
                        var colФамилияЛат = dbReader.GetOrdinal("ФамилияЛат");
                        var colИмяЛат = dbReader.GetOrdinal("ИмяЛат");
                        var colОтчествоЛат = dbReader.GetOrdinal("ОтчествоЛат");
                        var colПол = dbReader.GetOrdinal("Пол");
                        var colОКОНХ = dbReader.GetOrdinal("ОКОНХ");
                        var colОКВЭД = dbReader.GetOrdinal("ОКВЭД");
                        var colКПП = dbReader.GetOrdinal("КПП");
                        var colКодЖД = dbReader.GetOrdinal("КодЖД");
                        var colАдресЮридический = dbReader.GetOrdinal("АдресЮридический");
                        var colАдресЮридическийЛат = dbReader.GetOrdinal("АдресЮридическийЛат");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        while (dbReader.Read())
                        {
                            var row = new CardN();
                            row.Unavailable = false;
                            if (!dbReader.IsDBNull(colКодКарточкиФизЛица))
                                row.КодКарточкиФизЛица = dbReader.GetInt32(colКодКарточкиФизЛица);
                            if (!dbReader.IsDBNull(colКодЛица)) row.КодЛица = dbReader.GetInt32(colКодЛица);
                            row.От = dbReader.GetDateTime(colОт);
                            row.До = dbReader.GetDateTime(colДо);
                            if (!dbReader.IsDBNull(colКодОргПравФормы))
                                row.КодОргПравФормы = dbReader.GetInt32(colКодОргПравФормы);
                            row.ФамилияРус = dbReader.GetString(colФамилияРус);
                            row.ИмяРус = dbReader.GetString(colИмяРус);
                            row.ОтчествоРус = dbReader.GetString(colОтчествоРус);
                            if (!dbReader.IsDBNull(colФИОРус)) row.ФИОРус = dbReader.GetString(colФИОРус);
                            if (!dbReader.IsDBNull(colИОФРус)) row.ИОФРус = dbReader.GetString(colИОФРус);
                            row.ФамилияЛат = dbReader.GetString(colФамилияЛат);
                            row.ИмяЛат = dbReader.GetString(colИмяЛат);
                            row.ОтчествоЛат = dbReader.GetString(colОтчествоЛат);
                            row.Пол = dbReader.GetString(colПол);
                            row.ОКОНХ = dbReader.GetString(colОКОНХ);
                            row.ОКВЭД = dbReader.GetString(colОКВЭД);
                            row.КПП = dbReader.GetString(colКПП);
                            row.КодЖД = dbReader.GetString(colКодЖД);
                            row.АдресЮридический = dbReader.GetString(colАдресЮридический);
                            row.АдресЮридическийЛат = dbReader.GetString(colАдресЮридическийЛат);
                            row.Изменил = dbReader.GetInt32(colИзменил);
                            row.Changed = dbReader.GetDateTime(colИзменено);
                            list.Add(row);
                        }
                    }
                }

                return list;
            }

            #region Поля сущности

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодКарточкиФизЛица (int, null)
            /// </value>
            public int КодКарточкиФизЛица { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодОргПравФормы (int, null)
            /// </value>
            public int КодОргПравФормы { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ФамилияРус (nvarchar(50), not null)
            /// </value>
            public string ФамилияРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ИмяРус (nvarchar(50), not null)
            /// </value>
            public string ИмяРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОтчествоРус (nvarchar(50), not null)
            /// </value>
            public string ОтчествоРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ФИОРус (nvarchar(300), null)
            /// </value>
            public string ФИОРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ИОФРус (nvarchar(300), null)
            /// </value>
            public string ИОФРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ФамилияЛат (varchar(50), not null)
            /// </value>
            public string ФамилияЛат { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ИмяЛат (varchar(50), not null)
            /// </value>
            public string ИмяЛат { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОтчествоЛат (varchar(50), not null)
            /// </value>
            public string ОтчествоЛат { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     Пол (char(1), not null)
            /// </value>
            public string Пол { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОКОНХ (varchar(5), not null)
            /// </value>
            public string ОКОНХ { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОКВЭД (varchar(8), not null)
            /// </value>
            public string ОКВЭД { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодЖД (varchar(35), not null)
            /// </value>
            public string КодЖД { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     Изменил (int, not null)
            /// </value>
            public int Изменил { get; set; }

            #endregion
        }

        /// <summary>
        ///     Класс карточки Юридического лица
        /// </summary>
        public class CardJ : Card
        {
            private long _кодКарточкиЮрЛица;

            /// <summary>
            ///     Русское наименование
            /// </summary>
            public override string NameRus => КраткоеНазваниеРус;

            /// <summary>
            ///     Английское наименование
            /// </summary>
            public override string NameLat => КраткоеНазваниеЛат;

            /// <summary>
            ///     Заполнить данные по Id
            /// </summary>
            public void FillData(string id)
            {
                using (var dbReader = new DBReader(SQLQueries.SELECT_ID_КарточкаЮрЛица,
                    Convert.ToInt32(КодКарточкиЮрЛица), CommandType.Text, ConnString))
                {
                    if (dbReader.HasRows)
                    {
                        #region Получение порядкового номера столбца

                        var colКодКарточкиЮрЛица = dbReader.GetOrdinal("КодКарточкиЮрЛица");
                        var colКодЛица = dbReader.GetOrdinal("КодЛица");
                        var colОт = dbReader.GetOrdinal("От");
                        var colДо = dbReader.GetOrdinal("До");
                        var colКодОргПравФормы = dbReader.GetOrdinal("КодОргПравФормы");
                        var colКраткоеНазваниеРус = dbReader.GetOrdinal("КраткоеНазваниеРус");
                        var colКраткоеНазваниеРусРП = dbReader.GetOrdinal("КраткоеНазваниеРусРП");
                        var colКраткоеНазваниеЛат = dbReader.GetOrdinal("КраткоеНазваниеЛат");
                        var colПолноеНазвание = dbReader.GetOrdinal("ПолноеНазвание");
                        var colОКОНХ = dbReader.GetOrdinal("ОКОНХ");
                        var colОКВЭД = dbReader.GetOrdinal("ОКВЭД");
                        var colКПП = dbReader.GetOrdinal("КПП");
                        var colКодЖД = dbReader.GetOrdinal("КодЖД");
                        var colАдресЮридический = dbReader.GetOrdinal("АдресЮридический");
                        var colАдресЮридическийЛат = dbReader.GetOrdinal("АдресЮридическийЛат");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        if (dbReader.Read())
                        {
                            Unavailable = false;
                            if (!dbReader.IsDBNull(colКодКарточкиЮрЛица))
                                КодКарточкиЮрЛица = dbReader.GetInt32(colКодКарточкиЮрЛица);
                            if (!dbReader.IsDBNull(colКодЛица)) КодЛица = dbReader.GetInt32(colКодЛица);
                            От = dbReader.GetDateTime(colОт);
                            До = dbReader.GetDateTime(colДо);
                            if (!dbReader.IsDBNull(colКодОргПравФормы))
                                КодОргПравФормы = dbReader.GetInt32(colКодОргПравФормы);
                            КраткоеНазваниеРус = dbReader.GetString(colКраткоеНазваниеРус);
                            КраткоеНазваниеРусРП = dbReader.GetString(colКраткоеНазваниеРусРП);
                            КраткоеНазваниеЛат = dbReader.GetString(colКраткоеНазваниеЛат);
                            ПолноеНазвание = dbReader.GetString(colПолноеНазвание);
                            ОКОНХ = dbReader.GetString(colОКОНХ);
                            ОКВЭД = dbReader.GetString(colОКВЭД);
                            КПП = dbReader.GetString(colКПП);
                            КодЖД = dbReader.GetString(colКодЖД);
                            АдресЮридический = dbReader.GetString(colАдресЮридический);
                            АдресЮридическийЛат = dbReader.GetString(colАдресЮридическийЛат);
                            Изменил = dbReader.GetInt32(colИзменил);
                            Изменено = dbReader.GetDateTime(colИзменено);
                        }
                    }
                    else
                    {
                        Unavailable = true;
                    }
                }
            }

            /// <summary>
            ///     Получить список
            /// </summary>
            public static List<Card> GetCardJList(string query, int id)
            {
                List<Card> list = null;
                using (var dbReader = new DBReader(query, id, CommandType.Text, ConnString))
                {
                    if (dbReader.HasRows)
                    {
                        list = new List<Card>();

                        #region Получение порядкового номера столбца

                        var colКодКарточкиЮрЛица = dbReader.GetOrdinal("КодКарточкиЮрЛица");
                        var colКодЛица = dbReader.GetOrdinal("КодЛица");
                        var colОт = dbReader.GetOrdinal("От");
                        var colДо = dbReader.GetOrdinal("До");
                        var colКодОргПравФормы = dbReader.GetOrdinal("КодОргПравФормы");
                        var colКраткоеНазваниеРус = dbReader.GetOrdinal("КраткоеНазваниеРус");
                        var colКраткоеНазваниеРусРП = dbReader.GetOrdinal("КраткоеНазваниеРусРП");
                        var colКраткоеНазваниеЛат = dbReader.GetOrdinal("КраткоеНазваниеЛат");
                        var colПолноеНазвание = dbReader.GetOrdinal("ПолноеНазвание");
                        var colОКОНХ = dbReader.GetOrdinal("ОКОНХ");
                        var colОКВЭД = dbReader.GetOrdinal("ОКВЭД");
                        var colКПП = dbReader.GetOrdinal("КПП");
                        var colКодЖД = dbReader.GetOrdinal("КодЖД");
                        var colАдресЮридический = dbReader.GetOrdinal("АдресЮридический");
                        var colАдресЮридическийЛат = dbReader.GetOrdinal("АдресЮридическийЛат");
                        var colИзменил = dbReader.GetOrdinal("Изменил");
                        var colИзменено = dbReader.GetOrdinal("Изменено");

                        #endregion

                        while (dbReader.Read())
                        {
                            var row = new CardJ();
                            row.Unavailable = false;
                            if (!dbReader.IsDBNull(colКодКарточкиЮрЛица))
                                row.КодКарточкиЮрЛица = dbReader.GetInt32(colКодКарточкиЮрЛица);
                            if (!dbReader.IsDBNull(colКодЛица)) row.КодЛица = dbReader.GetInt32(colКодЛица);
                            row.От = dbReader.GetDateTime(colОт);
                            row.До = dbReader.GetDateTime(colДо);
                            if (!dbReader.IsDBNull(colКодОргПравФормы))
                                row.КодОргПравФормы = dbReader.GetInt32(colКодОргПравФормы);
                            row.КраткоеНазваниеРус = dbReader.GetString(colКраткоеНазваниеРус);
                            row.КраткоеНазваниеРусРП = dbReader.GetString(colКраткоеНазваниеРусРП);
                            row.КраткоеНазваниеЛат = dbReader.GetString(colКраткоеНазваниеЛат);
                            row.ПолноеНазвание = dbReader.GetString(colПолноеНазвание);
                            row.ОКОНХ = dbReader.GetString(colОКОНХ);
                            row.ОКВЭД = dbReader.GetString(colОКВЭД);
                            row.КПП = dbReader.GetString(colКПП);
                            row.КодЖД = dbReader.GetString(colКодЖД);
                            row.АдресЮридический = dbReader.GetString(colАдресЮридический);
                            row.АдресЮридическийЛат = dbReader.GetString(colАдресЮридическийЛат);
                            row.Изменил = dbReader.GetInt32(colИзменил);
                            row.Изменено = dbReader.GetDateTime(colИзменено);
                            list.Add(row);
                        }
                    }
                }

                return list;
            }

            #region Поля сущности

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодКарточкиЮрЛица (int, null)
            /// </value>
            public long КодКарточкиЮрЛица
            {
                get { return _кодКарточкиЮрЛица; }
                set
                {
                    _кодКарточкиЮрЛица = value;
                    Id = value.ToString();
                }
            }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодОргПравФормы (int, null)
            /// </value>
            public int КодОргПравФормы { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КраткоеНазваниеРус (nvarchar(200), not null)
            /// </value>
            public string КраткоеНазваниеРус { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КраткоеНазваниеРусРП (nvarchar(200), not null)
            /// </value>
            public string КраткоеНазваниеРусРП { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КраткоеНазваниеЛат (nvarchar(200), not null)
            /// </value>
            public string КраткоеНазваниеЛат { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ПолноеНазвание (nvarchar(300), not null)
            /// </value>
            public string ПолноеНазвание { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОКОНХ (varchar(5), not null)
            /// </value>
            public string ОКОНХ { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     ОКВЭД (varchar(8), not null)
            /// </value>
            public string ОКВЭД { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     КодЖД (varchar(35), not null)
            /// </value>
            public string КодЖД { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     Изменил (int, not null)
            /// </value>
            public int Изменил { get; set; }

            /// <summary>
            /// </summary>
            /// <value>
            ///     Изменено (datetime, not null)
            /// </value>
            public DateTime Изменено { get; set; }

            #endregion
        }

        #region Поля сущности "Лицо"

        /// <summary>
        ///     Тип лица, 1 - юридический 2 - физический
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
    }
}