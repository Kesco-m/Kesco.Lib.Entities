using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Атрибут шаблона IP-телефона
    /// </summary>
    [Serializable]
    [DBSource("vwШаблоныIPТелефоновАтрибуты")]
    public class VoipAttribute : Entity
    {
        /// <summary>
        ///     Конструктор сущности Атрибут шаблона IP-телефона
        /// </summary>
        public VoipAttribute()
        {
            Name = "";
            NameInTemplate = "";
            Description = "";
            DefaultValue = "";
            AvailableForUser = false;
            Group = "";
            ComputedValue = "";
        }

        /// <summary>
        ///     Конструктор сущности Атрибут шаблона IP-телефона
        /// </summary>
        public VoipAttribute(string id)
            : base(id)
        {
        }

        /// <summary>
        ///     Метод загрузки данных сущности Атрибут шаблона IP-телефона
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ТипыАтрибутовТелефонов_ID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Атрибут шаблона IP-телефона на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                Name = dt.Rows[0]["Атрибут"].ToString();
                TemplateId = dt.Rows[0]["КодШаблонаIPТелефонов"].ToString();
                Group = dt.Rows[0]["Группа"].ToString();
                NameInTemplate = dt.Rows[0]["НазваниеВШаблоне"].ToString();
                Description = dt.Rows[0]["Описание"].ToString();

                object profileNum = dt.Rows[0]["НомерПрофиля"];
                ProfileNum = !string.IsNullOrEmpty(profileNum.ToString()) ? (byte?)profileNum : null;

                object buttonNum = dt.Rows[0]["НомерКнопки"];
                ButtonNum = !string.IsNullOrEmpty(buttonNum.ToString()) ? (byte?)buttonNum : null;

                AvailableForUser = Convert.ToBoolean(dt.Rows[0]["ДоступноПользователю"]);
                DefaultValue = dt.Rows[0]["ЗначениеПоУмолчанию"].ToString();
                ComputedValue = dt.Rows[0]["ВычисляемоеЗначение"].ToString();

                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     КодШаблонаIPТелефонов
        /// </summary>
        /// <value>
        ///     КодШаблонаIPТелефонов (int, not null)
        /// </value>
        [DBField("КодШаблонаIPТелефонов")]
        public string TemplateId 
        {
            get { return TemplateIdBind.Value; }
            set { TemplateIdBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля КодШаблонаIPТелефонов для связывания с контролом
        /// </summary>
        public BinderValue TemplateIdBind = new BinderValue();

        /// <summary>
        ///     Группа
        /// </summary>
        /// <value>
        ///     Группа(nvarchar(max), not null)
        /// </value>
        [DBField("Группа")]
        public string Group
        {
            get { return GroupBind.Value; }
            set { GroupBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Название атрибута для связывания с контролом
        /// </summary>
        public BinderValue GroupBind = new BinderValue();

        /// <summary>
        ///     Название атрибута
        /// </summary>
        /// <value>
        ///     Название атрибута (varchar(100), not null)
        /// </value>
        [DBField("Атрибут")]
        public new string Name
        {
            get { return NameBind.Value; }
            set { NameBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Название атрибута для связывания с контролом
        /// </summary>
        public BinderValue NameBind = new BinderValue();

        /// <summary>
        ///     Название в шаблоне
        /// </summary>
        /// <value>
        ///     Название в шаблоне (varchar(50), not null)
        /// </value>
        [DBField("НазваниеВШаблоне")]
        public string NameInTemplate
        {
            get { return NameInTemplateBind.Value; }
            set { NameInTemplateBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Параметр в шаблоне для связывания с контролом
        /// </summary>
        public BinderValue NameInTemplateBind = new BinderValue();

        /// <summary>
        ///     Описание
        /// </summary>
        /// <value>
        ///     Описание (nvarchar(MAX), not null)
        /// </value>
        [DBField("Описание")]
        public string Description
        {
            get { return DescriptionBind.Value; }
            set { DescriptionBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Описание для связывания с контролом
        /// </summary>
        public BinderValue DescriptionBind = new BinderValue();

        /// <summary>
        ///     Номер профиля
        /// </summary>
        /// <value>
        ///     НомерПрофиля (tinyint, null)
        /// </value>
        [DBField("НомерПрофиля")]
        public byte? ProfileNum
        {
            get { return string.IsNullOrEmpty(ProfileNumBind.Value) ? (byte?)null : byte.Parse(ProfileNumBind.Value); }
            set { ProfileNumBind.Value = value == null ? string.Empty : value.ToString(); }
        }

        /// <summary>
        ///     Подготовка поля НомерПрофиля для связывания с контролом
        /// </summary>
        public BinderValue ProfileNumBind = new BinderValue();

        /// <summary>
        ///     Номер кнопки
        /// </summary>
        /// <value>
        ///     НомерКнопки (tinyint, null)
        /// </value>
        [DBField("НомерКнопки")]
        public byte? ButtonNum
        {
            get { return string.IsNullOrEmpty(ButtonNumBind.Value) ? (byte?)null : byte.Parse(ButtonNumBind.Value); }
            set { ButtonNumBind.Value = value == null ? string.Empty : value.ToString(); }
        }

        /// <summary>
        ///     Подготовка поля НомерКнопки для связывания с контролом
        /// </summary>
        public BinderValue ButtonNumBind = new BinderValue();

        /// <summary>
        ///     Доступно пользователю
        /// </summary>
        /// <value>
        ///     ДоступноПользователю (tinyint, not null)
        /// </value>
        [DBField("ДоступноПользователю")]
        public bool AvailableForUser
        {
            get { return AvailableForUserBind.Value == "1"; }
            set { AvailableForUserBind.Value = value ? "1" : "0"; }
        }

        /// <summary>
        ///     Подготовка поля ДоступноПользователю для связывания с контролом
        /// </summary>
        public BinderValue AvailableForUserBind = new BinderValue();

        /// <summary>
        ///     Значение по умолчанию
        /// </summary>
        /// <value>
        ///     ЗначениеПоУмолчанию (nvarchar(150), not null)
        /// </value>
        [DBField("ЗначениеПоУмолчанию")]
        public string DefaultValue
        {
            get { return DefaultValueBind.Value; }
            set { DefaultValueBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля ЗначениеПоУмолчанию для связывания с контролом
        /// </summary>
        public BinderValue DefaultValueBind = new BinderValue();

        /// <summary>
        ///     Вычисляемое значение
        /// </summary>
        /// <value>
        ///     ВычисляемоеЗначение (varchar(50), not null)
        /// </value>
        [DBField("ВычисляемоеЗначение")]
        public string ComputedValue
        {
            get { return ComputedValueBind.Value; }
            set { ComputedValueBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля ВычисляемоеЗначение для связывания с контролом
        /// </summary>
        public BinderValue ComputedValueBind = new BinderValue();

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
        ///     Атрибут является кнопкой
        /// </summary>
        public bool IsButton => ButtonNum.HasValue && ButtonNum.Value > 0;

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override string CN => Config.DS_user;

        /// <summary>
        ///     Метод сохранения сущности Атрибут шаблона IP-телефона
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодШаблонаIPТелефонов", TemplateId},
                {"@Группа", Group},
                {"@Атрибут", Name},
                {"@НазваниеВШаблоне", NameInTemplate},
                {"@Описание", Description},
                {"@НомерПрофиля", ProfileNum},
                {"@НомерКнопки", ButtonNum},
                {"@ДоступноПользователю", AvailableForUser},
                {"@ЗначениеПоУмолчанию", DefaultValue},
                {"@ВычисляемоеЗначение", ComputedValue}
            };

            if (IsNew)
            {
                var attributeId = DBManager.ExecuteScalar(SQLQueries.INSERT_ШаблоныIPТелефоновАтрибуты,
                    CommandType.Text, CN, sqlParams);

                if (attributeId != null)
                    Id = attributeId.ToString();
            }
            else
            {
                sqlParams.Add("@КодАтрибута", Id);

                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ШаблоныIPТелефоновАтрибуты,
                    CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Процедура удаления Атрибута шаблона IP-телефона
        /// </summary>
        public override void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодАтрибута", Id}
            };
            DBManager.ExecuteNonQuery(
                SQLQueries.DELETE_ШаблоныIPТелефоновАтрибуты, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Проверка настроен ли атрибут инженерами
        /// </summary>
        /// <returns>Возвращает true при наличии настроек</returns>
        public bool ExistsPoolAttributes()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", Id } };
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_АтрибутыТелефоновПула_Количество, CommandType.Text, Config.DS_user, sqlParams);
            return result != null && int.Parse(result.ToString()) > 0;
        }

        /// <summary>
        ///     Проверка настроен ли атрибут пользователями
        /// </summary>
        /// <returns>Возвращает true при наличии настроек</returns>
        public bool ExistsUserAttributes()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", Id } };
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_АтрибутыТелефоновПользователя_Количество, CommandType.Text, Config.DS_user, sqlParams);
            return result != null && int.Parse(result.ToString()) > 0;
        }

        /// <summary>
        ///     Удалить все настройки инженеров для данного атрибута
        /// </summary>
        public void DeletePoolAttributes()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", Id } };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ШаблоныIPТелефоновАтрибутыПулов_ПоАтрибуту, CommandType.Text, Config.DS_user, sqlParams);
        }

        /// <summary>
        ///     Удалить все пользовательские настройки данного атрибута
        /// </summary>
        public void DeleteUserAttributes()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", Id } };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ШаблоныIPТелефоновАтрибутыПользователя, CommandType.Text, Config.DS_user, sqlParams);
        }

        /// <summary>
        ///     Отключить атрибут для настройки пользователями
        /// </summary>
        public void DisableToUsers()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодАтрибута", Id } };
            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ШаблоныIPТелефоновАтрибуты_ОтключитьДляПользователей, CommandType.Text, Config.DS_user, sqlParams);
        }
    }
}