using System;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности Подразделение
    /// </summary>
    [Serializable]
    public class Subdivision : Entity
    {
        /// <summary>
        ///  Типизированный псевданим для Id
        /// </summary>
        public int SubDivisionId {get { return Id.ToInt(); }}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public Subdivision(string id)
            : base(id)
        {
            Load();
        }

        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Subdivision() { }

        /// <summary>
        /// Метод загрузки данных сущности "Подразделение"
        /// </summary>
        public sealed override void Load()
        {
            FillData(SubDivisionId);
        }

        /// <summary>
        /// Инициализация сущности Подразделение на основе таблицы данных
        /// </summary>
        /// <remarks>Данные, которые пока не нужны, закомментированы</remarks>
        public void FillData(int id)
        {
            if(id == 0) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_Должность, id, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодДолжности = dbReader.GetOrdinal("КодДолжности");
                    //int colДолжность = dbReader.GetOrdinal("Должность");
                    int colПодразделение = dbReader.GetOrdinal("Подразделение");
                    //int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    //int colКодСотрудника = dbReader.GetOrdinal("КодСотрудника");
                    //int colСовместитель = dbReader.GetOrdinal("Совместитель");
                    //int colОклад = dbReader.GetOrdinal("Оклад");
                    //int colТарифнаяСтавка = dbReader.GetOrdinal("ТарифнаяСтавка");
                    //int colParent = dbReader.GetOrdinal("Parent");
                    //int colL = dbReader.GetOrdinal("L");
                    //int colR = dbReader.GetOrdinal("R");
                    //int colИзменил = dbReader.GetOrdinal("Изменил");
                    //int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        Id = dbReader.GetInt32(colКодДолжности).ToString();
                        //Должность = dbReader.GetString(colДолжность);
                        Name = dbReader.GetString(colПодразделение);
                        //КодЛица = dbReader.GetInt32(colКодЛица);
                        //if (!dbReader.IsDBNull(colКодСотрудника)) { КодСотрудника = dbReader.GetInt32(colКодСотрудника); }
                        //Совместитель = dbReader.GetByte(colСовместитель);
                        //if (!dbReader.IsDBNull(colОклад)) { Оклад = dbReader.GetDecimal(colОклад); }
                        //if (!dbReader.IsDBNull(colТарифнаяСтавка)) { ТарифнаяСтавка = dbReader.GetDecimal(colТарифнаяСтавка); }
                        //if (!dbReader.IsDBNull(colParent)) { Parent = dbReader.GetInt32(colParent); }
                        //L = dbReader.GetInt32(colL);
                        //R = dbReader.GetInt32(colR);
                        //Изменил = dbReader.GetInt32(colИзменил);
                        //Изменено = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        } 
    }
}
