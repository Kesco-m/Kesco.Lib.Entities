using System;
using System.Collections.Generic;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///     Вытекающие документы
    /// </summary>
    [Serializable]
    public class DocSequels
    {
        /// <summary>
        ///     Вытекающие документы
        /// </summary>
        public List<Document> Documents;

        /// <summary>
        ///     Код поля документа
        /// </summary>
        public int FieldId { get; set; }
    }
}