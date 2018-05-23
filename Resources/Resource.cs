using System;
using System.Collections.Generic;
using System.Data;
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

        /// <summary>
        /// КодВидаПодакцизногоТовара
        /// </summary>
        public int ExciseProductTypeCode { get; set; }

        /// <summary>
        /// Точность
        /// </summary>
        public int Accuracy { get; set; }

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
                Accuracy = dt.Rows[0]["Точность"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[0]["Точность"]);
                NDS = dt.Rows[0]["СпецНДС"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[0]["СпецНДС"]);
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Parent"]);
                L = dt.Rows[0]["L"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["L"]);
                R = dt.Rows[0]["R"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["R"]);
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
                        Accuracy = dt.Rows[i]["Точность"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[i]["Точность"]),
                        NDS = dt.Rows[i]["СпецНДС"] == DBNull.Value ? 0 : Convert.ToInt16(dt.Rows[i]["СпецНДС"]),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["Parent"]),
                        L = dt.Rows[i]["L"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["L"]),
                        R = dt.Rows[i]["R"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["R"])
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
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;
    }
}

