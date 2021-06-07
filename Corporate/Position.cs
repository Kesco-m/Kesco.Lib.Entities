using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности Должность
    /// </summary>
    [Serializable]
    public class Position : Entity
    {
        #region Accessors fields names
        const string _КодДолжности = "КодДолжности";
        const string _Должность = "Должность";
        const string _Подразделение = "Подразделение";
        const string _КодЛица = "КодЛица";
        const string _КодСотрудника = "КодСотрудника";
        const string _Совместитель = "Совместитель";
        const string _ТелефонныйНомер = "ТелефонныйНомер";
        const string _СтационарныйТелефон = "СтационарныйТелефон";
        const string _ТрубкаDECT = "ТрубкаDECT";
        const string _НеобходимаSIMКарта = "НеобходимаSIMКарта";
        const string _SIMКартаНомер = "SIMКартаНомер";
        const string _ЛимитКомпании = "ЛимитКомпании";
        const string _ПодключитьGPRSПакет = "ПодключитьGPRSПакет";
        const string _Изменил = "Изменил";
        const string _Изменено = "Изменено";
        #endregion

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID должности</param>
        public Position(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Position()
        {
            Subdivision = string.Empty;
            IsCombining = 0;
            LandlineNumber = string.Empty;
            IsStationary = 0;
            IsDect = 0;
            IsSimCardRequired = 0;
            SimCardNumber = string.Empty;
            LimitCompany = 0;
            IsGprsPackage = 0;
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
        ///     Метод загрузки данных сущности "Должность"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Должность, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Должность на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                Id = dt.Rows[0][_КодДолжности].ToString();
                Name = PositionName = dt.Rows[0][_Должность].ToString();
                Subdivision = dt.Rows[0][_Подразделение].ToString();
                PersonId = Convert.ToInt32(dt.Rows[0][_КодЛица]);
                EmployeeId = dt.Rows[0][_КодСотрудника].Equals(DBNull.Value)
                    ? (int?)null
                    : Convert.ToInt32(dt.Rows[0][_КодСотрудника]);
                IsCombining = Convert.ToByte(dt.Rows[0][_Совместитель]);
                LandlineNumber = dt.Rows[0][_ТелефонныйНомер].ToString();
                IsStationary = Convert.ToByte(dt.Rows[0][_СтационарныйТелефон]);
                IsDect = Convert.ToByte(dt.Rows[0][_ТрубкаDECT]);
                IsSimCardRequired = Convert.ToByte(dt.Rows[0][_НеобходимаSIMКарта]);
                SimCardNumber = dt.Rows[0][_SIMКартаНомер].ToString();
                LimitCompany = Convert.ToDecimal(dt.Rows[0][_ЛимитКомпании]);
                IsGprsPackage = Convert.ToByte(dt.Rows[0][_ПодключитьGPRSПакет]);
                ChangedBy = Convert.ToInt32(dt.Rows[0][_Изменил]);
                Changed = Convert.ToDateTime(dt.Rows[0][_Изменено]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Сохранение информации о должности для администратора тарифов связи
        /// </summary>
        public void Save_AdminTarif()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодДолжности", Id},
                {"@ТелефонныйНомер", LandlineNumber},
                {"@SIMКартаНомер", SimCardNumber}
            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Должности_ТарифАдмин,
                CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        /// Сохранение информации о должности
        /// </summary>
        public void Save()
        {
            var currentUser = new Employee(true);
            var sqlParams = new Dictionary<string, object>();

            var isKadrovik = currentUser.HasRole((int)BaseExtention.Enums.Corporate.Role.Кадровик, PersonId);
            var isTarifAdmin =
                currentUser.HasRole((int)BaseExtention.Enums.Corporate.Role.Администратор__тарифов_связи);

            if (isKadrovik)
            {
                sqlParams.Add($"@{_КодЛица}", PersonId);
                sqlParams.Add($"@{_Должность}", PositionName);
                sqlParams.Add($"@{_Совместитель}", IsCombining);
                sqlParams.Add($"@{_КодСотрудника}", EmployeeId);
                sqlParams.Add($"@{_НеобходимаSIMКарта}", IsSimCardRequired);
                sqlParams.Add($"@{_ЛимитКомпании}", LimitCompany);
                sqlParams.Add($"@{_ПодключитьGPRSПакет}", IsGprsPackage);
            }

            if (isTarifAdmin && !IsNew)
            {
                sqlParams.Add($"@{_ТелефонныйНомер}", LandlineNumber);
                sqlParams.Add($"@{_СтационарныйТелефон}", IsStationary);
                sqlParams.Add($"@{_ТрубкаDECT}", IsDect);
                sqlParams.Add($"@{_SIMКартаНомер}", SimCardNumber);
            }

            if (IsNew)
            {
                if (isKadrovik)
                {
                    var id = DBManager.ExecuteScalar(SQLQueries.INSERT_Должности, CommandType.Text, CN, sqlParams);

                    if (id != null)
                        Id = id.ToString();
                }
                else
                    throw new Exception("У Вас нет прав для изменения должностей!");
            }
            else
            {
                sqlParams.Add("@КодДолжности", Id);
                var sql = "";

                if (isTarifAdmin && isKadrovik)
                    sql = SQLQueries.UPDATE_Должности_All;
                else if (isTarifAdmin)
                    sql = SQLQueries.UPDATE_Должности_ТарифАдмин;
                else if (isKadrovik)
                    sql = SQLQueries.UPDATE_Должности;
                else
                    throw new Exception("У Вас нет прав для изменения должностей!");

                DBManager.ExecuteNonQuery(sql, CommandType.Text, CN, sqlParams);
            }
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
        ///     КодДолжности
        /// </summary>
        /// <value>
        ///     Типизированный псевдоним для ID
        /// </value>
        [DBField(_КодДолжности)]
        public int PositionId => Id.ToInt();

        /// <summary>
        ///     Должность
        /// </summary>
        /// <value>
        ///     Должность (nvarchar(100), not null)
        /// </value>
        [DBField(_Должность)]
        public string PositionName
        {
            get { return PositionNameBind.Value; }
            set { PositionNameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Должность
        /// </summary>
        public BinderValue PositionNameBind = new BinderValue();


        /// <summary>
        ///     Подразделение
        /// </summary>
        /// <value>
        ///     Подразделение (varchar(100), not null)
        /// </value>
        [DBField(_Подразделение)]
        public string Subdivision
        {
            get { return SubdivisionBind.Value; }
            set { SubdivisionBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Подразделение
        /// </summary>
        public BinderValue SubdivisionBind = new BinderValue();


        /// <summary>
        ///     КодЛица
        /// </summary>
        /// <value>
        ///     КодЛица (int, not null)
        /// </value>
        [DBField(_КодЛица)]
        public int PersonId
        {
            get { return int.Parse(PersonIdBind.Value); }
            set { PersonIdBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодЛица
        /// </summary>
        public BinderValue PersonIdBind = new BinderValue();


        /// <summary>
        ///     КодСотрудника
        /// </summary>
        /// <value>
        ///     КодСотрудника (int, null)
        /// </value>
        [DBField(_КодСотрудника)]
        public int? EmployeeId
        {
            get
            {
                return string.IsNullOrEmpty(EmployeeIdBind.Value)
                    ? (int?)null
                    : int.Parse(EmployeeIdBind.Value);
            }
            set { EmployeeIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодСотрудника
        /// </summary>
        public BinderValue EmployeeIdBind = new BinderValue();


        /// <summary>
        ///     Совместитель
        /// </summary>
        /// <value>
        ///     Совместитель (tinyint, not null)
        /// </value>
        [DBField(_Совместитель)]
        public byte IsCombining
        {
            get { return byte.Parse(IsCombiningBind.Value); }
            set { IsCombiningBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Совместитель
        /// </summary>
        public BinderValue IsCombiningBind = new BinderValue();

        /// <summary>
        ///     ТелефонныйНомер
        /// </summary>
        /// <value>
        ///     ТелефонныйНомер (varchar(10), not null)
        /// </value>
        [DBField(_ТелефонныйНомер)]
        public string LandlineNumber
        {
            get { return LandlineNumberBind.Value; }
            set { LandlineNumberBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ТелефонныйНомер
        /// </summary>
        public BinderValue LandlineNumberBind = new BinderValue();


        /// <summary>
        ///     СтационарныйТелефон
        /// </summary>
        /// <value>
        ///     СтационарныйТелефон (tinyint, not null)
        /// </value>
        [DBField(_СтационарныйТелефон)]
        public byte IsStationary
        {
            get { return byte.Parse(IsStationaryBind.Value); }
            set { IsStationaryBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СтационарныйТелефон
        /// </summary>
        public BinderValue IsStationaryBind = new BinderValue();


        /// <summary>
        ///     ТрубкаDECT
        /// </summary>
        /// <value>
        ///     ТрубкаDECT (tinyint, not null)
        /// </value>
        [DBField(_ТрубкаDECT)]
        public byte IsDect
        {
            get { return byte.Parse(IsDectBind.Value); }
            set { IsDectBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ТрубкаDECT
        /// </summary>
        public BinderValue IsDectBind = new BinderValue();


        /// <summary>
        ///     НеобходимаSIMКарта
        /// </summary>
        /// <value>
        ///     НеобходимаSIMКарта (tinyint, not null)
        /// </value>
        [DBField(_НеобходимаSIMКарта)]
        public byte IsSimCardRequired
        {
            get { return byte.Parse(IsSimCardRequiredBind.Value); }
            set { IsSimCardRequiredBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля НеобходимаSIMКарта
        /// </summary>
        public BinderValue IsSimCardRequiredBind = new BinderValue();


        /// <summary>
        ///     SIMКартаНомер
        /// </summary>
        /// <value>
        ///     SIMКартаНомер (varchar(10), not null)
        /// </value>
        [DBField(_SIMКартаНомер)]
        public string SimCardNumber
        {
            get { return SimCardNumberBind.Value; }
            set { SimCardNumberBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля SIMКартаНомер
        /// </summary>
        public BinderValue SimCardNumberBind = new BinderValue();


        /// <summary>
        ///     ЛимитКомпании
        /// </summary>
        /// <value>
        ///     ЛимитКомпании (money, not null)
        /// </value>
        [DBField(_ЛимитКомпании)]
        public decimal LimitCompany
        {
            get { return Kesco.Lib.ConvertExtention.Convert.Str2Decimal(LimitCompanyBind.Value); }
            set { LimitCompanyBind.Value = ConvertExtention.Convert.Decimal2Str(value, 2); }
        }

        /// <summary>
        ///     Binder для поля ЛимитКомпании
        /// </summary>
        public BinderValue LimitCompanyBind = new BinderValue();


        /// <summary>
        ///     ПодключитьGPRSПакет
        /// </summary>
        /// <value>
        ///     ПодключитьGPRSПакет (tinyint, not null)
        /// </value>
        [DBField(_ПодключитьGPRSПакет)]
        public byte IsGprsPackage
        {
            get { return byte.Parse(IsGprsPackageBind.Value); }
            set { IsGprsPackageBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ПодключитьGPRSПакет
        /// </summary>
        public BinderValue IsGprsPackageBind = new BinderValue();
        #endregion
    }
}