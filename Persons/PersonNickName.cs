using System;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности псевдоним лица
    /// </summary>
    [Serializable]
    public class PersonNickName : Entity
    {
        /// <summary>
        ///     Псевдоним лица
        /// </summary>
        public new string Name { get; set; }
    }
}