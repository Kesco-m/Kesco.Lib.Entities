using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Stores
{
    /// <summary>
    /// Бизнес-объект - склад
    /// </summary>
    [Serializable]
    public class Store : Entity
    {
        #region Поля сущности

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// IBAN
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// КодТипаСклада
        /// </summary>
        public int StoreTypeId { get; set; }

        /// <summary>
        /// Название ТипаСклада
        /// </summary>
        public string StoreTypeName { get; set; }

        /// <summary>
        /// КодМестаХранения
        /// </summary>
        public int? ResidenceId { get; set; }

        /// <summary>
        /// КодРесурса
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// Название Ресурса
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// КодХранителя
        /// </summary>
        public int? KeeperId { get; set; }

        /// <summary>
        /// Название Хранителя
        /// </summary>
        public string KeeperName { get; set; }

        /// <summary>
        /// КодРаспорядителя
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// Название распорядителя
        /// </summary>
        public string ManagerName { get; set; }

        /// <summary>
        /// КодПодразделенияРаспорядителя
        /// </summary>
        public int? ManagerSubdivisionCode { get; set; }

        /// <summary>
        /// КодДоговора
        /// </summary>
        public int? AgreementCode { get; set; }

        /// <summary>
        /// Филиал
        /// </summary>
        public string Subsidiary { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Изменил
        /// </summary>
        public string ChangeBy { get; set; }

        /// <summary>
        /// Действует
        /// </summary>
        public bool Alive { get; set; }
        
        /// <summary>
		/// От
		/// </summary>
		public DateTime? From { get; set; }

		/// <summary>
		/// До
		/// </summary>
		public DateTime? To { get; set; }

        #endregion

        #region Свойства-аксессоры сущности

        /// <summary>
        /// ТипСклада
        /// </summary>
        public StoreType StoreType 
        {
            get
            {
                return StoreTypeId > 0 ? new StoreType(StoreTypeId.ToString()) : null;
            }
        }

        /// <summary>
        /// МестоХранения
        /// </summary>
        public Residence Residence
        {
            get
            {
                return ResidenceId != null && ResidenceId > 0 ? new Residence(ResidenceId.ToString()) : null;
            }
        }


        private Resource _resource;
        /// <summary>
        /// РесурсРус
        /// </summary>
        public Resource Resource
        {
            get
            {
                if (ResourceId == 0) _resource = null;
                else if (_resource == null || _resource.Id!=ResourceId.ToString()) _resource = new Resource(ResourceId.ToString());
                return _resource;
            }
        }

        /// <summary>
        /// Хранитель
        /// </summary>
        public Person Keeper
        {
            get
            {
                return KeeperId != null && KeeperId > 0 ? new Person(KeeperId.ToString()) : null;
            }
        }

        /// <summary>
        /// Распорядитель
        /// </summary>
        public Person Manager
        {
            get
            {
                return ManagerId != null && ManagerId > 0 ? new Person(ManagerId.ToString()) : null;
            }
        }

        #endregion

        #region Константы

        /// <summary>
        /// Код потерь
        /// </summary>
        public const string Poteri = "-3";

        /// <summary>
        /// Код производства
        /// </summary>
        public const string Proizvodstvo = "-4";

        /// <summary>
        /// Код полуфабриката
        /// </summary>
        public const string HalfStuff = "-5";

        /// <summary>
        /// Без имени
        /// </summary>
        public const string NoNameValue = "_____________________";

        #endregion

        /// <summary>
        /// Инициализация сущности "Склад" на основе таблицы данных о складе
        /// </summary>
        /// <param name="dt">Таблица данных склада</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;

                Id = dt.Rows[0]["КодСклада"].ToString();
                Name = dt.Rows[0]["Склад"].ToString();
                FullName = dt.Rows[0]["Название"].ToString();
                IBAN = dt.Rows[0]["IBAN"].ToString();
                StoreTypeId = Convert.ToInt32(dt.Rows[0]["КодТипаСклада"]);
                ResidenceId = dt.Rows[0]["КодМестаХранения"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодМестаХранения"];
                ResourceId = Convert.ToInt32(dt.Rows[0]["КодРесурса"]);
                KeeperId = dt.Rows[0]["КодХранителя"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодХранителя"];
                ManagerId = dt.Rows[0]["КодРаспорядителя"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодРаспорядителя"];
                ManagerSubdivisionCode = dt.Rows[0]["КодПодразделенияРаспорядителя"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодПодразделенияРаспорядителя"];
                AgreementCode = dt.Rows[0]["КодДоговора"] == DBNull.Value ? null : (int?)dt.Rows[0]["КодДоговора"];
                Subsidiary = dt.Rows[0]["Филиал"].ToString();
                Note = dt.Rows[0]["Примечание"].ToString();
                Changed = Convert.ToDateTime(dt.Rows[0]["Изменено"]);
                ChangeBy = dt.Rows[0]["Изменил"].ToString();
                Alive = Convert.ToBoolean(dt.Rows[0]["Действует"]);
				From = dt.Rows[0]["От"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["От"];
				To = dt.Rows[0]["До"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["До"];
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Инициализация сущности "Склад" на основе таблицы данных о складе
        /// </summary>
        /// <param name="dt">Таблица данных склада</param>
        public static List<Store> GetStoresList(DataTable dt)
        {
            List<Store> storeList = new List<Store>();
            
            for (int i = 0; i< dt.Rows.Count; i++)
            {
                storeList.Add(
                    new Store()
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодСклада"].ToString(),
                        Name = dt.Rows[i]["Склад"].ToString(),
                        IBAN = dt.Rows[i]["IBAN"].ToString(),
                        StoreTypeId = Convert.ToInt32(dt.Rows[i]["КодТипаСклада"]),
                        ResidenceId = dt.Rows[i]["КодМестаХранения"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодМестаХранения"],
                        ResourceId = Convert.ToInt32(dt.Rows[i]["КодРесурса"]),
                        KeeperId = dt.Rows[i]["КодХранителя"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодХранителя"],
                        ManagerId = dt.Rows[i]["КодРаспорядителя"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодРаспорядителя"],
                        ManagerSubdivisionCode = dt.Rows[i]["КодПодразделенияРаспорядителя"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодПодразделенияРаспорядителя"],
                        AgreementCode = dt.Rows[i]["КодДоговора"] == DBNull.Value ? null : (int?)dt.Rows[i]["КодДоговора"],
                        Subsidiary = dt.Rows[i]["Филиал"].ToString(),
                        Note = dt.Rows[i]["Примечание"].ToString(),
                        Changed = Convert.ToDateTime(dt.Rows[i]["Изменено"]),
                        ChangeBy = dt.Rows[i]["Изменил"].ToString(),
                        Alive = Convert.ToBoolean(dt.Rows[0]["Действует"]),
						From = dt.Rows[0]["От"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["От"],
						To = dt.Rows[0]["До"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["До"]
                    }
                );
            }
            return storeList;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
        public bool IsValidAt(DateTime d)
		{
			if(From.HasValue && d.Date<From) return false;
			if(To.HasValue && d.Date>=To) return false;

			var sqlParams = new Dictionary<string, object>();
			sqlParams.Add("@Дата", d);
			sqlParams.Add("@КодХранителя", KeeperId.HasValue ? (object)KeeperId : DBNull.Value);
			sqlParams.Add("@КодРаспорядителя", ManagerId.HasValue ? (object)ManagerId : DBNull.Value);

			DataTable dt = DBManager.GetData(SQLQueries.SELECT_TEST_ЛицаСкладаДействуют, CN, CommandType.Text, sqlParams);

			if (dt.Rows.Count < 1) return true;

			return (int)dt.Rows[0][0] != 0;
		}

        /// <summary>
        /// Действует
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool IsAlive(DateTime d)
        {
            if (From.HasValue && d.Date < From) return false;
            if (To.HasValue && d.Date >= To) return false;
            if (!Alive) return false;
            return true;
        }

        /// <summary>
        /// Загрузка данных по складу
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@КодСклада", new object[] { Id, DBManager.ParameterTypes.Int32 });

			FillData(DBManager.GetData(SQLQueries.SELECT_ID_СкладПодробно, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Store(){ }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID склада</param>
        public Store(string id)
            : base(id) 
        {
            Load();
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get
            {
                if(string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_person;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        #region Сохранение склада

        /// <summary>
        /// Сохранение/редактирование склада
        /// </summary>
        /// <param name="isNew">Маркер создания/редактирования склада: TRUE - создать новый, FALSE - редактировать существующий</param>
        public int Save(bool isNew = false)
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();

            sqlParams.Add("@КодСклада", (isNew ? DBNull.Value : (object)Id));
            sqlParams.Add("@Склад", Name);
            sqlParams.Add("@IBAN", IBAN);
            sqlParams.Add("@КодТипаСклада", StoreTypeId);
            sqlParams.Add("@КодМестаХранения", (StoreTypeId >= 20) ? ((ResidenceId == 0) ? DBNull.Value : (object)ResidenceId) : DBNull.Value);
            sqlParams.Add("@КодРесурса", ResourceId == 0 ? DBNull.Value : (object)ResourceId);
            sqlParams.Add("@КодХранителя", KeeperId == 0 ? DBNull.Value : (object)KeeperId);
            sqlParams.Add("@КодРаспорядителя", ManagerId == 0 ? DBNull.Value : (object)ManagerId);
            sqlParams.Add("@КодПодразделенияРаспорядителя", ((ManagerSubdivisionCode == 0) ? DBNull.Value : (object)ManagerSubdivisionCode));
            sqlParams.Add("@КодДоговора", ((AgreementCode == 0) ? DBNull.Value : (object)AgreementCode));
            sqlParams.Add("@Филиал", (StoreTypeId >= 10) ? string.Empty : Subsidiary);
            sqlParams.Add("@Примечание", Note);

			sqlParams.Add("@От", From.HasValue ? (object)From : DBNull.Value);
			sqlParams.Add("@По", To.HasValue ? (object)(To>DateTime.MinValue ? To.Value.AddDays(-1):To) : DBNull.Value);

            var outputParams = new Dictionary<string, object>();
            DBManager.ExecuteNonQuery(SQLQueries.SP_Лица_InsUpd_Склады, CommandType.StoredProcedure, Config.DS_person, sqlParams, outputParams);

            int storeID = 0;

            if (outputParams.ContainsKey("@RETURN_VALUE"))
                storeID = (int)outputParams["@RETURN_VALUE"];

            return storeID;
        }

        #endregion

        /// <summary>
        /// Получает остаток ресурса на складе
        /// </summary>
        /// <param name="_Sklad">КодСклада</param>
        /// <param name="_Resource">КодРесурса</param>
        /// <param name="_Unit">КодЕдиницыИзмерения</param>
        /// <param name="_DateTo">ДатаДо</param>
        /// <param name="IncludeSubRes">ВключатьПодчиенныеРесурсы</param>
        /// <param name="_VidOstatkov">ВидОстатка</param>
        /// <returns>Остаток</returns>
        public static string GetOstatokResource(string _Sklad, string _Resource, string _Unit, DateTime _DateTo, bool IncludeSubRes, string _VidOstatkov)
        {
            if (string.IsNullOrEmpty(_Sklad)) return "";
            string rez = "";
            SqlCommand cm = new SqlCommand("sp_ОстатокНаСкладе");
            cm.Parameters.AddWithValue("@КодСклада", int.Parse(_Sklad));
            cm.Parameters.AddWithValue("@КодРесурса", int.Parse(_Resource));
            if (_Unit.Length > 0)
                cm.Parameters.AddWithValue("@КодЕдиницыИзмерения", int.Parse(_Unit));
            else
                cm.Parameters.AddWithValue("@КодЕдиницыИзмерения", DBNull.Value);

            if (_DateTo != DateTime.MinValue)
                cm.Parameters.AddWithValue("@ДатаДо", _DateTo);
            else
                cm.Parameters.AddWithValue("@ДатаДо", DBNull.Value);

            SqlParameter vid_ost = cm.Parameters.Add("@ВидОстатка", SqlDbType.Int);
            if (_VidOstatkov.Equals("0"))
                vid_ost.Value = 0;
            else if (_VidOstatkov.Equals("1"))
                vid_ost.Value = 1;
            else
                vid_ost.Value = DBNull.Value;

            cm.Parameters.AddWithValue("@ВключатьПодчиенныеРесурсы", IncludeSubRes);
            SqlParameter ost = cm.Parameters.Add("@Остаток", SqlDbType.Float);
            ost.Direction = ParameterDirection.Output;

            cm.CommandType = CommandType.StoredProcedure;
            cm.Connection = new SqlConnection(Config.DS_document);
            try
            {
                cm.Connection.Open();
                cm.ExecuteNonQuery();
                rez = Kesco.Lib.ConvertExtention.Convert.Object2Str(ost.Value);
            }
            catch (Exception ex)
            {
                throw new Log.DetailedException(ex.Message, ex, cm);
            }
            finally
            {
                cm.Connection.Close();
            }
            return rez;
        }

        /// <summary>
        /// Метод получения даты последнего изменения
        /// </summary>
        public override DateTime GetLastChanged(string id)
        {
            var param = new Dictionary<string, object> { { "@Id", id } };
            var res = DBManager.ExecuteScalar(SQLQueries.SELECT_Склад_LastChanged, CommandType.Text, CN, param);

            if (res is DateTime)
                return (DateTime)res;

            return DateTime.MinValue;
        }
    }
}
