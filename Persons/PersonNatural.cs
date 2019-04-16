using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Attributes;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Web;
using Kesco.Lib.Web.Settings;
using Attribute = Kesco.Lib.Entities.Persons.Attributes.Attribute;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    /// Класс сущности Лицо
    /// </summary>
    [Serializable]
    public class PersonNatural: PersonBase
    {
        #region Поля сущности "Лицо"
        /// <summary>
        /// ФИО на языке страны регистрации
        /// </summary>
        public Attribute NameReg { get; set; }
        /// <summary>
        /// ФИО на латинице
        /// </summary>
        public Attribute NameLat { get; set; }


        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PersonNatural(string id)
            : base(id)
        {
            NameReg = new Attribute();
            NameLat = new Attribute();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public PersonNatural()
        {
            NameReg = new Attribute();
            NameLat = new Attribute();

        }

        /// <summary>
        /// Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override object Create()
        {
            if(!String.IsNullOrEmpty(NameReg.AttributeValue1) && !String.IsNullOrEmpty(Name)  && String.IsNullOrEmpty(Id) && ResponsibleEmployes.Count != 0 && RegionID != 0)
            {
                //СОздание лица
                var sqlParams = new Dictionary<string, object>
                                    {
                                        {"@ВыполнятьПроверкуНаПохожиеЛица", Cheching ? 0 : 1},
                                        {"@ОтображаемоеИмя", this.Name},
                                        {"@КодБизнесПроекта", BusinessProjectID},
                                        {"@КодТерритории", RegionID},
                                        {"@Примечание", this.Note},
                                        {"@ФамилияРус", NameReg.AttributeValue1},
                                        {"@ИмяРус", NameReg.AttributeValue2},
                                        {"@ОтчествоРус", NameReg.AttributeValue3},
                                        {"@ФамилияЛат", String.IsNullOrEmpty(NameLat.AttributeValue1) ? TranslateRusToEng(NameLat.AttributeValue1) : NameLat.AttributeValue1 },
                                        {"@ИмяЛат", String.IsNullOrEmpty(NameLat.AttributeValue2) ? TranslateRusToEng(NameLat.AttributeValue2) : NameLat.AttributeValue2 },
                                        {"@ОтчествоЛат", String.IsNullOrEmpty(NameLat.AttributeValue3) ? TranslateRusToEng(NameLat.AttributeValue3) : NameLat.AttributeValue3 },
                                        {"@Пол", Sex}
                                    };
                if(BirthDate != DateTime.MinValue)
                    sqlParams.Add("@ДатаРождения", BirthDate.ToString("dd.MM.yyyy"));

                //sqlParams.Add("@ДатаРождения", DateTime.Parse(BirthDate.ToString("dd.MM.yyyy"), new CultureInfo("ru-RU")).ToString("dd.MM.yyyy"));
                //sqlParams.Add("@КодБизнесПроекта", this.IBAN);
                //sqlParams.Add("@КодДоговора", ((this.AgreementCode == 0) ? System.DBNull.Value : (object)this.AgreementCode));

                object result = DBManager.GetData(SQLQueries.SP_Лица_ФизическоеЛицо_Ins, Config.DS_person, CommandType.StoredProcedure,sqlParams);

                //object result = null;
                //DBManager.SaveData(SQLQueries.INSERT_ФизическоеЛицо, Web.Config.DS_person, CommandType.StoredProcedure, sqlParams, out result);
                int personID = 0;
                var dataTable = result as DataTable;
                if (dataTable != null && dataTable.Columns.Count == 6)
                {
                    return dataTable;
                }
                if(dataTable.Columns.Count == 1)
                {
                    personID = Convert.ToInt32(dataTable.Rows[0]["ID"]);
                }

                
                try
                {
                    
                }
                catch (Exception ex)
                {
                    Lib.Log.Logger.WriteEx(new Lib.Log.DetailedException("Ошибка при создании лица", ex));
                    throw ex;
                }

                if (personID == 0) return 0;

                //Назначение отвественных
                foreach (var responsibleEmploye in ResponsibleEmployes)
                {
                    AddPersonEmployes(responsibleEmploye.PersonEmployeeId, personID);
                }

                //Типы лиц 
                foreach (var personType in PersonTypes)
                {
                    personType.AddPersonType(personID);
                }

                //Атрибуты лица
                foreach (var personAttribute in PersonAttributes)
                {
                    if ((personAttribute.DateStart == null || personAttribute.DateStart == DateTime.MinValue) && BirthDate != null && BirthDate != DateTime.MinValue)
                        personAttribute.DateStart = BirthDate;
                    personAttribute.PersonID = personID;
                    personAttribute.Create((byte) (Cheching ? 1 : 0));
                }
                Id = personID.ToString();
                return personID;
            }
            return 0;
        }

        



        /// <summary>
        /// Инициализация сущности Лицо на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            base.FillData(dt);

            //if (dt.Rows.Count == 1)
            //{
            //    Unavailable = false;
            //    Id = dt.Rows[0]["КодЛица"].ToString();
            //    Name = dt.Rows[0]["Кличка"].ToString();
            //    Type = Convert.ToInt32(dt.Rows[0]["ТипЛица"]);
            //    BusinessProjectID = dt.Rows[0]["КодБизнесПроекта"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодБизнесПроекта"]);
            //    IsChecked = Convert.ToBoolean(dt.Rows[0]["Проверено"]);
            //    NickRL = dt.Rows[0]["КличкаRL"].ToString();
            //    NameRL = dt.Rows[0]["НазваниеRL"].ToString();
            //    RegionID = dt.Rows[0]["КодТерритории"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодТерритории"]);
            //    IsStateCompany = Convert.ToBoolean(dt.Rows[0]["ГосОрганизация"]);
            //    BirthDate = dt.Rows[0]["ДатаРождения"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["ДатаРождения"]);
            //    EndDate = dt.Rows[0]["ДатаКонца"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["ДатаКонца"]);
            //    Note = dt.Rows[0]["Примечание"] == DBNull.Value ? string.Empty : dt.Rows[0]["Примечание"].ToString();
            //}
            //else
            //{
            //    Unavailable = true;
            //}
        }


        /// <summary>
        /// Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override void Load()
        {
            base.Load();


        }


    }
}
