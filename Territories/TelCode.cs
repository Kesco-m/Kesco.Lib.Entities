using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Territories
{
    /// <summary>
    ///     Класс сущности Телефонный код
    /// </summary>
    [Serializable]
    public class TelCode : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID территории</param>
        public TelCode(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID телефонного кода</param>
        /// <param name="langName">Текущий язык интерфейса</param>
        public TelCode(string id, string langName) : base(id)
        {
            LangName = langName;
            Load();
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public TelCode()
        {
        }

        /// <summary>
        ///     Язык пользователя
        /// </summary>
        private string LangName { get; }

        /// <summary>
        ///     Телефонный Код
        /// </summary>
        [DBField("ТелефонныйКод")]
        public string TelefonCode
        {
            get { return TelefonCodeBind.Value; }
            set { TelefonCodeBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля ТелефонныйКод
        /// </summary>
        public BinderValue TelefonCodeBind = new BinderValue();

        /// <summary>
        ///     КодТерритории
        /// </summary>
        [DBField("КодТерритории")]
        public int TerritoryId
        {
            get { return string.IsNullOrEmpty(TerritoryIdBind.Value) ? 0 : int.Parse(TerritoryIdBind.Value); }
            set { TerritoryIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодТерритории
        /// </summary>
        public BinderValue TerritoryIdBind = new BinderValue();

        /// <summary>
        ///     ДлинаКодаОбласти
        /// </summary>
        [DBField("ДлинаКодаОбласти")]
        public int AreaCodeLength
        {
            get { return string.IsNullOrEmpty(AreaCodeLengthBind.Value) ? 0 : int.Parse(AreaCodeLengthBind.Value); }
            set { AreaCodeLengthBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля ДлинаКодаОбласти
        /// </summary>
        public BinderValue AreaCodeLengthBind = new BinderValue();

        /// <summary>
        ///     КодТипаТелефоннойСвязи
        /// </summary>
        [DBField("КодТипаТелефоннойСвязи")]
        public int TelephoneTypeId
        {
            get { return string.IsNullOrEmpty(TelephoneTypeIdBind.Value) ? 0 : int.Parse(TelephoneTypeIdBind.Value); }
            set { TelephoneTypeIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодТипаТелефоннойСвязи
        /// </summary>
        public BinderValue TelephoneTypeIdBind = new BinderValue();

        /// <summary>
        ///     Комментарий
        /// </summary>
        [DBField("Комментарий")]
        public string Comment
        {
            get { return CommentBind.Value; }
            set { CommentBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Комментарий
        /// </summary>
        public BinderValue CommentBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        public int ChangedId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        public DateTime ChangedDate { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТелефонныеКоды, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["ТелефонныйКод"].ToString();
                TelefonCode = (string)dt.Rows[0]["ТелефонныйКод"];
                TerritoryId = (int)dt.Rows[0]["КодТерритории"];
                AreaCodeLength = Convert.ToInt32(dt.Rows[0]["ДлинаКодаОбласти"]);
                TelephoneTypeId = (int)dt.Rows[0]["КодТипаТелефоннойСвязи"];
                Comment = (string) dt.Rows[0]["Комментарий"];
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedDate = (DateTime)dt.Rows[0]["Изменено"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодТерритории", TerritoryId },
                { "@ДлинаКодаОбласти", AreaCodeLength },
                { "@КодТипаТелефоннойСвязи", TelephoneTypeId },
                { "@Комментарий", Comment }
            };

            if (IsNew)
            {
                var netId = DBManager.ExecuteScalar(SQLQueries.INSERT_ТелефонныйКод, CommandType.Text, Config.DS_user, sqlParams);

                if (netId != null)
                    Id = netId.ToString();
            }
            else
            {
                sqlParams.Add("@ТелефонныйКод", Id);
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ТелефонныйКод, CommandType.Text, Config.DS_user, sqlParams);
            }
        }

    }
}