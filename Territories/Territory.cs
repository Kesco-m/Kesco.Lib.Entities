using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Territories
{
    /// <summary>
    ///     Класс сущности Територия
    /// </summary>
    [Serializable]
    public class Territory : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID территории</param>
        public Territory(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID территории</param>
        /// <param name="langName">Текущий язык интерфейса</param>
        public Territory(string id, string langName) : base(id)
        {
            LangName = langName;
            Load();
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Territory()
        {
        }

        /// <summary>
        ///     Язык пользователя
        /// </summary>
        private string LangName { get; }

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
        ///     Parent
        /// </summary>
        public int? Parent { get; set; }

        /// <summary>
        ///     КодТТерритории
        /// </summary>
        [DBField("КодТТерритории")]
        public int TerritoryCode
        {
            get { return string.IsNullOrEmpty(TerritoryCodeBind.Value) ? 0 : int.Parse(TerritoryCodeBind.Value); }
            set { TerritoryCodeBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодТТерритории
        /// </summary>
        public BinderValue TerritoryCodeBind = new BinderValue();

        /// <summary>
        ///     КодОКСМ
        /// </summary>
        [DBField("КодОКСМ")]
        public int? OKCMCode
        {
            get { return string.IsNullOrEmpty(OKCMCodeBind.Value) ? (int?)null : int.Parse(OKCMCodeBind.Value); }
            set { OKCMCodeBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }
        /// <summary>
        ///     Binder для поля КодОКСМ
        /// </summary>
        public BinderValue OKCMCodeBind = new BinderValue();

        /// <summary>
        ///     Caption
        /// </summary>
        [DBField("Caption")]
        public string Caption
        {
            get { return CaptionBind.Value; }
            set { CaptionBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Caption
        /// </summary>
        public BinderValue CaptionBind = new BinderValue();

        /// <summary>
        ///     Caption
        /// </summary>
        [DBField("Caption1")]
        public string Caption1
        {
            get { return Caption1Bind.Value; }
            set { Caption1Bind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Caption1
        /// </summary>
        public BinderValue Caption1Bind = new BinderValue();

        /// <summary>
        ///     Территория
        /// </summary>
        [DBField("Территория")]
        public string TerritoryName
        {
            get { return TerritoryBind.Value; }
            set { TerritoryBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Территория
        /// </summary>
        public BinderValue TerritoryBind = new BinderValue();

        /// <summary>
        ///     Территория1
        /// </summary>
        [DBField("Территория1")]
        public string Territory1Name
        {
            get { return Territory1Bind.Value; }
            set { Territory1Bind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Территория1
        /// </summary>
        public BinderValue Territory1Bind = new BinderValue();

        /// <summary>
        ///     Аббревиатура территории
        /// </summary>
        [DBField("Аббревиатура")]
        public string Abbreviation
        {
            get { return AbbreviationBind.Value; }
            set { AbbreviationBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Аббревиатура
        /// </summary>
        public BinderValue AbbreviationBind = new BinderValue();

        /// <summary>
        ///     Направление
        /// </summary>
        [DBField("Направление")]
        public string Direction
        {
            get { return DirectionBind.Value; }
            set { DirectionBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Направление
        /// </summary>
        public BinderValue DirectionBind = new BinderValue();

        /// <summary>
        ///     Телефонный код территории
        /// </summary>
        [DBField("ТелКодСтраны")]
        public string TelephoneCode
        {
            get { return TelephoneCodeBind.Value; }
            set { TelephoneCodeBind.Value = value; }
        }
        /// <summary>
        ///     Binder для поля Телефонный код территории
        /// </summary>
        public BinderValue TelephoneCodeBind = new BinderValue();

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
        ///     Метод загрузки данных сущности "Территория"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            //FillData(DBManager.GetData(SQLQueries.SELECT_ID_Территории_Страна, CN, CommandType.Text, sqlParams));
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Территории, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Територия на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТерритории"].ToString();
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["Parent"]);
                TerritoryId = (int)dt.Rows[0]["КодТерритории"];
                TerritoryCode = (int)dt.Rows[0]["КодТТерритории"];
                Name = LangName == "ru" ? dt.Rows[0]["Территория"].ToString() : (string) dt.Rows[0]["Caption"];
                TerritoryName = (string) dt.Rows[0]["Территория"];
                Caption = (string)dt.Rows[0]["Caption"];
                Territory1Name = (string)dt.Rows[0]["Территория1"];
                Caption1 = (string)dt.Rows[0]["Caption1"];
                Abbreviation = (string) dt.Rows[0]["Аббревиатура"];
                TelephoneCode = (string)dt.Rows[0]["ТелКодСтраны"];
                OKCMCode = dt.Rows[0]["КодОКСМ"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодОКСМ"]);
                Direction = (string)dt.Rows[0]["Направление"];
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
                { "@КодТТерритории", TerritoryCode },
                { "@Территория", TerritoryName },
                { "@Caption", Caption },
                { "@Территория1", Territory1Name },
                { "@Caption1", Caption1 },
                { "@Аббревиатура", Abbreviation },
                { "@ТелКодСтраны", TelephoneCode },
                { "@КодОКСМ", OKCMCode },
                { "@Направление", Direction },
                { "@Parent", Parent }
            };

            if (IsNew)
            {
                var netId = DBManager.ExecuteScalar(SQLQueries.INSERT_Территория, CommandType.Text, Config.DS_user, sqlParams);

                if (netId != null)
                    Id = netId.ToString();
            }
            else
            {
                sqlParams.Add("@КодТерритории", Id);
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Территория, CommandType.Text, Config.DS_user, sqlParams);
            }
        }

    }
}