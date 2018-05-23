using System.Collections.Generic;
using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities.Documents
{
    /// <summary>
    ///     Инетерфейс для объекта типа Document, если документ имеет редактируемые позиции
    /// </summary>
    public interface IDocumentWithPositions
    {
        /// <summary>
        ///     Общая функция, реализующая вызов сохранения позиций документа
        /// </summary>
        /// <param name="reloadPostions">Нужно ли заново получить позиции после сохранения</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        void SaveDocumentPositions(bool reloadPostions, List<DBCommand> cmds = null );

        /// <summary>
        /// Загрузка всех позиций документа
        /// </summary>
        void LoadDocumentPositions();
    }
}