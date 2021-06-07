using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Шаблон конфигурации IP-телефона
    /// </summary>
    [Serializable]
    [DBSource("vwШаблоныIPТелефонов")]
    public class VoipTemplate : Entity
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public VoipTemplate()
        {
            Name = "";
            Firmware = "";
        }

        /// <summary>
        ///     Конструктор с загрузкой данных
        /// </summary>
        public VoipTemplate(string id)
            : base(id)
        {
        }

        /// <summary>
        ///     Метод загрузки данных сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public override void Load()
        {
            //var sqlParams = new Dictionary<string, object> { { "@КодШаблонаIPТелефонов", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            //FillData(DBManager.GetData(SQLQueries.SELECT_ШаблоныIPТелефонов_ID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Шаблон конфигурации IP-телефона на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Name = dt.Rows[0]["МоделиОборудования"].ToString();    
                Content = dt.Rows[0]["Шаблон"].ToString();
                Firmware = dt.Rows[0]["Прошивка"].ToString();

                object maxProfiles = dt.Rows[0]["КоличествоПрофилей"];
                MaxProfiles = !string.IsNullOrEmpty(maxProfiles.ToString()) ? (byte?)maxProfiles : null;

                object maxButtons = dt.Rows[0]["КоличествоАппаратныхКнопок"];
                MaxButtons = !string.IsNullOrEmpty(maxButtons.ToString()) ? (byte?)maxButtons : null;

                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Модели оборудования
        /// </summary>
        /// <value>
        ///     МоделиОборудования
        /// </value>
        [DBField("МоделиОборудования")]
        public new string Name { get; set; }

        /// <summary>
        ///     Содержимое шаблона конфигурации IP-телефона
        /// </summary>
        /// <value>
        ///     Шаблон (nvarchar(max), not null)
        /// </value>
        [DBField("Шаблон")]
        public string Content { get; set; }

        /// <summary>
        ///     Версия прошивки шаблона конфигурации IP-телефона
        /// </summary>
        /// <value>
        ///     Прошивка (varchar(50), not null)
        /// </value>
        [DBField("Прошивка")]
        public string Firmware { get; set; }

        /// <summary>
        ///     Количество поддерживаемых профилей
        /// </summary>
        /// <value>
        ///     КоличествоПрофилей (tinyint, null)
        /// </value>
        [DBField("КоличествоПрофилей")]
        public byte? MaxProfiles { get; set; }

        /// <summary>
        ///      Количество поддерживаемых аппаратных кнопок
        /// </summary>
        /// <value>
        ///     КоличествоАппаратныхКнопок (tinyint, null)
        /// </value>
        [DBField("КоличествоАппаратныхКнопок")]
        public byte? MaxButtons { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        [DBField("Изменил", "", false)]
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        [DBField("Изменено", "", false)]
        public DateTime ChangedTime { get; set; }


        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => Config.DS_user;

        /// <summary>
        ///     Метод сохранения сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@Шаблон", Content },
                { "@Прошивка", Firmware },
                { "@КоличествоПрофилей", MaxProfiles },
                { "@КоличествоАппаратныхКнопок", MaxButtons },
            };

            //if (IsNew)
            //{
            //    var templateId = DBManager.ExecuteScalar(SQLQueries.INSERT_ШаблоныIPТелефонов,
            //        CommandType.Text, CN, sqlParams);

            //    if (templateId != null)
            //        Id = templateId.ToString();
            //}
            //else
            //{
            //    sqlParams.Add("@КодШаблонаIPТелефонов", Id);

            //    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ШаблоныIPТелефонов,
            //        CommandType.Text, CN, sqlParams);
            //}
        }
    }
}