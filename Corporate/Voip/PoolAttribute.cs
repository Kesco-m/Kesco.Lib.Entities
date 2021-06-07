using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Kesco.Lib.Log;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Атрибут пула
    /// </summary>
    [Serializable]
    [DBSource("vwАтрибутыТелефоновПула")]
    public class PoolAttribute : Entity
    {
        private bool _isNew;

        /// <summary>
        ///     Признак нового документа
        /// </summary>
        public override bool IsNew => _isNew;

        /// <summary>
        ///     Конструктор сущности Атрибут пула
        /// </summary>
        public PoolAttribute(string id) : base("0")
        {
            var ids = id.Split(':');
            if (ids.Count() != 2)
                throw new LogicalException("Не удалось установить идентификатор сущности Атрибут пула", "",
                    Assembly.GetExecutingAssembly().GetName(), Priority.Info);
            PoolId = ids[0];
            Type = ids[1];
            _isNew = !HasValue();
            Load();
        }

        /// <summary>
        ///     Код пула телефонов
        /// </summary>
        /// <value>
        ///     КодПулаТелефонов (int, not null)
        /// </value>
        [DBField("КодПулаТелефонов")]
        public string PoolId { get; set; }

        /// <summary>
        ///     Тип атрибута телефона
        /// </summary>
        /// <value>
        ///     ТипАтрибутаТелефона (varchar(50), not null)
        /// </value>
        [DBField("ТипАтрибутаТелефона")]
        public string Type { get; set; }

        /// <summary>
        ///     Значение атрибута телефона
        /// </summary>
        /// <value>
        ///     Значение (varchar(150), not null)
        /// </value>
        [DBField("Значение")]
        public string Value
        {
            get { return ValueBind.Value; }
            set { ValueBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Значение для связывания с контролом
        /// </summary>
        public BinderValue ValueBind = new BinderValue();

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
        ///     Строка подключения к БД
        /// </summary>
        public override string CN => Config.DS_user;

        /// <summary>
        ///     Метод загрузки данных сущности Атрибут пула
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодПулаТелефонов", PoolId },
                { "@ТипАтрибутаТелефона", Type }
            };
            FillData(DBManager.GetData(SQLQueries.SELECT_АтрибутыТелефоновПула_ID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        { 
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Value = dt.Rows[0]["Значение"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод сохранения сущности Атрибут пула
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодПулаТелефонов", PoolId },
                { "@ТипАтрибутаТелефона", Type },
                { "@Значение", Value }
            };

            DBManager.ExecuteNonQuery(
                _isNew
                    ? SQLQueries.INSERT_АтрибутыТелефоновПула
                    : SQLQueries.UPDATE_АтрибутыТелефоновПула,
                CommandType.Text, CN, sqlParams);

            _isNew = false;
        }

        /// <summary>
        ///     Метод удаления сущности Атрибут пула
        /// </summary>
        public override void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодПулаТелефонов", PoolId },
                { "@ТипАтрибутаТелефона", Type }
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_АтрибутыТелефоновПула, CommandType.Text, CN, sqlParams); ;
        }

        /// <summary>
        ///     Проверка настроен ли атрибут инженерами
        /// </summary>
        /// <returns>Возвращает true при наличии настроек</returns>
        public bool HasValue()
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодПулаТелефонов", PoolId },
                { "@ТипАтрибутаТелефона", Type },
            };
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_АтрибутыТелефоновПула_Количество, CommandType.Text, CN, sqlParams);
            return result != null && int.Parse(result.ToString()) > 0;
        }

        /// <summary>
        ///     Проверить, существуют ли значения атрибутов для заданного пула
        /// </summary>
        /// <param name="poolId">Код пула</param>
        /// <returns>Возвращает true, если значения существуют</returns>
        public static bool CheckExists(string poolId)
        {
            var sqlParams = new Dictionary<string, object> { { "@КодПулаТелефонов", poolId } };
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_АтрибутыТелефоновПула_Количество,
                CommandType.Text, Config.DS_user, sqlParams);
            return result != null && int.Parse(result.ToString()) > 0;
        }
    }
}