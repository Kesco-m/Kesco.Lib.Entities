using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Пул IP-Телефонов
    /// </summary>
    [Serializable]
    [DBSource("vwПулыТелефонов")]
    public class PhonePool : Entity
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PhonePool()
        {
            Name = "";
        }

        /// <summary>
        ///     Конструктор с загрузкой данных
        /// </summary>
        public PhonePool(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => Config.DS_user;

        /// <summary>
        ///     Метод загрузки данных сущности Пул IP-Телефонов
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@id", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ПулТелефона, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Пул IP-Телефонов на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Name = dt.Rows[0]["ПулТелефона"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@ПулТелефона", Name },
            };

            if (IsNew)
            {
                var poolId = DBManager.ExecuteScalar(SQLQueries.INSERT_ПулыТелефонов, CommandType.Text, CN, sqlParams);

                if (poolId != null)
                    Id = poolId.ToString();
            }
            else
            {
                sqlParams.Add("@КодПулаТелефонов", Id);
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ПулыТелефонов, CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Функция удаления пула
        /// </summary>
        public override void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодПулаТелефонов", Id}
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_ПулыТелефонов, CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Привязать к пулу оборудование
        /// </summary>
        /// <param name="equipmentId">Код оборудования</param>
        /// <param name="message">Сообщение с результатом операции</param>
        /// <returns>Возвращает true, если операция выполнена успешно</returns>
        public bool SetEquipment(string equipmentId, out string message)
        {
            message = string.Empty;

            if (equipmentId.IsNullEmptyOrZero()) return false;

            var sqlParams = new Dictionary<string, object>
            {
                {"@КодОборудования", equipmentId},
                {"@КодПулаТелефонов", Id}
            };

            DBManager.ExecuteScalar(SQLQueries.UPDATE_ОборудованиеПулТелефона, 
                CommandType.Text, Config.DS_user, sqlParams);

            message = string.Format(@"К пулу ""{0}"" добавлено оборудование с кодом ""{1}""", Name, equipmentId);
            return true;
        }

        /// <summary>
        ///     Сбросить пул у оборудования
        /// </summary>
        /// <param name="equipmentId">Код оборудования</param>
        /// <param name="message">Сообщение с результатом операции</param>
        /// <returns>Возвращает true, если операция выполнена успешно</returns>
        public bool ResetEquipment(string equipmentId, out string message)
        {
            message = string.Empty;

            if (equipmentId.Length == 0) return false;

            var sqlParams = new Dictionary<string, object>
            {
                {"@КодОборудования", equipmentId},
                {"@КодПулаТелефонов", null}
            };

            DBManager.ExecuteScalar(SQLQueries.UPDATE_ОборудованиеПулТелефона, CommandType.Text, Config.DS_user,
                sqlParams);

            message = string.Format(@"От пула ""{0}"" отвязано оборудование с кодом {1}", Name, equipmentId);
            return true;
        }

        #region Поля сущности

        /// <summary>
        ///     КодПулаТелефонов
        /// </summary>
        /// <value>
        ///     КодПулаТелефонов (int, not null)
        /// </value>
        [DBField("КодПулаТелефонов", 0)]
        public override string Id
        {
            get { return string.IsNullOrEmpty(PoolIdBind.Value) ? "" : PoolIdBind.Value; }
            set { PoolIdBind.Value = value == null || value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Подготовка поля КодПулаТелефонов для связывания с контролом
        /// </summary>
        public BinderValue PoolIdBind = new BinderValue();

        /// <summary>
        ///     ПулТелефонов
        /// </summary>
        /// <value>
        ///     ПулТелефонов (varchar(50), not null)
        /// </value>
        [DBField("ПулТелефонов")]
        public new string Name
        {
            get { return PoolNameBind.Value; }
            set { PoolNameBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля ПулТелефонов для связывания с контролом
        /// </summary>
        public BinderValue PoolNameBind = new BinderValue();

        #endregion
    }
}