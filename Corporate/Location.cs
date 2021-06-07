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
    ///     Бизнес-объект - Расположения
    /// </summary>
    [Serializable]
    public class Location : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        private List<Equipment> _equipmentsIt;

        /// <summary>
        ///     Сотрудники на расположении
        /// </summary>
        private List<Employee> _coWorkers { get; set; }

        private bool? _isOffice;

        private bool? _isGroupWorkplace;

        private bool? _isOrganized;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID расположения</param>
        public Location(string id)
            : base(id)
        {
            
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Location()
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
        ///     Компьютеризированное рабочее место
        /// </summary>
        public bool IsComputeredWorkPlace =>  WorkPlace.Equals((int) ТипыРабочихМест.КомпьютеризированноеРабочееМесто);

        /// <summary>
        ///     Гостевое рабочее место
        /// </summary>
        public bool IsGuestWorkPlace =>  WorkPlace.Equals((int) ТипыРабочихМест.ГостевоеРабочееМесто);

        /// <summary>
        ///     Расположение находится в офисе
        /// </summary>
        public bool IsOffice
        {
            get
            {
                var cacheKey = "location._isOffice";
                if (_isOffice.HasValue && LoadedExternalProperties.ContainsKey(cacheKey))
                    return _isOffice.Value;

                _isOffice = false;
                var sqlParams = new Dictionary<string, object> {{"@КодРасположения", Id.ToInt()}};
                using (var dbReader =
                    new DBReader(SQLQueries.SELECT_РасположениеВОфисе, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) _isOffice = true;
                }

                AddLoadedExternalProperties(cacheKey);
                return _isOffice.Value;
            }
        }

        /// <summary>
        ///     На расположении работает группа совместной работы
        /// </summary>
        public bool IsGroupWorkplace
        {
            get
            {
                var cacheKey = "location._isGroupWorkplace";
                if (_isGroupWorkplace.HasValue && LoadedExternalProperties.ContainsKey(cacheKey))
                    return _isGroupWorkplace.Value;

                _isGroupWorkplace = false;
                var sqlParams = new Dictionary<string, object> { { "@КодРасположения", Id.ToInt() } };
                using (var dbReader =
                    new DBReader(SQLQueries.SELECT_РасположениеГруппы, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) _isGroupWorkplace = true; 
                }

                AddLoadedExternalProperties(cacheKey);
                return _isGroupWorkplace.Value;
            }
        }

        /// <summary>
        ///     Получение ИТ-оборудования на данном расположении
        /// </summary>
        public List<Equipment> EquipmentsIt
        {
            get
            {
                var cacheKey = "location._equipmentsIt";
                if (LoadedExternalProperties.ContainsKey(cacheKey)) return _equipmentsIt;
                
                _equipmentsIt = new List<Equipment>();
                var sqlParams = new Dictionary<string, object> {{"@Id", int.Parse(Id)}, {"@IT", 1}};
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ID_ОборудованиеНаРасположении, CommandType.Text,
                        CN,
                        sqlParams))
                {
                    _equipmentsIt = Equipment.GetEquipmentList(dbReader);
                }

                AddLoadedExternalProperties(cacheKey);

                return _equipmentsIt;
            }
        }

        /// <summary>
        /// Сотрудники на расположении
        /// </summary>
        public List<Employee> CoWorkers
        {
            get
            {
                var cacheKey = "location._coWorkers";
                if (LoadedExternalProperties.ContainsKey(cacheKey)) return _coWorkers;

                _coWorkers = new List<Employee>();
                var sqlParams = new Dictionary<string, object>
                    {{"@КодСотрудника", 0}, {"@КодРасположения", int.Parse(Id)}};
                using (var dbReader = new DBReader(SQLQueries.SELECT_СовместнаяРаботаНаРабочемМесте, CommandType.Text, CN,
                    sqlParams))
                {
                    if (dbReader.HasRows)
                        while (dbReader.Read())
                        {
                            var tempUserCoWorker = new Employee();
                            tempUserCoWorker.LoadFromDbReader(dbReader, true);
                            _coWorkers.Add(tempUserCoWorker);
                        }
                }
                
                return _coWorkers;
            }
        }

        /// <summary>
        ///     Существует ли It-оборудование на данном расположении
        /// </summary>
        public bool ExistEquipmentsIt
        {
            get
            {
                var sqlParams = new Dictionary<string, object> {{"@Id", int.Parse(Id)}};
                using (
                    var dbReader = new DBReader(SQLQueries.SELECT_ID_ОборудованиеITНаРасположении, CommandType.Text, CN,
                        sqlParams))
                {
                    return dbReader.HasRows;
                }
            }
        }

        /// <summary>
        ///  Существует ли  на данном расположении It-оборудование указанного сотрудника
        /// </summary>
        /// <param name="idEmpl">КодСотрудника</param>
        /// <returns></returns>
        public bool ExistEquipmentsItByEmployee(string idEmpl)
        {
            var sqlParams = new Dictionary<string, object> { { "@Id", int.Parse(Id) }, { "@КодСотрудника", int.Parse(idEmpl) } };
            using (
                var dbReader = new DBReader(SQLQueries.SELECT_ID_ОборудованиеITСотрудникаНаРасположении, CommandType.Text, CN,
                    sqlParams))
            {
                return dbReader.HasRows;
            }
        
        }

        /// <summary>
        ///     Оганизовано рабочее место
        /// </summary>
        public bool IsOrganized
        {
            get
            {
                var cacheKey = "location._isOrganized";
                if (_isOrganized.HasValue && LoadedExternalProperties.ContainsKey(cacheKey)) return _isOrganized.Value;
                
                _isOrganized = false;
                var sqlParams = new Dictionary<string, object> {{"@КодРасположения", Id.ToInt()}};
                using (var dbReader = new DBReader(SQLQueries.SELECT_РасположенияОрганизованыРабочиеМеста, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) _isOrganized = true;
                }

                AddLoadedExternalProperties(cacheKey);
                return _isOrganized.Value;
            }
        }

        private Location _locationOffice = null;
        /// <summary>
        /// В расположении содержится расположение с типом офис
        /// </summary>
        public Location LocationOffice
        {
            get
            {
                if (_locationOffice == null)
                {
                    var sqlParams = new Dictionary<string, object> {
                        { "@LeftId", new object[] { L, DBManager.ParameterTypes.Int32 } },
                        { "@RightId", new object[] { R, DBManager.ParameterTypes.Int32 } } };

                    var dt = (DBManager.GetData(SQLQueries.SELECT_РасположениеОфис, CN, CommandType.Text, sqlParams));
                    if (dt.Rows.Count > 0)
                    {
                        _locationOffice = new Location(dt.Rows[0]["КодРасположения"].ToString());
                    }
                }

                return _locationOffice;
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
                Id = dt.Rows[0]["КодРасположения"].ToString();
                ShortName = dt.Rows[0]["Расположение"].ToString();
                Name = dt.Rows[0]["РасположениеPath1"].ToString();
                WorkPlace = Convert.ToInt16(dt.Rows[0]["РабочееМесто"]);
                Office = Convert.ToInt16(dt.Rows[0]["Офис"]);
                LocationClose = Convert.ToInt16(dt.Rows[0]["Закрыто"]);
                NamePath0 = dt.Rows[0]["РасположениеPath0"].ToString();
                NamePath1 = dt.Rows[0]["РасположениеPath1"].ToString();
                TerritoryId = (dt.Rows[0]["КодТерритории"]== DBNull.Value) ? (int?)null : Convert.ToInt32(dt.Rows[0]["КодТерритории"]);
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString();
                L = Convert.ToInt32(dt.Rows[0]["L"]);
                R = Convert.ToInt32(dt.Rows[0]["R"]);
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Создание объекта через datareader
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
            //var colTerritory = dbReader.GetOrdinal("КодТерритории");
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
            //if (!dbReader.IsDBNull(colTerritory)) TerritoryId = dbReader.GetInt32(colTerritory);
            if (!dbReader.IsDBNull(colL)) L = dbReader.GetInt32(colL);
            if (!dbReader.IsDBNull(colR)) R = dbReader.GetInt32(colR);

            Unavailable = false;
        }

        /// <summary>
        ///     Получение списка расположений из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<Location> GetLocationsList(DataTable dt)
        {
            var locatinList = new List<Location>();

            for (var i = 0; i < dt.Rows.Count; i++)
                locatinList.Add(
                    new Location
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
                        TerritoryId = Convert.ToInt32(dt.Rows[0]["КодТерритории"]),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString(),
                        L = Convert.ToInt16(dt.Rows[i]["L"]),
                        R = Convert.ToInt16(dt.Rows[i]["R"])
                    }
                );
            return locatinList;
        }

        /// <summary>
        ///     Получение списка подчиненных раположений
        /// </summary>
        public List<Location> GetChildLocationsList()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] {L, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@rightKey", new object[] {R, DBManager.ParameterTypes.Int32});
            var dt = DBManager.GetData(string.Format(SQLQueries.SELECT_РасположенияПодчиненные), CN,
                CommandType.Text, sqlParams);

            return GetLocationsList(dt);
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Расположение"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_РасположениеПоID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Расположение
        /// </summary>
        /// <param name="icon">иконка</param>
        /// <param name="title">описание</param>
        public void GetLocationSpecifications(out string icon, out string title)
        {
            switch (WorkPlace)
            {
                case (int) ТипыРабочихМест.КомпьютеризированноеРабочееМесто:
                    if (IsOrganized)
                        icon = ТипыРабочихМест.КомпьютеризированноеРабочееМесто
                            .GetAttribute<Specifications.InventoryWorkPlaceType>().Icon;
                    else
                        icon = ТипыРабочихМест.КомпьютеризированноеРабочееМесто
                            .GetAttribute<Specifications.InventoryWorkPlaceType>().IconGrayed;
                    title = ТипыРабочихМест.КомпьютеризированноеРабочееМесто
                        .GetAttribute<Specifications.InventoryWorkPlaceType>().Name;

                    break;
                case (int) ТипыРабочихМест.НомерГостиницы:
                    icon = ТипыРабочихМест.НомерГостиницы.GetAttribute<Specifications.InventoryWorkPlaceType>().Icon;
                    title = ТипыРабочихМест.НомерГостиницы.GetAttribute<Specifications.InventoryWorkPlaceType>().Name;
                    break;
                case (int) ТипыРабочихМест.Оборудование:
                    icon = ТипыРабочихМест.Оборудование.GetAttribute<Specifications.InventoryWorkPlaceType>().Icon;
                    title = ТипыРабочихМест.Оборудование.GetAttribute<Specifications.InventoryWorkPlaceType>().Name;
                    break;
                case (int) ТипыРабочихМест.CкладОборудования:
                    icon = ТипыРабочихМест.CкладОборудования.GetAttribute<Specifications.InventoryWorkPlaceType>().Icon;
                    title = ТипыРабочихМест.CкладОборудования.GetAttribute<Specifications.InventoryWorkPlaceType>().Name;
                    break;
                case (int) ТипыРабочихМест.ГостевоеРабочееМесто:
                    icon = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<Specifications.InventoryWorkPlaceType>().Icon;
                    title = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<Specifications.InventoryWorkPlaceType>().Name;
                    break;
                default:
                    icon = "";
                    title = "";
                    break;
            }
        }

        #region Поля сущности "Расположения"

        /// <summary>
        ///     Признак РабочееМесто
        /// </summary>
        public int WorkPlace { get; set; }

        /// <summary>
        ///     Признак Офис
        /// </summary>
        public int Office { get; set; }

        /// <summary>
        ///     Признак Закрыто
        /// </summary>
        public int LocationClose { get; set; }


        /// <summary>
        ///     Расположение
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     КодТерритории
        /// </summary>
        public int? TerritoryId { get; set; }

        /// <summary>
        ///     РасположениеPath0
        /// </summary>
        public string NamePath0 { get; set; }

        /// <summary>
        ///     РасположениеPath1
        /// </summary>
        public string NamePath1 { get; set; }

        /// <summary>
        ///     РасположениеPath1 с пробелами для переноса
        /// </summary>
        public string NamePath1_WhiteSpace => string.IsNullOrEmpty(NamePath1) ? NamePath1 : NamePath1.Replace("/", "/ ");

        /// <summary>
        ///     Родительский ID
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        ///     Левый ключ
        /// </summary>
        public int L { get; set; }

        /// <summary>
        ///     Правый ключ
        /// </summary>
        public int R { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public DateTime ChangedTime { get; set; }


        #endregion
    }
}