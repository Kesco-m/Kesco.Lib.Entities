using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "ЛичныйКабинет"
    /// </summary>
    [Serializable]
    [DBSource("ЛичныеКабинеты")]
    public class PersonCabinet : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">КодЛичногоКабинета</param>
        public PersonCabinet(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки 
        /// </summary>
        public PersonCabinet()
        {
            
        }

        /// <summary>
        ///     Selenium script
        /// </summary>
        public string SeleniumScript { get; set; }

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
        ///     Подготовка связывания поля объекта КодТипаЛичногоКабинета с контролом
        /// </summary>
        public BinderValue PersonalCabinetTypeIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Название с контролом
        /// </summary>
        public BinderValue PersonalCabinetNameBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта PersonalCabinetUrl с контролом
        /// </summary>
        public BinderValue PersonalCabinetUrlBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта PersonalCabinetLogin с контролом
        /// </summary>
        public BinderValue PersonalCabinetLoginBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта PersonalCabinetPassword с контролом
        /// </summary>
        public BinderValue PersonalCabinetPasswordBind = new BinderValue();

        /// <summary>
        ///     КодТипаЛичногоКабинета (int, not null)
        /// </summary>
        [DBField("КодТипаЛичногоКабинета", 0)]
        public int PersonalCabinetType
        {
            get { return string.IsNullOrEmpty(PersonalCabinetTypeIdBind.Value) ? 0 : int.Parse(PersonalCabinetTypeIdBind.Value); }
            set { PersonalCabinetTypeIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Название
        /// </value>
        [DBField("Название")]
        public new string Name
        {
            get { return string.IsNullOrEmpty(PersonalCabinetNameBind.Value) ? "" : PersonalCabinetNameBind.Value; }
            set { PersonalCabinetNameBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Url
        /// </value>
        [DBField("Url")]
        public string Url
        {
            get { return string.IsNullOrEmpty(PersonalCabinetUrlBind.Value) ? "" : PersonalCabinetUrlBind.Value; }
            set { PersonalCabinetUrlBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Url
        /// </value>
        [DBField("Логин")]
        public string Login
        {
            get { return string.IsNullOrEmpty(PersonalCabinetLoginBind.Value) ? "" : PersonalCabinetLoginBind.Value; }
            set { PersonalCabinetLoginBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Url
        /// </value>
        [DBField("Пароль")]
        public string Password
        {
            get { return string.IsNullOrEmpty(PersonalCabinetPasswordBind.Value) ? "" : PersonalCabinetPasswordBind.Value; }
            set { PersonalCabinetPasswordBind.Value = value.Length == 0 ? "" : value; }
        }

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
        ///     Метод загрузки данных сущности "Тип личного кабинета"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", Id}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_ЛичныйКабинетПоID, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодЛичногоКабинета = dbReader.GetOrdinal("КодЛичногоКабинета");
                    var colНазвание = dbReader.GetOrdinal("Название");
                    var colКодТипаЛичногоКабинета = dbReader.GetOrdinal("КодТипаЛичногоКабинета");
                    var colUrl = dbReader.GetOrdinal("Url");
                    var colЛогин = dbReader.GetOrdinal("Логин");
                    var colПароль = dbReader.GetOrdinal("Пароль");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодЛичногоКабинета).ToString();
                        Name = dbReader.GetString(colНазвание);
                        PersonalCabinetType = dbReader.GetInt32(colКодТипаЛичногоКабинета);
                        Url = dbReader.GetString(colUrl);
                        Login = dbReader.GetString(colЛогин);
                        Password = dbReader.GetString(colПароль);
                        ChangedId = dbReader.GetInt32(colИзменил);
                        ChangedTime = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодЛичногоКабинета"].ToString();
                Name = dt.Rows[0]["Название"].ToString();
                PersonalCabinetType = Convert.ToInt32(dt.Rows[0]["Название"].ToString());
                Url = dt.Rows[0]["Название"].ToString();
                Login = dt.Rows[0]["Название"].ToString();
                Password = dt.Rows[0]["Название"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКодТипаЛичногоКабинета = dbReader.GetOrdinal("КодТипаЛичногоКабинета");
            var colТипЛичногоКабинета = dbReader.GetOrdinal("ТипЛичногоКабинета");

            Unavailable = false;

            if (!dbReader.IsDBNull(colКодТипаЛичногоКабинета)) Id = dbReader.GetInt32(colКодТипаЛичногоКабинета).ToString();
            if (!dbReader.IsDBNull(colТипЛичногоКабинета)) Name = dbReader.GetString(colТипЛичногоКабинета);
        }
    }
}