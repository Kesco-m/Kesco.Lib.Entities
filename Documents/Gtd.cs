using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Gtd
{
    /// <summary>
    ///     Класс сущности "ГТД"
    /// </summary>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Number = {Number}, Desc = {Description}")]
    public class Gtd : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Байдинг поля "Description"
        /// </summary>
        public BinderValue DescriptionBinder = new BinderValue();

        /// <summary>
        ///     Документ основание
        /// </summary>
        public BinderValue NumberBind = new BinderValue();

        /// <summary>
        ///     ID. Поле КодДокумента
        /// </summary>
        public int DocId => Id.ToInt();

        /// <summary>
        ///     Номер документа
        /// </summary>
        public string Number
        {
            get { return NumberBind.Value; }
            set { NumberBind.Value = value; }
        }

        /// <summary>
        ///     Дата документа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Описание
        /// </summary>
        public string Description
        {
            get { return DescriptionBinder.Value; }
            set { DescriptionBinder.Value = value; }
        }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int ChangePersonID { get; set; }

        /// <summary>
        ///     Дата изменения
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        ///     Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_document
            : _connectionString;

        /// <summary>
        ///     Получение списка документов из таблицы данных
        /// </summary>
        /// <param name="query">Запрос на множество строк Document</param>
        public static List<Gtd> GetGTDList(string query)
        {
            var docsList = new List<Gtd>();

            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    var colНомерДокумента = dbReader.GetOrdinal("НомерДокумента");
                    var colДатаДокумента = dbReader.GetOrdinal("ДатаДокумента");
                    var colОписание = dbReader.GetOrdinal("Описание");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var docRow = new Gtd
                        {
                            Unavailable = false,
                            Id = dbReader.GetInt32(colКодДокумента).ToString(),
                            Number = dbReader.GetString(colНомерДокумента),
                            Description = dbReader.GetString(colОписание),
                            ChangePersonID = dbReader.GetInt32(colИзменил),
                            ChangeDate = dbReader.GetDateTime(colИзменено)
                        };

                        if (!dbReader.IsDBNull(colДатаДокумента)) docRow.Date = dbReader.GetDateTime(colДатаДокумента);

                        docsList.Add(docRow);
                    }
                }
            }

            return docsList;
        }
    }
}