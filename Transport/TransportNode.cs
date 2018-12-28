using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Transport
{
    /// <summary>
    /// Транспортный узел
    /// </summary>
    public class TransportNode:Entity
    {
        #region Поля сущности
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодВидаТранспорта (int, not null)
        /// </value>
        public int TransportTypeCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// НазваниеЛат (varchar(50), not null)
        /// </value>
        public string NameLat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// НазваниеRL (varchar(100), not null)
        /// </value>
        public string НазваниеRL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодЖелезнойДороги (int, null)
        /// </value>
        public int? CodeRailway { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодТерритории (int, not null)
        /// </value>
        public int TerritoryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ПогранПереход (bit, not null)
        /// </value>
        public bool IsCustom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Транспорт (int, not null)
        /// </value>
        public string TypeTransport { get; set; }

        
        #endregion

        /// Конструктор
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ID</param>
        public TransportNode(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TransportNode() { }

        /// <summary>
        /// Инициализация сущности Транспортный узел на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодТранспортногоУзла"].ToString();
                TransportTypeCode = (int)dt.Rows[0]["КодВидаТранспорта"];
                Name = (string)dt.Rows[0]["Название"];
                NameLat = (string)dt.Rows[0]["НазваниеЛат"];
                CodeRailway = (int?)dt.Rows[0]["КодЖелезнойДороги"];
                TerritoryCode = (int)dt.Rows[0]["КодТерритории"];
                TypeTransport = (string)dt.Rows[0]["Транспорт"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Метод загрузки данных сущности "Транспортный узел"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТранспортныеУзлы, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Инициализация сущности "Транспортный узел" на основе таблицы данных о складе
        /// </summary>
        /// <param name="dt">Таблица данных склада</param>
        public static List<TransportNode> GetTransportNodeList(DataTable dt)
        {
            List<TransportNode> transportNodeList = new List<TransportNode>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                transportNodeList.Add(
                    new TransportNode()
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодТранспортногоУзла"].ToString(),
                        Name = dt.Rows[i]["Название"].ToString(),
                        NameLat = dt.Rows[i]["НазваниеЛат"].ToString(),
                        TransportTypeCode = Convert.ToInt32(dt.Rows[i]["КодВидаТранспорта"]),
                        CodeRailway = dt.Rows[i]["КодЖелезнойДороги"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодЖелезнойДороги"],
                        TerritoryCode = Convert.ToInt32(dt.Rows[i]["КодТерритории"]),
                        TypeTransport = dt.Rows[i]["Транспорт"].ToString()
                    }
                );
            }
            return transportNodeList;
        }

    }

}
