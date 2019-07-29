using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Transport
{
    /// <summary>
    ///     Базис поставки
    /// </summary>
    [Serializable]
    public sealed class Basis : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID базиса</param>
        public Basis(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Basis()
        {
        }

        /// <summary>
        ///     КодБазисаПоставки
        /// </summary>
        public int CodeBasis { get; set; }

        /// <summary>
        ///     КодТранспортногоУзла
        /// </summary>
        public int CodeHub { get; set; }

        /// <summary>
        ///     КодЖелезнойДороги
        /// </summary>
        public int? CodeRailway { get; set; }

        /// <summary>
        ///     КодВидаТранспорта
        /// </summary>
        public int CodeTypeTransport { get; set; }

        /// <summary>
        ///     Инкотермс
        /// </summary>
        public string Incoterms { get; set; }

        /// <summary>
        ///     Инкотермс2010
        /// </summary>
        public string Incoterms2010 { get; set; }

        /// <summary>
        ///     НазваниеЛат
        /// </summary>
        public string NameEn { get; set; }

        /// <summary>
        ///     Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     ТранспортВЦене
        /// </summary>
        public byte InPrice { get; set; }

        /// <summary>
        ///     ВидТранспорта
        /// </summary>
        public string TypeTransport { get; set; }

        /// <summary>
        ///     Текст для контрола
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Строка подключения к БД.
        /// </summary>
        public override string CN => ConnString;

        /// <summary>
        ///     Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString => string.IsNullOrEmpty(_connectionString)
            ? _connectionString = Config.DS_document
            : _connectionString;

        /// <summary>
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_БазисПоставки, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                CodeBasis = (int) dt.Rows[0]["КодБазисаПоставки"];
                CodeTypeTransport = (int) dt.Rows[0]["КодВидаТранспорта"];
                Incoterms = dt.Rows[0]["Инкотермс"].ToString();
                Incoterms2010 = dt.Rows[0]["Инкотермс2010"].ToString();
                Name = dt.Rows[0]["Название"].ToString();
                NameEn = dt.Rows[0]["НазваниеЛат"].ToString();
                Description = dt.Rows[0]["Описание"].ToString();
                InPrice = (byte) dt.Rows[0]["ТранспортВЦене"];
                Text = dt.Rows[0]["ВидТранспорта"].ToString();
            }
            else
            {
                Unavailable = true;
            }
        }
    }
}