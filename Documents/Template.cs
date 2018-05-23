using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Бизнес-объект - шаблон печатной формы
    /// </summary>
    [Serializable]
    public sealed class Template : Entity
    {
        /// <summary>
        ///КодЛица
        /// </summary>
        public int? CodePerson { get; set; }
        /// <summary>
        ///КодКонтрагента
        /// </summary>
        public int? CodeContractor { get; set; }
        /// <summary>
        ///КодШаблонаПечатнойФормы
        /// </summary>
        public int CodeTemplate { get; set; }
        /// <summary>
        ///НазваниеШаблона
        /// </summary>
        public string NameTemplate { get; set; }
        /// <summary>
        ///НазваниеШаблонаЛат
        /// </summary>
        public string NameTemplateLat { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID Template</param>
        public Template(string id) : base(id, Web.Config.DS_document)
        {
            Load();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Template() { }

        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SQLПолучитьШаблонПоID, CN, CommandType.Text, sqlParams));
        }

        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                CodePerson = dt.Rows[0]["КодЛица"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодЛица"];
                CodeContractor = dt.Rows[0]["КодКонтрагента"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодКонтрагента"];
                CodeTemplate = (int)dt.Rows[0]["КодШаблонаПечатнойФормы"];
                NameTemplate = dt.Rows[0]["НазваниеШаблона"].ToString();
                NameTemplateLat = dt.Rows[0]["НазваниеШаблонаЛат"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}
