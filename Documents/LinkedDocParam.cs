using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс параметров запроса по связанным документам
    /// </summary>
    public class LinkedDocParam
    {
        /// <summary>
        /// ID документа
        /// </summary>
        public string DocID { get; set; }

        /// <summary>
        /// Тип запроса связанных документов
        /// </summary>
        public LinkedDocsType QueryType { get; set; }
    }

  
}
