using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Log;
using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Атрибут телефона пользователя
    /// </summary>
    [Serializable]
    [DBSource("vwАтрибутыТелефоновПользователя")]
    public class UserAttribute : Entity
    {
        private bool _isNew;

        /// <summary>
        ///     Признак нового документа
        /// </summary>
        public override bool IsNew => _isNew;

        /// <summary>
        ///     Конструктор сущности Атрибут пользователя
        /// </summary>
        public UserAttribute(string equipmentId)
        {
            _isNew = true;
            Name = "";
            AttributeValue = "";
            EquipmentId = equipmentId;
        }

        /// <summary>
        ///     Конструктор сущности Атрибут пользователя
        /// </summary>
        public UserAttribute(string attributeType, string equipmentId)
            : base(attributeType)
        {
            _isNew = false;
            EquipmentId = equipmentId;
        }

        /// <summary>
        ///     КодОборудования
        /// </summary>
        /// <value>
        ///     КодОборудования (int, not null)
        /// </value>
        [DBField("КодОборудования")]
        public string EquipmentId { get; set; }

        /// <summary>
        ///     ТипАтрибутаТелефона
        /// </summary>
        /// <value>
        ///     ТипАтрибутаТелефона (varchar(50), not null)
        /// </value>
        [DBField("ТипАтрибутаТелефона")]
        public override string Id
        {
            get { return AttributeTypeBind.Value; }
            set { AttributeTypeBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля ТипАтрибутаТелефона для связывания с контролом
        /// </summary>
        public BinderValue AttributeTypeBind = new BinderValue();

        /// <summary>
        ///     Значение
        /// </summary>
        /// <value>
        ///     Значение (varchar(150), not null)
        /// </value>
        [DBField("Значение")]
        public string AttributeValue
        {
            get { return AttributeValueBind.Value; }
            set { AttributeValueBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Описание для связывания с контролом
        /// </summary>
        public BinderValue AttributeValueBind = new BinderValue();

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
        public override string CN => Config.DS_user;

        /// <summary>
        ///     Метод загрузки данных сущности Атрибут телефона пользователя
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодОборудования", EquipmentId },
                { "@ТипАтрибутаТелефона", Id }
            };
            FillData(DBManager.GetData(SQLQueries.SELECT_АтрибутыТелефоновПользователя_ID, CN, CommandType.Text, sqlParams));
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
                AttributeValue = dt.Rows[0]["Значение"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод сохранения сущности Атрибут телефона пользователя
        /// </summary>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            if (IsNew && HasValue())
                throw new DetailedException("Атрибут телефона уже был настроен ранее", null);

            var sqlParams = new Dictionary<string, object>
            {
                { "@ТипАтрибутаТелефона", Id },
                { "@КодОборудования", EquipmentId },
                { "@Значение", AttributeValue }
            };

            DBManager.ExecuteNonQuery(
                IsNew
                    ? SQLQueries.INSERT_АтрибутыТелефоновПользователя
                    : SQLQueries.UPDATE_АтрибутыТелефоновПользователя,
                CommandType.Text, CN, sqlParams);
        }

        /// <summary>
        ///     Процедура удаления сущности Атрибут телефона пользователя
        /// </summary>
        public override void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@ТипАтрибутаТелефона", Id },
                { "@КодОборудования", EquipmentId }
            };
            DBManager.ExecuteNonQuery(SQLQueries.DELETE_АтрибутыТелефоновПользователя_ID, CommandType.Text, CN, sqlParams);;
        }

        /// <summary>
        ///     Проверка настроен ли атрибут пользователями
        /// </summary>
        /// <returns>Возвращает true при наличии настроек</returns>
        public bool HasValue()
        {
            var sqlParams = new Dictionary<string, object>
            {
                { "@ТипАтрибутаТелефона", Id },
                { "@КодОборудования", EquipmentId }
            };
            var result = DBManager.ExecuteScalar(SQLQueries.SELECT_АтрибутыТелефоновПользователя_Количество, CommandType.Text, CN, sqlParams);
            return result != null && int.Parse(result.ToString()) > 0;
        }
    }
}