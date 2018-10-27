using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;

namespace Kesco.Lib.Entities
{
    /// <summary>
    /// Базовый класс сущности
    /// </summary>
    /// <remarks>
    /// Существует программа, преобразования запроса или ХП в класс сущности и начитки данных:
    /// https://titan.kescom.com:8443/svn/web/GeneratorORM
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("ID = {Id}, Name = {Name}, Unavailable = {Unavailable}")]
    public abstract class Entity
    {
        private bool requiredRefreshInfo = false;
        /// <summary>
        /// Доступность сущности. Удалось по ключу создать экземпляр объекта. false = ok
        /// </summary>
        public bool Unavailable { get; set; }

        /// <summary>
        /// Необходимо заново получить данные для закэшированных свойств объекта
        /// </summary>
        public bool RequiredRefreshInfo {
            get { return requiredRefreshInfo; }
            set { requiredRefreshInfo = value; }
        }

        /// <summary>
        /// Доступность сущности. Удалось по ключу создать экземпляр объекта.
        /// </summary>
        /// <remarks>
        ///  Unavailable - двойное отрицание слишком тяжелое для восприятия человеком
        /// </remarks>
        public bool Available
        {
            get { return !Unavailable; } 
            set { Unavailable = !value; }
        }

        /// <summary>
        /// Строка подключения к БД. 
        /// Обязательно к переопределению для классов подключающихся к БД
        /// </summary>
        public virtual string CN { get { throw new NotImplementedException("Строка подключения не перопределена для класса наследника"); } }

        /// <summary>
        /// Код объекта
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Имя объекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Метод получения сущности
        /// </summary>
        public virtual void Load()
        {
            throw new NotImplementedException("Метод Load не реализован для класса наследника");
        }

        /// <summary>
        ///  Сохранение сущности
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        public virtual void Save(bool evalLoad, List<DBCommand> cmds = null)
        {
            throw new NotImplementedException("Метод Save не реализован для класса наследника");
        }

        /// <summary>
        ///  Удаление сущности
        /// </summary>
        /// <param name="evalLoad">Выполнить повторную загрузку</param>
        /// <param name="cmds">Объект sql-команд для анализа запроса</param>
        public virtual void Delete(bool evalLoad, List<DBCommand> cmds = null)
        {
            throw new NotImplementedException("Метод Delete не реализован для класса наследника");
        }

        /// <summary>
        /// Метод заполнения таблицы с данными сущности
        /// </summary>
        /// <param name="dt">Таблица для записи в нее данных сущности</param>
        protected virtual void FillData(DataTable dt)
        {
            throw new NotImplementedException("Метод FillData не реализован для класса наследника");
        }

       
        /// <summary>
        ///  Событие изменения значения
        /// </summary>
        public event ValueChangedEventHandler ValueChangedEvent;

        /// <summary>
        ///  Выполяняет действия события ValueChangedEvent
        /// </summary>
        public void ValueChangedEvent_Invoke(string newVal, string oldVal)
        {
            var handler = ValueChangedEvent;

            if (handler != null)
                handler(this, new ValueChangedEventArgs(newVal, oldVal));
        }

        /// <summary>
        /// Конструктор базовой сущности
        /// </summary>
        protected Entity()
        {
            // по умолчанию сущность не доступна
            Unavailable = true;
        }

        /// <summary>
        /// Конструктор базовой сущности с параметрами
        /// </summary>
        /// <param name="id">Код сущности</param>
        protected Entity(string id) :this()
        {
            Id = id;
        }
    }
}
