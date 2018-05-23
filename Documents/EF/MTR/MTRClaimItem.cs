using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

// ReSharper disable RedundantArgumentNameForLiteralExpression

namespace Kesco.Lib.Entities.Documents.EF.MTR
{
    /// <summary>
    /// Позиция заявки
    /// </summary>
    [DebuggerDisplay("ID = {MtrPositionId}, Document = {DocumentId}, Order = {MtrOrder},  Name = {MtrName}")]
    public class MTRClaimItem : ICloneable<MTRClaimItem>
    {
        #region Поля сущности

        /// <summary>
        /// Код позиции заявок МТР
        /// </summary>
        /// <value>
        /// КодПозицииЗаявокМТР (int, not null)
        /// </value>
        public int MtrPositionId { get; set; }

        /// <summary>
        ///  Код кокумента(Id)
        /// </summary>
        /// <value>
        /// КодДокумента (int, not null)
        /// </value>
        public int DocumentId { get; set; }

        /// <summary>
        ///  Порядок документа, он же номер, в пределах одного документа уникальный
        /// </summary>
        public int MtrOrder { get; set; }

        /// <summary>
        ///  Наименование МТР
        /// </summary>
        /// <value>
        /// Наименование (nvarchar(300), not null)
        /// </value>
        public string MtrName { get; set; }

        /// <summary>
        /// Технические характеристики
        /// </summary>
        /// <value>
        /// ТехническиеХарактеристики (nvarchar(300), not null)
        /// </value>
        public string Specifications { get; set; }

        /// <summary>
        /// Цель приобретения
        /// </summary>
        /// <value>
        /// ЦельПриобретения (nvarchar(500), not null)
        /// </value>
        public string PurposeOfAcquisition { get; set; }
        /// <summary>
        /// Срок закупки
        /// </summary>
        /// <value>
        /// СрокЗакупки (datetime, not null)
        /// </value>
        public DateTime PurchasesTerm { get; set; }

        /// <summary>
        ///  Единица измерения
        /// </summary>
        /// <value>
        /// ЕдиницаИзмерения (nvarchar(10), not null)
        /// </value>
        public string Unit { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        /// <value>
        /// Количество (float, not null)
        /// </value>
        public decimal? Quantity { get; set; }

        /// <summary>
        ///  Примечание
        /// </summary>
        /// <value>
        /// Примечание (nvarchar(500), not null)
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///  Изменил id пользователя
        /// </summary>
        /// <value>
        /// Изменил (int, not null)
        /// </value>
        public int UserChangedId { get; set; }

        /// <summary>
        ///  Дата и время изменения
        /// </summary>
        /// <value>
        /// Изменено (datetime, not null)
        /// </value>
        public DateTime ChangedDateTime { get; set; }
        #endregion

        #region Рендер

        /// <summary>
        ///  Отрисовать связь позиции и документа
        /// </summary>
        public void RenderPositionDocLinks(TextWriter w, List<MtrChildDoc> childDocs, MtrChildType type)
        {
            foreach (var p in childDocs)
            {
                if (p.LinkType == type && p.MtrPositionId == MtrPositionId)
                {
                    if (type == MtrChildType.ДокументОснованиеПлатежа)
                        w.Write("<div class=\"holder\" draggable='true' ondragstart='SetDragInfo({0});'><a onclick='OpenDoc({0});' href='#'> <img border='0' src='/styles/DocMain.gif'>", p.DocId);
                    else
                        w.Write("<div><a href='javascript:OpenDoc({0});'> <img src='/styles/DocMain.gif'>", p.DocId);

                    w.Write(p.DocumentName);
                    w.Write("</a>");


                    if (type == MtrChildType.ДокументОснованиеПлатежа)
                    {
                        if (p.PartialQuantity != 0 && p.PartialQuantity != Quantity.Value)
                        {

                            if (p.PartialQuantity > Quantity.Value)
                            {
                                w.Write("<span style='color: red;' title='Количество в позиции меньше ожидаемого'>");
                                w.Write(" сверх. {0} {1}", p.PartialQuantity.ToString("G"), Unit);
                            }
                            else if (p.PartialQuantity < Quantity.Value)
                            {
                                w.Write("<span style='color: red;' title='Частичная оплата'>");
                                w.Write(" част. {0} {1}", p.PartialQuantity.ToString("G"), Unit);
                            }

                            w.Write("</span>");
                        }
                        w.Write(
                            "<img class=\"block\" src=\"../../STYLES/Delete.gif\" border=\"0\" style=\"cursor: pointer;\" onclick=\"cmd('cmd', 'RemoveLinkDoc','mtrPos', '{0}', 'LnkDocId', '{1}', 'ask', '1');\">",
                            p.MtrPositionId, p.DocId);
                    }

                    w.Write("</div>");
                }
            }

        }

        #endregion

        /// <summary>
        ///  Интерфейсное свойство, 
        ///  для диалога привязывания ссылок документов
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        public MTRClaimItem()
        {
        }

        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        public MTRClaimItem(int itemId)
        {
            FillData(itemId);
        }

        /// <summary>
        ///  Получить одну позицию по ID
        /// </summary>
        private void FillData(int id)
        {
            using (var dbReader = new DBReader(GetMtrClaim, id, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colНаименование = dbReader.GetOrdinal("Наименование");
                    int colТехническиеХарактеристики = dbReader.GetOrdinal("ТехническиеХарактеристики");
                    int colЦельПриобретения = dbReader.GetOrdinal("ЦельПриобретения");
                    int colСрокЗакупки = dbReader.GetOrdinal("СрокЗакупки");
                    int colЕдиницаИзмерения = dbReader.GetOrdinal("ЕдиницаИзмерения");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colПримечание = dbReader.GetOrdinal("Примечание");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");
                    #endregion

                    if (dbReader.Read())
                    {
                        MtrPositionId = id;
                        DocumentId = dbReader.GetInt32(colКодДокумента);
                        MtrOrder = dbReader.GetInt32(colПорядок);
                        MtrName = dbReader.GetString(colНаименование);
                        Specifications = dbReader.GetString(colТехническиеХарактеристики);
                        PurposeOfAcquisition = dbReader.GetString(colЦельПриобретения);
                        PurchasesTerm = dbReader.GetDateTime(colСрокЗакупки);
                        Unit = dbReader.GetString(colЕдиницаИзмерения);
                        Quantity = dbReader.GetDecimal(colКоличество);
                        Description = dbReader.GetString(colПримечание);
                        UserChangedId = dbReader.GetInt32(colИзменил);
                        ChangedDateTime = dbReader.GetDateTime(colИзменено);
                    }
                }
            }
        }

        /// <summary>
        ///  Получить список позиций на преобретение МТР
        /// </summary>
        public static List<MTRClaimItem> GetClaimItemList(int docid)
        {
            if (docid == 0)
                return new List<MTRClaimItem>();

            var list = new List<MTRClaimItem>();

            using (var dbReader = new DBReader(GetClaimItems, docid, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодПозицииЗаявокМТР = dbReader.GetOrdinal("КодПозицииЗаявокМТР");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colНаименование = dbReader.GetOrdinal("Наименование");
                    int colТехническиеХарактеристики = dbReader.GetOrdinal("ТехническиеХарактеристики");
                    int colЦельПриобретения = dbReader.GetOrdinal("ЦельПриобретения");
                    int colСрокЗакупки = dbReader.GetOrdinal("СрокЗакупки");
                    int colЕдиницаИзмерения = dbReader.GetOrdinal("ЕдиницаИзмерения");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colПримечание = dbReader.GetOrdinal("Примечание");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");
                    #endregion

                    // нет смысла получать из базы, то что у нас уже есть
                    var docId = Convert.ToInt32(docid);

                    while (dbReader.Read())
                    {
                        var row = new MTRClaimItem
                        {
                            MtrPositionId = dbReader.GetInt32(colКодПозицииЗаявокМТР),
                            DocumentId = docId,
                            MtrOrder = dbReader.GetInt32(colПорядок),
                            MtrName = dbReader.GetString(colНаименование),
                            Specifications = dbReader.GetString(colТехническиеХарактеристики),
                            PurposeOfAcquisition = dbReader.GetString(colЦельПриобретения),
                            PurchasesTerm = dbReader.GetDateTime(colСрокЗакупки),
                            Unit = dbReader.GetString(colЕдиницаИзмерения),
                            Quantity = Convert.ToDecimal(dbReader.GetDouble(colКоличество)),
                            Description = dbReader.GetString(colПримечание),
                            UserChangedId = dbReader.GetInt32(colИзменил),
                            ChangedDateTime = dbReader.GetDateTime(colИзменено)
                        };
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///  Сохранение позиции заявки
        /// </summary>
        public void SavePosition()
        {
            if (MtrPositionId == 0)
                CreateDeadlockSafety();
            else
                UpdateData();
        }

        /// <summary>
        ///  Сохранение (Insert)
        /// </summary>
        public void Create()
        {
            if (DocumentId == 0)
                throw new ArgumentException("Документ не создан, дальнейшее сохранение не возможно");

                var sqlParams = SetSqlParams(isNew: true);

                var outputParams = new Dictionary<string, object> { { "@MtrPositionId", -1 } };
                DBManager.ExecuteNonQuery(InsertClaimItem, CommandType.Text, ConnString, sqlParams, outputParams);

                if (outputParams["@MtrPositionId"] is int)
                    MtrPositionId = (int)outputParams["@MtrPositionId"];
        }

        /// <summary>
        ///  Сохранение (Insert) c повтором в случае взаимоблокировки
        /// </summary>
        public void CreateDeadlockSafety()
        {
            var retryCount = 0;
            var maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    Create();
                    break;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)// Deadlock 
                    {
                        retryCount++;
                        Thread.Sleep(5000);
                    }                        
                    else
                       throw;
                }
            }
        }

        /// <summary>
        ///  Обновление существующих (Update)
        /// </summary>
        public void UpdateData()
        {
            if (DocumentId == 0)
                throw new ArgumentException("Документ не создан, дальнейшее сохранение не возможно");

            var sqlParams = SetSqlParams(isNew: false);
            DBManager.ExecuteNonQuery(UpdateClaimItem, CommandType.Text, ConnString, sqlParams);
        }

        /// <summary>
        ///  Обновить порядок документа МТР
        /// </summary>
        public void UpdateRowOrder()
        {
            if (DocumentId == 0)
                throw new ArgumentException("Документ не создан, дальнейшее сохранение не возможно");

            var sqlParams = new Dictionary<string, object> {{"@КодПозицииЗаявокМТР", MtrPositionId}, {"@Порядок", MtrOrder}};
            DBManager.ExecuteNonQuery(SqlUpdateRowOrder, CommandType.Text, ConnString, sqlParams);
        }

        /// <summary>
        ///  Удалить позицию МТР
        /// </summary>
        public void Delete()
        {
            var sqlParam = new Dictionary<string, object> {{"@КодПозицииЗаявокМТР", MtrPositionId}};

            DBManager.ExecuteNonQuery(DeleteClaimItem, CommandType.Text, ConnString, sqlParam);
        }

        /// <summary>
        ///  Установка параметров для записи в БД
        /// </summary>
        private Dictionary<string, object> SetSqlParams(bool isNew)
        {
            var sqlParams = new Dictionary<string, object>();

            if (!isNew)
                sqlParams.Add("@КодПозицииЗаявокМТР", MtrPositionId);

            sqlParams.Add("@КодДокумента", DocumentId);
            sqlParams.Add("@Порядок", MtrOrder);
            sqlParams.Add("@Наименование", MtrName ?? "");
            sqlParams.Add("@ТехническиеХарактеристики", Specifications ?? "");
            sqlParams.Add("@ЦельПриобретения", PurposeOfAcquisition ?? "");
            sqlParams.Add("@СрокЗакупки", PurchasesTerm);
            sqlParams.Add("@ЕдиницаИзмерения", Unit ?? "");
            sqlParams.Add("@Количество", Quantity);
            sqlParams.Add("@Примечание", Description ?? "-");

            return sqlParams;
        }

        #region SQL запросы

        /// <summary>
        ///  Получить позицию МТР по Id
        /// </summary>
        const string GetMtrClaim = "SELECT [КодДокумента] " +
                                         ",[Порядок] " +
                                         ",[Наименование] " +
                                         ",[ТехническиеХарактеристики] " +
                                         ",[ЦельПриобретения] " +
                                         ",[СрокЗакупки] " +
                                         ",[ЕдиницаИзмерения] " +
                                         ",[Количество] " +
                                         ",[Примечание] " +
                                         ",[Изменил] " +
                                         ",[Изменено] " +
                                         "FROM [dbo].[vwПозицииЗаявокМТР] (nolock) where [КодПозицииЗаявокМТР] = @id";

        /// <summary>
        ///  Получить все позиции по id документа
        /// </summary>
        const string GetClaimItems = "SELECT [КодПозицииЗаявокМТР] " +
                                         ",[Порядок] " +
                                         ",[Наименование] " +
                                         ",[ТехническиеХарактеристики] " +
                                         ",[ЦельПриобретения] " +
                                         ",[СрокЗакупки] " +
                                         ",[ЕдиницаИзмерения] " +
                                         ",[Количество] " +
                                         ",[Примечание] " +
                                         ",[Изменил] " +
                                         ",[Изменено] " +
                                         "FROM [dbo].[vwПозицииЗаявокМТР] (nolock) where [КодДокумента] = @id";

        /// <summary>
        ///  Добавить новую позицию
        /// </summary>
        const string InsertClaimItem = "INSERT INTO dbo.vwПозицииЗаявокМТР " +
                                       "(КодДокумента, Порядок, Наименование, ТехническиеХарактеристики, ЦельПриобретения, СрокЗакупки, ЕдиницаИзмерения, Количество, Примечание) " +
                                       "VALUES  (@КодДокумента, @Порядок, @Наименование, @ТехническиеХарактеристики, @ЦельПриобретения, @СрокЗакупки,@ЕдиницаИзмерения,@Количество,@Примечание) " +
                                       "SET @MtrPositionId = SCOPE_IDENTITY()";

        /// <summary>
        ///  Обновить действующую
        /// </summary>
        const string UpdateClaimItem = "UPDATE dbo.vwПозицииЗаявокМТР " +
                                       "SET КодДокумента = @КодДокумента, Порядок = @Порядок, Наименование = @Наименование, ТехническиеХарактеристики = @ТехническиеХарактеристики, " +
                                       "ЦельПриобретения = @ЦельПриобретения, СрокЗакупки = @СрокЗакупки, ЕдиницаИзмерения = @ЕдиницаИзмерения, Количество = @Количество, "+
                                       "Примечание = @Примечание WHERE КодПозицииЗаявокМТР = @КодПозицииЗаявокМТР";

        /// <summary>
        ///  Обновить порядок в позиции
        /// </summary>
        const string SqlUpdateRowOrder = "UPDATE dbo.vwПозицииЗаявокМТР " +
                                        "SET Порядок = @Порядок WHERE КодПозицииЗаявокМТР = @КодПозицииЗаявокМТР";


        /// <summary>
        ///  Удалить существующую позицию
        /// </summary>
        const string DeleteClaimItem = "DELETE " +
                                       "FROM  dbo.vwПозицииЗаявокМТР " +
                                       "WHERE  КодПозицииЗаявокМТР = @КодПозицииЗаявокМТР";

        #endregion

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public MTRClaimItem Clone()
        {
            return (MTRClaimItem)MemberwiseClone();
        }
    }

}