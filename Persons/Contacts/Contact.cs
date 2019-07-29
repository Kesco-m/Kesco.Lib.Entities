using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Contacts;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.Contacts
{
    /// <summary>
    ///     Класс сущности Контакт
    /// </summary>
    [Serializable]
    public class Contact : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор сущности Контакт
        /// </summary>
        public Contact()
        {
            FillData(new DataTable());
        }

        /// <summary>
        ///     Конструктор сущности Контакт
        /// </summary>
        public Contact(string id)
            : base(id)
        {
            Id = id;
            Load();
        }

        /// <summary>
        ///     Код лица
        /// </summary>
        public int? PersonID { get; set; }

        /// <summary>
        ///     Код связи
        /// </summary>
        public int? LinkID { get; set; }

        /// <summary>
        ///     Тип контакта
        /// </summary>
        public ContactType ContactType { get; set; }

        /// <summary>
        ///     Контакт
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        ///     Контакт RL
        /// </summary>
        public string ContactNameRL { get; set; }

        /// <summary>
        ///     Код территории
        /// </summary>
        public int? CountryCode { get; set; }

        /// <summary>
        ///     Индекс
        /// </summary>
        public int? AddressIndex { get; set; }

        /// <summary>
        ///     Регион
        /// </summary>
        public string AddressRegion { get; set; }

        /// <summary>
        ///     Город
        /// </summary>
        public string AddressCity { get; set; }

        /// <summary>
        ///     Название города на русском
        /// </summary>
        public string AddressCityRUS { get; set; }

        /// <summary>
        ///     Адрес
        /// </summary>
        public string Adress { get; set; }

        /// <summary>
        ///     Корпоративный телефон
        /// </summary>
        public int? CorporateTelephone { get; set; }

        /// <summary>
        ///     Другой контакт
        /// </summary>
        public string AnotherContact { get; set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public new DateTime? Changed { get; set; }

        /// <summary>
        ///     Телефонный номер
        /// </summary>
        public TelephoneNumber TelephoneNumber { get; set; }

        /// <summary>
        ///     Иконка типа контакта
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString =>
            string.IsNullOrEmpty(_connectionString)
                ? _connectionString = Config.DS_person
                : _connectionString;

        /// <summary>
        ///     Метод загрузки данных сущности "Контакт"
        /// </summary>
        public sealed override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Контакт, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Редактирование записи сущности Контакт
        /// </summary>
        public void EditContact()
        {
            if (string.IsNullOrEmpty(Id)) return;
            var sqlParams = GetContactParametrs();
            sqlParams.Add("@КодКонтакта", new object[] {Id, DBManager.ParameterTypes.Int32});
            var sql = LinkID != null ? SQLQueries.UPDATE_Контакт_ПоСвязи : SQLQueries.UPDATE_Контакт_ПоЛицу;
            DBManager.GetData(sql, CN, CommandType.Text, sqlParams);
        }

        /// <summary>
        ///     Создание записи сущности Контакт
        /// </summary>
        public string CreateContact()
        {
            if (!string.IsNullOrEmpty(Id)) return null;
            var sqlParams = GetContactParametrs();
            var sql = LinkID != null ? SQLQueries.INSERT_Контакт_ПоСвязи : SQLQueries.INSERT_Контакт_ПоЛицу;
            var dt = DBManager.GetData(sql, CN, CommandType.Text, sqlParams);

            if (dt != null && dt.Rows.Count == 1 && dt.Rows[0]["КодКонтакта"] != null)
                return dt.Rows[0]["КодКонтакта"].ToString();

            return null;
        }

        /// <summary>
        ///     Удаление записи сущности Контакт
        /// </summary>
        public void DeleteContact()
        {
            if (string.IsNullOrEmpty(Id)) return;
            var sqlParams = new Dictionary<string, object>
                {{"@КодКонтакта", new object[] {Id, DBManager.ParameterTypes.Int32}}};

            DBManager.GetData(SQLQueries.DELETE_ID_Контакт, CN, CommandType.Text, sqlParams);
        }

        private Dictionary<string, object> GetContactParametrs()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@АдресГородRus", new object[] {AddressCityRUS ?? "", DBManager.ParameterTypes.String});
            if (LinkID != null)
                sqlParams.Add("@КодСвязиЛиц", new object[] {LinkID, DBManager.ParameterTypes.Int32});
            else
                sqlParams.Add("@КодЛица", new object[] {PersonID, DBManager.ParameterTypes.Int32});

            sqlParams.Add("@КодТипаКонтакта", new object[] {ContactType.Id, DBManager.ParameterTypes.Int32});

            if (ContactType.Categoty == 1)
                sqlParams.Add("@КодСтраны", new object[] {CountryCode, DBManager.ParameterTypes.Int32});
            else
                sqlParams.Add("@КодСтраны", new object[] {"", DBManager.ParameterTypes.Int32});

            sqlParams.Add("@АдресИндекс",
                new object[]
                {
                    ContactType.Categoty == 1 && AddressIndex != null ? AddressIndex.ToString() : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@АдресОбласть",
                new object[]
                {
                    ContactType.Categoty == 1 && AddressRegion != null ? AddressRegion : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@АдресГород",
                new object[]
                {
                    ContactType.Categoty == 1 && AddressCity != null ? AddressCity : "", DBManager.ParameterTypes.String
                });
            sqlParams.Add("@Адрес",
                new object[]
                    {ContactType.Categoty == 1 && Adress != null ? Adress : "", DBManager.ParameterTypes.String});
            sqlParams.Add("@ТелефонСтрана",
                new object[]
                {
                    (ContactType.Categoty == 2 || ContactType.Categoty == 3) && TelephoneNumber.CountryCode != null
                        ? TelephoneNumber.CountryCode.ToString()
                        : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@ТелефонГород",
                new object[]
                {
                    (ContactType.Categoty == 2 || ContactType.Categoty == 3) && TelephoneNumber.CityCode != null
                        ? TelephoneNumber.CityCode.ToString()
                        : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@ТелефонНомер",
                new object[]
                {
                    (ContactType.Categoty == 2 || ContactType.Categoty == 3) && TelephoneNumber.PhoneNumber != null
                        ? TelephoneNumber.PhoneNumber
                        : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@ТелефонДоп",
                new object[]
                {
                    (ContactType.Categoty == 2 || ContactType.Categoty == 3) &&
                    TelephoneNumber.AdditionalNumber != null
                        ? TelephoneNumber.AdditionalNumber.ToString()
                        : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@ДругойКонтакт",
                new object[]
                {
                    (ContactType.Categoty == 4 || ContactType.Categoty == 0) && AnotherContact != null
                        ? AnotherContact
                        : "",
                    DBManager.ParameterTypes.String
                });
            sqlParams.Add("@Примечание", new object[] {Description ?? "", DBManager.ParameterTypes.String});
            return sqlParams;
        }

        /// <summary>
        ///     Инициализация сущности Контакт на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected sealed override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодКонтакта"].ToString();
                Name = dt.Rows[0]["Контакт"].ToString();
                CountryCode = dt.Rows[0]["КодСтраны"] == DBNull.Value ? null : (int?) dt.Rows[0]["КодСтраны"];
                PersonID = dt.Rows[0]["КодЛица"] == DBNull.Value ? null : (int?) dt.Rows[0]["КодЛица"];
                LinkID = dt.Rows[0]["КодСвязиЛиц"] == DBNull.Value ? null : (int?) dt.Rows[0]["КодСвязиЛиц"];
                ContactName = dt.Rows[0]["Контакт"].ToString();
                ContactNameRL = dt.Rows[0]["КонтактRL"].ToString();
                AddressIndex = ReferenceEquals(dt.Rows[0]["АдресИндекс"], "")
                    ? (int?) null
                    : Convert.ToInt32(dt.Rows[0]["АдресИндекс"]);
                AddressRegion = dt.Rows[0]["АдресОбласть"].ToString();
                AddressCity = dt.Rows[0]["АдресГород"].ToString();
                AddressCityRUS = dt.Rows[0]["АдресГородRus"].ToString();
                Adress = dt.Rows[0]["Адрес"].ToString();
                CorporateTelephone = ReferenceEquals(dt.Rows[0]["ТелефонКорпНомер"], "")
                    ? (int?) null
                    : Convert.ToInt32(dt.Rows[0]["ТелефонКорпНомер"]);
                AnotherContact = dt.Rows[0]["ДругойКонтакт"].ToString();
                Description = dt.Rows[0]["Примечание"].ToString();
                ChangedBy = (int?) dt.Rows[0]["Изменил"];
                Changed = (DateTime?) dt.Rows[0]["Изменено"];

                TelephoneNumber = new TelephoneNumber
                {
                    PhoneNumber = dt.Rows[0]["ТелефонНомер"].ToString(),
                    CountryCode =
                        ReferenceEquals(dt.Rows[0]["ТелефонСтрана"], "")
                            ? (int?) null
                            : Convert.ToInt32(dt.Rows[0]["ТелефонСтрана"]),
                    CityCode =
                        ReferenceEquals(dt.Rows[0]["ТелефонГород"], "")
                            ? (int?) null
                            : Convert.ToInt32(dt.Rows[0]["ТелефонГород"]),
                    AdditionalNumber =
                        ReferenceEquals(dt.Rows[0]["ТелефонДоп"], "")
                            ? (int?) null
                            : Convert.ToInt32(dt.Rows[0]["ТелефонДоп"])
                };

                ContactType = new ContactType(dt.Rows[0]["КодТипаКонтакта"].ToString(), "rus");
            }
            else
            {
                Unavailable = true;
                Id = null;
                Name = null;
                CountryCode = null;
                PersonID = null;
                LinkID = null;
                ContactNameRL = null;
                AddressIndex = null;
                AddressRegion = null;
                AddressCity = null;
                AddressCityRUS = null;
                Adress = null;
                CorporateTelephone = null;
                AnotherContact = null;
                Description = null;
                ChangedBy = null;
                Changed = null;
                TelephoneNumber = new TelephoneNumber();
                ContactType = new ContactType();
            }
        }

        /// <summary>
        ///     Инициализация сущности Контакт на основе строки данных
        /// </summary>
        /// <param name="dr">Строка данных места хранения</param>
        public void FillDataFromDataRow(DataRow dr)
        {
            Unavailable = false;
            Id = dr["КодКонтакта"].ToString();
            Name = dr["Контакт"].ToString();
            CountryCode = dr["КодСтраны"] == DBNull.Value ? null : (int?) dr["КодСтраны"];
            PersonID = dr["КодЛица"] == DBNull.Value ? null : (int?) dr["КодЛица"];
            LinkID = dr["КодСвязиЛиц"] == DBNull.Value ? null : (int?) dr["КодСвязиЛиц"];

            ContactNameRL = dr["КонтактRL"].ToString();
            AddressIndex = ReferenceEquals(dr["АдресИндекс"], "") ? (int?) null : Convert.ToInt32(dr["АдресИндекс"]);
            AddressRegion = dr["АдресОбласть"].ToString();
            AddressCity = dr["АдресГород"].ToString();
            AddressCityRUS = dr["АдресГородRus"].ToString();
            Adress = dr["Адрес"].ToString();
            CorporateTelephone = ReferenceEquals(dr["ТелефонКорпНомер"], "")
                ? (int?) null
                : Convert.ToInt32(dr["ТелефонКорпНомер"]);
            AnotherContact = dr["ДругойКонтакт"].ToString();
            Description = dr["Примечание"].ToString();
            ChangedBy = (int?) dr["Изменил"];
            Changed = (DateTime?) dr["Изменено"];

            TelephoneNumber = new TelephoneNumber
            {
                PhoneNumber = dr["ТелефонНомер"].ToString(),
                CountryCode =
                    ReferenceEquals(dr["ТелефонСтрана"], "") ? (int?) null : Convert.ToInt32(dr["ТелефонСтрана"]),
                CityCode = ReferenceEquals(dr["ТелефонГород"], "") ? (int?) null : Convert.ToInt32(dr["ТелефонГород"]),
                AdditionalNumber =
                    ReferenceEquals(dr["ТелефонДоп"], "") ? (int?) null : Convert.ToInt32(dr["ТелефонДоп"])
            };

            ContactType = new ContactType(dr["КодТипаКонтакта"].ToString(), "rus");
        }

        /// <summary>
        ///     Инициализация сущности Контакт на основе строки данных
        /// </summary>
        /// <param name="dr">Строка данных места хранения</param>
        public void FillDataShortFromDataRow(DataRow dr)
        {
            Unavailable = false;
            Id = dr["КодКонтакта"].ToString();
            Icon = dr["icon"].ToString();
            ContactType = new ContactType(dr["КодТипаКонтакта"].ToString(), "rus");
            Name = dr["Контакт"].ToString();
            Description = dr["Примечание"].ToString();
            Changed = (DateTime?) dr["Изменено"];
        }

        /// <summary>
        ///     Формирование строки контакта
        /// </summary>
        /// <param name="contactType">Тип контакта</param>
        /// <param name="anotherContact">Другой контакт</param>
        /// <returns>Строка контакта</returns>
        public static string FormatingContact(int contactType, string anotherContact)
        {
            return FormatingContact(contactType, "", "", "", "", "", -1, "", "", "", "", anotherContact);
        }

        /// <summary>
        ///     Формирование строки контакта
        /// </summary>
        /// <param name="contactType">Тип контакта</param>
        /// <param name="phoneCountry">Телефонный код страны</param>
        /// <param name="phoneCity">Телефонный код города</param>
        /// <param name="phoneNumber">Телефонный нмер</param>
        /// <param name="phoneAdv">Номер дополнительного набора</param>
        /// <returns>Строка контакта</returns>
        public static string FormatingContact(int contactType, string phoneCountry, string phoneCity,
            string phoneNumber,
            string phoneAdv)
        {
            return FormatingContact(contactType, "", "", "", "", "", -1, phoneCountry, phoneCity, phoneNumber, phoneAdv,
                "");
        }

        /// <summary>
        ///     Формирование строки контакта
        /// </summary>
        /// <param name="contactType">Тип контакта</param>
        /// <param name="addressIndex">Почтовый индекс</param>
        /// <param name="addressRegion">Область</param>
        /// <param name="addressCity">Город</param>
        /// <param name="addressCityRus">Город на русском</param>
        /// <param name="adress">Адрес</param>
        /// <param name="countryCode">Код страны</param>
        /// <returns>Строка контакта</returns>
        public static string FormatingContact(int contactType, string addressIndex, string addressRegion,
            string addressCity, string addressCityRus, string adress, int countryCode)
        {
            return FormatingContact(contactType, addressIndex, addressRegion, addressCity, addressCityRus, adress,
                countryCode, "", "", "", "", "");
        }

        /// <summary>
        /// </summary>
        /// <param name="contactType">Тип контакта</param>
        /// <param name="addressIndex">Почтовый индекс</param>
        /// <param name="addressRegion">Область</param>
        /// <param name="addressCity">Город</param>
        /// <param name="addressCityRus">Город на русском</param>
        /// <param name="adress">Адрес</param>
        /// <param name="countryCode">Код страны</param>
        /// <param name="phoneCountry">Телефонный код страны</param>
        /// <param name="phoneCity">Телефонный код города</param>
        /// <param name="phoneNumber">Телефонный нмер</param>
        /// <param name="phoneAdv">Номер дополнительного набора</param>
        /// <param name="anotherContact">Другой контакт</param>
        /// <returns>Строка контакта</returns>
        public static string FormatingContact(int contactType, string addressIndex, string addressRegion,
            string addressCity, string addressCityRus, string adress, int countryCode,
            string phoneCountry, string phoneCity, string phoneNumber, string phoneAdv, string anotherContact)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@ТипКонтакта", contactType},
                {"@АдресИндекс", addressIndex},
                {"@АдресОбласть", addressRegion},
                {"@АдресГород", addressCity},
                {"@АдресГородRus", addressCityRus},
                {"@Адрес", adress},
                {"@КодСтраны", countryCode},
                {"@ТелефонСтрана", phoneCountry},
                {"@ТелефонГород", phoneCity},
                {"@ТелефонНомер", phoneNumber},
                {"@ТелефонДоп", phoneAdv},
                {"@ДругойКонтакт", anotherContact}
            };

            var ret = DBManager.ExecuteScalar(SQLQueries.SELECT_FN_Лица_ФормированиеКонтакта, CommandType.Text,
                Config.DS_person, sqlParams);
            return ret == null ? "" : ret.ToString();
        }
    }
}