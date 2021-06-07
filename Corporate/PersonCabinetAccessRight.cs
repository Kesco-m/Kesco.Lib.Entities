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
    [DBSource("ЛичныеКабинетыПрава")]
    public class PersonCabinetAccessRight : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="pc_id">КодЛичногоКабинета</param>
        /// <param name="empl_id">КодСотрудника</param>
        public PersonCabinetAccessRight(string pc_id, string empl_id)
            : base(pc_id)
        {
            Load(pc_id, empl_id);
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
        ///     Подготовка связывания поля объекта КодТипаЛичногоКабинета с контролом
        /// </summary>
        public BinderValue PersonalCabinetIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Сотрудник с контролом
        /// </summary>
        public BinderValue EmployeeIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Может давать права с контролом
        /// </summary>
        public BinderValue IsRoleAccessRightBind = new BinderValue();

        /// <summary>
        ///     КодТипаЛичногоКабинета (int, not null)
        /// </summary>
        [DBField("КодЛичногоКабинета", 0)]
        public int PersonalCabinetId
        {
            get { return string.IsNullOrEmpty(PersonalCabinetIdBind.Value) ? 0 : int.Parse(PersonalCabinetIdBind.Value); }
            set { PersonalCabinetIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     КодТипаЛичногоКабинета (int, not null)
        /// </summary>
        [DBField("КодСотрудника", 0)]
        public int EmployeeId
        {
            get { return string.IsNullOrEmpty(EmployeeIdBind.Value) ? 0 : int.Parse(EmployeeIdBind.Value); }
            set { EmployeeIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Может давать права
        /// </value>
        [DBField("МожетДаватьПрава")]
        public bool IsRoleAccessRight
        {
            get { return !string.IsNullOrEmpty(IsRoleAccessRightBind.Value) && Convert.ToBoolean(IsRoleAccessRightBind.Value); }
            set { IsRoleAccessRightBind.Value = value.ToString(); }
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
        ///     Метод загрузки данных сущности "Права личного кабинета"
        /// </summary>
        public void Load(string pc_id, string empl_id)
        {
            var sqlParams = new Dictionary<string, object> {{ "@КодЛичногоКабинета", pc_id }, { "@КодСотрудника", empl_id } };

            using (var dbReader = new DBReader(SQLQueries.SELECT_ЛичныйКабинетПраваПоID, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца
                    var colКодЛичногоКабинета = dbReader.GetOrdinal("КодЛичногоКабинета");
                    var colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    var colМожетДаватьПрава = dbReader.GetOrdinal("МожетДаватьПрава");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        var s = dbReader.GetByte(colМожетДаватьПрава);

                        Unavailable = false;
                        PersonalCabinetId = dbReader.GetInt32(colКодЛичногоКабинета);
                        EmployeeId = dbReader.GetInt32(colКодСотрудника);
                        IsRoleAccessRight = Convert.ToBoolean(dbReader.GetByte(colМожетДаватьПрава));
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
                PersonalCabinetId = Convert.ToInt32(dt.Rows[0]["КодЛичногоКабинета"].ToString());
                Id = PersonalCabinetId.ToString();
                EmployeeId = Convert.ToInt32(dt.Rows[0]["КодСотрудника"].ToString());
                IsRoleAccessRight = Convert.ToBoolean(dt.Rows[0]["Название"].ToString());
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

    }
}