using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    ///     Класс сущности "Розетки"
    /// </summary>
    [Serializable]
    [DBSource("vwРозетки")]
    public class Socket : Entity
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        protected string _connectionString;


        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">КодРозетки</param>
        public Socket(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        ///     Конструктор для загрузки 
        /// </summary>
        public Socket()
        {
            SocketId = 0;
            LocationId = 0;
            SocketName = "";
            IsActive = false;
            Notes = "";
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
        ///     Подготовка связывания поля объекта КодРозетки с контролом
        /// </summary>
        public BinderValue SocketIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта КодРасположения с контролом
        /// </summary>
        public BinderValue LocationIdBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Розетка с контролом
        /// </summary>
        public BinderValue SocketBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Работает с контролом
        /// </summary>
        public BinderValue IsActiveBind = new BinderValue();
        /// <summary>
        ///     Подготовка связывания поля объекта Примечание с контролом
        /// </summary>
        public BinderValue NotesBind = new BinderValue();

        /// <summary>
        ///     КодРозетки (int, not null)
        /// </summary>
        [DBField("КодРозетки", 0)]
        public int SocketId
        {
            get { return string.IsNullOrEmpty(SocketIdBind.Value) ? 0 : int.Parse(SocketIdBind.Value); }
            set { SocketIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     КодРасположения (int, not null)
        /// </summary>
        [DBField("КодРасположения", 0)]
        public int LocationId
        {
            get { return string.IsNullOrEmpty(LocationIdBind.Value) ? 0 : int.Parse(LocationIdBind.Value); }
            set { LocationIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Розетка
        /// </value>
        [DBField("Розетка")]
        public string SocketName
        {
            get { return string.IsNullOrEmpty(SocketBind.Value) ? "" : SocketBind.Value; }
            set { SocketBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Работает
        /// </value>
        [DBField("Работает")]
        public bool IsActive
        {
            get { return Convert.ToBoolean(IsActiveBind.Value); }
            set { IsActiveBind.Value = value.ToString(); }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Примечание
        /// </value>
        [DBField("Примечание")]
        public string Notes
        {
            get { return string.IsNullOrEmpty(NotesBind.Value) ? "" : NotesBind.Value; }
            set { NotesBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Изменил
        /// </summary>
        [DBField("Изменил", "", false)]
        public int ChangedId { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        [DBField("Изменено", "", false)]
        public DateTime ChangedTime { get; set; }

        /// <summary>
        ///     Метод загрузки данных сущности "Тип личного кабинета"
        /// </summary>
        public override void Load()
        {
            var sqlParams = new Dictionary<string, object> {{"@id", Id}};

            using (var dbReader = new DBReader(SQLQueries.SELECT_РозеткаПоID, CommandType.Text, CN, sqlParams))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    var colКодРозетки = dbReader.GetOrdinal("КодРозетки");
                    var colКодРасположения = dbReader.GetOrdinal("КодРасположения");
                    var colРозетка = dbReader.GetOrdinal("Розетка");
                    var colРаботает = dbReader.GetOrdinal("Работает");
                    var colПримечание = dbReader.GetOrdinal("Примечание");
                    var colИзменил = dbReader.GetOrdinal("Изменил");
                    var colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        SocketId = dbReader.GetInt32(colКодРозетки);
                        Id = SocketId.ToString();
                        LocationId = dbReader.GetInt32(colКодРасположения);
                        SocketName = Name = dbReader.GetString(colРозетка);
                        IsActive = dbReader.GetBoolean(colРаботает);
                        Notes = dbReader.GetString(colПримечание);
                        ChangedId = dbReader.GetInt32(colИзменил);
                        ChangedTime = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        protected override void FillData(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                Unavailable = false;
                SocketId = Convert.ToInt32(dt.Rows[0]["КодЛичногоКабинета"]);
                Id = SocketId.ToString();
                LocationId = Convert.ToInt32(dt.Rows[0]["КодРасположения"]);
                SocketName = dt.Rows[0]["Розетка"].ToString();
                IsActive = Convert.ToBoolean(dt.Rows[0]["Работает"]);
                Notes = dt.Rows[0]["Примечание"].ToString();
                ChangedId = Convert.ToInt32(dt.Rows[0]["Изменил"]);
                ChangedTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
            }
            else
            {
                Unavailable = true;
            }
        }

    }
}