using System;

namespace TrpoLab
{
    /// <summary>
    /// Услуга телефонной станции (например, «Определитель номера»).
    /// Абонент может выбрать одну или несколько услуг.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Валюта расчётов. Константа класса: значение задано при
        /// объявлении, общее для всех объектов, доступно по имени класса.
        /// </summary>
        public const string Currency = "руб.";

        /// <summary>Название услуги.</summary>
        public string Name { get; }

        /// <summary>Стоимость услуги за месяц.</summary>
        public decimal MonthlyCost { get; }

        /// <summary>
        /// Создаёт услугу с заданными названием и месячной стоимостью.
        /// Свойства только для чтения: значения проверяются только здесь.
        /// </summary>
        public Service(string name, decimal monthlyCost)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Название услуги не может быть пустым.", nameof(name));
            }
            if (monthlyCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthlyCost),
                    "Стоимость услуги не может быть отрицательной.");
            }
            Name = name;
            MonthlyCost = monthlyCost;
        }

        /// <summary>
        /// Возвращает описание услуги: название и стоимость за месяц.
        /// </summary>
        public string Info()
        {
            return $"{Name}: {MonthlyCost:F2} {Currency}/мес.";
        }
    }
}
