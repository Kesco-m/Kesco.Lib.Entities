using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    /// Класс параметров запроса по типу документа
    /// </summary>
    public class DocTypeParam
    {
        /// <summary>
        /// ID типа документа
        /// </summary>
        public string DocTypeID { get; set; }

        /// <summary>
        /// alias для DocTypeID
        /// </summary>
        public DocTypeEnum DocTypeEnum
        {
            get { return (DocTypeEnum) DocTypeID.ToInt(); }
            set { DocTypeID = ((int) value).ToString(); }
        }

        /// <summary>
        /// Тип запроса типов документа
        /// </summary>
        public DocTypeQueryType QueryType { get; set; }
    }

   
}
