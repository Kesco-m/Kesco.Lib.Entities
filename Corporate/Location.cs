using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Corporate.Equipments;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities
{
    

    /// <summary>
    /// Бизнес-объект - Расположения
    /// </summary>
    [Serializable]
    public class Location : Entity
    {
        #region Поля сущности "Расположения"

        /// <summary>
        /// Признак РабочееМесто
        /// </summary>
        public int WorkPlace { get; set; }

        /// <summary>
        /// Признак Офис
        /// </summary>
        public int Office { get; set; }

        /// <summary>
        /// Признак Закрыто
        /// </summary>
        public int LocationClose { get; set; }


        /// <summary>
        /// Расположение
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// РасположениеPath0
        /// </summary>
        public string NamePath0 { get; set; }

        /// <summary>
        /// РасположениеPath1
        /// </summary>
        public string NamePath1 { get; set; }

        /// <summary>
        /// Родительский ID
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Левый ключ
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Правый ключ
        /// </summary>
        public int R { get; set; }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID расположения</param>
        public Location(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Location()
        {
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public override sealed string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        private List<Equipment> _equipmentsIt;

        /// <summary>
        /// Инициализация сущности "Расположения" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодРасположения"].ToString();
                ShortName = dt.Rows[0]["Расположение"].ToString();
                Name = dt.Rows[0]["РасположениеPath1"].ToString();
                WorkPlace = Convert.ToInt16(dt.Rows[0]["РабочееМесто"]);
                Office = Convert.ToInt16(dt.Rows[0]["Офис"]);
                LocationClose = Convert.ToInt16(dt.Rows[0]["Закрыто"]);
                NamePath0 = dt.Rows[0]["РасположениеPath0"].ToString();
                NamePath1 = dt.Rows[0]["РасположениеPath1"].ToString();
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString();
                L = Convert.ToInt32(dt.Rows[0]["L"]);
                R = Convert.ToInt32(dt.Rows[0]["R"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Создание объекта через datareader
        /// </summary>
        /// <param name="dbReader">Объект-DBReader</param>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colId = dbReader.GetOrdinal("КодРасположения");
            var colShortName = dbReader.GetOrdinal("Расположение");
            var colName = dbReader.GetOrdinal("РасположениеPath1");
            var colWorkPlace = dbReader.GetOrdinal("РабочееМесто");
            var colOffice = dbReader.GetOrdinal("Офис");
            var colLocationClose = dbReader.GetOrdinal("Закрыто");
            var colNamePath0 = dbReader.GetOrdinal("РасположениеPath0");
            var colNamePath1 = dbReader.GetOrdinal("РасположениеPath1");
            var colL = dbReader.GetOrdinal("L");
            var colR = dbReader.GetOrdinal("R");

            if (!dbReader.IsDBNull(colId)) Id = dbReader.GetInt32(colId).ToString();
            if (!dbReader.IsDBNull(colShortName)) ShortName = dbReader.GetString(colShortName);
            if (!dbReader.IsDBNull(colName)) Name = dbReader.GetString(colName);
            if (!dbReader.IsDBNull(colWorkPlace)) WorkPlace = dbReader.GetByte(colWorkPlace);
            if (!dbReader.IsDBNull(colOffice)) Office = dbReader.GetByte(colOffice);
            if (!dbReader.IsDBNull(colLocationClose)) LocationClose = dbReader.GetByte(colLocationClose);
            if (!dbReader.IsDBNull(colNamePath0)) NamePath0 = dbReader.GetString(colNamePath0);
            if (!dbReader.IsDBNull(colNamePath1)) NamePath1 = dbReader.GetString(colNamePath1);
            if (!dbReader.IsDBNull(colL)) L = dbReader.GetInt32(colL);
            if (!dbReader.IsDBNull(colR)) R = dbReader.GetInt32(colR);
            
            Unavailable = false;
        }

        /// <summary>
        /// Получение списка расположений из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<Location> GetLocationsList(DataTable dt)
        {
            List<Location> locatinList = new List<Location>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                locatinList.Add(
                    new Location()
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодРасположения"].ToString(),
                        Name = dt.Rows[i]["Расположение"].ToString(),
                        ShortName = dt.Rows[0]["Расположение"].ToString(),
                        WorkPlace = Convert.ToInt16(dt.Rows[i]["РабочееМесто"]),
                        Office = Convert.ToInt16(dt.Rows[i]["Офис"]),
                        LocationClose = Convert.ToInt16(dt.Rows[i]["Закрыто"]),
                        NamePath0 = dt.Rows[i]["РасположениеPath0"].ToString(),
                        NamePath1 = dt.Rows[i]["РасположениеPath1"].ToString(),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString(),
                        L = Convert.ToInt16(dt.Rows[i]["L"]),
                        R = Convert.ToInt16(dt.Rows[i]["R"])
                    }
                    );
            }
            return locatinList;
        }

        /// <summary>
        /// Получение списка подчиненных раположений
        /// </summary>
        public List<Location> GetChildLocationsList()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] {L, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@rightKey", new object[] {R, DBManager.ParameterTypes.Int32});
            DataTable dt = DBManager.GetData(string.Format(SQLQueries.SELECT_РасположенияПодчиненные), CN,
                CommandType.Text, sqlParams);

            return GetLocationsList(dt);
        }

        /// <summary>
        /// Метод загрузки данных сущности "Расположение"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_РасположениеПоID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Сотрудники на расположении
        /// </summary>
        public List<Employee> CoWorkers { get; set; }

        /// <summary>
        /// Компьютеризированное рабочее место
        /// </summary>
        public bool IsComputeredWorkPlace { get { return ((WorkPlace.Equals((int)ТипыРабочихМест.КомпьютеризированноеРабочееМесто))); } }

        /// <summary>
        /// Гостевое рабочее место
        /// </summary>
        public bool IsGuestWorkPlace { get { return ((WorkPlace.Equals((int)ТипыРабочихМест.ГостевоеРабочееМесто))); } }

        private bool? _isOffice = null;

        /// <summary>
        /// Расположение находится в офисе
        /// </summary>
        public bool IsOffice
        {
            get
            {
                if (_isOffice.HasValue && !RequiredRefreshInfo)
                    return _isOffice.Value;

                _isOffice = false;
                var sqlParams = new Dictionary<string, object> {{"@КодРасположения", Id.ToInt()}};
                using (var dbReader = new DBReader(SQLQueries.SELECT_РасположениеВОфисе, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) _isOffice = true;
                }
                return _isOffice.Value;

            }
        }

        /// <summary>
        /// Получение ИТ-оборудования на данном расположении
        /// </summary>
        public List<Equipment> EquipmentsIt
        {
            get
            {
                if (RequiredRefreshInfo || _equipmentsIt == null)
                {
                    _equipmentsIt = new List<Equipment>();
                    var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) }, {"@IT", 1}  };
                    using (
                        var dbReader = new DBReader(SQLQueries.SELECT_ID_ОборудованиеНаРасположении, CommandType.Text, CN,
                            sqlParams))
                    {
                        _equipmentsIt = Equipment.GetEquipmentList(dbReader);
                    }
                }
                return _equipmentsIt;
            }
        }

        private bool? _isOrganized = null;

        /// <summary>
        /// Оганизовано рабочее место
        /// </summary>
        public bool IsOrganized
        {
            get
            {
                if (_isOrganized.HasValue && !RequiredRefreshInfo)
                    return _isOrganized.Value;

                _isOrganized = false;
                var sqlParams = new Dictionary<string, object> { { "@КодРасположения", Id.ToInt() } };
                using (var dbReader = new DBReader(SQLQueries.SELECT_РасположенияОрганизованыРабочиеМеста, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) _isOrganized = true;
                }
                return _isOrganized.Value;
            }
        }

        /// <summary>
        /// Расположение
        /// </summary>
        /// <param name="icon">иконка</param>
        /// <param name="title">описание</param>
        public void GetLocationSpecifications(out string icon, out string title)
        {
            switch (WorkPlace)
            {
                case (int)ТипыРабочихМест.КомпьютеризированноеРабочееМесто:
                    if (IsOrganized)
                    {
                        icon = ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                        title = ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Name + " - организовано";
                    }
                    else
                    {
                        icon = ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().IconGrayed;
                        title = ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().NameGrayed;
                    }
                    break;
                case (int)ТипыРабочихМест.НомерГостиницы:
                    icon = ТипыРабочихМест.НомерГостиницы.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.НомерГостиницы.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int)ТипыРабочихМест.Оборудование:
                    icon = ТипыРабочихМест.Оборудование.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.Оборудование.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int)ТипыРабочихМест.CкладОборудования:
                    icon = ТипыРабочихМест.CкладОборудования.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.CкладОборудования.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int)ТипыРабочихМест.ГостевоеРабочееМесто:
                    icon = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                default:
                    icon = "";
                    title = "";
                    break;
            }
        }

    }
}
