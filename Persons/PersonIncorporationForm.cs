﻿using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Орг.-прав. форма
    /// </summary>
    [Serializable]
    public class PersonIncorporationForm : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор сущности Орг.-прав. форма
        /// </summary>
        public PersonIncorporationForm()
        {
        }

        /// <summary>
        ///     Конструктор сущности Орг.-прав. форма
        /// </summary>
        public PersonIncorporationForm(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Краткое название
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     Тип лица
        /// </summary>
        public byte PersonType { get; set; }

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
        ///     Метод загрузки данных сущности "Контакт"
        /// </summary>
        public sealed override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ОрганизационноПравоваяФорма, CN, CommandType.Text,
                sqlParams));
        }

        /// <summary>
        ///     Метод заполнения полей сущности из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодОргПравФормы"].ToString();
                Name = dt.Rows[0]["ОргПравФорма"].ToString();
                ShortName = dt.Rows[0]["КраткоеНазвание"] != DBNull.Value
                    ? dt.Rows[0]["КраткоеНазвание"].ToString()
                    : "";
                PersonType = Convert.ToByte(dt.Rows[0]["ТипЛица"]);
            }
            else
            {
                Unavailable = true;
                Id = null;
                Name = null;
                ShortName = null;
                PersonType = 0;
            }
        }
    }
}