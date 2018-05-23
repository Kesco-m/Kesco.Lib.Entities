using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents.EF.Invoice
{
    /// <summary>
    /// Документ Invoice
    /// </summary>
    /// <remarks>
    /// Cоответствует документу Счет-Фактура, но в 1С не переносится
    /// </remarks>
    public class InvoiceDocument : InvoiceBase
    {
        /// <summary>
        ///  Конструктор с параметром
        /// </summary>
        public InvoiceDocument(string id)
        {
            Id = id;
            LoadDocument(id, true);
            Initialization();
        }

        private void Initialization()
        {
            Type = DocTypeEnum.Инвойс;
        }
    }
}
