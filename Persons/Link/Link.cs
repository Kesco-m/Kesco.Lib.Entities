using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Link
{

    /// <summary>
    /// Класс сущности связь лиц
    /// </summary>
    [Serializable]
    public class JSONLink
    {
        /// <summary>
        /// Код лица
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// Код ссылки
        /// </summary>
        public string LinkID { get; set; }
        /// <summary>
        /// Имя ссылки
        /// </summary>
        public string LinkName { get; set; }
    }

    /// <summary>
    /// Класс сущности связь лиц
    /// </summary>
    [Serializable]
    public class Link : Entity
    {
        #region Свойтва
        /// <summary>
        /// КодТипаСвязиЛиц
        /// </summary>
        public int LinkTypeID { get; set; }
        /// <summary>
        /// От
        /// </summary>
        public DateTime? DateStart { get; set; }
        /// <summary>
        /// До
        /// </summary>
        public DateTime? DateEnd { get; set; }
        /// <summary>
        /// КодЛицаРодителя
        /// </summary>
        public int? ParentID { get; set; }
        /// <summary>
        /// КодЛицаПотомка
        /// </summary>
        public int? ChildID { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Параметр
        /// </summary>
        public int Parametr { get; set; }
        /// <summary>
        /// Изменено
        /// </summary>
        public DateTime? Changed { get; set; }
        /// <summary>
        /// Изменил
        /// </summary>
        public int? ChangedBy { get; set; }
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
        #endregion

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Link()
            
        {
            ClearModel();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID связи лиц</param>
        public Link(string id): base(id)
        {
            Id = id;
            FillData(id);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID связи лиц</param>
        /// <param name="name"></param>
        public Link(string id, string name)
            : base(id)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Метод загрузки данных сущности "связь лиц"
        /// </summary>
        public override void Load()
        {
            FillData(Id);
        }

        /// <summary>
        /// Метод создания сущности связи лиц
        /// </summary>
        public object Create()
        {
            return ExecModelOperation(0);
        }

        /// <summary>
        /// Метод редактинование сущности связи лиц
        /// </summary>
        public object Edit()
        {
            return ExecModelOperation(1);
        }

        /// <summary>
        /// Метод удаления сущности связи лиц
        /// </summary>
        public void Delete()
        {
            ExecModelOperation(2);
        }

        /// <summary>
        /// Метод вызывающий хранимую процедуру Удаления/Создания/Изменения связей лиц
        /// </summary>
        private object ExecModelOperation(int whatDo)
        {
            var sqlParams = new Dictionary<string, object>(100)
                                {
                                    {"@WhatDo", whatDo},
                                    {"@КодЛицаРодителя", ParentID},
                                    {"@КодЛицаПотомка", ChildID},
                                    {"@КодСвязиЛиц", Id},
                                    {"@КодТипаСвязиЛиц", LinkTypeID},
                                    {"@Описание", Description},
                                    {"@Параметр", Parametr}
                                };

            if(DateStart != null)
                sqlParams.Add("@От", DateStart);

            if (DateEnd != null)
                sqlParams.Add("@До", DateEnd);

            var outputParams = new Dictionary<string, object>();
            DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_InsUpdDel_СвязиЛиц, CommandType.StoredProcedure, CN, sqlParams, outputParams);
            return outputParams["@RETURN_VALUE"].ToString();
        }

        /// <summary>
        /// Инициализация сущности Формат атрибута на основе DBReader
        /// </summary>
        /// <param name="dbReader">dbReader</param>
        protected void FillDataFromDataReader(DBReader dbReader)
        {
            if (dbReader.HasRows)
            {
                #region Получение порядкового номера столбца

                int colId = dbReader.GetOrdinal("КодСвязиЛиц");
                int colName = dbReader.GetOrdinal("Описание");
                int colLinkTypeID = dbReader.GetOrdinal("КодТипаСвязиЛиц");
                int colDateStart = dbReader.GetOrdinal("От");
                int colDateEnd = dbReader.GetOrdinal("До");
                int colParentID = dbReader.GetOrdinal("КодЛицаРодителя");
                int colChildID = dbReader.GetOrdinal("КодЛицаПотомка");
                int colDescription = dbReader.GetOrdinal("Описание");
                int colParametr = dbReader.GetOrdinal("Параметр");
                int colChanged = dbReader.GetOrdinal("Изменено");
                int colChangedBy = dbReader.GetOrdinal("Изменил");
                #endregion
                Unavailable = false;
                dbReader.Read();

                Id = dbReader.GetInt32(colId).ToString();
                Name = dbReader.GetString(colName);
                LinkTypeID = dbReader.GetInt32(colLinkTypeID);
                if (!dbReader.IsDBNull(colDateStart)) { DateStart = dbReader.GetDateTime(colDateStart); }
                if (!dbReader.IsDBNull(colDateEnd)) { DateEnd = dbReader.GetDateTime(colDateEnd); }
                if (!dbReader.IsDBNull(colParentID)) { ParentID = dbReader.GetInt32(colParentID); }
                if (!dbReader.IsDBNull(colChildID)) { ChildID = dbReader.GetInt32(colChildID); }
                Description = dbReader.GetString(colDescription);
                Parametr = dbReader.GetInt32(colParametr);
                if (!dbReader.IsDBNull(colChanged)) { Changed = dbReader.GetDateTime(colChanged); }
                if (!dbReader.IsDBNull(colChangedBy)) { ChangedBy = dbReader.GetInt32(colChangedBy); }
            }
            else
            {
                ClearModel();
            }
        }

        /// <summary>
        /// Инициализация сущности связь лиц на основе ID сущности
        /// </summary>
        /// <param name="id">ID сущности</param>
        protected  void FillData(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ClearModel();
                return;
            }
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_СвязиЛиц, id.ToInt(), CommandType.Text, CN))
            {
                FillDataFromDataReader(dbReader);
            }

        }

        private void ClearModel()
        {
            Unavailable = true;
            Id = "";
            Name = null;
            LinkTypeID = 0;
            DateStart = null;
            DateEnd = null;
            ParentID = null;
            ChildID = null;
            Description = "";
            Parametr = -1;
            Changed = null;
            ChangedBy = null;
        }
    }
}
