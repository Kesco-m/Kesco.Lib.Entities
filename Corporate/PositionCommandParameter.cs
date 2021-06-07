using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности ДолжностиПараметрыОпераций
    /// </summary>
    [Serializable]
    public class PositionCommandParameter : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="parameterId">Код параметра</param>
        /// <param name="commandId">Код операции</param>
        public PositionCommandParameter(int parameterId, int commandId)
            : base(string.Empty)
        {
            ParameterId = parameterId;
            CommandId = commandId;
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PositionCommandParameter()
        {
        }

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
        ///     Метод загрузки данных сущности ДолжностиПараметрыОпераций
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодПараметра", new object[] {ParameterId, DBManager.ParameterTypes.Int32}},
                {"@КодОперации", new object[] {CommandId, DBManager.ParameterTypes.Int32}}
            };
            FillData(
                DBManager.GetData(SQLQueries.SELECT_ID_ДолжностиПараметрыОпераций, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности ДолжностиПараметрыОпераций на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                CommandId = Convert.ToInt32(dt.Rows[0][_КодОперации]);
                DataColumn = dt.Rows[0][_Колонка].ToString();
                IsRequired = Convert.ToBoolean(dt.Rows[0][_Обязательный]);
                IsReadOnly = Convert.ToBoolean(dt.Rows[0][_ТолькоЧтение]);
                IsAutoFill = Convert.ToBoolean(dt.Rows[0][_Автозаполнение]);
                Ordinal = Convert.ToByte(dt.Rows[0][_Порядок]);
                LocalizationElement = dt.Rows[0][_ЭлементЛокализации].ToString();
                V4ControlClassName = dt.Rows[0][_V4ControlClassName].ToString();
                URLKey = dt.Rows[0][_КлючURL].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Сохранение информации о ДолжностиПараметрыОпераций
        /// </summary>
        public void Save()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {$"@{_КодПараметра}", ParameterId},
                {$"@{_КодОперации}", CommandId},
                {$"@{_Колонка}", DataColumn},
                {$"@{_Обязательный}", IsRequired},
                {$"@{_ТолькоЧтение}", IsReadOnly},
                {$"@{_Автозаполнение}", IsAutoFill},
                {$"@{_Порядок}", Ordinal},
                {$"@{_ЭлементЛокализации}", LocalizationElement},
                {$"@{_V4ControlClassName}", V4ControlClassName},
                {$"@{_КлючURL}", URLKey}
            };

            if (IsNew)
            {
                var id = DBManager.ExecuteScalar(SQLQueries.INSERT_ДолжностиПараметрыОпераций, CommandType.Text, CN,
                    sqlParams);
                if (id != null)
                    Id = id.ToString();
            }
            else
            {
                var sql = SQLQueries.UPDATE_ДолжностиПараметрыОпераций;
                DBManager.ExecuteNonQuery(sql, CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Получить актуальное значение параметра
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата вступления изменений</param>
        /// <returns></returns>
        public object GetValue(int positionId, DateTime dateFrom)
        {
            return GetValue(positionId, dateFrom, ParameterId);
        }

        /// <summary>
        ///     Получить актуальное значение параметра
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата вступления изменений</param>
        /// <param name="parameterId">Код параметра</param>
        /// <returns></returns>
        public static object GetValue(int positionId, DateTime dateFrom, int parameterId)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@От", new object[] {dateFrom, DBManager.ParameterTypes.String}},
                {"@КодДолжности", new object[] {positionId, DBManager.ParameterTypes.Int32}},
                {"@КодПараметра", new object[] { parameterId, DBManager.ParameterTypes.Int32}}
            };
            var dt = DBManager.GetData(SQLQueries.SELECT_ДолжностиЗначениеПараметра, Config.DS_user,
                CommandType.Text, sqlParams);

            return dt.Rows.Count == 1 ? dt.Rows[0][1] : DBNull.Value;
        }

        /// <summary>
        ///     Получение списка параметров операции
        /// </summary>
        /// <param name="dt">Набор данных</param>
        /// <returns></returns>
        public static List<PositionCommandParameter> GetList(DataTable dt)
        {
            return dt.AsEnumerable().Select(r => new PositionCommandParameter()
            {
                ParameterId = Convert.ToInt32(r[_КодПараметра]),
                CommandId = Convert.ToInt32(r[_КодОперации]),
                DataColumn = r[_Колонка].ToString(),
                IsRequired = Convert.ToBoolean(r[_Обязательный]),
                IsReadOnly = Convert.ToBoolean(r[_ТолькоЧтение]),
                IsAutoFill = Convert.ToBoolean(r[_Автозаполнение]),
                Ordinal = Convert.ToByte(r[_Порядок]),
                LocalizationElement = r[_ЭлементЛокализации].ToString(),
                V4ControlClassName = r[_V4ControlClassName].ToString(),
                URLKey = r[_КлючURL].ToString(),
            }).ToList();
        }

        #region Accessors fields names

        private const string _КодПараметра = "КодПараметра";
        private const string _КодОперации = "КодОперации";
        private const string _Колонка = "Колонка";
        private const string _Обязательный = "Обязательный";
        private const string _ТолькоЧтение = "ТолькоЧтение";
        private const string _Автозаполнение = "Автозаполнение";
        private const string _Порядок = "Порядок";
        private const string _ЭлементЛокализации = "ЭлементЛокализации";
        private const string _V4ControlClassName = "V4ControlClassName";
        private const string _КлючURL = "КлючURL";

        #endregion

        #region Поля сущности

        /// <summary>
        ///     КодПараметра
        /// </summary>
        /// <value>
        ///     КодПараметра (int, not null)
        /// </value>
        [DBField(_КодПараметра)]
        public int ParameterId
        {
            get { return int.Parse(ParameterIdBind.Value); }
            set { ParameterIdBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодПараметра
        /// </summary>
        public BinderValue ParameterIdBind = new BinderValue();


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
        public BinderValue CommandIdBind = new BinderValue();

        /// <summary>
        ///     Колонка
        /// </summary>
        /// <value>
        ///     Колонка (varchar(50), not null)
        /// </value>
        [DBField(_Колонка)]
        public string DataColumn
        {
            get { return DataColumnBind.Value; }
            set { DataColumnBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Колонка
        /// </summary>
        public BinderValue DataColumnBind = new BinderValue();

        /// <summary>
        ///     Обязательный
        /// </summary>
        /// <value>
        ///     Обязательный (bit, not null)
        /// </value>
        [DBField(_Обязательный)]
        public bool IsRequired
        {
            get { return bool.Parse(IsRequiredBind.Value); }
            set { IsRequiredBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Обязательный
        /// </summary>
        public BinderValue IsRequiredBind = new BinderValue();

        /// <summary>
        ///     ТолькоЧтение
        /// </summary>
        /// <value>
        ///     ТолькоЧтение (bit, not null)
        /// </value>
        [DBField(_ТолькоЧтение)]
        public bool IsReadOnly
        {
            get { return bool.Parse(IsReadOnlyBind.Value); }
            set { IsReadOnlyBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ТолькоЧтение
        /// </summary>
        public BinderValue IsReadOnlyBind = new BinderValue();

        /// <summary>
        ///     Автозаполнение
        /// </summary>
        /// <value>
        ///     Автозаполнение (bit, not null)
        /// </value>
        [DBField(_Автозаполнение)]
        public bool IsAutoFill
        {
            get { return bool.Parse(IsAutoFillBind.Value); }
            set { IsAutoFillBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Автозаполнение
        /// </summary>
        public BinderValue IsAutoFillBind = new BinderValue();

        /// <summary>
        ///     Порядок
        /// </summary>
        /// <value>
        ///     Порядок (tinyint, not null)
        /// </value>
        [DBField(_Порядок)]
        public byte Ordinal
        {
            get { return byte.Parse(OrdinalBind.Value); }
            set { OrdinalBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Порядок
        /// </summary>
        public BinderValue OrdinalBind = new BinderValue();

        /// <summary>
        ///     ЭлементЛокализации
        /// </summary>
        /// <value>
        ///     ЭлементЛокализации (varchar(50), not null)
        /// </value>
        [DBField(_ЭлементЛокализации)]
        public string LocalizationElement
        {
            get { return LocalizationElementBind.Value; }
            set { LocalizationElementBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля V4Control
        /// </summary>
        public BinderValue LocalizationElementBind = new BinderValue();

        /// <summary>
        ///     V4ControlClassName
        /// </summary>
        /// <value>
        ///     V4ControlClassName (varchar(30), not null)
        /// </value>
        [DBField(_V4ControlClassName)]
        public string V4ControlClassName
        {
            get { return V4ControlClassNameBind.Value; }
            set { V4ControlClassNameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля V4ControlClassName
        /// </summary>
        public BinderValue V4ControlClassNameBind = new BinderValue();

        /// <summary>
        ///     КлючURL
        /// </summary>
        /// <value>
        ///     КлючURL (varchar(50), not null)
        /// </value>
        [DBField(_КлючURL)]
        public string URLKey
        {
            get { return URLKeyBind.Value; }
            set { URLKeyBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ЭлементЛокализации
        /// </summary>
        public BinderValue URLKeyBind = new BinderValue();

        #endregion
    }
}