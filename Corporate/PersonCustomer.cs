﻿using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности сотрудник заказчик
    /// </summary>
    [Serializable]
    public class PersonCustomer : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        public PersonCustomer(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Кличка
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        ///     КраткоеНазваниеРус
        /// </summary>
        public string ShortNameRus { get; set; }

        /// <summary>
        ///     Короткое имя на латинице
        /// </summary>
        public string ShortNameLat { get; set; }

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
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@Id", Id.ToInt()}};
            using (var dbReader =
                new DBReader(SQLQueries.SELECT_ID_ЛицоЗаказчика, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКличка = dbReader.GetOrdinal("Кличка");
                    var colКраткоеНазваниеРус = dbReader.GetOrdinal("КраткоеНазваниеРус");
                    var colShortNameLat = dbReader.GetOrdinal("КраткоеНазваниеЛат");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        if (!dbReader.IsDBNull(colКличка)) Nickname = dbReader.GetString(colКличка);
                        if (!dbReader.IsDBNull(colКраткоеНазваниеРус))
                            ShortNameRus = dbReader.GetString(colКраткоеНазваниеРус);
                        if (!dbReader.IsDBNull(colShortNameLat)) ShortNameLat = dbReader.GetString(colShortNameLat);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }
    }
}