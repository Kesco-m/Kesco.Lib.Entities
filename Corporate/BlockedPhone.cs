using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности: заблокированный телефонный номер
    /// </summary>
    [Serializable]
    public class BlockedPhone : Entity
    {

        #region Accessors fields names

        const string _КодНомераТфОП_BlockList = "КодНомераТфОП_BlockList";
        const string _НомерТфОП = "НомерТфОП";
        const string _ПричинаБлокировки = "ПричинаБлокировки";
        const string _Создал = "Создал";
        const string _Создано = "Создано";
        const string _ВременноДо = "ВременноДо";
        const string _Изменил = "Изменил";
        const string _Изменено = "Изменено";

        #endregion

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public BlockedPhone()
        {

        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">Код заблокированного телефонного номера</param>
        public BlockedPhone(string id)
            : base(id)
        {
          
        }


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
        ///     Метод загрузки данных сущности "Черный список"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ЧерныйСписок_ТелефонныйНомер, CN, CommandType.Text,
                sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности "Черный список" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных Роли</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0][_КодНомераТфОП_BlockList].ToString();
                Name = PhoneNumber = dt.Rows[0][_НомерТфОП].ToString();
                ReasonLock = dt.Rows[0][_ПричинаБлокировки].ToString();
                CreatedBy = Convert.ToInt32(dt.Rows[0][_Создал]);
                Created = Convert.ToDateTime(dt.Rows[0][_Создано]);
                Temporarily = dt.Rows[0][_ВременноДо].Equals(DBNull.Value)
                    ? (DateTime?) null
                    : Convert.ToDateTime(dt.Rows[0][_ВременноДо]);
                ChangedBy = Convert.ToInt32(dt.Rows[0][_Изменил]);
                Changed = Convert.ToDateTime(dt.Rows[0][_Изменено]);

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
            var colID = dbReader.GetOrdinal("КодНомераТфОП_BlockList");
            var colНомерТелефона = dbReader.GetOrdinal("НомерТфОП");

            Unavailable = false;

            if (!dbReader.IsDBNull(colID)) Id = dbReader.GetInt32(colID).ToString();
            if (!dbReader.IsDBNull(colНомерТелефона)) Name = dbReader.GetString(colНомерТелефона);

        }


        #region Поля сущности

        /// <summary>
        ///     Изменено
        /// </summary>
        public new DateTime? Changed { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        /// <summary>
        ///     Создал
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        ///     КодНомераТфОП_BlockList
        /// </summary>
        /// <value>
        ///     Типизированный псевданим для ID
        /// </value>
        [DBField(_КодНомераТфОП_BlockList)]
        public int PositionId => Id.ToInt();


        /// <summary>
        ///     НомерТфОП
        /// </summary>
        /// <value>
        ///     НомерТфОП (varchar(50), not null)
        /// </value>
        [DBField(_НомерТфОП)]
        public string PhoneNumber
        {
            get { return PhoneNumberBind.Value; }
            set { PhoneNumberBind.Value = value; }
        }


        /// <summary>
        ///     Binder для поля НомерТфОП
        /// </summary>
        public BinderValue PhoneNumberBind = new BinderValue();



        /// <summary>
        ///     ПричинаБлокировки
        /// </summary>
        /// <value>
        ///     ПричинаБлокировки (varchar(300), not null)
        /// </value>
        [DBField(_ПричинаБлокировки)]
        public string ReasonLock
        {
            get { return ReasonLockBind.Value; }
            set { ReasonLockBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ПричинаБлокировки
        /// </summary>
        public BinderValue ReasonLockBind = new BinderValue();


        /// <summary>
        ///     Создано
        /// </summary>
        /// <value>
        ///     Создано (datetime, null)
        /// </value>
        [DBField(_Создано)]
        public DateTime Created
        {
            get { return ConvertExtention.Convert.Str2DateTime(CreatedBind.Value); }
            set { CreatedBind.Value = ConvertExtention.Convert.DateTime2Str(value); }
        }

        /// <summary>
        ///     Binder для поля Создано
        /// </summary>
        public BinderValue CreatedBind = new BinderValue();


        /// <summary>
        ///     ВременноДо
        /// </summary>
        /// <value>
        ///     ВременноДо (datetime, null)
        /// </value>
        [DBField(_ВременноДо)]
        public DateTime? Temporarily
        {
            get
            {
                return string.IsNullOrEmpty(TemporarilyBind.Value)
                    ? (DateTime?) null
                    : DateTime.Parse(TemporarilyBind.Value);
            }
            set { TemporarilyBind.Value = value == null ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Создано
        /// </summary>
        public BinderValue TemporarilyBind = new BinderValue();


        #endregion


        /// <summary>
        /// Сохранение информации о заблокированном телефонном номере
        /// </summary>
        /// <param name="permLock">Заблокировать постоянно</param>
        public void Save(bool permLock)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {$"@{_НомерТфОП}", PhoneNumber},
                { $"@{_ПричинаБлокировки}", ReasonLock}
            };


            if (IsNew)
            {
                var id = DBManager.ExecuteScalar(SQLQueries.INSERT_ЧерныйСписок, CommandType.Text, CN, sqlParams);

                if (id != null)
                    Id = id.ToString();
            }
            else
            {
                sqlParams.Add("@id", Id);

                var sql = permLock ? SQLQueries.UPDATE_ID_ЧерныйСписок_Постоянно : SQLQueries.UPDATE_ID_ЧерныйСписок;
                DBManager.ExecuteNonQuery(sql, CommandType.Text, CN, sqlParams);

            }
        }

        /// <summary>
        /// Разблокировать телефонный номер
        /// </summary>
        public void Delete()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {$"@id", Id}
            };

            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ID_ЧерныйСписок, CommandType.Text, CN, sqlParams);

        }
    }
}
