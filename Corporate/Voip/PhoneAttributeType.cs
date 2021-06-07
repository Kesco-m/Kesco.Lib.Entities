using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kesco.Lib.Entities.Corporate.Voip
{
    /// <summary>
    ///     Класс сущности Тип атрибута телефона
    /// </summary>
    [Serializable]
    [DBSource("ТипыАтрибутовТелефонов")]
    public class PhoneAttributeType : Entity
    {
        /// <summary>
        ///     Конструктор сущности Тип атрибута телефона
        /// </summary>
        public PhoneAttributeType()
        {
            Name = "";
            ValuesSource = 0;
            Description = "";
        }

        /// <summary>
        ///     Конструктор сущности Тип атрибута телефона
        /// </summary>
        public PhoneAttributeType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Метод загрузки данных сущности Тип атрибута телефона
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> { { "@ТипАтрибутаТелефона", new object[] { Id, DBManager.ParameterTypes.String } } };
            FillData(DBManager.GetData(SQLQueries.SELECT_ТипыАтрибутовТелефонов_ID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности Тип атрибута телефона на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                Name = dt.Rows[0]["ТипАтрибутаТелефона"].ToString();
                Description = dt.Rows[0]["Описание"].ToString();
                ValuesSource = byte.Parse(dt.Rows[0]["ИсточникЗначений"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Источник значений
        /// </summary>
        /// <value>
        ///     Источник значений (tinyint, not null)
        /// </value>
        [DBField("ИсточникЗначений")]
        public byte ValuesSource
        {
            get { return byte.Parse(ValuesSourceBind.Value); }
            set { ValuesSourceBind.Value = value.ToString(); }
        }

        /// <summary>
        ///     Подготовка поля Источник значений для связывания с контролом
        /// </summary>
        public BinderValue ValuesSourceBind = new BinderValue();

        /// <summary>
        ///     Описание
        /// </summary>
        /// <value>
        ///     Описание (varchar(1000), not null)
        /// </value>
        [DBField("Описание")]
        public string Description
        {
            get { return DescriptionBind.Value; }
            set { DescriptionBind.Value = value; }
        }

        /// <summary>
        ///     Подготовка поля Описание для связывания с контролом
        /// </summary>
        public BinderValue DescriptionBind = new BinderValue();

        /// <summary>
        ///     Строка подключения к БД
        /// </summary>
        public sealed override string CN => Config.DS_user;
    }
}