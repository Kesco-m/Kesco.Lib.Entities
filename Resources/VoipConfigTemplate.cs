using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Resources
{
    /// <summary>
    ///     Класс сущности Шаблон конфигурации IP-телефона
    /// </summary>
    [Serializable]
    public class VoipConfigTemplate : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public VoipConfigTemplate()
        {
        }

        /// <summary>
        ///     Конструктор сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public VoipConfigTemplate(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Содержимое шаблона конфигурации IP-телефона
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public sealed override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_user
            : _connectionString;

        /// <summary>
        ///     Метод загрузки данных сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public sealed override void Load()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Метод сохранения сущности Шаблон конфигурации IP-телефона
        /// </summary>
        public void Save()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@Шаблон", Content}
            };

            if (string.IsNullOrEmpty(Id))
            {
                var templateId = DBManager.ExecuteScalar(SQLQueries.INSERT_ШаблоныIPТелефонов,
                    CommandType.Text, CN, sqlParams);

                if (templateId != null)
                    Id = templateId.ToString();
            }
            else
            {
                sqlParams.Add("@КодШаблонаIPТелефонов", Id);

                DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ШаблоныIPТелефонов,
                    CommandType.Text, CN, sqlParams);
            }
        }

        /// <summary>
        ///     Процедура удаления шаблона конфигурации IP-телефона
        /// </summary>
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}