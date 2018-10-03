using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Persons;

namespace Kesco.Lib.Entities.Documents.EF.Directions
{
    /// <summary>
    ///     Класс позиций документа УказанияИТ: ПозицииУказанийИТРоли
    /// </summary>
    [DBSource("vwПозицииУказанийИТРоли", SQLQueries.SUBQUERY_ID_ПозицииУказанийИТРоли, SQLQueries.SUBQUERY_ID_DOC_ПозицииУказанийИТРоли)]
    public class PositionRole : DocumentPosition<PositionRole>
    {
        /// <summary>
        ///     Конструктор дл загрузки по коду документа
        /// </summary>
        public PositionRole()
        {
        }

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public PositionRole(int id)
        {
            Id = id.ToString();
            Load();
        }

        #region Поля

        /// <summary>
        ///     КодПозицииУказанийИТРоль
        /// </summary>
        [DBField("КодПозицииУказанийИТРоль", 0)]
        public override int? PositionId
        {
            get { return base.PositionId; }
            set { base.PositionId = value; }
        }

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     КодРоли
        /// </summary>
        [DBField("КодРоли")]
        public int RoleId { get; set; }


        private Role _role;
        private Persons.Person _person;

        /// <summary>
        ///     Используем объект, т.к. vwРоли не реплицируются
        ///     Возвращает объект типа Role в зависимости от значения RoleId
        /// </summary>
        public Role RoleObject
        {
            get
            {
                if (RoleId == 0)
                    _role = null;
                else if (_role == null || _role.Id != RoleId.ToString())
                    _role = new Role(RoleId.ToString());

                return _role;
            }
        }

        /// <summary>
        ///     КодЛица
        /// </summary>
        [DBField("КодЛица")]
        public int PersonId { get; set; }

        /// <summary>
        ///     Название лица заказчика
        /// </summary>
        [DBField("НазваниеЛица","" , false)]
        public string PersonName { get; set; }

        /// <summary>
        ///     Возвращает объект типа Person в зависимости от значения PersonId
        /// </summary>
        public Person PersonObject
        {
            get
            {
                if (PersonId == 0)
                    _person = null;
                else if (_person == null || _person.Id != PersonId.ToString())
                    _person = new Person(PersonId.ToString());

                return _person;
            }
        }

        #endregion
    }
}