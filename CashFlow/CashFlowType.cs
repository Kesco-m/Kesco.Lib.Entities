using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.CashFlow
{
    /// <summary>
    ///     Вид движения денежных средств
    /// </summary>
    [Serializable]
    [DBSource("ВидыДвиженийДенежныхСредств")]
    public class CashFlowType : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        public static string _connectionString;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public CashFlowType()
        {
            Id = "";
            Name = "";
            Name1С = "";
        }

        /// <summary>
        ///     Конструктор с загрузкой данных
        /// </summary>
        public CashFlowType(string id)
        {
            Id = id;
            FillData(id);
        }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_resource;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString =>
            string.IsNullOrEmpty(_connectionString)
                ? _connectionString = Config.DS_resource
                : _connectionString;

        /// <summary>
        ///     Заполнить данные текущего
        /// </summary>
        public void FillData(string id)
        {
            if (id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ВидДвиженияДенежныхСредств, id.ToInt(),
                CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодВидаДвиженияДенежныхСредств = dbReader.GetOrdinal("КодВидаДвиженияДенежныхСредств");
                    var colВидДвиженияДенежныхСредств = dbReader.GetOrdinal("ВидДвиженияДенежныхСредств");
                    var colНазвание1С = dbReader.GetOrdinal("Название1С");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодВидаДвиженияДенежныхСредств).ToString();
                        Name = CashFlowItemType = dbReader.GetString(colВидДвиженияДенежныхСредств);
                        Name1С = dbReader.GetString(colНазвание1С);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///     Получить список запросом
        /// </summary>
        public List<CashFlowType> GetCashFlowTypeList(string query)
        {
            List<CashFlowType> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    list = new List<CashFlowType>();

                    #region Получение порядкового номера столбца

                    var colКодВидаДвиженияДенежныхСредств = dbReader.GetOrdinal("КодВидаДвиженияДенежныхСредств");
                    var colВидДвиженияДенежныхСредств = dbReader.GetOrdinal("ВидДвиженияДенежныхСредств");
                    var colНазвание1С = dbReader.GetOrdinal("Название1С");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new CashFlowType();
                        row.Unavailable = false;
                        row.Id = dbReader.GetString(colКодВидаДвиженияДенежныхСредств);
                        row.Name = dbReader.GetString(colВидДвиженияДенежныхСредств);
                        row.Name1С = dbReader.GetString(colНазвание1С);
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Метод сохранения
        /// </summary>
        /// <param name="isNew">Новый вид движения денежных стредств</param>
        public void Save(bool isNew)
        {
            if (isNew)
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодВидаДвиженияДенежныхСредств", Id},
                    {"@ВидДвиженияДенежныхСредств", Name},
                    {"@Название1С", Name1С}
                };
                var сashFlowTypeId = DBManager.ExecuteScalar(SQLQueries.INSERT_ВидыДвиженияДенежныхСредств,
                    CommandType.Text, Config.DS_resource, sqlParams);
            }
            else
            {
                var sqlParams = new Dictionary<string, object>
                {
                    {"@КодВидаДвиженияДенежныхСредств", Id},
                    {"@ВидДвиженияДенежныхСредств", Name},
                    {"@Название1С", Name1С}
                };
                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ВидыДвиженияДенежныхСредств, CommandType.Text,
                    Config.DS_resource, sqlParams);
            }
        }

        /// <summary>
        ///     Функция удаления вида движения денежных средств
        /// </summary>
        public void Delete()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодВидаДвиженияДенежныхСредств", Id}
            };
            DBManager.ExecuteNonQuery(SQLQueries.Delete_ВидыДвиженияДенежныхСредств, CommandType.Text,
                Config.DS_resource, sqlParams);
        }

        /// <summary>
        ///     Метод проверки на дубль кода
        /// </summary>
        public bool ExistIdDubl()
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ВидДвиженияДенежныхСредств, Id.ToInt(),
                CommandType.Text, CN))
            {
                if (dbReader.HasRows) return true;
            }

            return false;
        }

        #region Поля сущности

        /// <summary>
        ///     КодВидаДвиженияДенежныхСредств
        /// </summary>
        /// <value>
        ///     КодВидаДвиженияДенежныхСредств (int, not null)
        /// </value>
        [DBField("КодВидаДвиженияДенежныхСредств", 0)]
        public override string Id
        {
            get { return string.IsNullOrEmpty(CashFlowTypeIdBind.Value) ? "" : CashFlowTypeIdBind.Value; }
            set { CashFlowTypeIdBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Подготовка поля КодВидаДвиженияДенежныхСредств для связывания с контролом
        /// </summary>
        public BinderValue CashFlowTypeIdBind = new BinderValue();

        /// <summary>
        ///     ВидДвиженияДенежныхСредств
        /// </summary>
        /// <value>
        ///     ВидДвиженияДенежныхСредств (nvarchar(100), not null)
        /// </value>
        [DBField("ВидДвиженияДенежныхСредств")]
        public string CashFlowItemType
        {
            get { return string.IsNullOrEmpty(CashFlowItemTypeBind.Value) ? "" : CashFlowItemTypeBind.Value; }
            set { CashFlowItemTypeBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Подготовка поля ВидДвиженияДенежныхСредств для связывания с контролом
        /// </summary>
        public BinderValue CashFlowItemTypeBind = new BinderValue();

        /// <summary>
        ///     Название
        /// </summary>
        /// <value>
        ///     Название (nvarchar(100), not null)
        /// </value>
        [DBField("Название")]
        public new string Name
        {
            get { return string.IsNullOrEmpty(CashFlowTypeNameBind.Value) ? "" : CashFlowTypeNameBind.Value; }
            set { CashFlowTypeNameBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Объект связывания вида движения денежных средств
        /// </summary>
        public BinderValue CashFlowTypeNameBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Название1С (nvarchar(100), not null)
        /// </value>
        [DBField("Название1С")]
        public string Name1С
        {
            get { return string.IsNullOrEmpty(CashFlowTypeName1CBind.Value) ? "" : CashFlowTypeName1CBind.Value; }
            set { CashFlowTypeName1CBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Объект связывания объетка на форме и объекта данных
        /// </summary>
        public BinderValue CashFlowTypeName1CBind = new BinderValue();

        #endregion
    }
}