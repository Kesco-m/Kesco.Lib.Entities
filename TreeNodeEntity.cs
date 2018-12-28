using System;
using System.Collections.Generic;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.Entities.Documents;

namespace Kesco.Lib.Entities
{
    /// <summary>
    ///  Базовый класс древовидных структур данных
    /// </summary>
    [Serializable]
    public abstract class TreeNodeEntity : Entity
    {
        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        protected TreeNodeEntity(string id) : base(id)
        {}


        /// <summary>
        /// Поле Parent, родительский элемент дерева
        /// </summary>
        public int Parent { get; set; }

        /// <summary>
        /// Поле L, левый узел
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Поле R, правый узел
        /// </summary>
        public int R { get; set; }

        /// <summary>
        /// Родительский элемент
        /// </summary>
        public abstract TreeNodeEntity TreeNodeParent { get; }

        /// <summary>
        /// Все подчиненные элементы
        /// </summary>
        public List<TreeNodeEntity> TreeNodeChildren
        {
            get { return LoadChildren(); }
        }

        /// <summary>
        ///  Проверка является ли данный элемент подчиненным для данного элемента
        /// </summary>
        public bool ChildOf(DocTypeEnum type)
        {
            var typeInt = (int) type;
            return ChildOf(typeInt.ToString());
        }

        /// <summary>
        ///  Проверка является ли данный элемент подчиненным для данного элемента
        /// </summary>
        public bool ChildOf(string ch)
        {
            if (Parent == 0) return false;
            var child = ch.ToInt();

            if (Parent == child) return true;

            return TreeNodeParent.ChildOf(ch);
        }

        /// <summary>
        ///  Загрузить подчиненные узлы
        /// </summary>
        public abstract List<TreeNodeEntity> LoadChildren();

        /// <summary>
        ///  Загрузить родительские узлы
        /// </summary>
        public abstract List<TreeNodeEntity> LoadParents();

    }
}
