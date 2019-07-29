using System;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     Класс для простых сущностей
    /// </summary>
    [Serializable]
    public class Ghost : Entity
    {
        /// <summary>
        ///     Статус сущности
        /// </summary>
        public string State { get; set; }

        /// <summary>
        ///     Код сотрудника
        /// </summary>
        public string PersonId { get; set; }
    }
}