using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Stores
{
    /// <summary>
    ///     Бизнес-объект - Место хранения
    /// </summary>
    [Serializable]
    public class Residence : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID места хранения</param>
        public Residence(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Residence()
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
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Инициализация сущности "МестоХранения" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодМестаХранения"].ToString();
                Name = dt.Rows[0]["МестоХранения"].ToString();
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
        ///     Получение списка мест хранения из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<Residence> GetResidencesList(DataTable dt)
        {
            var residenceList = new List<Residence>();

            for (var i = 0; i < dt.Rows.Count; i++)
                residenceList.Add(
                    new Residence
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодМестаХранения"].ToString(),
                        Name = dt.Rows[i]["МестоХранения"].ToString(),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString(),
                        L = Convert.ToInt16(dt.Rows[i]["L"]),
                        R = Convert.ToInt16(dt.Rows[i]["R"])
                    }
                );
            return residenceList;
        }

        /// <summary>
        ///     Получение списка подчиненных мест хранения
        /// </summary>
        public List<Residence> GetChildResidencesList()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] {L, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@rightKey", new object[] {R, DBManager.ParameterTypes.Int32});
            var dt = DBManager.GetData(string.Format(SQLQueries.SELECT_МестоХранения_Подчиненные), CN, CommandType.Text,
                sqlParams);

            return GetResidencesList(dt);
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Место хранения"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_МестоХранения, CN, CommandType.Text, sqlParams));
        }

        #region Поля сущности "Место хранения"

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

        #endregion
    }
}