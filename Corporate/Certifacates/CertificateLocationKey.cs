using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate.Equipments
{
    /// <summary>
    ///     Класс сущности расположение ключа сертификата
    /// </summary>
    [Serializable]
    public class CertificateLocationKey : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID</param>
        public CertificateLocationKey(string id) : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public CertificateLocationKey()
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
        ///     Инициализация сущности на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодСертификатаРасположенияКлюча"].ToString();
                CertificatelId = (int)dt.Rows[0]["КодСертификата"];
                Location = (string) dt.Rows[0]["Расположение"];
                EmployeeId = dt.Rows[0]["КодСотрудникаПользователя"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["КодСотрудникаПользователя"]);
                Employee = (string) dt.Rows[0]["Сотрудник"];
                ECP = (string) dt.Rows[0]["КодVirtualPC"];
                ChangeEmployeeId = (int) dt.Rows[0]["Изменил"];
                ChangedDate = (DateTime) dt.Rows[0]["Изменено"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        ///     Метод загрузки данных сущности 
        /// </summary>
        public sealed override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_ID_СертификатРасположенияКлюча, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        ///     Инициализация сущности на основе DBReader
        /// </summary>
        /// <param name="dbReader">Объект типа DBReader</param>
        /// <returns>Список расположений ключей</returns>
        public static List<CertificateLocationKey> GetLocationKeysList(DBReader dbReader)
        {
            var list = new List<CertificateLocationKey>();
            if (!dbReader.HasRows) return list;

            var colКодСертификатаРасположенияКлюча = dbReader.GetOrdinal("КодСертификатаРасположенияКлюча");
            var colКодСертификата = dbReader.GetOrdinal("КодСертификата");
            var colРасположение = dbReader.GetOrdinal("Расположение");
            var colКодСотрудникаПользователя = dbReader.GetOrdinal("КодСотрудникаПользователя");
            var colСотрудник = dbReader.GetOrdinal("Сотрудник");
            var colИдентификаторЭЦП = dbReader.GetOrdinal("КодVirtualPC");
            var colИзменил = dbReader.GetOrdinal("Изменил");
            var colИзменено = dbReader.GetOrdinal("Изменено");

            while (dbReader.Read())
            {
                var row = new CertificateLocationKey();
                row.Unavailable = false;
                row.Id = dbReader.GetInt32(colКодСертификатаРасположенияКлюча).ToString();
                row.CertificatelId = dbReader.GetInt32(colКодСертификата);
                row.Location = dbReader.GetString(colРасположение);
                if (!dbReader.IsDBNull(colКодСотрудникаПользователя))
                {
                    row.EmployeeId = dbReader.GetInt32(colКодСотрудникаПользователя);
                }
                row.Employee = dbReader.GetString(colСотрудник);
                row.ECP = dbReader.GetString(colИдентификаторЭЦП);
                row.ChangeEmployeeId = dbReader.GetInt32(colИзменил);
                if (!dbReader.IsDBNull(colИзменено)) {
                    row.ChangedDate = dbReader.GetDateTime(colИзменено);
                }

                list.Add(row);
            }

            return list;
        }

        #region Поля сущности

        /// <summary>
        ///     Код Сертификата
        /// </summary>
        /// <value>
        ///     КодСертификата (int, null)
        /// </value>
        public int CertificatelId { get; set; }

        /// <summary>
        ///     Расположение
        /// </summary>
        /// <value>
        ///     Location (varchar(100), not null)
        /// </value>
        public string Location { get; set; }

        /// <summary>
        ///     Код Сотрудника Пользователя
        /// </summary>
        /// <value>
        ///     КодСотрудникаПользователя (int, null)
        /// </value>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        /// <value>
        ///     Сотрудник (varchar(150), null)
        /// </value>
        public string Employee { get; set; }

        /// <summary>
        ///     КодVirtualPC
        /// </summary>
        /// <value>
        ///     КодVirtualPC (varchar(50), null)
        /// </value>
        public string ECP { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменил (int, not null)
        /// </value>
        public int ChangeEmployeeId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Изменено (datetime, not null)
        /// </value>
        public DateTime ChangedDate { get; set; }

        #endregion

    }
}