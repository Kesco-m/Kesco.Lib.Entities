using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Язык сотрудника
    /// </summary>
    [Serializable]
    public class Language : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктов
        /// </summary>
        /// <param name="id">Язык</param>
        public Language(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки по dbReader
        /// </summary>
        public Language()
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
        ///     Метод загрузки данных сущности "Языки"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>
                {{"@id", new object[] {Id, DBManager.ParameterTypes.String}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_Язык, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            var colЯзык = dbReader.GetOrdinal("Язык");
            var colОписание = dbReader.GetOrdinal("Описание");

            Unavailable = false;

            if (!dbReader.IsDBNull(colЯзык)) Id = dbReader.GetString(colЯзык);
            if (!dbReader.IsDBNull(colОписание)) Name = dbReader.GetString(colОписание);
        }
    }
}