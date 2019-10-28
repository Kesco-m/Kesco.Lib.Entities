using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "Дополнительные права для указаний"
    /// </summary>
    [Serializable]
    public class AdvancedGrant : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">AdvancedGrant</param>
        public AdvancedGrant(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public AdvancedGrant()
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
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///     Описание права на английском
        /// </summary>
        public string NameEn { get; set; }

        /// <summary>
        ///     Можно выбирать
        /// </summary>
        public bool NotAlive { get; set; }

        /// <summary>
        ///     Порядок
        /// </summary>
        public int OrderOutput { get; set; }

        /// <summary>
        ///     ПараметрОтноситсяК
        /// </summary>
        public int RefersTo { get; set; }

        /// <summary>
        ///     Метод загрузки данных сущности "Дополнительное право"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_AdvancedGrant, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colКод = dbReader.GetOrdinal("КодДопПараметраУказанийИТ");
            var colОписание = dbReader.GetOrdinal("Описание");
            var colNameEn = dbReader.GetOrdinal("ОписаниеЛат");
            var colNotAlive = dbReader.GetOrdinal("НельзяВыбрать");
            var colOrderOutput = dbReader.GetOrdinal("ПорядокВывода");
            var colRefersTo = dbReader.GetOrdinal("ПараметрОтноситсяК");
            Unavailable = false;

            if (!dbReader.IsDBNull(colКод)) Id = dbReader.GetInt32(colКод).ToString();
            if (!dbReader.IsDBNull(colОписание)) Name = dbReader.GetString(colОписание);
            if (!dbReader.IsDBNull(colNameEn)) NameEn = dbReader.GetString(colNameEn);
            if (!dbReader.IsDBNull(colNotAlive)) NotAlive = Convert.ToBoolean(dbReader.GetByte(colNotAlive));
            if (!dbReader.IsDBNull(colOrderOutput)) OrderOutput = Convert.ToInt32(dbReader.GetByte(colOrderOutput));
            if (!dbReader.IsDBNull(colRefersTo)) RefersTo = dbReader.GetInt32(colRefersTo);
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
                Id = dt.Rows[0]["КодДопПараметраУказанийИТ"].ToString();
                Name = dt.Rows[0]["Описание"].ToString();
                NameEn = dt.Rows[0]["ОписаниеЛат"].ToString();
                NotAlive = int.Parse(dt.Rows[0]["НельзяВыбрать"].ToString()) == 1;
                OrderOutput = int.Parse(dt.Rows[0]["ПорядокВывода"].ToString());
                RefersTo = int.Parse(dt.Rows[0]["ПараметрОтноситсяК"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}