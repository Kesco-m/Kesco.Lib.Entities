using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Entities.Persons
{    
    /// <summary>
    /// Класс сущности ФИО лица на латинице
    /// </summary>
    [Serializable]
    public class PersonNameLat : Entity
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string SecondName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public PersonNameLat(string secondName = "", string firstName = "", string middleName = "")
        {
            FirstName = firstName;
            SecondName = secondName;
            MiddleName = middleName;
        }
    }
}
