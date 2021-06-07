using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    public enum ДолжностьПараметр
    {
        КодОрганизации = 3,
        НазваниеПодразделения = 4,
        НазваниеДолжности = 5
    }

    public enum ДолжностьТипПодчинения
    {
        Любой = 0,
        Прямое,
        Административное
    }

    public enum ДолжностьУровеньПримененияОперации
    {
        Любой = 0,
        ТолькоНаДолжностях,
        ТолькоНаПодразделениях
    }

    public enum ДолжностьСуществует
    {
        НеИмеетЗначения = 0,
        Да,
        Нет
    }

    public enum ДолжностьОбразуетПодразделение
    {
        НеИмеетЗначения = 0,
        Да,
        Нет
    }

    public enum ДолжностьВакантна
    {
        НеИмеетЗначения = 0,
        Да,
        Нет
    }

    public enum ДолжностьНаличиеПодчинений
    {
        НеИмеетЗначения = 0,
        Да,
        Нет
    }

    public enum ДолжностьГоловноеПодразделение
    {
        НеИмеетЗначения = 0,
        Да,
        Нет
    }

    public enum ДолжностьУровеньПодчинения
    {
        Все = 0,
        Корневой = 1
    }

    /// <summary>
    ///     Класс сущности ДолжностиОперации
    /// </summary>
    [Serializable]
    public class PositionCommand : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PositionCommand(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PositionCommand()
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
        ///     Метод загрузки данных сущности ДолжностиОперации
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодОперации", new object[] {Id, DBManager.ParameterTypes.Int32}}
            };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ДолжностиОперации, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности ДолжностиОперации на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                CommandId = Convert.ToInt32(dt.Rows[0][_КодОперации]);
                CommandName = dt.Rows[0][_Операция].ToString();
                Description = dt.Rows[0][_Описание].ToString();
                CustomForm = dt.Rows[0][_НестандартнаяФорма].ToString();
                LocalizationElement = dt.Rows[0][_ЭлементЛокализации].ToString();
                Icon = dt.Rows[0][_Пиктограмма].ToString();
                RoleId = Convert.ToInt32(dt.Rows[0][_КодРоли]);
                SubordinationType = Convert.ToByte(dt.Rows[0][_ТипПодчинения]);
                AvailableAtLevel = Convert.ToByte(dt.Rows[0][_УровеньПрименения]);
                IsExists = Convert.ToByte(dt.Rows[0][_Существует]);
                IsDepartment = Convert.ToByte(dt.Rows[0][_ОбразуетПодразделение]);
                IsVacant = Convert.ToByte(dt.Rows[0][_Вакантна]);
                HasSubordinates = Convert.ToByte(dt.Rows[0][_НаличиеПодчинений]);
                IsHeadDepartment = Convert.ToByte(dt.Rows[0][_ГоловноеПодразделение]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Сохранение информации о ДолжностиОперации
        /// </summary>
        public void Save()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {$"@{_КодОперации}", CommandId},
                {$"@{_Операция}", CommandName},
                {$"@{_Описание}", Description},
                {$"@{_НестандартнаяФорма}", CustomForm},
                {$"@{_ЭлементЛокализации}", LocalizationElement},
                {$"@{_Пиктограмма}", Icon},
                {$"@{_КодРоли}", RoleId},
                {$"@{_ТипПодчинения}", SubordinationType},
                {$"@{_УровеньПрименения}", AvailableAtLevel},
                {$"@{_Существует}", IsExists},
                {$"@{_ОбразуетПодразделение}", IsDepartment},
                {$"@{_Вакантна}", IsVacant},
                {$"@{_НаличиеПодчинений}", HasSubordinates},
                {$"@{_ГоловноеПодразделение}", IsHeadDepartment}
            };

            if (IsNew)
            {
                var id = DBManager.ExecuteScalar(SQLQueries.INSERT_ДолжностиОперации, CommandType.Text, CN, sqlParams);
                if (id != null)
                    Id = id.ToString();
            }
            else
            {
                var sql = SQLQueries.UPDATE_ДолжностиОперации;
                DBManager.ExecuteNonQuery(sql, CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Получение списка параметров операции
        /// </summary>
        public List<PositionCommandParameter> GetParameterList()
        {
            var sqlParams = new Dictionary<string, object>();

            sqlParams.Add("@КодОперации", new object[] { CommandId, DBManager.ParameterTypes.Int32 });

            var dt = DBManager.GetData(string.Format(SQLQueries.SELECT_ДолжностиПараметрыОпераций_Список), CN,
                CommandType.Text, sqlParams);
           
            return PositionCommandParameter.GetList(dt);
        }

        /// <summary>
        ///     Получение списка поддерживаемых операций
        /// </summary>
        /// <param name="roles">Роли сотрудника</param>
        /// <param name="subordinateTypes">Типы подчинения</param>
        /// <param name="availableAtLevels">Уровни применения операции</param>
        /// <returns></returns>
        public static List<PositionCommand> GetList(
            List<EmployeeRole> roles, List<ДолжностьТипПодчинения> subordinateTypes,
            List<ДолжностьУровеньПримененияОперации> availableAtLevels)
        {
            var rolesIds = "0," + string.Join(",", roles.Select(x => x.RoleId));
            var subordinateTypesIds = string.Join(",", subordinateTypes.Select(x => (int) x));
            var availableAtLevelsIds = string.Join(",", availableAtLevels.Select(x => (int) x));

            var sqlParams = new Dictionary<string, object>
            {
                {"@Роли", new object[] {rolesIds, DBManager.ParameterTypes.String}},
                {"@ТипыПодчинения", new object[] {subordinateTypesIds, DBManager.ParameterTypes.String}},
                {"@УровниПрименения", new object[] {availableAtLevelsIds, DBManager.ParameterTypes.String}}
            };

            var dt = DBManager.GetData(SQLQueries.SELECT_ДолжностиОперации, Config.DS_user, CommandType.Text,
                sqlParams);

            return dt.AsEnumerable().Select(dr => new PositionCommand
            {
                Id = dr.Field<int>(_КодОперации).ToString(),
                CommandId = dr.Field<int>(_КодОперации),
                CommandName = dr.Field<string>(_Операция),
                Description = dr.Field<string>(_Описание),
                CustomForm = dr.Field<string>(_НестандартнаяФорма),
                LocalizationElement = dr.Field<string>(_ЭлементЛокализации),
                Icon = dr.Field<string>(_Пиктограмма),
                RoleId = dr.Field<int>(_КодРоли),
                SubordinationType = dr.Field<byte>(_ТипПодчинения),
                AvailableAtLevel = dr.Field<byte>(_УровеньПрименения),
                IsExists = dr.Field<byte>(_Существует),
                IsDepartment = dr.Field<byte>(_ОбразуетПодразделение),
                IsVacant = dr.Field<byte>(_Вакантна),
                HasSubordinates = dr.Field<byte>(_НаличиеПодчинений),
                IsHeadDepartment = dr.Field<byte>(_ГоловноеПодразделение)
            }).ToList();
        }

        /// <summary>
        ///     Получить параметры операции в виде строки запроса, состоящей из пар ключ-значение
        /// </summary>
        /// <param name="positionId">Код должности</param>
        /// <param name="dateFrom">Дата От</param>
        /// <returns></returns>
        public string GetQueryString(int positionId, DateTime dateFrom)
        {
            var paramList = GetParameterList()
                .Where(x => !string.IsNullOrEmpty(x.URLKey) && x.IsAutoFill)
                .Select(x => $"{x.URLKey}={x.GetValue(positionId, dateFrom)}");

            return string.Join("&", paramList);
        }

        #region Accessors fields names

        private const string _КодОперации = "КодОперации";
        private const string _Операция = "Операция";
        private const string _Описание = "Описание";
        private const string _НестандартнаяФорма = "НестандартнаяФорма";
        private const string _ЭлементЛокализации = "ЭлементЛокализации";
        private const string _Пиктограмма = "Пиктограмма";
        private const string _КодРоли = "КодРоли";
        private const string _ТипПодчинения = "ТипПодчинения";
        private const string _УровеньПрименения = "УровеньПрименения";
        private const string _Существует = "Существует";
        private const string _ОбразуетПодразделение = "ОбразуетПодразделение";
        private const string _Вакантна = "Вакантна";
        private const string _НаличиеПодчинений = "НаличиеПодчинений";
        private const string _ГоловноеПодразделение = "ГоловноеПодразделение";

        #endregion

        #region Поля сущности

        /// <summary>
        ///     КодОперации
        /// </summary>
        /// <value>
        ///     КодОперации (int, not null)
        /// </value>
        [DBField(_КодОперации)]
        public int CommandId { get; set; }

        /// <summary>
        ///     Операция
        /// </summary>
        /// <value>
        ///     Операция (varchar(100), not null)
        /// </value>
        [DBField(_Операция)]
        public string CommandName
        {
            get { return CommandNameBind.Value; }
            set { CommandNameBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Операция
        /// </summary>
        public BinderValue CommandNameBind = new BinderValue();

        /// <summary>
        ///     Описание
        /// </summary>
        /// <value>
        ///     Описание (varchar(1000), not null)
        /// </value>
        [DBField(_Описание)]
        public string Description
        {
            get { return DescriptionBind.Value; }
            set { DescriptionBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Описание
        /// </summary>
        public BinderValue DescriptionBind = new BinderValue();

        /// <summary>
        ///     НестандартнаяФорма
        /// </summary>
        /// <value>
        ///     НестандартнаяФорма (varchar(50), not null)
        /// </value>
        [DBField(_НестандартнаяФорма)]
        public string CustomForm
        {
            get { return CustomFormBind.Value; }
            set { CustomFormBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля НестандартнаяФорма
        /// </summary>
        public BinderValue CustomFormBind = new BinderValue();

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
        ///     Binder для поля ЭлементЛокализации
        /// </summary>
        public BinderValue LocalizationElementBind = new BinderValue();

        /// <summary>
        ///     Пиктограмма
        /// </summary>
        /// <value>
        ///     Пиктограмма (varchar(50), not null)
        /// </value>
        [DBField(_Пиктограмма)]
        public string Icon
        {
            get { return IconBind.Value; }
            set { IconBind.Value = value; }
        }

        /// <summary>
        ///     Binder для поля Пиктограмма
        /// </summary>
        public BinderValue IconBind = new BinderValue();

        /// <summary>
        ///     КодРоли
        /// </summary>
        /// <value>
        ///     КодРоли (int, not null)
        /// </value>
        [DBField(_КодРоли)]
        public int RoleId
        {
            get { return int.Parse(RoleIdBind.Value); }
            set { RoleIdBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодРоли
        /// </summary>
        public BinderValue RoleIdBind = new BinderValue();

        /// <summary>
        ///     ТипПодчинения
        /// </summary>
        /// <value>
        ///     ТипПодчинения (tinyint, not null)
        /// </value>
        [DBField(_ТипПодчинения)]
        public byte SubordinationType
        {
            get { return byte.Parse(SubordinationTypeBind.Value); }
            set { SubordinationTypeBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ТипПодчинения
        /// </summary>
        public BinderValue SubordinationTypeBind = new BinderValue();

        /// <summary>
        ///     УровеньПрименения
        /// </summary>
        /// <value>
        ///     УровеньПрименения (tinyint, not null)
        /// </value>
        [DBField(_УровеньПрименения)]
        public byte AvailableAtLevel
        {
            get { return byte.Parse(AvailableAtLevelBind.Value); }
            set { AvailableAtLevelBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля УровеньПрименения
        /// </summary>
        public BinderValue AvailableAtLevelBind = new BinderValue();

        /// <summary>
        ///     Существует
        /// </summary>
        /// <value>
        ///     Существует (tinyint, not null)
        /// </value>
        [DBField(_Существует)]
        public byte IsExists
        {
            get { return byte.Parse(IsExistsBind.Value); }
            set { IsExistsBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Существует
        /// </summary>
        public BinderValue IsExistsBind = new BinderValue();

        /// <summary>
        ///     ОбразуетПодразделение
        /// </summary>
        /// <value>
        ///     ОбразуетПодразделение (tinyint, not null)
        /// </value>
        [DBField(_ОбразуетПодразделение)]
        public byte IsDepartment
        {
            get { return byte.Parse(IsDepartmentBind.Value); }
            set { IsDepartmentBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ОбразуетПодразделение
        /// </summary>
        public BinderValue IsDepartmentBind = new BinderValue();

        /// <summary>
        ///     Вакантна
        /// </summary>
        /// <value>
        ///     Вакантна (tinyint, not null)
        /// </value>
        [DBField(_Вакантна)]
        public byte IsVacant
        {
            get { return byte.Parse(IsVacantBind.Value); }
            set { IsVacantBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Вакантна
        /// </summary>
        public BinderValue IsVacantBind = new BinderValue();

        /// <summary>
        ///     НаличиеПодчинений
        /// </summary>
        /// <value>
        ///     НаличиеПодчинений (tinyint, not null)
        /// </value>
        [DBField(_НаличиеПодчинений)]
        public byte HasSubordinates
        {
            get { return byte.Parse(HasSubordinatesBind.Value); }
            set { HasSubordinatesBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля НаличиеПодчинений
        /// </summary>
        public BinderValue HasSubordinatesBind = new BinderValue();

        /// <summary>
        ///     ГоловноеПодразделение
        /// </summary>
        /// <value>
        ///     ГоловноеПодразделение (tinyint, not null)
        /// </value>
        [DBField(_ГоловноеПодразделение)]
        public byte IsHeadDepartment
        {
            get { return byte.Parse(IsHeadDepartmentBind.Value); }
            set { IsHeadDepartmentBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ГоловноеПодразделение
        /// </summary>
        public BinderValue IsHeadDepartmentBind = new BinderValue();

        #endregion
    }
}