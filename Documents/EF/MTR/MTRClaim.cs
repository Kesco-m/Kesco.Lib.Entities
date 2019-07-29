using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.MTR
{
    /// <summary>
    ///     Документ заявки на МТР
    /// </summary>
    [Serializable]
    public class MTRClaim : Document
    {
        /// <summary>
        ///     SQL запрос: получить id руководителя подразделения
        /// </summary>
        private const string SQLGetHeadDivision = "SELECT TOP 1 Parent.КодСотрудника " +
                                                  "FROM vwДолжности Parent " +
                                                  "INNER JOIN vwДолжности Child ON Parent.L <= Child.L AND Parent.R >= Child.R " +
                                                  "WHERE Child.Подразделение=@Подразделение AND Parent.КодЛица=@КодЛица AND Parent.КодСотрудника IS NOT NULL " +
                                                  "ORDER BY Parent.R";

        /// <summary>
        ///     Получает один единственный раз строку подключения инвентаризация
        /// </summary>
        private static readonly string _inventConnString = Config.DS_user;

        /// <summary>
        ///     Конструктор заявок МТР
        /// </summary>
        public MTRClaim()
        {
            Type = DocTypeEnum.ЗаявкаНаПриобретениеМТР;
            Organization = GetDocField("1799");
            Subdivision = GetDocField("1800");
            PerformerOfSubdivision = GetDocField("1801");
            Basis = GetDocField("1802");
            RequestItems = GetDocField("1803");
            Positions = new List<MTRClaimItem>();
            PositionDocLinks = new List<MtrChildDoc>();
        }

        /// <summary>
        ///     Организация
        /// </summary>
        public DocField Organization { get; private set; }

        /// <summary>
        ///     Подразделение
        /// </summary>
        public DocField Subdivision { get; private set; }

        /// <summary>
        ///     Исполнитель от подразделения
        /// </summary>
        public DocField PerformerOfSubdivision { get; private set; }

        /// <summary>
        ///     Документ - основание
        /// </summary>
        public DocField Basis { get; private set; }

        /// <summary>
        ///     Позиции заявки
        /// </summary>
        public DocField RequestItems { get; }

        /// <summary>
        ///     Позиции заяки
        /// </summary>
        public List<MTRClaimItem> Positions { get; set; }

        /// <summary>
        ///     Связи документов с позициями МТР
        /// </summary>
        public List<MtrChildDoc> PositionDocLinks { get; set; }


        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public override Document Clone()
        {
            var mtrDoc = (MTRClaim) base.Clone();
            mtrDoc.Organization = Organization.Clone();
            mtrDoc.Subdivision = Subdivision.Clone();
            mtrDoc.PerformerOfSubdivision = PerformerOfSubdivision.Clone();
            mtrDoc.Basis = Basis.Clone();

            if (Positions != null)
            {
                mtrDoc.Positions = new List<MTRClaimItem>(Positions.Capacity);
                foreach (var p in Positions)
                {
                    var clonePos = p.Clone();
                    clonePos.MtrPositionId = 0;
                    clonePos.DocumentId = 0;
                    clonePos.UserChangedId = 0;
                    clonePos.ChangedDateTime = DateTime.MinValue;
                    mtrDoc.Positions.Add(clonePos);
                }
            }

            mtrDoc.BaseDocsLinks.CloneList(BaseDocsLinks);

            return mtrDoc;
        }

        /// <summary>
        ///     Обнулить выбор
        /// </summary>
        public void ClearAllChecksPositions()
        {
            if (Positions != null)
                foreach (var p in Positions)
                    p.Checked = false;
        }

        /// <summary>
        ///     Обновляет позиции из БД
        /// </summary>
        public void ReloadPositions()
        {
            if (!IsNew)
                Positions = MTRClaimItem.GetClaimItemList(DocId);
        }

        /// <summary>
        ///     Сохраняем текущий документ
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команды для анализа запроса</param>
        public override void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            base.Save(evalLoad, cmds);

            // для сохранения нужен ID документа
            if (!IsNew)
                foreach (var p in Positions)
                {
                    p.DocumentId = DocId;
                    p.SavePosition();
                }
        }

        /// <summary>
        ///     Получить руководителя подразделения
        /// </summary>
        public static int GetHeadDivision(int organizationId, string subDivision)
        {
            var parameters = new Dictionary<string, object>
                {{"@Подразделение", subDivision}, {"@КодЛица", organizationId}};
            var result = DBManager.ExecuteScalar(SQLGetHeadDivision, CommandType.Text, _inventConnString, parameters);

            if (result is int)
                return (int) result;

            return 0;
        }
    }
}