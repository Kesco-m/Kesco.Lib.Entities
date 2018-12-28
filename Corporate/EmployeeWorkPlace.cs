using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс сущности рабочее место сотрудника
    /// </summary>
    [Serializable]
    public class EmployeeWorkPlace : Entity
    {
        /// <summary>
        /// РасположениеPath
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// РабочееМесто
        /// </summary>
        public int WorkPlacePar { get; set; }
        /// <summary>
        /// Замещающие сотрудники
        /// </summary>
        public List<EmployeeCoWorker> CoWorkers { get; set; }

        public void GetWorkPlaceSpecifications(out string icon, out string title)
        {
            switch (WorkPlacePar)
            {
                case (int) ТипыРабочихМест.КомпьютеризированноеРабочееМесто:
                    icon =ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title =ТипыРабочихМест.КомпьютеризированноеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int) ТипыРабочихМест.НомерГостиницы:
                    icon = ТипыРабочихМест.НомерГостиницы.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.НомерГостиницы.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int) ТипыРабочихМест.Оборудование:
                    icon = ТипыРабочихМест.Оборудование.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.Оборудование.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                case (int) ТипыРабочихМест.CкладОборудования:
                    icon = ТипыРабочихМест.CкладОборудования.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.CкладОборудования.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
                default:
                    icon = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Icon;
                    title = ТипыРабочихМест.ГостевоеРабочееМесто.GetAttribute<ТипыРабочихМестSpecifications>().Name;
                    break;
            }
        }
       
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
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_user) : _connectionString; }
        }
        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;
        /// <summary>
        /// Заполнение из dbReader
        /// </summary>
        public void LoadFromDbReader(DBReader dbReader)
        {
            int colКодРасположения = dbReader.GetOrdinal("КодРасположения");
            int colPath = dbReader.GetOrdinal("РасположениеPath");
            int colWorkPlacePar = dbReader.GetOrdinal("РабочееМесто");
            if (!dbReader.IsDBNull(colКодРасположения)) Id = dbReader.GetInt32(colКодРасположения).ToString();
            if (!dbReader.IsDBNull(colPath)) Path = dbReader.GetString(colPath);
            if (!dbReader.IsDBNull(colWorkPlacePar)) WorkPlacePar = dbReader.GetByte(colWorkPlacePar);
            Unavailable = false;
        }
    }
}
