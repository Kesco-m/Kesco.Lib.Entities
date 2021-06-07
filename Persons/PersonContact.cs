using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    /// Контакт лица  для звонилки
    /// </summary>
    [Serializable]
    public class PersonContact
    {

        /// <summary>
        ///     КодКонтакта
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        ///     КодСотрудника
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     КодЛица
        /// </summary>
        public int? PersonId { get; set; }
        
        /// <summary>
        ///     icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     КодТипаКонтакта
        /// </summary>
        public int TypeContactId { get; set; }

        /// <summary>
        ///     ТипКонтакта
        /// </summary>
        public string TypeContact { get; set; }

        /// <summary>
        ///     Контакт
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        ///     Порядок
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///     НомерМеждународный
        /// </summary>
        public string InternationalNumber { get; set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public string Description { get; set; }

        

    }
}
