using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities
{
    

    /// <summary>
    /// Бизнес-объект - Расположения
    /// </summary>
    [Serializable]
    public class Location : Entity
    {
        #region Поля сущности "Расположения"

        /// <summary>
        /// Признак РабочееМесто
        /// </summary>
        public int WorkPlace { get; set; }

        /// <summary>
        /// Признак Офис
        /// </summary>
        public int Office { get; set; }

        /// <summary>
        /// Признак Закрыто
        /// </summary>
        public int LocationClose { get; set; }


        /// <summary>
        /// Расположение
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// РасположениеPath0
        /// </summary>
        public string NamePath0 { get; set; }

        /// <summary>
        /// РасположениеPath1
        /// </summary>
        public string NamePath1 { get; set; }

        /// <summary>
        /// Родительский ID
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Левый ключ
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Правый ключ
        /// </summary>
        public int R { get; set; }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID расположения</param>
        public Location(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Location()
        {
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public override sealed string CN
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    return _connectionString = Config.DS_user;

                return _connectionString;
            }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;

        /// <summary>
        /// Инициализация сущности "Расположения" на основе таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                Id = dt.Rows[0]["КодРасположения"].ToString();
                ShortName = dt.Rows[0]["Расположение"].ToString();
                Name = dt.Rows[0]["РасположениеPath1"].ToString();
                WorkPlace = Convert.ToInt16(dt.Rows[0]["РабочееМесто"]);
                Office = Convert.ToInt16(dt.Rows[0]["Офис"]);
                LocationClose = Convert.ToInt16(dt.Rows[0]["Закрыто"]);
                NamePath0 = dt.Rows[0]["РасположениеPath0"].ToString();
                NamePath1 = dt.Rows[0]["РасположениеPath1"].ToString();
                Parent = dt.Rows[0]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString();
                L = Convert.ToInt32(dt.Rows[0]["L"]);
                R = Convert.ToInt32(dt.Rows[0]["R"]);
            }
            else
            {
                Unavailable = true;
            }
        }

        /// <summary>
        /// Получение списка расположений из таблицы данных
        /// </summary>
        /// <param name="dt">Таблица данных места хранения</param>
        public static List<Location> GetLocationsList(DataTable dt)
        {
            List<Location> locatinList = new List<Location>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                locatinList.Add(
                    new Location()
                    {
                        Unavailable = false,
                        Id = dt.Rows[i]["КодРасположения"].ToString(),
                        Name = dt.Rows[i]["Расположение"].ToString(),
                        ShortName = dt.Rows[0]["Расположение"].ToString(),
                        WorkPlace = Convert.ToInt16(dt.Rows[i]["РабочееМесто"]),
                        Office = Convert.ToInt16(dt.Rows[i]["Офис"]),
                        LocationClose = Convert.ToInt16(dt.Rows[i]["Закрыто"]),
                        NamePath0 = dt.Rows[i]["РасположениеPath0"].ToString(),
                        NamePath1 = dt.Rows[i]["РасположениеPath1"].ToString(),
                        Parent = dt.Rows[i]["Parent"] == DBNull.Value ? string.Empty : dt.Rows[0]["Parent"].ToString(),
                        L = Convert.ToInt16(dt.Rows[i]["L"]),
                        R = Convert.ToInt16(dt.Rows[i]["R"])
                    }
                    );
            }
            return locatinList;
        }

        /// <summary>
        /// Получение списка подчиненных раположений
        /// </summary>
        public List<Location> GetChildLocationsList()
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@leftKey", new object[] {L, DBManager.ParameterTypes.Int32});
            sqlParams.Add("@rightKey", new object[] {R, DBManager.ParameterTypes.Int32});
            DataTable dt = DBManager.GetData(string.Format(SQLQueries.SELECT_РасположенияПодчиненные), CN,
                CommandType.Text, sqlParams);

            return GetLocationsList(dt);
        }

        /// <summary>
        /// Метод загрузки данных сущности "Расположение"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", new object[] {Id, DBManager.ParameterTypes.Int32}}};
            FillData(DBManager.GetData(SQLQueries.SELECT_РасположениеПоID, CN, CommandType.Text, sqlParams));
        }

        /// <summary>
        /// Компьютеризированное рабочее место
        /// </summary>
        public bool IsComputeredWorkPlace { get { return ((WorkPlace.Equals((int)ТипыРабочихМест.КомпьютеризированноеРабочееМесто))); } }

        /// <summary>
        /// Гостевое рабочее место
        /// </summary>
        public bool IsGuestWorkPlace { get { return ((WorkPlace.Equals((int)ТипыРабочихМест.ГостевоеРабочееМесто))); } }

        private bool? isOffice = null;

        /// <summary>
        /// Расположение находится в офисе
        /// </summary>
        public bool IsOffice
        {
            get
            {
                if (isOffice.HasValue && !RequiredRefreshInfo)
                    return isOffice.Value;

                isOffice = false;
                var sqlParams = new Dictionary<string, object> {{"@КодРасположения", Id.ToInt()}};
                using (var dbReader = new DBReader(SQLQueries.SELECT_РасположениеВОфисе, CommandType.Text, CN, sqlParams))
                {
                    if (dbReader.HasRows) isOffice = true;
                }
                return isOffice.Value;

            }
        }



    }
}
