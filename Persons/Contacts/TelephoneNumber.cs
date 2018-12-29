using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Contacts
{
    /// <summary>
    /// Класс сущности Телефонный номер
    /// </summary>
    [Serializable]
    public class TelephoneNumber : Entity
    {
        /// <summary>
        /// Код территории
        /// </summary>
        public int? TerritoryID { get; set; }
        /// <summary>
        /// Имя территории
        /// </summary>
        public string TerritoryName { get; set; }
        /// <summary>
        /// Телефонный код страны
        /// </summary>
        public int? CountryCode { get; set; }
        /// <summary>
        /// Телефонный код города
        /// </summary>
        public int? CityCode { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Дополнительый номер
        /// </summary>
        public int? AdditionalNumber { get; set; }
        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_person) : _connectionString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;
        /// <summary>
        /// Конструктор сущности Телефонный номер
        /// </summary>
        public TelephoneNumber()
        {
            FillData(new DataTable());
        }

        /// <summary>
        /// Конструктор сущности Телефонный номер
        /// </summary>
        public TelephoneNumber(string phoneNumber)
            
        {
            PhoneNumber = phoneNumber;
            LoadWithPhoneNumber();
        }

        /// <summary>
        /// Конструктор сущности Телефонный номер
        /// </summary>
        public void FillWithTerritoryCode(int territoryId)
        {
            TerritoryID = territoryId;
            LoadWithTerritoryCode();
        }

        /// <summary>
        /// Метод загрузки имени территории по телефонному номеру
        /// </summary>
        public void GetTerritoryName()
        {
            var sqlParams = new Dictionary<string, object> { { "@phone", new object[] { CountryCode.ToString()+CityCode.ToString(), DBManager.ParameterTypes.String } } };
            var dt = DBManager.GetData(SQLQueries.SELECT_ТелефонныеКоды, CN, CommandType.Text, sqlParams);
            if (dt != null && dt.Rows.Count == 1 && CountryCode.ToString() + CityCode.ToString() == dt.Rows[0]["ТелКодСтраны"].ToString()+dt.Rows[0]["ТелКодВСтране"])
                TerritoryName = Name = dt.Rows[0]["Направление"].ToString();
        }

        /// <summary>
        /// Метод загрузки данных сущности "Тип контакта"
        /// </summary>
        public void LoadWithPhoneNumber()
        {
            var sqlParams = new Dictionary<string, object> { { "@phone", new object[] { PhoneNumber, DBManager.ParameterTypes.String } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ТелефонныеКоды, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Метод загрузки данных сущности "Тип контакта"
        /// </summary>
        private void LoadWithTerritoryCode()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { TerritoryID, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Территории_ТелефонныеКоды, CN, CommandType.Text, sqlParams));
        }

        

        /// <summary>
        /// Инициализация сущности Телефонный номер на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                string countyTel = dt.Rows[0]["ТелКодСтраны"].ToString();
                string cityTel = dt.Rows[0]["ТелКодВСтране"].ToString();

                Unavailable     = false;
                Id              = dt.Rows[0]["КодТерритории"].ToString();
                Name            = dt.Rows[0]["Направление"].ToString();
                TerritoryID     = (int?)dt.Rows[0]["КодТерритории"];
                TerritoryName   = dt.Rows[0]["Направление"].ToString();
                PhoneNumber     = dt.Rows[0]["Телефон"].ToString();

                if (!String.IsNullOrEmpty(countyTel))
                    CountryCode = Convert.ToInt32(dt.Rows[0]["ТелКодСтраны"]);
                else
                    CountryCode = null;

                if (!String.IsNullOrEmpty(cityTel))
                    CityCode = Convert.ToInt32(dt.Rows[0]["ТелКодВСтране"]);
                else
                    CityCode = null;

                
            }
            else
            {
                Unavailable     = true;
                Id              = null;
                Name            = null;
                TerritoryID     = null;
                TerritoryName   = null;
                CountryCode     = null;
                CityCode        = null;
                PhoneNumber     = null;
                AdditionalNumber = null;
            }
        }
    }
}
