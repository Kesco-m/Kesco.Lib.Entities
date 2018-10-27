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
    /// Класс сущности пункт меню досье
    /// </summary>
    [Serializable]
    public class DossierMenuItem : Entity
    {
        #region Свойтва
        /// <summary>
        /// Имя пункта меню
        /// </summary>
        public int ItemID{ get; set; }
        /// <summary>
        /// Имя пункта меню
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// Ключ пункта меню
        /// </summary>
        public string ItemPointKey { get; set; }
        /// <summary>
        /// Картинка пункта меню
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Диалоговая форма
        /// </summary>
        public string DialogForm { get; set; }
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Параметры редактирования
        /// </summary>
        public string ParamEdit { get; set; }
        /// <summary>
        /// Тип связи лиц
        /// </summary>
        public int? LinkTypeID { get; set; }
        /// <summary>
        /// Тип ссылки
        /// </summary>
        public byte? UrlType { get; set; }
        /// <summary>
        /// Разделитель
        /// </summary>
        public byte? Separator { get; set; }
        /// <summary>
        /// Тип лица родетеля
        /// </summary>
        public byte? ParentType { get; set; }
        /// <summary>
        /// Тип лица потомка
        /// </summary>
        public byte? ChildType { get; set; }
        /// <summary>
        /// Ссылка на форму
        /// </summary>
        public string FormURL { get; set; }
        /// <summary>
        /// Параметры ссылки
        /// </summary>
        public string ParametrsURL { get; set; }
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
        public override void Load()
        {
            //FillData(DBManager.GetData(SQLQueries.SELECT_КонтактПоID, CN, CommandType.Text, null));
        }

        /// <summary>
        /// Метод возвращает коллекцию пунктов меню досье
        /// </summary>
        public List<DossierMenuItem> GetMenuItems(string personID, string personType)
        {
            var itemList = new List<DossierMenuItem>();
            var parameters = new Dictionary<string, object> { { "@КодЛица", personID }, { "@ТипЛица", personType } };
            var dtDosserMenuItems = DBManager.GetData(SQLQueries.SP_Лица_Досье_Меню, CN, CommandType.StoredProcedure, parameters);
            if (dtDosserMenuItems.Rows.Count != 0)
            {
                foreach (DataRow row in dtDosserMenuItems.Rows)
                {
                    var item = new DossierMenuItem
                                   {
                                       ItemID = (int)row["КодПунктаМенюЛиц"],
                                       Id = row["КодПунктаМенюЛиц"].ToString(),
                                       ItemName = row["Название"].ToString(),
                                       ItemPointKey = row["ПунктМенюKey"].ToString(),
                                       Image = row["Рисунок"].ToString(),
                                       DialogForm = row["dialogForm"].ToString(),
                                       Title = row["title"].ToString(),
                                       ParamEdit = row["paramEdit"].ToString(),
                                       LinkTypeID =  row["КодТипаСвязейЛиц"] != DBNull.Value ? (int?)row["КодТипаСвязейЛиц"] : null,
                                       UrlType = row["ТипURL"] != DBNull.Value ? (byte?)row["ТипURL"] : null,
                                       Separator = row["Разделитель"] != DBNull.Value ?(byte?)row["Разделитель"] : null,
                                       ParentType = row["ТипЛицаРодителя"] != DBNull.Value ?(byte?)row["ТипЛицаРодителя"] : null,
                                       ChildType = row["ТипЛицаПотомка"] != DBNull.Value ?(byte?)row["ТипЛицаПотомка"] : null,
                                       FormURL = row["URLФормы"].ToString(),
                                       ParametrsURL = row["URLПараметры"].ToString()

                                   };
                    itemList.Add(item);
                }
            }
            return itemList;
        }



        #endregion
    }
}
