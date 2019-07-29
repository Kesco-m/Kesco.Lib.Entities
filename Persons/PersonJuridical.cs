using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Attribute = Kesco.Lib.Entities.Persons.Attributes.Attribute;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности Юридическое лицо
    /// </summary>
    [Serializable]
    public class PersonJuridical : PersonBase
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="id">ID лица</param>
        public PersonJuridical(string id) : base(id)
        {
            CompanyName = new Attribute();
            CompanyNameInParentNominative = new Attribute();
            GovermentCompany = false;
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonJuridical()
        {
            GovermentCompany = false;
            CompanyName = new Attribute();
            CompanyNameInParentNominative = new Attribute();
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override void Load()
        {
        }

        /// <summary>
        ///     Метод загрузки данных сущности "Лицо"
        /// </summary>
        public override object Create()
        {
            if (ResponsibleEmployes.Count != 0 && !string.IsNullOrEmpty(CompanyName.AttributeValue1) && RegionID != 0 &&
                PersonTypes.Count != 0)
            {
                //СОздание лица
                var sqlParams = new Dictionary<string, object>
                {
                    {"@ВыполнятьПроверкуНаПохожиеЛица", Cheching ? 0 : 1},
                    {"@ОтображаемоеИмя", Name},
                    {"@КодБизнесПроекта", BusinessProjectID},
                    {"@КодТерритории", RegionID},
                    {"@ГосОрганизация", Convert.ToInt32(GovermentCompany)},
                    {"@Примечание", Note},
                    {"@КраткоеНазваниеРус", CompanyName.AttributeValue1},
                    {"@ПолноеНазвание", CompanyName.AttributeValue2},
                    {"@КраткоеНазваниеЛат", CompanyName.AttributeValue3},
                    {
                        "@КраткоеНазваниеРусРП",
                        string.IsNullOrEmpty(CompanyNameInParentNominative.AttributeValue1)
                            ? TranslateRusToEng(CompanyNameInParentNominative.AttributeValue1)
                            : CompanyNameInParentNominative.AttributeValue1
                    }
                };
                if (BirthDate != DateTime.MinValue)
                    sqlParams.Add("@ДатаРождения", BirthDate.ToString("dd.MM.yyyy"));

                var result = new DataTable();

                #region СОздание колонок таблицы возврата результата

                var column = new DataColumn {DataType = System.Type.GetType("System.Int32"), ColumnName = "R"};
                result.Columns.Add(column);

                column = new DataColumn {DataType = System.Type.GetType("System.Int32"), ColumnName = "КодЛица"};
                result.Columns.Add(column);

                column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "Кличка"};
                result.Columns.Add(column);

                column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "Поле"};
                result.Columns.Add(column);

                column = new DataColumn {DataType = System.Type.GetType("System.String"), ColumnName = "Значение"};
                result.Columns.Add(column);

                column = new DataColumn {DataType = System.Type.GetType("System.Byte"), ColumnName = "Доступ"};
                result.Columns.Add(column);

                #endregion

                var parametersOut = new Dictionary<string, object>();
                using (var dbReader = new DBReader(SQLQueries.SP_Лица_ЮридическоеЛицо_Ins, CommandType.StoredProcedure,
                    CN, sqlParams, parametersOut))
                {
                    if (dbReader.HasRows)
                    {
                        #region Получение порядкового номера столбца

                        var colR = dbReader.GetOrdinal("R");
                        var colКодЛица = dbReader.GetOrdinal("КодЛица");
                        var colКличка = dbReader.GetOrdinal("Кличка");
                        var colПоле = dbReader.GetOrdinal("Поле");
                        var colЗначение = dbReader.GetOrdinal("Значение");
                        var colДоступ = dbReader.GetOrdinal("Доступ");

                        #endregion

                        while (dbReader.Read())
                        {
                            var row = result.NewRow();

                            if (!dbReader.IsDBNull(colR)) row["R"] = dbReader.GetInt32(colR);
                            if (!dbReader.IsDBNull(colКодЛица)) row["КодЛица"] = dbReader.GetInt32(colКодЛица);
                            if (!dbReader.IsDBNull(colКличка)) row["Кличка"] = dbReader.GetString(colКличка);
                            if (!dbReader.IsDBNull(colПоле)) row["Поле"] = dbReader.GetString(colПоле);
                            if (!dbReader.IsDBNull(colЗначение)) row["Значение"] = dbReader.GetString(colЗначение);
                            if (!dbReader.IsDBNull(colДоступ)) row["Доступ"] = dbReader.GetByte(colДоступ);
                            result.Rows.Add(row);
                        }
                    }

                    dbReader.Close();
                    if (parametersOut["@RETURN_VALUE"] != null && Convert.ToInt32(parametersOut["@RETURN_VALUE"]) != 0)
                        return Convert.ToInt32(parametersOut["@RETURN_VALUE"]);
                }

                return result;
            }

            return null;
        }

        #region Поля Юридическое лицо

        /// <summary>
        ///     Является гос. компанией
        /// </summary>
        public bool GovermentCompany { get; set; }

        /// <summary>
        ///     Наименование компании
        /// </summary>
        public Attribute CompanyName { get; set; }

        /// <summary>
        ///     Наименование компании в родительском падеже
        /// </summary>
        public Attribute CompanyNameInParentNominative { get; set; }

        #endregion
    }
}