﻿using System;

namespace Kesco.Lib.Entities.Persons
{
    /// <summary>
    ///     Класс сущности ФИО лица на латинице
    /// </summary>
    [Serializable]
    public class PersonNameLat : Entity
    {
        /// <summary>
        ///     Конструктор
        /// </summary>
        public PersonNameLat(string secondName = "", string firstName = "", string middleName = "")
        {
            FirstName = firstName;
            SecondName = secondName;
            MiddleName = middleName;
        }

        /// <summary>
        ///     Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Фамилия
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        ///     Отчество
        /// </summary>
        public string MiddleName { get; set; }
    }
}