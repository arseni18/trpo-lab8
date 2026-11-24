namespace TrpoLab
{
    /// <summary>
    /// Человек. Базовый класс для сущностей «Администратор» и «Абонент»:
    /// хранит фамилию, имя и отчество и формирует полное ФИО.
    /// </summary>
    public class Person
    {
        /// <summary>Фамилия.</summary>
        public string LastName { get; set; }

        /// <summary>Имя.</summary>
        public string FirstName { get; set; }

        /// <summary>Отчество (может быть пустым).</summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Создаёт человека с заданными фамилией, именем и отчеством.
        /// </summary>
        public Person(string lastName, string firstName, string middleName)
        {
            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
        }

        /// <summary>
        /// Возвращает ФИО одной строкой: «Фамилия Имя Отчество».
        /// </summary>
        public string GetFIO()
        {
            // Trim() убирает лишний пробел в конце, если отчества нет
            return (LastName + " " + FirstName + " " + MiddleName).Trim();
        }
    }
}
