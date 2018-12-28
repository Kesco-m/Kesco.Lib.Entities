using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    /// Бизнес-объект - Ресурс
    /// </summary>
    [Serializable]
    public class Resource : Entity
    {
        #region Поля сущности "Ресурс/валюта"

        /// <summary>
        ///  ID. Поле КодРесурса
        /// </summary>
        /// <remarks>
        ///  Типизированный псевданим для ID
        /// </remarks>
        public int ResourceId {get { return Id.ToInt(); }}

        /// <summary>
        /// РесурсЛат
        /// </summary>
        public string ResourceLat { get; set; }

        /// <summary>
        /// РесурсRL
        /// </summary>
        public string ResourceRL { get; set; }

        /// <summary>
        /// КодЕдиницыИзмерения
        /// </summary>
        public int UnitCode { get; set; }


        private Unit _unit { get; set; }
        /// <summary>
        ///  Единица Измерения
        /// </summary>
        public Unit Unit {
            get
            {
                if (_unit != null && UnitCode.ToString() == _unit.Id)
                {
                    return _unit;
                }

                _unit = new Unit(UnitCode.ToString());
                return _unit;
            }
        }

        /// <summary>
        /// КодВидаПодакцизногоТовара
        /// </summary>
        public int ExciseProductTypeCode { get; set; }

        /// <summary>
        /// Точность
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// СпецНДС
        /// </summary>
        public int NDS { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        public int Parent { get; set; }

        /// <summary>
        /// L
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// R
        /// </summary>
        public int R { get; set; }

        /// <summary>
        /// КодЛица
        /// </summary>
        public int OwnerPersonId { get; set; }
        
        /// <summary>
        /// Получение точности единицы измерения у лица, которому 
        /// </summary>
        public string UnitScale
        {
            get
            {
			    var sqlParams = new Dictionary<string, object> { { "@КодРесурса", ResourceId }
                                                                , { "@КодЛица", OwnerPersonId} };
                var dt = DBManager.GetData(SQLQueries.SELECT_РесурсыЛица, CN, CommandType.Text, sqlParams);
                if (dt.Rows.Count != 0) return (dt.Rows[0]["Точность"].ToString());

                return Scale.ToString();
            }
        }

        #endregion

        /// <summary>
        /// Инициализация сущности "Ресурс" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных ресурса склада</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодРесурса"].ToString();
                Name = dt.Rows[0]["РесурсРус"].ToString();
                ResourceLat = dt.Rows[0]["РесурсЛат"].ToString();
                ResourceRL = dt.Rows[0]["РесурсRL"].ToString();
                UnitCode = dt.Rows[0]["КодЕдиницыИзмерения"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодЕдиницыИзмерения"]);
                ExciseProductTypeCode = dt.Rows[0]["КодВидаПодакцизногоТовара"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодВидаПодакцизногоТовара"]);
                Scale = dt.Rows[0]["Точность"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[0]["Точность"]);
                NDS = dt.Rows[0]["СпецНДС"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[0]["СпецНДС"]);
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Parent"]);
                L = dt.Rows[0]["L"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["L"]);
                R = dt.Rows[0]["R"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["R"]);
                Changed = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Получение списка ресурсов из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных ресурсов</param>
        public static List<Resource> GetResourcesList(DataTable dt)
        {
            List<Resource> storeResourceList = new List<Resource>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                storeResourceList.Add(
                    new Resource
                    {
                        Id = dt.Rows[i]["КодРесурса"].ToString(),
                        Name = dt.Rows[i]["РесурсРус"].ToString(),
                        ResourceLat = dt.Rows[i]["РесурсЛат"].ToString(),
                        ResourceRL = dt.Rows[i]["РесурсRL"].ToString(),
                        UnitCode = dt.Rows[i]["КодЕдиницыИзмерения"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["КодЕдиницыИзмерения"]),
                        ExciseProductTypeCode = dt.Rows[i]["КодВидаПодакцизногоТовара"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["КодВидаПодакцизногоТовара"]),
                        Scale = dt.Rows[i]["Точность"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[i]["Точность"]),
                        NDS = dt.Rows[i]["СпецНДС"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[i]["СпецНДС"]),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["Parent"]),
                        L = dt.Rows[i]["L"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["L"]),
                        R = dt.Rows[i]["R"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["R"]),
                        Changed = Convert.ToDateTime(dt.Rows[0]["Изменено"])
                    }
                );
            }
            return storeResourceList;
        }

        /// <summary>
        /// Получение списка подчиненных ресурсов
        /// </summary>
        public List<Resource> GetChildResourcesList()
        {
            List<Resource> storePlaceList = new List<Resource>();

            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] { L, DBManager.ParameterTypes.Int32 });
            sqlParams.Add("@rightKey", new object[] { R, DBManager.ParameterTypes.Int32 });
            DataTable dt = DBManager.GetData(string.Format(SQLQueries.SELECT_Ресурс_Подчиненные), CN, CommandType.Text, sqlParams);

            return GetResourcesList(dt);
        }

        /// <summary>
        /// Получение ID родительского ресурса
        /// </summary>
        public int GetParentResourceID()
        {
            return this.Parent;
        }

        /// <summary>
        /// Загрузка данных по ресурсу
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@id", new object[] { Id, DBManager.ParameterTypes.Int32 });
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Ресурс, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Resource() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID ресурса</param>
        public Resource(string id)
            : base(id) 
        {
            Load();
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public override string CN
        {
            get
            {
                if(string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        //
        //public int GetScaleForUnit(string unit, int @default)
        //{
        //    //if(UnitCode.Equals(unit.ToInt()))
        //    //    return 
        //}

        /// <summary>
        ///  Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_resource) : _connectionString; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldUnit"></param>
        /// <param name="newUnit"></param>
        /// <returns></returns>
        public double ConvertionCoefficient(Unit oldUnit, Unit newUnit)
        {
            if (oldUnit == null)
                throw new ArgumentNullException("oldUnit", "Необходимо указать");
            if (newUnit == null)
                throw new ArgumentNullException("newUnit", "Необходимо указать");

            double oldCoef = double.MinValue;
            double newCoef = double.MinValue;

            if (Unit == null) throw new Exception("Ресурс не имеет единиц измерения");

            if (oldUnit.КодЕдиницыИзмерения.Equals(Unit.КодЕдиницыИзмерения))
                oldCoef = 1;
            if (newUnit.КодЕдиницыИзмерения.Equals(Unit.КодЕдиницыИзмерения))
                newCoef = 1;

            var resourceList = UnitAdvList;
            if (resourceList != null)
            {
                foreach (UnitAdv unitAdv in resourceList)
                {
                    if (oldUnit.КодЕдиницыИзмерения.Equals(unitAdv.Unit.КодЕдиницыИзмерения))
                        oldCoef = ConvertExtention.Convert.Str2Double(unitAdv.Коэффициент.ToString());
                    if (newUnit.КодЕдиницыИзмерения.Equals(unitAdv.Unit.КодЕдиницыИзмерения))
                        newCoef = ConvertExtention.Convert.Str2Double(unitAdv.Коэффициент.ToString());
                }
            }

            //if (oldCoef == double.MinValue || newCoef == double.MinValue)
            //    throw new LogicalException("Ресурс " + this.Name + " не имеет единицы измерения - " + oldUnit._Name, "", System.Reflection.Assembly.GetExecutingAssembly().GetName(), Priority.Info);

            return newCoef / oldCoef;
        }

        private List<UnitAdv> unitAdvList { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<UnitAdv> UnitAdvList
        {
            get { return unitAdvList ?? (unitAdvList = GetUnitAdvList(ResourceId)); }
            set { unitAdvList = value; }
        }
        
        /// <summary>
        /// Получить список запросом
        /// </summary>
        public List<UnitAdv> GetUnitAdvList(Int32 resourceId)
        {
            List<UnitAdv> list = null;
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодРесурса", resourceId}
            };
            using (var dbReader = new DBReader(SQLQueries.SELECT_ЕдиницыИзмеренияДоп, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    list = new List<UnitAdv>();

                    #region Получение порядкового номера столбца

                    int colКодЕдиницыИзмеренияДополнительной = dbReader.GetOrdinal("КодЕдиницыИзмеренияДополнительной");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colТочность = dbReader.GetOrdinal("Точность");
                    int colКоличествоЕдиниц = dbReader.GetOrdinal("КоличествоЕдиниц");
                    int colКоличествоЕдиницОсновных = dbReader.GetOrdinal("КоличествоЕдиницОсновных");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colМассаБрутто = dbReader.GetOrdinal("МассаБрутто");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new UnitAdv();
                        row.Unavailable = false;
                        row.КодЕдиницыИзмеренияДополнительной = dbReader.GetInt32(colКодЕдиницыИзмеренияДополнительной);
                        row.КодРесурса = dbReader.GetInt32(colКодРесурса);
                        row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        row.Точность = dbReader.GetByte(colТочность);
                        row.КоличествоЕдиниц = dbReader.GetDecimal(colКоличествоЕдиниц);
                        row.КоличествоЕдиницОсновных = dbReader.GetDecimal(colКоличествоЕдиницОсновных);
                        row.Коэффициент = dbReader.GetDouble(colКоэффициент);
                        if (!dbReader.IsDBNull(colМассаБрутто)) { row.МассаБрутто = dbReader.GetDecimal(colМассаБрутто); }
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="default"></param>
        /// <param name="_ownerId"></param>
        /// <returns></returns>
        public int GetScale4Unit(string _unit, int @default, string _ownerId)
        {
            OwnerPersonId = int.Parse(_ownerId);
            if (UnitCode.ToString().Equals(_unit) || (_unit == string.Empty && UnitCode == 0))
                return int.Parse(UnitScale);

            var resourceList = UnitAdvList;
            if (resourceList != null)
            {
                var unitAdv = 0;
                var res = resourceList.Find(r => r.Unit.Id == _unit);
                if (res != null) unitAdv = res.Точность;
                return unitAdv == 0 ? @default : unitAdv;
                /*
                foreach (UnitAdv unitAdv in resourceList)
                    if (unitAdv.Unit.Equals(_unit))
                        return unitAdv.Точность == 0 ? @default : unitAdv.Точность;
                */
            }
            return @default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public int GetScale4Unit(string _unit, int @default)
        {
            if (UnitCode.ToString().Equals(_unit))
                return int.Parse(UnitScale);

            var resourceList = UnitAdvList;
            if (resourceList != null)
            {
                foreach (UnitAdv unitAdv in resourceList)
                    if (unitAdv.Unit.Equals(_unit))
                        return unitAdv.Точность == 0 ? @default : unitAdv.Точность;
            }
            return @default;
        }


        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;

        /// <summary>
        /// Метод получения даты последнего изменения
        /// </summary>
        public override DateTime GetLastChanged(string id)
        {
            var param = new Dictionary<string, object> { { "@Id", id } };
            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_Склад_LastChanged, CommandType.Text, CN, param);

            if (res is DateTime)
                return (DateTime)res;

            return DateTime.MinValue;
        }

    }


}

