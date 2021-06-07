using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.CashFlow
{
    /// <summary>
    ///     Класс сущности Статьи движения денежных средств
    /// </summary>
    [Serializable]
    [DBSource("СтатьиДвиженияДенежныхСредств")]
    public class CashFlowItem : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Подготовка связывания поля объекта СтатьяДвиженияДенежныхСредств с контролом
        /// </summary>
        public BinderValue CashFlowItemNameBind = new BinderValue();

        /// <summary>
        ///     Подготовка связывания поля объекта КодВидаДвиженияДенежныхСредств с контролом
        /// </summary>
        public BinderValue CashFlowTypeIdBind = new BinderValue();

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID</param>
        public CashFlowItem(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public CashFlowItem()
        {
            Name = "";
        }

        /// <summary>
        ///     Родительский ID
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        ///     Полный путь
        /// </summary>
        public string FullPatch { get; set; }

        /// <summary>
        ///     Левый ключ
        /// </summary>
        public int L { get; set; }

        /// <summary>
        ///     Правый ключ
        /// </summary>
        public int R { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Статья движения денежных средств
        /// </value>
        [DBField("СтатьяДвиженияДенежныхСредств")]
        public new string Name
        {
            get { return string.IsNullOrEmpty(CashFlowItemNameBind.Value) ? "" : CashFlowItemNameBind.Value; }
            set { CashFlowItemNameBind.Value = value.Length == 0 ? "" : value; }
        }


        /// <summary>
        ///     Код вида движения денежных средств
        /// </summary>
        [DBField("КодВидаДвиженияДенежныхСредств", 0)]
        public int CashFlowTypeId
        {
            get { return string.IsNullOrEmpty(CashFlowTypeIdBind.Value) ? 0 : int.Parse(CashFlowTypeIdBind.Value); }
            set { CashFlowTypeIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Вид
        /// </summary>
        public string CashFlowTypeName { get; set; }

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
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Инициализация сущности на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодСтатьиДвиженияДенежныхСредств"].ToString();
                Name = dt.Rows[0]["СтатьяДвиженияДенежныхСредств"].ToString();
                CashFlowTypeId = Convert.ToInt16(dt.Rows[0]["КодВидаДвиженияДенежныхСредств"]);
                CashFlowTypeName = dt.Rows[0]["ВидДвиженияДенежныхСредств"].ToString();
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString();
                L = Convert.ToInt32(dt.Rows[0]["L"]);
                R = Convert.ToInt32(dt.Rows[0]["R"]);
                FullPatch = dt.Rows[0]["FullPath"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Получение списка из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<CashFlowItem> GetCashFlowItemList(DataTable dt)
        {
            var cashFlowItemList = new List<CashFlowItem>();

            for (var i = 0; i < dt.Rows.Count; i++)
                cashFlowItemList.Add(
                    new CashFlowItem
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодСтатьиДвиженияДенежныхСредств"].ToString(),
                        Name = dt.Rows[i]["СтатьяДвиженияДенежныхСредств"].ToString(),
                        CashFlowTypeId = Convert.ToInt16(dt.Rows[i]["КодВидаДвиженияДенежныхСредств"]),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString(),
                        L = Convert.ToInt16(dt.Rows[i]["L"]),
                        R = Convert.ToInt16(dt.Rows[i]["R"])
                    }
                );
            return cashFlowItemList;
        }

        /// <summary>
        ///     Получение списка подчиненных
        /// </summary>
        public List<CashFlowItem> GetChildLocationsList()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] {L, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@rightKey", new object[] {R, DBManager.ParameterTypes.Int32});
            var dt = DBManager.GetData(string.Format(SQLQueries.SELECT_РасположенияПодчиненные), CN,
                CommandType.Text, sqlParams);

            return GetCashFlowItemList(dt);
        }

        /// <summary>
        ///     Метод загрузки данных сущности
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ДенежныеСредстваПоID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        /// <param name="isNew">Выполнить повторную загрузку</param>
        public void Save(bool isNew)
        {
            if (isNew)
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@СтатьяДвиженияДенежныхСредств", Name},
                    {"@КодВидаДвиженияДенежныхСредств", CashFlowTypeId},
                    {"@Parent", Parent}
                };
                var сashFlowItemId = DBManager.ExecuteScalar(SQLQueries.INSERT_СтатьиДвиженияДенежныхСредств,
                    CommandType.Text, Config.DS_resource, sqlParams);

                if (сashFlowItemId != null)
                    Id = сashFlowItemId.ToString();
            }
            else
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодСтатьиДвиженияДенежныхСредств", Id},
                    {"@СтатьяДвиженияДенежныхСредств", Name},
                    {"@КодВидаДвиженияДенежныхСредств", CashFlowTypeId}
                };
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_СтатьиДвиженияДенежныхСредств, CommandType.Text,
                    Config.DS_resource, sqlParams);
            }
        }

        /// <summary>
        ///     Процедура удаления статьи движения денежных средств
        /// </summary>
        public void Delete()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодСтатьиДвиженияДенежныхСредств", Id}
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_СтатьиДвиженияДенежныхСредств, CommandType.Text,
                Config.DS_resource, sqlParams);
        }
    }
}