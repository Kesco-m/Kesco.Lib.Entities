using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Entities.Corporate
{
    /// <summary>
    /// Класс текущего пользователя
    /// </summary>
    public class EmployeeCurrent: Employee
    {
        /// <summary>
        /// Конструктор, заполнющий класс User
        /// </summary>
        public EmployeeCurrent()
        {
            GetCurrentUser();
        }

    }
}
