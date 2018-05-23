﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности контакты сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeContact : Entity
    {
        /// <summary>
        /// Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// КодТипаКонтакта
        /// </summary>
        public int ContactType { get; set; }
        /// <summary>
        /// ТипКонтакта
        /// </summary>
        public string ContactTypeDesc { get; set; }
        /// <summary>
        /// Контакт
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// ТелефонныйНомер
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// БыстрыйНабор
        /// </summary>
        public string QuickCall { get; set; }
        /// <summary>
        /// КодТелефоннойСтанции
        /// </summary>
        public int? PhoneCenterID { get; set; }
        /// <summary>
        /// КодТипаТелефонныхНомеров
        /// </summary>
        public int PhoneNumberTypeID { get; set; }
        /// <summary>
        /// ТипТелефонныхНомеров
        /// </summary>
        public string PhoneNumberType { get; set; }
        /// <summary>
        /// ВСправочнике
        /// </summary>
        public bool InDictionary { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        public int Order { get; set; }
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
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString; }
        }
        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;


        /// <summary>
        /// Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
         
            int colIcon = dbReader.GetOrdinal("Icon");
            int colКодТипаКонтакта = dbReader.GetOrdinal("КодТипаКонтакта");
            int colТипКонтакта = dbReader.GetOrdinal("ТипКонтакта");
            int colКонтакт = dbReader.GetOrdinal("Контакт");
            int colПримечание = dbReader.GetOrdinal("Примечание");
            int colТелефонныйНомер = dbReader.GetOrdinal("ТелефонныйНомер");
            int colБыстрыйНабор = dbReader.GetOrdinal("БыстрыйНабор");
            int colКодТелефоннойСтанции = dbReader.GetOrdinal("КодТелефоннойСтанции");
            int colКодТипаТелефонныхНомеров = dbReader.GetOrdinal("КодТипаТелефонныхНомеров");
            int colТипТелефонныхНомеров = dbReader.GetOrdinal("ТипТелефонныхНомеров");
            int colInDictionary = dbReader.GetOrdinal("ВСправочнике");
            int colOrder = dbReader.GetOrdinal("Порядок");

              
            Unavailable = false;

            if (!dbReader.IsDBNull(colIcon)) Icon = dbReader.GetString(colIcon);
            if (!dbReader.IsDBNull(colКодТипаКонтакта)) ContactType = dbReader.GetInt32(colКодТипаКонтакта);
            if (!dbReader.IsDBNull(colТипКонтакта)) ContactTypeDesc = dbReader.GetString(colТипКонтакта);
            if (!dbReader.IsDBNull(colКонтакт)) Contact = dbReader.GetString(colКонтакт);
            if (!dbReader.IsDBNull(colПримечание)) Note = dbReader.GetString(colПримечание);
            if (!dbReader.IsDBNull(colТелефонныйНомер)) PhoneNumber = dbReader.GetString(colТелефонныйНомер);
            if (!dbReader.IsDBNull(colБыстрыйНабор)) QuickCall = dbReader.GetString(colБыстрыйНабор);
            if (!dbReader.IsDBNull(colКодТелефоннойСтанции)) PhoneCenterID = dbReader.GetInt32(colКодТелефоннойСтанции);
            if (!dbReader.IsDBNull(colКодТипаТелефонныхНомеров)) PhoneNumberTypeID = dbReader.GetInt32(colКодТипаТелефонныхНомеров);
            if (!dbReader.IsDBNull(colТипТелефонныхНомеров)) PhoneNumberType = dbReader.GetString(colТипТелефонныхНомеров);
            if (!dbReader.IsDBNull(colInDictionary)) InDictionary = dbReader.GetValue(colInDictionary).ToString() =="1";
            if (!dbReader.IsDBNull(colOrder)) Order = dbReader.GetInt32(colOrder);
 

        }
    }
}
