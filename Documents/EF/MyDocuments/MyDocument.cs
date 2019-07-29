using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents.EF.MyDocuments
{
    /// <summary>
    /// </summary>
    public class MyDocument : Document
    {
        /// <summary>
        /// </summary>
        public MyDocument()
        {
            Type = DocTypeEnum.СчетФактура;
            MyField = GetDocField("75"); //Продавец
        }

        /// <summary>
        /// </summary>
        public DocField MyField { get; }
    }
}