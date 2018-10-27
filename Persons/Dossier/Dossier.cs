using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons.Attributes;
using Kesco.Lib.Web.Settings;
using Attribute = Kesco.Lib.Entities.Persons.Attributes.Attribute;

namespace Kesco.Lib.Entities.Persons.Dossier
{
    /// <summary>
    /// Класс сущности досье
    /// </summary>
    [Serializable]
    public class Dossier : Entity
    {
        #region Свойтва
        /// <summary>
        /// Данные досье об атрибутах лица
        /// </summary>
        public List<Attribute> PersonAttributes { get; set; }
        /// <summary>
        /// Данные досье об ответсвенных сотрудниках лица
        /// </summary>
        public List<DossierContext> ResponsibleEmployes { get; set; }
        /// <summary>
        /// Данные досье о типах лица
        /// </summary>
        public List<PersonType> PersonTypes { get; set; }
        /// <summary>
        /// Данные досье о контактах лица
        /// </summary>
        public List<DossierContext> PersonContacts { get; set; }
        /// <summary>
        /// Данные досье о связях лица
        /// </summary>
        public List<DossierContext> PersonLinks { get; set; }
        /// <summary>
        /// Данные досье о складах и счетах лица
        /// </summary>
        public List<DossierContext> PersonStores { get; set; }
        /// <summary>
        /// Данные досье о складах и счетах лица
        /// </summary>
        public List<DossierContext> PersonInfo { get; set; }
        /// <summary>
        /// Лицо является физическим
        /// </summary>
        public bool IsNaturalPerson { get; set; }

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
        ///  Аонкструктор по умолчанию
        /// </summary>
        public Dossier()
        {

        }

        /// <summary>
        ///  Аонкструктор с ID лица
        /// </summary>
        public Dossier(string personID, bool showOldValues)
            : base(personID)
        {
            PersonAttributes = new List<Attribute>();
            ResponsibleEmployes = new List<DossierContext>();
            PersonTypes = new List<PersonType>();
            PersonContacts = new List<DossierContext>();
            PersonLinks = new List<DossierContext>();

            LoadPeronInfoFromDoisser(0, showOldValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionID"></param>
        /// <param name="showOldValues"></param>
        public void LoadPeronInfoFromDoisser(int sectionID, bool showOldValues)
        {
            var contextList = new List<DossierContext>();
            var sqlParams = new Dictionary<string, object> { { "@КодЛица", Id.ToInt() }, { "@DateOld", showOldValues ? 1 : 0} };
            if (sectionID != 0) sqlParams.Add("@КодВкладки", sectionID);
            using (var dbReader = new DBReader(SQLQueries.SP_Лица_Досье_Context_NEW, CommandType.StoredProcedure, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    //Unavailable = true;
                    //return;

                    Unavailable = false;
                    //Досье
                    while (dbReader.Read())
                    {
                        if (!dbReader.HasRows) continue;
                        if (!dbReader.IsDBNull(dbReader.GetOrdinal("Надпись")) &&
                            dbReader.GetString(dbReader.GetOrdinal("Надпись")) == "ТипЛица")
                        {
                            IsNaturalPerson = dbReader.GetString(dbReader.GetOrdinal("Поле")) == "2";
                        }
                        else
                        {
                            var tempContext = new DossierContext();
                            tempContext.LoadFromDbReader(dbReader);
                            contextList.Add(tempContext);
                        }

                    }
                }


                //Атрибуты
                if ((sectionID == 0 || sectionID == 41) && dbReader.NextResult())
                {
                    PersonAttributes = new List<Attribute>();
                    while (dbReader.Read())
                    {
                        if (dbReader.HasRows)
                        {
                            var tempAttribute = new Attribute();
                            tempAttribute.FillDataFromDataRow(dbReader, true);
                            PersonAttributes.Add(tempAttribute);
                        }
                    }
                }

                //Темы и типы
                if ((sectionID == 0 || sectionID == 42) && dbReader.NextResult())
                {
                    PersonTypes = new List<PersonType>();
                    while (dbReader.Read())
                    {
                        if (dbReader.HasRows)
                        {
                            var tempType = new PersonType();
                            tempType.FillDataFromDataReader(dbReader, true);
                            PersonTypes.Add(tempType);
                        }
                    }
                }

                if (PersonTypes.Count == 0 && PersonAttributes.Count == 0 && contextList.Count == 0)
                {
                    Unavailable = true;
                    return;
                }
            }

         


            if (sectionID == 0)
            {
                PersonInfo = contextList.Where(t => t.КодВкладки == (IsNaturalPerson ? 2 : 1)).Select(t => t).ToList();
                PersonContacts = contextList.Where(t => t.КодВкладки == 7 || t.КодВкладки == 8).Select(t => t).ToList();
                ResponsibleEmployes =
                    contextList.Where(t => t.КодВкладки == 3 || t.КодВкладки == 4).Select(t => t).ToList();
                PersonLinks =
                    contextList.Where(
                        t =>
                        t.КодВкладки == 11 || t.КодВкладки == 12 || t.КодВкладки == 13 || t.КодВкладки == 14 ||
                        t.КодВкладки == 15 || t.КодВкладки == 16 || t.КодВкладки == 17 || t.КодВкладки == 18).Select(
                            t => t).ToList();

                PersonStores =
                    contextList.Where(
                        t =>
                        t.КодВкладки == 19 || t.КодВкладки == 20 || t.КодВкладки == 21 || t.КодВкладки == 23 ||
                        t.КодВкладки == 40 || t.КодВкладки == 24 || t.КодВкладки == 22 || t.КодВкладки == 25).Select(
                            t => t).ToList();
            }
            else
            {

                if(sectionID == 7 || sectionID == 8)
                {
                    PersonContacts.RemoveAll(r => r.КодВкладки == sectionID);
                    PersonContacts.AddRange(contextList.Where(t => t.КодВкладки == sectionID).Select(t => t));
                }
                else if (sectionID == 3 || sectionID == 4)
                {
                    ResponsibleEmployes.RemoveAll(r => r.КодВкладки == sectionID);
                    ResponsibleEmployes.AddRange(contextList.Where(t => t.КодВкладки == sectionID).Select(t => t));
                }
                else if (sectionID == 11 || sectionID == 12 || sectionID == 13 || sectionID == 14 || sectionID == 15 || sectionID == 16 || sectionID == 17 || sectionID == 18)
                {
                    PersonLinks.RemoveAll(r => r.КодВкладки == sectionID);
                    PersonLinks.AddRange(contextList.Where(t => t.КодВкладки == sectionID).Select(t => t));
                }
                else if (sectionID == 19 || sectionID == 20 || sectionID == 21 || sectionID == 22 || sectionID == 23 || sectionID == 24 || sectionID == 25 || sectionID == 40)
                {
                    PersonStores.RemoveAll(r => r.КодВкладки == sectionID);
                    PersonStores.AddRange(contextList.Where(t => t.КодВкладки == sectionID).Select(t => t));
                }
                else if(sectionID == 1 || sectionID == 2)
                {
                    PersonInfo.RemoveAll(r => r.КодВкладки == (IsNaturalPerson ? 2 : 1));
                    PersonInfo.AddRange(contextList.Where(t => t.КодВкладки == (IsNaturalPerson ? 2 : 1)).Select(t => t));
                }
                else
                {
                    PersonInfo.RemoveAll(r => r.КодВкладки == sectionID);
                    PersonInfo.AddRange(contextList.Where(t => t.КодВкладки == sectionID).Select(t => t));
                }
            }
        }

    }
}
