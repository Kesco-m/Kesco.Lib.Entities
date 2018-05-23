using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс параметров поиска по номеру документа
    /// </summary>
    public class DocNumberParam
    {
        /// <summary>
        /// ID типа документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Тип поиска по номеру документа
        /// </summary>
        public SearchType DocNumberQueryType { get; set; }

    }

   
}

