using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Corporate.Equipments;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     Бизнес-объект - Услуги оператора
    /// </summary>
    [Serializable]
    public class ProviderService : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID сервиса</param>
        public ProviderService(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Для заполнения через datareader
        /// </summary>
        public ProviderService()
        {
            LoadedExternalProperties = new Dictionary<string, DateTime>();
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
        ///     Инициализация сущности "Расположения" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодУслуги"].ToString();
                PhoneNumber = dt.Rows[0]["НомерАбонента"].ToString();
                FIO = dt.Rows[0]["ФИОАбонента"].ToString();
                Email = dt.Rows[0]["Email"].ToString();
                Status = dt.Rows[0]["СтатусАбонента"].ToString();
                StatusChangeDate = Convert.ToDateTime(dt.Rows[0]["СтатусАбонента"].ToString());
                LastChangeReason = dt.Rows[0]["ПричинаПоследнегоИзменения"].ToString();
                Description = dt.Rows[0]["ОписаниеУслугиТарифногоПлана"].ToString();
                StartDate = Convert.ToDateTime(dt.Rows[0]["ДатаНачалаДействия"].ToString());
                EndDate = dt.Rows[0]["ДатаОкончанияДействия"] == null ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["ДатаОкончанияДействия"].ToString());
                ServiceCode = dt.Rows[0]["КодУслугиТарифногоПлана"].ToString();
                ActivationDate = Convert.ToDateTime(dt.Rows[0]["ДатаАктивацииАбонента"].ToString());
                CostInMonth = Convert.ToDecimal(dt.Rows[0]["СтоимостьВмесяц"]);
                ContractId = Convert.ToInt16(dt.Rows[0]["КодДоговора"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получение списка услуг из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<ProviderService> GetServicesList(DataTable dt)
        {
            var providerServiceList = new List<ProviderService>();

            for (var i = 0; i < dt.Rows.Count; i++)
                providerServiceList.Add(
                    new ProviderService
                    {
                        Id = dt.Rows[0]["КодУслуги"].ToString(),
                        PhoneNumber = dt.Rows[0]["НомерАбонента"].ToString(),
                        FIO = dt.Rows[0]["ФИОАбонента"].ToString(),
                        Email = dt.Rows[0]["Email"].ToString(),
                        Status = dt.Rows[0]["СтатусАбонента"].ToString(),
                        StatusChangeDate = Convert.ToDateTime(dt.Rows[0]["ДатаПоследнегоИзмененияСтатуса"].ToString()),
                        LastChangeReason = dt.Rows[0]["ПричинаПоследнегоИзменения"].ToString(),
                        Description = dt.Rows[0]["ОписаниеУслугиТарифногоПлана"].ToString(),
                        StartDate = Convert.ToDateTime(dt.Rows[0]["ДатаНачалаДействия"].ToString()),
                        EndDate = dt.Rows[0]["ДатаОкончанияДействия"] == null ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["ДатаОкончанияДействия"].ToString()),
                        ServiceCode = dt.Rows[0]["КодУслугиТарифногоПлана"].ToString(),
                        ActivationDate = Convert.ToDateTime(dt.Rows[0]["ДатаАктивацииАбонента"].ToString()),
                        CostInMonth = Convert.ToDecimal(dt.Rows[0]["СтоимостьВмесяц"]),
                        ContractId = Convert.ToInt16(dt.Rows[0]["КодДоговора"])
                    }
                );
            return providerServiceList;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Расположение"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_УслугиПровайдераПоID, CN, CommandType.Text, sqlParams));
        }

        #region Поля сущности

        /// <summary>
        ///     КодУслуги
        /// </summary>
        /// <value>
        ///     Типизированный псевданим для ID
        /// </value>
        [DBField("КодУслуги")]
        public int ServiceId => Id.ToInt();

        /// <summary>
        ///     Номер Абонента
        /// </summary>
        /// <value>
        ///     НомерАбонента (nvarchar(50), not null)
        /// </value>
        [DBField("НомерАбонента")]
        public string PhoneNumber
        {
            get { return PhoneNumberBind.Value; }
            set { PhoneNumberBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля НомерАбонента
        /// </summary>
        public BinderValue PhoneNumberBind = new BinderValue();

        /// <summary>
        ///     ФИО Абонента
        /// </summary>
        /// <value>
        ///     ФИОАбонента (nvarchar(200), not null)
        /// </value>
        [DBField("ФИОАбонента")]
        public string FIO
        {
            get { return FIOBind.Value; }
            set { FIOBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ФИОАбонента
        /// </summary>
        public BinderValue FIOBind = new BinderValue();

        /// <summary>
        ///     Email
        /// </summary>
        /// <value>
        ///     Email (nvarchar(50), not null)
        /// </value>
        [DBField("Email")]
        public string Email
        {
            get { return EmailBind.Value; }
            set { EmailBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Email
        /// </summary>
        public BinderValue EmailBind = new BinderValue();

        /// <summary>
        ///     Статус Абонента
        /// </summary>
        /// <value>
        ///     СтатусАбонента (nvarchar(50), not null)
        /// </value>
        [DBField("СтатусАбонента")]
        public string Status
        {
            get { return StatusBind.Value; }
            set { StatusBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля СтатусАбонента
        /// </summary>
        public BinderValue StatusBind = new BinderValue();

        /// <summary>
        ///     Дата последнего изменения статуса
        /// </summary>
        /// <value>
        ///     ДатаПоследнегоИзмененияСтатуса (datetime, not null)
        /// </value>
        [DBField("ДатаПоследнегоИзмененияСтатуса")]
        public DateTime StatusChangeDate
        {
            get { return Convert.ToDateTime(StatusChangeDateBind.Value); }
            set { StatusChangeDateBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ДатаПоследнегоИзмененияСтатуса
        /// </summary>
        public BinderValue StatusChangeDateBind = new BinderValue();

        /// <summary>
        ///     Причина Последнего Изменения
        /// </summary>
        /// <value>
        ///     ПричинаПоследнегоИзменения (nvarchar(200), not null)
        /// </value>
        [DBField("ПричинаПоследнегоИзменения")]
        public string LastChangeReason
        {
            get { return LastChangeReasonBind.Value; }
            set { LastChangeReasonBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ПричинаПоследнегоИзменения
        /// </summary>
        public BinderValue LastChangeReasonBind = new BinderValue();

        /// <summary>
        ///     Описание услуги тарифного плана
        /// </summary>
        /// <value>
        ///     ОписаниеУслугиТарифногоПлана (nvarchar(100), not null)
        /// </value>
        [DBField("ОписаниеУслугиТарифногоПлана")]
        public string Description
        {
            get { return DescriptionBind.Value; }
            set { DescriptionBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля ОписаниеУслугиТарифногоПлана
        /// </summary>
        public BinderValue DescriptionBind = new BinderValue();

        /// <summary>
        ///     Дата начала действия
        /// </summary>
        /// <value>
        ///     ДатаНачалаДействия (datetime, not null)
        /// </value>
        [DBField("ДатаНачалаДействия")]
        public DateTime? StartDate
        {
            get { return Convert.ToDateTime(StartDateBind.Value); }
            set { StartDateBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ДатаНачалаДействия
        /// </summary>
        public BinderValue StartDateBind = new BinderValue();


        /// <summary>
        ///     Дата окончания действия
        /// </summary>
        /// <value>
        ///     ДатаОкончанияДействия (datetime, not null)
        /// </value>
        [DBField("ДатаОкончанияДействия")]
        public DateTime? EndDate
        {
            get { return Convert.ToDateTime(EndDateBind.Value); }
            set { EndDateBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ДатаОкончанияДействия
        /// </summary>
        public BinderValue EndDateBind = new BinderValue();


        /// <summary>
        ///     Код услуги тарифного плана
        /// </summary>
        /// <value>
        ///     КодУслугиТарифногоПлана (nvarchar(10), not null)
        /// </value>
        [DBField("КодУслугиТарифногоПлана")]
        public string ServiceCode
        {
            get { return ServiceCodeBind.Value; }
            set { ServiceCodeBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля КодУслугиТарифногоПлана
        /// </summary>
        public BinderValue ServiceCodeBind = new BinderValue();


        /// <summary>
        ///     Дата Активации Абонента
        /// </summary>
        /// <value>
        ///     ДатаАктивацииАбонента (datetime, not null)
        /// </value>
        [DBField("ДатаАктивацииАбонента")]
        public DateTime? ActivationDate
        {
            get { return Convert.ToDateTime(ActivationDateBind.Value); }
            set { ActivationDateBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ДатаАктивацииАбонента
        /// </summary>
        public BinderValue ActivationDateBind = new BinderValue();


        /// <summary>
        ///     Стоимость в месяц
        /// </summary>
        /// <value>
        ///     СтоимостьВмесяц (money, null)
        /// </value>
        [DBField("КодУслугиТарифногоПлана")]
        public decimal? CostInMonth
        {
            get
            {
                return string.IsNullOrEmpty(CostInMonthBind.Value)
                    ? (int?)null
                    : int.Parse(CostInMonthBind.Value);
            }
            set { CostInMonthBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СтоимостьВмесяц
        /// </summary>
        public BinderValue CostInMonthBind = new BinderValue();


        /// <summary>
        ///     Код Договора
        /// </summary>
        /// <value>
        ///     КодДоговора (int, null)
        /// </value>
        [DBField("КодДоговора")]
        public int? ContractId
        {
            get
            {
                return string.IsNullOrEmpty(ContractIdBind.Value)
                    ? (int?)null
                    : int.Parse(ContractIdBind.Value);
            }
            set { ContractIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодДоговора
        /// </summary>
        public BinderValue ContractIdBind = new BinderValue();



        #endregion
    }
}