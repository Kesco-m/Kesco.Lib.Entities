using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Grid
{
    /// <summary>
    ///     Класс сущности Колонки запроса
    /// </summary>
    [Serializable]
    public class QueryColumn : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public QueryColumn()
        {
        }

        /// <summary>
        ///     Код колонки
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        ///     Код запроса
        /// </summary>
        public int QueryId { get; set; }

        /// <summary>
        ///     Колонка
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     Заголовок колонки
        /// </summary>
        public string ColumnHeader { get; set; }

        /// <summary>
        ///     Формат
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        ///     Тип
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Порядок
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        ///     Значение фильтра
        /// </summary>
        public string FilterValue { get; set; }

        /// <summary>
        ///     Обязательность фильтра
        /// </summary>
        public bool FilterRequired { get; set; }

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
        ///     Получение набора колонок по коду запроса
        /// </summary>
        /// <param name="queryId">Код запроса</param>
        public static List<QueryColumn> GetQueryColumn(string queryId)
        {
            var sqlColumnsParams = new Dictionary<string, object>
                {{"@КодЗапроса", new object[] { queryId, DBManager.ParameterTypes.Int32}}};
            var dt = DBManager.GetData(SQLQueries.SELECT_ЗапросыКолонкиПоКодуЗапроса, Config.DS_user,
                CommandType.Text, sqlColumnsParams);

            return GetQueryColumnList(dt);
        }

        /// <summary>
        ///     Получение списка колонок из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных ресурсов</param>
        public static List<QueryColumn> GetQueryColumnList(DataTable dt)
        {
            var queryColumnList = new List<QueryColumn>();

            for (var i = 0; i < dt.Rows.Count; i++)
                queryColumnList.Add(
                    new QueryColumn
                    {
                        QueryId = Convert.ToInt32(dt.Rows[i]["КодКолонки"].ToString()),
                        Id = dt.Rows[i]["КодКолонки"].ToString(),
                        ColumnId = Convert.ToInt32(dt.Rows[i]["КодКолонки"].ToString()),
                        ColumnName = dt.Rows[i]["Колонка"].ToString(),
                        ColumnHeader = dt.Rows[i]["ЗаголовокКолонки"].ToString(),
                        Format = dt.Rows[i]["Формат"].ToString(),
                        Type = Convert.ToInt32(dt.Rows[i]["Тип"].ToString()),
                        Key = dt.Rows[i]["КлючUrl"].ToString(),
                        Order = dt.Rows[i]["Порядок"].ToString().IsNullEmptyOrZero()? (int?)null : Convert.ToInt32(dt.Rows[i]["Порядок"].ToString()),

                    });

            return queryColumnList;
        }
    }
}