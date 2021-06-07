using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;


namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности Расписание дежурного инженера
    /// </summary>
    [Serializable]
    public class DutyTiming : Entity
    {
        /// <summary>
        /// Ошибка при загрузке Entity
        /// </summary>
        public Exception EntityLoadException;

        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">DutyTiming</param>
        public DutyTiming(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public DutyTiming()
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
        ///     Метод загрузки данных сущности "Доменные имена"
        /// </summary>
        public override void Load()
        {
            var idODBC = Convert.ToDateTime(Id).ToString("yyyyMMdd");
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] { idODBC, DBManager.ParameterTypes.String}}};

            try
            {
                FillData(DBManager.GetData(SQLQueries.SELECT_ID_ДежурныеИнженерыРасписание, CN, CommandType.Text, sqlParams));
            }
            catch (Exception ex) {
                EntityLoadException = ex;
            }
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
        ///     Дата дежурства
        /// </summary>
        [DBField("Дата")]
        public DateTime DutyDate
        {
            get { return string.IsNullOrEmpty(DutyDateBind.Value) ? DateTime.MinValue : Convert.ToDateTime(DutyDateBind.Value); }
            set { DutyDateBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля DutyDate
        /// </summary>
        public BinderValue DutyDateBind = new BinderValue();

        /// <summary>
        ///     КодСотрудника
        /// </summary>
        [DBField("КодСотрудника")]
        public int EmployeeId
        {
            get { return string.IsNullOrEmpty(EmployeeIdBind.Value) ? 0 : int.Parse(EmployeeIdBind.Value); }
            set { EmployeeIdBind.Value = value > 0 ? value.ToString() : string.Empty; }
        }

        /// <summary>
        ///     Binder для поля КодСотрудника
        /// </summary>
        public BinderValue EmployeeIdBind = new BinderValue();


        /// <summary>
        ///     КодСотрудникаФакт
        /// </summary>
        [DBField("КодСотрудникаФакт")]
        public int? EmployeeIdFact
        {
            get {
                if (string.IsNullOrEmpty(EmployeeIdFactBind.Value))
                    return null;
                else
                    return int.Parse(EmployeeIdFactBind.Value); 
            }
            set { EmployeeIdFactBind.Value = value > 0 ? value.ToString() : string.Empty; }

        }

        /// <summary>
        ///     Binder для поля КодСотрудника
        /// </summary>
        public BinderValue EmployeeIdFactBind = new BinderValue();


        /// <summary>
        ///     КодЛицаРаботодателяФакт
        /// </summary>
        [DBField("КодЛицаРаботодателяФакт")]
        public int? PersonIdFact { get; set; }

        /// <summary>
        ///     РабочийДень
        /// </summary>
        [DBField("РабочийДень")]
        public byte? WorkDay { get; set; }

        /// <summary>
        /// Дата дежурства является выходным
        /// </summary>
        public bool IsHoliday => WorkDay.HasValue && WorkDay != 1;

        /// <summary>
        ///     Оплата
        /// </summary>
        [DBField("Оплата")]
        public decimal? Payment { get; set; }




        /// <summary>
        ///     Дата отгула
        /// </summary>
        [DBField("ДатаОтгула")]
        public DateTime? DateOff
        {
            get { return string.IsNullOrEmpty(DutyDateOffBind.Value) ? DateTime.MinValue : Convert.ToDateTime(DutyDateOffBind.Value); }
            set { DutyDateOffBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля DutyDate
        /// </summary>
        public BinderValue DutyDateOffBind = new BinderValue();


        /// <summary>
        ///     Закрыто
        /// </summary>
        [DBField("Закрыто")]
        public byte Closed { get; set; }

        /// <summary>
        /// Дежурство закрыто
        /// </summary>
        public bool IsClosed => Closed == 1;

        /// <summary>
        /// Можно закрыть дежурство
        /// </summary>
        public bool CanClosed => !IsNew && !IsClosed && DutyDate < DateTime.Today;

        /// <summary>
        /// Как будут закрывать дежурство
        /// </summary>
        public BaseExtention.Enums.Corporate.ВариантыЗакрытияДежурства ClosedType { get; set; }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colId = dbReader.GetOrdinal("Дата");
            var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
            var colКодСотрудникаФакт = dbReader.GetOrdinal("КодСотрудникаФакт");
            var colКодЛицаРаботодателяФакт = dbReader.GetOrdinal("КодЛицаРаботодателяФакт");
            var colРабочийДень = dbReader.GetOrdinal("РабочийДень");
            var colОплата = dbReader.GetOrdinal("Оплата");
            var colДатаОтгула = dbReader.GetOrdinal("ДатаОтгула");
            var colЗакрыто = dbReader.GetOrdinal("Закрыто");

            var colИзменил = dbReader.GetOrdinal("Изменил");
            var colИзменено = dbReader.GetOrdinal("Изменено");

            Unavailable = false;

            if (!dbReader.IsDBNull(colId))
            {
                DutyDate = dbReader.GetDateTime(colId);
                Id = DutyDate.ToString("yyyyMMdd");
            }

            if (!dbReader.IsDBNull(colКодСотрудника)) EmployeeId = dbReader.GetInt32(colКодСотрудника);
            if (!dbReader.IsDBNull(colКодСотрудникаФакт)) EmployeeIdFact = dbReader.GetInt32(colКодСотрудникаФакт);
            if (!dbReader.IsDBNull(colКодЛицаРаботодателяФакт)) PersonIdFact = dbReader.GetInt32(colКодЛицаРаботодателяФакт);
            if (!dbReader.IsDBNull(colРабочийДень)) WorkDay = dbReader.GetByte(colРабочийДень);
            if (!dbReader.IsDBNull(colОплата)) Payment = dbReader.GetDecimal(colОплата);
            if (!dbReader.IsDBNull(colДатаОтгула)) DateOff = dbReader.GetDateTime(colДатаОтгула);
            if (!dbReader.IsDBNull(colЗакрыто)) Closed = dbReader.GetByte(colЗакрыто);

            ChangedId = dbReader.GetInt32(colИзменил);
            ChangedTime = dbReader.GetDateTime(colИзменено);

            SetClosedType();

        }


        /// <summary>
        ///     Инициализация сущности ДежурныеИнженерыРасписание на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                DutyDate = Convert.ToDateTime(dt.Rows[0]["Дата"]);
                Id = DutyDate.ToString("yyyyMMdd");

                EmployeeId = Convert.ToInt32(dt.Rows[0]["КодСотрудника"]);

                if (dt.Rows[0]["КодСотрудникаФакт"] == DBNull.Value)
                    EmployeeIdFact = null;
                else
                    EmployeeIdFact = Convert.ToInt32(dt.Rows[0]["КодСотрудникаФакт"]);


                if (dt.Rows[0]["КодЛицаРаботодателяФакт"] == DBNull.Value)
                    PersonIdFact = null;
                else
                    PersonIdFact = Convert.ToInt32(dt.Rows[0]["КодЛицаРаботодателяФакт"]);

                if (dt.Rows[0]["РабочийДень"] == DBNull.Value)
                    WorkDay = null;
                else
                    WorkDay = Convert.ToByte(dt.Rows[0]["РабочийДень"]);

                if (dt.Rows[0]["Оплата"] == DBNull.Value)
                    Payment = null;
                else
                    Payment = Convert.ToDecimal(dt.Rows[0]["Оплата"]);


                if (dt.Rows[0]["ДатаОтгула"] == DBNull.Value)
                    DateOff = null;
                else
                    DateOff = Convert.ToDateTime(dt.Rows[0]["ДатаОтгула"]);


                Closed = Convert.ToByte(dt.Rows[0]["Закрыто"]);

                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"]);

                SetClosedType();
            }
            else
            {
                Unavailable = true;
            }

        }

        /// <summary>
        /// Добавление расписания
        /// </summary>
        public void SaveInsert()
        {
            var sqlParams = new Dictionary<string, object>() {

                {"@Дата", DutyDate},
                {"@КодСотрудника", EmployeeId }

            };

            DBManager.ExecuteNonQuery(SQLQueries.INSERT_ДежурныеИнженерыРасписание, CommandType.Text, Config.DS_user, sqlParams);

        }

        /// <summary>
        /// Обновление расписания - только для будущих записей и только инженер
        /// </summary>
        public void SaveUpdateSimple()
        {
            var sqlParams = new Dictionary<string, object>() {

                {"@Дата", DutyDate},
                {"@КодСотрудника", EmployeeId }

            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ДежурныеИнженерыРасписание_Инженер, CommandType.Text, Config.DS_user, sqlParams); ;

        }

        /// <summary>
        /// Обновление расписания - только состоявшееся дежурство
        /// </summary>
        public void SaveUpdateFact(bool close)
        {
            var sqlParams = new Dictionary<string, object>() {
                
                {"@Дата", DutyDate},
                {"@Закрыто", close?1:0},
                {"@КодСотрудника", EmployeeId},
                {"@КодСотрудникаФакт", EmployeeIdFact},
                {"@ДатаОтгула", DateOff},
                {"@РабочийДень", WorkDay},
                {"@ТипЗакрытия", ClosedType},
            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ДежурныеИнженерыРасписание, CommandType.Text, Config.DS_user, sqlParams); ;
        }



        /// <summary>
        /// Дата ближайшего дня, когда нет дежурства
        /// </summary>
        public DateTime? DateOfNextDuty
        {
            get
            {
                try
                {
                    var dt = DBManager.GetData(SQLQueries.SELECT_ДежурныеИнженеры_НетДежурстваНаСледующийДень, Config.DS_user);
                    if (dt.Rows.Count != 1) return null;
                    var d = Convert.ToDateTime(dt.Rows[0][0]);
                    d = d.AddDays(1);
                    return d;
                }
                catch(Exception ex)
                {
                    EntityLoadException = ex;
                }

                return DateTime.Today;
            }
        }

        private void SetClosedType()
        {

            if (CanClosed || IsClosed)
            {
                if (EmployeeIdFact.HasValue && EmployeeId == EmployeeIdFact.Value)
                    ClosedType = BaseExtention.Enums.Corporate.ВариантыЗакрытияДежурства.ЗамечанийКДежурствуНет;
                else if (EmployeeIdFact.HasValue && EmployeeId != EmployeeIdFact.Value)
                    ClosedType = BaseExtention.Enums.Corporate.ВариантыЗакрытияДежурства.ДежурилДругойСотрудник;
                else
                    ClosedType = BaseExtention.Enums.Corporate.ВариантыЗакрытияДежурства.НиктоНеДежурил;

                return;
            }

            ClosedType = BaseExtention.Enums.Corporate.ВариантыЗакрытияДежурства.НевозможноОпределить;
        }


        
    }
}
