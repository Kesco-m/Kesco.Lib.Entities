using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Тип лица
    /// </summary>
    [Serializable]
    public class PersonType : Entity
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID темы</param>
        public PersonType(string id)
            : base(id)
        {
            FillData(id);
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonType()
        {
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonType(DataRow dr)
        {
            FillDataFromDataRow(dr);
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Тема лица"
        /// </summary>
        public override void Load()
        {
            FillData(Id);
        }

        /// <summary>
        ///     Метод вызывающий хранимую процедуру Создания типов лиц
        /// </summary>
        public object AddPersonType(int personID)
        {
            try
            {
                if (personID == 0) return null;
                var sqlParams = new Dictionary<string, object>(100)
                {
                    {"@WhatDo", 1},
                    {"@КодЛица", personID},
                    {"@КодТипаЛица", Id}
                };

                var outputParams = new Dictionary<string, object>();
                DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_InsDel_ТипыЛиц, CommandType.StoredProcedure, CN, sqlParams,
                    outputParams);
                Id = outputParams["@RETURN_VALUE"].ToString();

                var typeID = 0;

                if (outputParams.ContainsKey("@RETURN_VALUE"))
                    typeID = (int) outputParams["@RETURN_VALUE"];

                return typeID;
            }
            catch (Exception ex)
            {
                Logger.WriteEx(new DetailedException("Ошибка при создании типов лица", ex));
                throw ex;
            }
        }

        /// <summary>
        ///     Метод вызывающий хранимую процедуру Удаления типов лиц
        /// </summary>
        public object DeletePersonType(int personID)
        {
            try
            {
                if (personID == 0) return null;
                var sqlParams = new Dictionary<string, object>(100)
                {
                    {"@WhatDo", 2},
                    {"@КодЛица", personID},
                    {"@КодТипаЛица", Id}
                };

                var outputParams = new Dictionary<string, object>();
                DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_InsDel_ТипыЛиц, CommandType.StoredProcedure, CN, sqlParams,
                    outputParams);
                Id = outputParams["@RETURN_VALUE"].ToString();

                var typeID = 0;

                if (outputParams.ContainsKey("@RETURN_VALUE"))
                    typeID = (int) outputParams["@RETURN_VALUE"];

                return typeID;
            }
            catch (Exception ex)
            {
                Logger.WriteEx(new DetailedException("Ошибка при создании типов лица", ex));
                throw ex;
            }
        }


        /// <summary>
        ///     Инициализация сущности Тип лица на основе таблицы данных
        /// </summary>
        /// <param name="id">id сущности</param>
        public void FillData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ClearModel();
                return;
            }

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ТипЛица, id.ToInt(), CommandType.Text, CN))
            {
                FillDataFromDataReader(dbReader);
            }
        }

        /// <summary>
        ///     Инициализация сущности Формат атрибута на основе DBReader
        /// </summary>
        /// <param name="dbReader">dbReader</param>
        /// <param name="fromOutSourse">Из внешнего источника</param>
        public void FillDataFromDataReader(DBReader dbReader, bool fromOutSourse = false)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца

                var colId = dbReader.GetOrdinal("КодТипаЛица");
                var colThemeID = dbReader.GetOrdinal("КодТемыЛица");
                var colCatalogID = dbReader.GetOrdinal("КодКаталога");
                var colTypeID = dbReader.GetOrdinal("КодТипаЛица");
                var colCatalog = dbReader.GetOrdinal("Каталог");

                #endregion

                Unavailable = false;
                if (!fromOutSourse) dbReader.Read();

                Name = "";
                Id = dbReader.GetInt32(colId).ToString();
                ThemeID = new PersonTheme(dbReader.GetInt32(colThemeID).ToString());
                if (!dbReader.IsDBNull(colCatalogID)) CatalogID = dbReader.GetInt32(colCatalogID);
                if (!dbReader.IsDBNull(colTypeID)) TypeID = dbReader.GetInt32(colTypeID);
                Catalog = dbReader.GetString(colCatalog);
            }
            else
            {
                ClearModel();
            }
        }

        /// <summary>
        ///     Инициализация сущности Формат атрибута на основе таблицы данных
        /// </summary>
        /// <param name="dr">Запись данных типа лица</param>
        public void FillDataFromDataRow(DataRow dr)
        {
            if (dr != null)
            {
                Unavailable = false;
                Id = dr["КодТипаЛица"].ToString();
                ThemeID = new PersonTheme(dr["КодТемыЛица"].ToString());
                CatalogID = Convert.ToInt32(dr["КодКаталога"]);
                TypeID = Convert.ToInt32(dr["КодТипаЛица"]);
                Catalog = dr["Каталог"].ToString();
                Name = "";
            }
            else
            {
                ClearModel();
            }
        }

        /// <summary>
        ///     Метод очищает модель данных сущности "Формат атрибута по умолчанию"
        /// </summary>
        public void ClearModel()
        {
            TypeID = null;
            CatalogID = null;
            ThemeID = new PersonTheme();
            Id = "";
            Name = "";
            Unavailable = true;
            Catalog = "";
        }

        #region Свойства

        /// <summary>
        ///     КодТипаЛица
        /// </summary>
        public int? TypeID { get; set; }

        /// <summary>
        ///     КодКаталога
        /// </summary>
        public int? CatalogID { get; set; }

        /// <summary>
        ///     Каталога
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        ///     ТемаЛица
        /// </summary>
        public PersonTheme ThemeID { get; set; }

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