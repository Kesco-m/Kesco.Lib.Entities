using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Persons.Dossier
{
    /// <summary>
    /// Класс сущности Контекст досье лица
    /// </summary>
    [Serializable]
    public class DossierContext : Entity
    {
        #region Поля сущности "Контекст досье лица"
        /// <summary>
        /// Код
        /// </summary>
        public int? Код { get; set; }
        /// <summary>
        /// Надпись
        /// </summary>
        public string Надпись { get; set; }
        /// <summary>
        /// Поле
        /// </summary>
        public string Поле { get; set; }
        /// <summary>
        /// КодВкладки
        /// </summary>
        public int? КодВкладки { get; set; }
        /// <summary>
        /// Ссылка
        /// </summary>
        public int? Ссылка { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        public int? Порядок { get; set; }
        /// <summary>
        /// Сортировка
        /// </summary>
        public int? Сортировка { get; set; }
        /// <summary>
        /// Изменил
        /// </summary>
        public string Изменил { get; set; }
        /// <summary>
        /// Изменено
        /// </summary>
        public DateTime Изменено { get; set; }
        /// <summary>
        /// ТекстСсылки
        /// </summary>
        public string ТекстСсылки { get; set; }
        /// <summary>
        /// КодСотрудникаКтоИзменил
        /// </summary>
        public int? КодСотрудникаКтоИзменил { get; set; }
        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_person) : _connectionString; }
        }
        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        public DossierContext()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public DossierContext(string id):base(id)
        {
            Load();
        }

        /// <summary>
        /// Метод загрузки данных сущности "Контекст досье лица"
        /// </summary>
        public override void Load()
        {
            //var sqlParams = new Dictionary<string, object> { { "@КодЛица", new object[] { Id, DBManager.ParameterTypes.Int32 } } };
            //using (var dbReader = new DBReader(SQLQueries.SQLДосьеЛица, CommandType.Text, CN, sqlParams))
            //{
            //    while (dbReader.Read())
            //    {
            //        if (dbReader.HasRows)
            //        {

            //        }
            //    }

            //    if (dbReader.NextResult())
            //    {
            //        while (dbReader.Read())
            //        {
            //            if (dbReader.HasRows)
            //            {

            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbReader"></param>
        public void LoadFromDbReader(DBReader dbReader)
        {
            
                #region Получение порядкового номера столбца

                int colКод = dbReader.GetOrdinal("Код");
                int colНадпись = dbReader.GetOrdinal("Надпись");
                int colПоле = dbReader.GetOrdinal("Поле");
                int colКодВкладки = dbReader.GetOrdinal("КодВкладки");
                int colСсылка = dbReader.GetOrdinal("Ссылка");
                int colПорядок = dbReader.GetOrdinal("Порядок");
                int colСортировка = dbReader.GetOrdinal("Sort");
                int colИзменил = dbReader.GetOrdinal("Изменил");
                int colИзменено = dbReader.GetOrdinal("Изменено");
                int colТекстСсылки = dbReader.GetOrdinal("ТекстСсылки");
                int colКодСотрудникаКтоИзменил = dbReader.GetOrdinal("КодСотрудникаКтоИзменил");

                #endregion
                
                Unavailable = false;

                Код = dbReader.GetInt32(colКод);
                if (!dbReader.IsDBNull(colНадпись)) { Надпись = dbReader.GetString(colНадпись); }
                if (!dbReader.IsDBNull(colПоле)) { Поле = dbReader.GetString(colПоле); }
                if (!dbReader.IsDBNull(colКодВкладки)) { КодВкладки = dbReader.GetInt32(colКодВкладки); }
                if (!dbReader.IsDBNull(colСсылка)) { Ссылка = dbReader.GetInt32(colСсылка); }
                if (!dbReader.IsDBNull(colПорядок)) { Порядок = dbReader.GetInt32(colПорядок); }
                if (!dbReader.IsDBNull(colСортировка)) { Сортировка = dbReader.GetInt32(colСортировка); }
                if (!dbReader.IsDBNull(colИзменил)) { Изменил = dbReader.GetString(colИзменил); }
                if (!dbReader.IsDBNull(colИзменено)) { Изменено = dbReader.GetDateTime(colИзменено); }
                if (!dbReader.IsDBNull(colТекстСсылки)) { ТекстСсылки = dbReader.GetString(colТекстСсылки); }
                if (!dbReader.IsDBNull(colКодСотрудникаКтоИзменил)) { КодСотрудникаКтоИзменил = dbReader.GetInt32(colКодСотрудникаКтоИзменил); }

        }

        #endregion
    }
}
