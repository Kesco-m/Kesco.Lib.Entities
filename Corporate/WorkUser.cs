using System;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Сотрудники, работающие сейчас с документами
    /// </summary>
    [Serializable]
    public class WorkUser
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        public Employee User { get; set; }

        /// <summary>
        /// Код сущности
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Признак возможности редактирования документа
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Скрипт для обновления документа
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// ID страницы
        /// </summary>
        public string IDPage { get; set; }

        /// <summary>
        /// Количество экземпляров одной страницы
        /// </summary>
        public int CountPages { get; set; }
    }
}
