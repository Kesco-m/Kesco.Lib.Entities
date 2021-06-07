using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности ДолжностиИстория
    /// </summary>
    [Serializable]
    public class PositionHistory : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        private bool _isNew;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="dateFrom">Дата От</param>
        /// <param name="positionId">Код должности</param>
        /// <param name="commandId">Код операции</param>
        /// <param name="isNew">Признак новой записи</param>
        public PositionHistory(DateTime dateFrom, int positionId, int commandId, bool isNew = true)
            : base(string.Empty)
        {
            DateFrom = dateFrom;
            PositionId = positionId;
            CommandId = commandId;
            _isNew = isNew;

            if (!isNew)
                Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="commandId">Код операции</param>
        public PositionHistory(int positionId, int commandId)
            : this(DateTime.UtcNow, positionId, commandId)
        { }

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="commandId">Код операции</param>
        public PositionHistory(int commandId) 
            : this(DateTime.UtcNow, 0, commandId)
        { }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Признак новой записи
        /// </summary>
        public override bool IsNew => _isNew;

        /// <summary>
        ///     Метод загрузки данных сущности ДолжностиИстория
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@От", new object[] {DateFrom, DBManager.ParameterTypes.String}},
                {"@КодДолжности", new object[] {PositionId, DBManager.ParameterTypes.Int32}},
                {"@КодОперации", new object[] {CommandId, DBManager.ParameterTypes.Int32}}
            };
            FillData(DBManager.GetData(SQLQueries.SELECT_ДолжностиИстория, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности ДолжностиИстория на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                var dr = dt.Rows[0];

                PersonId = dr[_КодЛица].Equals(DBNull.Value)
                    ? (int?) null
                    : Convert.ToInt32(dr[_КодЛица]);

                DocumentId1 = dr[_КодДокумента1].Equals(DBNull.Value)
                    ? (int?) null
                    : Convert.ToInt32(dr[_КодДокумента1]);

                ValueInt1 = dr[_ЗначениеInt1].Equals(DBNull.Value)
                    ? (int?) null
                    : Convert.ToInt32(dr[_ЗначениеInt1]);

                ValueInt2 = dr[_ЗначениеInt2].Equals(DBNull.Value)
                    ? (int?) null
                    : Convert.ToInt32(dr[_ЗначениеInt2]);

                ValueInt3 = dr[_ЗначениеInt3].Equals(DBNull.Value)
                    ? (int?) null
                    : Convert.ToInt32(dr[_ЗначениеInt3]);

                ValueNvarchar1 = dr[_ЗначениеNvarchar1].ToString();
                ValueNvarchar2 = dr[_ЗначениеNvarchar2].ToString();
                ChangedBy = Convert.ToInt32(dr[_Изменил]);
                Changed = Convert.ToDateTime(dr[_Изменено]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Сохранение информации о ДолжностиИстория
        /// </summary>
        public void Save()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {$"@{_От}", DateFrom},
                {$"@{_КодДолжности}", PositionId},
                {$"@{_КодОперации}", CommandId},
                {$"@{_КодЛица}", PersonId},
                {$"@{_КодДокумента1}", DocumentId1},
                {$"@{_ЗначениеInt1}", ValueInt1},
                {$"@{_ЗначениеInt2}", ValueInt2},
                {$"@{_ЗначениеInt3}", ValueInt3},
                {$"@{_ЗначениеNvarchar1}", ValueNvarchar1 ?? string.Empty},
                {$"@{_ЗначениеNvarchar2}", ValueNvarchar2 ?? string.Empty}
            };

            if (IsNew)
            {
                var id = DBManager.ExecuteScalar(SQLQueries.INSERT_ДолжностиИстория, CommandType.Text, CN, sqlParams);
                //if (id != null)
                //    Id = id.ToString();
            }
            else
            {
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ДолжностиИстория, CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Название организации
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата вступления изменений</param>
        /// <returns></returns>
        public static string GetCompanyName(int positionId, DateTime dateFrom)
        {
            var personId = PositionCommandParameter.GetValue(positionId, dateFrom, (int)ДолжностьПараметр.КодОрганизации).ToString();
            var company = new Persons.PersonCustomer(personId);
            return company.Name;
        }

        /// <summary>
        ///     Название подразделения
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата вступления изменений</param>
        /// <returns></returns>
        public static string GetSubdivisionName(int positionId, DateTime dateFrom)
        {
            return PositionCommandParameter.GetValue(positionId, dateFrom, (int)ДолжностьПараметр.НазваниеПодразделения).ToString();
        }

        /// <summary>
        ///     Название должности
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата вступления изменений</param>
        /// <returns></returns>
        public static string GetPositionName(int positionId, DateTime dateFrom)
        {
            return PositionCommandParameter.GetValue(positionId, dateFrom, (int)ДолжностьПараметр.НазваниеДолжности).ToString();
        }

        #region Accessors fields names

        private const string _От = "От";
        private const string _КодДолжности = "КодДолжности";
        private const string _КодОперации = "КодОперации";
        private const string _КодЛица = "КодЛица";
        private const string _КодДокумента1 = "КодДокумента1";
        private const string _ЗначениеInt1 = "ЗначениеInt1";
        private const string _ЗначениеInt2 = "ЗначениеInt2";
        private const string _ЗначениеInt3 = "ЗначениеInt3";
        private const string _ЗначениеNvarchar1 = "ЗначениеNvarchar1";
        private const string _ЗначениеNvarchar2 = "ЗначениеNvarchar2";
        private const string _Изменил = "Изменил";
        private const string _Изменено = "Изменено";

        #endregion

        #region Поля сущности

        /// <summary>
        ///     От
        /// </summary>
        /// <value>
        ///     От (datetime, not null)
        /// </value>
        [DBField(_От)]
        public DateTime DateFrom
        {
            get { return string.IsNullOrEmpty(DateFromBind.Value) ? DateTime.MinValue : Convert.ToDateTime(DateFromBind.Value); }
            set { DateFromBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля От
        /// </summary>
        [BinderDBField(_От)]
        public BinderValue DateFromBind = new BinderValue();

        /// <summary>
        ///     КодДолжности
        /// </summary>
        /// <value>
        ///     КодДолжности (int, not null)
        /// </value>
        [DBField(_КодДолжности)]
        public int PositionId
        {
            get { return int.Parse(PositionIdBind.Value); }
            set { PositionIdBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодДолжности
        /// </summary>
        [BinderDBField(_КодДолжности)]
        public BinderValue PositionIdBind = new BinderValue();

        /// <summary>
        ///     КодОперации
        /// </summary>
        /// <value>
        ///     КодОперации (int, not null)
        /// </value>
        [DBField(_КодОперации)]
        public int CommandId
        {
            get { return int.Parse(CommandIdBind.Value); }
            set { CommandIdBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодОперации
        /// </summary>
        [BinderDBField(_КодОперации)]
        public BinderValue CommandIdBind = new BinderValue();

        /// <summary>
        ///     КодЛица
        /// </summary>
        /// <value>
        ///     КодЛица (int, null)
        /// </value>
        [DBField(_КодЛица)]
        public int? PersonId
        {
            get
            {
                return string.IsNullOrEmpty(PersonIdBind.Value)
                    ? (int?) null
                    : int.Parse(PersonIdBind.Value);
            }
            set { PersonIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодЛица
        /// </summary>
        [BinderDBField(_КодЛица)]
        public BinderValue PersonIdBind = new BinderValue();

        /// <summary>
        ///     КодДокумента1
        /// </summary>
        /// <value>
        ///     КодДокумента1 (int, null)
        /// </value>
        [DBField(_КодДокумента1)]
        public int? DocumentId1
        {
            get
            {
                return string.IsNullOrEmpty(DocumentId1Bind.Value)
                    ? (int?) null
                    : int.Parse(DocumentId1Bind.Value);
            }
            set { DocumentId1Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодДокумента1
        /// </summary>
        [BinderDBField(_КодДокумента1)]
        public BinderValue DocumentId1Bind = new BinderValue();

        /// <summary>
        ///     ЗначениеInt1
        /// </summary>
        /// <value>
        ///     ЗначениеInt1 (int, null)
        /// </value>
        [DBField(_ЗначениеInt1)]
        public int? ValueInt1
        {
            get
            {
                return string.IsNullOrEmpty(ValueInt1Bind.Value)
                    ? (int?) null
                    : int.Parse(ValueInt1Bind.Value);
            }
            set { ValueInt1Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ЗначениеInt1
        /// </summary>
        [BinderDBField(_ЗначениеInt1)]
        public BinderValue ValueInt1Bind = new BinderValue();

        /// <summary>
        ///     ЗначениеInt2
        /// </summary>
        /// <value>
        ///     ЗначениеInt2 (int, null)
        /// </value>
        [DBField(_ЗначениеInt2)]
        public int? ValueInt2
        {
            get
            {
                return string.IsNullOrEmpty(ValueInt2Bind.Value)
                    ? (int?) null
                    : int.Parse(ValueInt2Bind.Value);
            }
            set { ValueInt2Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ЗначениеInt2
        /// </summary>
        [BinderDBField(_ЗначениеInt2)]
        public BinderValue ValueInt2Bind = new BinderValue();

        /// <summary>
        ///     ЗначениеInt3
        /// </summary>
        /// <value>
        ///     ЗначениеInt3 (int, null)
        /// </value>
        [DBField(_ЗначениеInt3)]
        public int? ValueInt3
        {
            get
            {
                return string.IsNullOrEmpty(ValueInt3Bind.Value)
                    ? (int?) null
                    : int.Parse(ValueInt2Bind.Value);
            }
            set { ValueInt3Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ЗначениеInt3
        /// </summary>
        [BinderDBField(_ЗначениеInt3)]
        public BinderValue ValueInt3Bind = new BinderValue();

        /// <summary>
        ///     ЗначениеNvarchar1
        /// </summary>
        /// <value>
        ///     ЗначениеNvarchar1 (nvarchar(1000), null)
        /// </value>
        [DBField(_ЗначениеNvarchar1)]
        public string ValueNvarchar1
        {
            get { return ValueNvarchar1Bind.Value; }
            set { ValueNvarchar1Bind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ЗначениеNvarchar1
        /// </summary>
        [BinderDBField(_ЗначениеNvarchar1)]
        public BinderValue ValueNvarchar1Bind = new BinderValue();

        /// <summary>
        ///     ЗначениеNvarchar2
        /// </summary>
        /// <value>
        ///     ЗначениеNvarchar2 (nvarchar(1000), null)
        /// </value>
        [DBField(_ЗначениеNvarchar2)]
        public string ValueNvarchar2
        {
            get { return ValueNvarchar2Bind.Value; }
            set { ValueNvarchar2Bind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ЗначениеNvarchar2
        /// </summary>
        [BinderDBField(_ЗначениеNvarchar2)]
        public BinderValue ValueNvarchar2Bind = new BinderValue();


        /// <summary>
        ///     Изменено
        /// </summary>
        public new DateTime? Changed { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int? ChangedBy { get; set; }

        #endregion
    }
}