using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Stores
{
    /// <summary>
    ///     Тип склада
    /// </summary>
    [Serializable]
    public class StoreType : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID типа склада</param>
        public StoreType(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Типы складов
        /// </summary>
        public StoreType()
        {
        }

        /// <summary>
        ///     Псевдоним
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        ///     ID Корневого ресурса
        /// </summary>
        public string RootSource { get; set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_person;

                return _connectionString;
            }
        }

        private StoreType SetData(DataRow r)
        {
            Unavailable = false;
            Id = r["КодТипаСклада"].ToString();
            Name = r["ТипСклада"].ToString();
            Alias = r["Псевдоним"].ToString();
            RootSource = r["КорневойРесурс"].ToString();
            Note = r["Примечание"].ToString();
            return this;
        }

        /// <summary>
        ///     Запрос списка типов складов
        /// </summary>
        /// <returns></returns>
        public static List<StoreType> GetList()
        {
            var dt = DBManager.GetData(SQLQueries.SELECT_ТипыСкладов, Config.DS_person, CommandType.Text, null);
            if (null == dt) return null;

            var l = new List<StoreType>(dt.Rows.Count);
            foreach (DataRow r in dt.Rows)
                l.Add(new StoreType().SetData(r));

            return l;
        }

        /// <summary>
        ///     Метод заполнения полей сущности "Тип склада" из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
                SetData(dt.Rows[0]);
            else
                Unavailable = true;
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Тип склада"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_ТипСклада, CN, CommandType.Text, sqlParams));
        }

        #region Методы проверки категории типа склада (валютный/продуктовый/денежный)

        /// <summary>
        ///     Проверка типа склада: Валютный/не валютный
        /// </summary>
        /// <param name="storeTypeID">Код типа склада</param>
        /// <returns>True - валютный / False - не валютный</returns>
        public static bool IsCurrensyAccount(int storeTypeID)
        {
            return storeTypeID == 2;
        }

        /// <summary>
        ///     Проверка типа склада: Продуктовый/не продуктовый
        /// </summary>
        /// <param name="storeTypeID">Код типа склада</param>
        /// <param name="withVirtual">Учесть виртуальный склад</param>
        /// <returns>True - продуктовый / False - не продуктовый</returns>
        public static bool IsProdStore(int storeTypeID, bool withVirtual = true)
        {
            var isProdStore = storeTypeID >= 21 && storeTypeID <= 23;
            return withVirtual ? isProdStore || storeTypeID == -1 : isProdStore;
        }

        /// <summary>
        ///     Проверка типа склада: Денежный/не денежный
        /// </summary>
        /// <param name="storeTypeID">Код типа склада</param>
        /// <returns>True - денежный / False - не денежный</returns>
        public static bool IsMoneyStore(int storeTypeID)
        {
            return storeTypeID >= 1 && storeTypeID <= 12;
        }

        #endregion
    }
}