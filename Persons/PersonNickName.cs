using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    /// Класс сущности псевдоним лица
    /// </summary>
    [Serializable]
    public class PersonNickName : Entity
    {
        /// <summary>
        /// Псевдоним лица
        /// </summary>
        public string Name { get; set; }
    }
}
