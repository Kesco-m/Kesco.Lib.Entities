using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF
{
    /// <summary>
    ///     Абстрактный класс позиции документа
    /// </summary>
    [Serializable]
    public abstract class DocumentPosition<T> : Entity, ICloneable<T> where T : new()
    {
        /// <summary>
        ///     Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///     Защищенный конструктор для генерации нового GUID позиции
        /// </summary>
        protected DocumentPosition()
        {
            GuidId = Guid.NewGuid();
        }

        /// <summary>
        ///     Строка подключения
        /// </summary>
        public static string ConnString =>
            string.IsNullOrEmpty(_connectionString)
                ? _connectionString = Config.DS_document
                : _connectionString;

        /// <summary>
        ///     В большинстве случаев,первичный ключ позиции документа - int
        /// </summary>
        public virtual int? PositionId
        {
            get { return string.IsNullOrEmpty(Id) ? (int?) null : int.Parse(Id); }
            set { Id = value == null ? string.Empty : value.ToString(); }
        }

        /// <summary>
        ///     Код документа
        /// </summary>
        public abstract int DocumentId { get; set; }

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
        ///     Уникальный идентификатор позиции документа
        /// </summary>
        public Guid GuidId { get; set; }

        /// <summary>
        ///     Оригинальный, загруженный из БД объект
        /// </summary>
        public T DbOriginal { get; set; }

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public T Clone()
        {
            //return (T) MemberwiseClone();

            var newObj = Activator.CreateInstance(GetType());
            foreach (var pi in GetType().GetProperties()
                .Where(pi => pi.CanRead && pi.CanWrite && pi.PropertyType.IsSerializable))
                pi.SetValue(newObj, pi.GetValue(this, null), null);
            return (T) newObj;
        }

        /// <summary>
        ///     Заполнение по идентификатору позиции
        /// </summary>
        public sealed override void Load()
        {
            var dbProps = GetDbProperties();
            var sqlSelectId = GetSqlSelect(dbProps);
            sqlSelectId =
                $"{Environment.NewLine}/*автоматически сформированный запрос для загрузки информации о позиции документа по идентификатору*/{Environment.NewLine}{sqlSelectId}";
            using (var dbReader = new DBReader(sqlSelectId, int.Parse(Id), CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    if (!dbReader.Read()) return;

                    Unavailable = false;
                    foreach (var p in dbProps)
                    {
                        var pInfo = p.Key;
                        var fInfo = p.Value;

                        //для первичного ключа Entity
                        var type = p.Value.IsPK && pInfo.PropertyType == typeof(string)
                            ? typeof(int)
                            : pInfo.PropertyType;

                        var value = DBManager.GetReaderValue(dbReader, type, fInfo.Ordinal);

                        if (p.Value.IsPK && pInfo.PropertyType == typeof(string) && type == typeof(int))
                            value = value.ToString();
                        //=============================================

                        pInfo.SetValue(this, value, null);
                    }

                    DbOriginal = Clone();
                }
            }
        }

        /// <summary>
        ///     Загрузка позиций по Id документа
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static List<T> LoadByDocId(int docId)
        {
            var list = new List<T>();
            var dbProps = GetDbProperties();
            var sql = GetSqlSelectByDocId(dbProps);
            sql =
                $"{Environment.NewLine}/*автоматически сформированный запрос для загрузки информации о позициях документа по коду документа*/{Environment.NewLine}{sql}";
            using (var dbReader = new DBReader(sql, docId, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                    while (dbReader.Read())
                    {
                        var position = new T();

                        var prop = position.GetType()
                            .GetProperty("Unavailable", BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                            prop.SetValue(position, false, null);

                        foreach (var p in dbProps)
                        {
                            var pInfo = p.Key;
                            var fInfo = p.Value;

                            //для первичного ключа Entity
                            var type = p.Value.IsPK && pInfo.PropertyType == typeof(string)
                                ? typeof(int)
                                : pInfo.PropertyType;

                            var value = DBManager.GetReaderValue(dbReader, type, fInfo.Ordinal);

                            if (p.Value.IsPK && pInfo.PropertyType == typeof(string) && type == typeof(int))
                                value = value.ToString();
                            //=============================================

                            pInfo.SetValue(position, value, null);
                        }

                        var method = position.GetType().GetMethod("Clone");

                        prop = position.GetType()
                            .GetProperty("DbOriginal", BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                            prop.SetValue(position, method.Invoke(position, null), null);

                        list.Add(position);
                    }
            }

            return list;
        }

        /// <summary>
        ///     Метод сохранения позиции документа
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            var dbProps = GetDbProperties();
            var param = new Dictionary<string, object>();
            var sqlSave = "";

            if (string.IsNullOrEmpty(Id))
            {
                var paramOut = new Dictionary<string, object>();
                var pkParamName = "";
                sqlSave = GetSqlInsert(dbProps, param, paramOut, out pkParamName);
                sqlSave =
                    $"{Environment.NewLine}/*автоматически сформированный запрос для сохранения информации о новой позиции документа*/{Environment.NewLine}{sqlSave}";
                if (cmds != null)
                {
                    cmds.Add(new DBCommand
                    {
                        Appointment = "Создание позиции документа",
                        Text = sqlSave,
                        Type = CommandType.Text,
                        ConnectionString = ConnString,
                        ParamsIn = param,
                        ParamsOut = paramOut
                    });
                    return;
                }

                DBManager.ExecuteScalar(sqlSave, CommandType.Text, ConnString, param, paramOut);
                Id = paramOut[pkParamName].ToString();
            }
            else
            {
                sqlSave = GetSqlUpdate(dbProps, param);
                if (sqlSave != string.Empty)
                {
                    sqlSave =
                        $"{Environment.NewLine}/*автоматически сформированный запрос для сохранения информации о позиции документа*/{Environment.NewLine}{sqlSave}";

                    if (cmds != null)
                    {
                        cmds.Add(new DBCommand
                        {
                            Appointment = "Изменение позиции документа",
                            Text = sqlSave,
                            Type = CommandType.Text,
                            ConnectionString = ConnString,
                            ParamsIn = param,
                            ParamsOut = null
                        });
                        return;
                    }

                    DBManager.ExecuteNonQuery(sqlSave, CommandType.Text, ConnString, param);
                }
            }

            if (evalLoad) Load();
        }

        /// <summary>
        ///     Метод удаления позиции документа
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        public override void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            var dbProps = GetDbProperties();
            var param = new Dictionary<string, object>();
            var sql = GetSqlDelete(dbProps, param);
            if (!string.IsNullOrEmpty(sql))
            {
                sql =
                    $"{Environment.NewLine}/*автоматически сформированный запрос для удаления позиции документа*/{Environment.NewLine}{sql}";

                if (cmds != null)
                {
                    cmds.Add(new DBCommand
                    {
                        Appointment = "Удаление позиции документа",
                        Text = sql,
                        Type = CommandType.Text,
                        ConnectionString = ConnString,
                        ParamsIn = param,
                        ParamsOut = null
                    });
                    return;
                }

                DBManager.ExecuteNonQuery(sql, CommandType.Text, ConnString, param);
            }

            if (evalLoad) Load();
        }

        /// <summary>
        ///     Получение DB свойств из текущего класса
        /// </summary>
        /// <returns>Возвращает словать со всеми свойствами текущего класса, которые помечены как DBField</returns>
        private static Dictionary<PropertyInfo, DBFieldAttribute> GetDbProperties()
        {
            var dict = new Dictionary<PropertyInfo, DBFieldAttribute>();
            var props = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DBFieldAttribute)));

            var inx = 0;
            var propertyInfos = props as PropertyInfo[] ?? props.ToArray();
            foreach (var p in propertyInfos)
            {
                var attributes = p.GetCustomAttributes(typeof(DBFieldAttribute), true);
                if (attributes.Length != 1) continue;

                var field = (DBFieldAttribute) attributes[0];
                field.Ordinal = inx;
                dict.Add(p, field);
                inx++;
            }

            return dict;
        }

        /// <summary>
        ///     Получение строки запроса на загрузку сущности по ID
        /// </summary>
        /// <param name="props">Словарь свойств БД</param>
        /// <returns>SQL строка</returns>
        private string GetSqlSelect(Dictionary<PropertyInfo, DBFieldAttribute> props)
        {
            var w = new StringWriter();
            var pkFieldName = "";
            var inx = 1;

            if (props.Count == 0) return "";

            w.Write(" SELECT");

            foreach (var p in props)
            {
                var fInfo = p.Value;

                if (fInfo.IsPK) pkFieldName = fInfo.FieldName;

                w.Write(" {0}", fInfo.FieldName);

                if (inx < props.Count) w.Write(", ");
                inx++;
            }

            if (pkFieldName.Length == 0) return "";

            var isSubquery = false;
            var recordSource = GetRecordSourceSelect(false, out isSubquery);
            w.Write(" FROM {0} ", recordSource);

            if (!isSubquery)
                w.Write(" (nolock) WHERE {0} = @id", pkFieldName);

            return w.ToString();
        }

        /// <summary>
        ///     Получение строки запроса на загрузку списка позиций по коду родителя
        /// </summary>
        /// <param name="props">Словарь свойств БД</param>
        /// <returns>SQL строка</returns>
        private static string GetSqlSelectByDocId(Dictionary<PropertyInfo, DBFieldAttribute> props)
        {
            var w = new StringWriter();
            var docFieldName = "";
            var inx = 1;

            if (props.Count == 0) return "";

            w.Write(Environment.NewLine + "SELECT");

            foreach (var p in props)
            {
                var fInfo = p.Value;

                if (fInfo.IsLinkParent) docFieldName = fInfo.FieldName;

                w.Write(" {0}", fInfo.FieldName);

                if (inx < props.Count) w.Write(", ");
                inx++;
            }

            if (docFieldName.Length == 0) return "";

            var isSubquery = false;

            var recordSource = GetRecordSourceSelect(true, out isSubquery);
            w.Write(" FROM {0} ", recordSource);

            if (!isSubquery)
                w.Write(" (nolock) WHERE {0} = @id", docFieldName);

            return w.ToString();
        }

        /// <summary>
        ///     Получение строки запроса на обновление сущности по ID
        /// </summary>
        /// <param name="props">Словарь свойств БД</param>
        /// <param name="param">Словарь параметров sql</param>
        /// <returns>SQL строка</returns>
        private string GetSqlUpdate(Dictionary<PropertyInfo, DBFieldAttribute> props, Dictionary<string, object> param)
        {
            var w = new StringWriter();
            var pkFieldName = "";
            var pkParamName = "";
            foreach (var p in props)
            {
                var pInfo = p.Key;
                var fInfo = p.Value;
                var pValue = pInfo.GetValue(this, null);
                var pOriginalValue = pInfo.GetValue(DbOriginal, null);

                var paramName = string.Format("@{0}", fInfo.ParamName.Length == 0 ? fInfo.FieldName : fInfo.ParamName);

                if (fInfo.IsPK)
                {
                    pkFieldName = fInfo.FieldName;
                    pkParamName = paramName;
                    param.Add(pkParamName, int.Parse(pValue.ToString()));
                }

                if (!fInfo.IsUpdateble
                    || pValue == null && pOriginalValue == null
                    || pValue != null && pValue.Equals(pOriginalValue)
                    || pOriginalValue != null && pOriginalValue.Equals(pValue))
                    continue;

                if (w.ToString().Length > 0) w.Write(", ");
                param.Add(paramName, pValue);
                w.Write(" {0} = {1}", fInfo.FieldName, paramName);
            }

            if (w.ToString().Length == 0) return "";
            return string.Format("{4}UPDATE {0} SET {1} WHERE {2} = {3}", GetRecordSouceEdit(), w, pkFieldName,
                pkParamName, Environment.NewLine);
        }

        /// <summary>
        ///     Получение строки запроса на добавление сущности
        /// </summary>
        /// <param name="props">Словарь свойств БД</param>
        /// <param name="param">Словарь параметров sql</param>
        /// <param name="paramOut">Словарь выходных параметров sql</param>
        /// <param name="pkParamName">Названиен параметра, в который присваивается SCOPE_IDENTITY</param>
        /// <returns>SQL строка</returns>
        private string GetSqlInsert(Dictionary<PropertyInfo, DBFieldAttribute> props, Dictionary<string, object> param,
            Dictionary<string, object> paramOut, out string pkParamName)
        {
            var fieldString = new StringWriter();
            var paramString = new StringWriter();
            pkParamName = "";

            foreach (var p in props)
            {
                var pInfo = p.Key;
                var fInfo = p.Value;
                var pValue = pInfo.GetValue(this, null) ?? DBNull.Value;
                var paramName = string.Format("@{0}", fInfo.ParamName.Length == 0 ? fInfo.FieldName : fInfo.ParamName);

                if (fInfo.IsPK)
                {
                    pkParamName = paramName;
                    paramOut.Add(pkParamName, fInfo.DefaultValue ?? pValue);
                }

                if (!fInfo.IsUpdateble) continue;
                param.Add(paramName, pValue);

                if (fieldString.ToString().Length > 0)
                {
                    fieldString.Write(", ");
                    paramString.Write(", ");
                }

                fieldString.Write(fInfo.FieldName);
                paramString.Write(paramName);
            }

            if (fieldString.ToString().Length == 0) return "";
            return string.Format("{4}INSERT INTO {0}({1}) VALUES ({2}) SET {3} = SCOPE_IDENTITY()",
                GetRecordSouceEdit(),
                fieldString,
                paramString,
                pkParamName,
                Environment.NewLine);
        }

        /// <summary>
        ///     Получение строки запроса на удаление сущности по ID
        /// </summary>
        /// <param name="props">Словарь свойств БД</param>
        /// <param name="param">Словарь параметров sql</param>
        /// <returns>SQL строка</returns>
        private string GetSqlDelete(Dictionary<PropertyInfo, DBFieldAttribute> props, Dictionary<string, object> param)
        {
            var p = props.FirstOrDefault(t => t.Value.IsPK);
            if (p.Key == null) return "";

            var paramName = string.Format("@{0}",
                p.Value.ParamName.Length == 0 ? p.Value.FieldName : p.Value.ParamName);
            var pValue = p.Key.GetValue(this, null);
            if (pValue == null) return "";
            param.Add(paramName, pValue);

            return string.Format("{3}DELETE FROM {0} WHERE {1} = {2}", GetRecordSouceEdit(), p.Value.FieldName,
                paramName, Environment.NewLine);
        }

        /// <summary>
        ///     Получение названия источника данных текущего класса
        /// </summary>
        /// <returns>Название таблицы/представления</returns>
        public static string GetRecordSourceSelect(bool forList, out bool isSubquery)
        {
            var dnAttribute = typeof(T).GetCustomAttributes(
                typeof(DBSourceAttribute), true
            ).FirstOrDefault() as DBSourceAttribute;
            isSubquery = false;
            if (dnAttribute != null)
            {
                var ret = "";
                if (forList)
                {
                    if (string.IsNullOrEmpty(dnAttribute.RecordsSource))
                    {
                        ret = dnAttribute.TableName;
                    }
                    else
                    {
                        ret = dnAttribute.RecordsSource;
                        isSubquery = true;
                    }
                }
                else if (string.IsNullOrEmpty(dnAttribute.RecordSource))
                {
                    ret = dnAttribute.TableName;
                }
                else
                {
                    ret = dnAttribute.RecordSource;
                    isSubquery = true;
                }

                return ret;
            }

            return null;
        }

        /// <summary>
        ///     Получение названия источника данных текущего класса
        /// </summary>
        /// <returns>Название таблицы/представления</returns>
        public static string GetRecordSouceEdit()
        {
            var dnAttribute = typeof(T).GetCustomAttributes(
                typeof(DBSourceAttribute), true
            ).FirstOrDefault() as DBSourceAttribute;

            if (dnAttribute != null)
                return dnAttribute.TableName;

            return null;
        }
    }
}