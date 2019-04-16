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
        public virtual string CN { get { throw new NotImplementedException("Строка подключения не переопределена для класса наследника"); } }

        /// <summary>
        /// Код объекта
        /// </summary>
        public virtual string Id { get; set; }

        private string _name;
        /// <summary>
        /// Имя объекта
        /// </summary>
        public string Name {
            get
            {
                if (Unavailable)
                    return "#" + Id;
                return _name;
            }
            set { _name = value; }
        }

        /// <summary>
        /// Тип документа (RU)
        /// </summary>
        public string TypeDocRu { get; set; }

        /// <summary>
        /// Тип документа (EN)
        /// </summary>
        public string TypeDocEn { get; set; }

        /// <summary>
        /// Метод получения сущности
        /// </summary>
        public virtual void Load()
        {
            throw new NotImplementedException("Метод Load не реализован для класса наследника");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Изменено (datetime, not null)
        /// </value>
        public DateTime Changed { get; set; }

      
        /// <summary>
        /// Используется для кэширования объектов базы данных(Entity) в рамках одного поста
        /// </summary>
        /// <value>
        /// Устанавливаем при каждом post-запросе для определения необходимости кеширования объектов
        /// </value>
        public string CurrentPostRequest { get; set; }
        
        /// <summary>
        /// Метод получения даты последнего изменения
        /// </summary>
        public virtual DateTime GetLastChanged(string id)
        {
            //throw new NotImplementedException("Метод GetLastChanged не реализован для класса наследника");
            return DateTime.MinValue;
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
        /// При использовании данного конструктора, сущность автоматически становиться доступной
        /// еще до заполнения всех свойств, т.е. подразумевается, что программист заполнит все необходимые ему свойства 
        /// самостоятельно
        /// </summary>
        protected Entity()
        {
            Unavailable = false;
        }

        /// <summary>
        /// Конструктор базовой сущности с параметрами
        /// При использовании данного конструктора, сущность является НЕ доступной по-умолчанию
        /// Свойство Unavailable устанавливается в наследнике в методе Load, только после успешной загрузки свойств объекта
        /// </summary>
        /// <param name="id">Код сущности</param>
        protected Entity(string id) :this()
        {
            Unavailable = true;
            Id = id;
        }
    }
}
