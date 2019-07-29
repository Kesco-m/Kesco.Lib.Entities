using Kesco.Lib.BaseExtention;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     Структура для коллекции выбранных элементов контрола Select
    /// </summary>
    public struct Item : ICloneable<Item>
    {
        /// <summary>
        ///     ID элемента
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Элемент
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Вспомогательное свойство для совместимости с контролами типа Select
        /// </summary>
        public object Name => Value;

        /// <summary>
        ///     Создает новый объект, являющийся копией текущего экземпляра.
        /// </summary>
        public Item Clone()
        {
            return (Item) MemberwiseClone();
        }
    }
}