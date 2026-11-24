using System;
using System.Collections.Generic;

namespace TrpoLab
{
    /// <summary>
    /// Администратор телефонной станции. ФИО наследует от класса Person.
    /// Подключает абонентов, просматривает их список и неоплаченные
    /// счета, блокирует абонентов.
    /// </summary>
    public class Administrator : Person
    {
        // Линия-разделитель для консольного вывода. Объявлена как поле
        // static readonly: значение вычисляется один раз при запуске.
        private static readonly string Separator = new string('-', 70);

        // Абоненты, подключённые данным администратором (private).
        private readonly List<Subscriber> _subscribers;

        /// <summary>
        /// Создаёт администратора с заданным ФИО и пустым списком
        /// подключённых абонентов.
        /// </summary>
        public Administrator(string lastName, string firstName,
            string middleName)
            : base(lastName, firstName, middleName)
        {
            _subscribers = new List<Subscriber>();
        }

        /// <summary>
        /// Подключает абонента: назначает ему номер телефона и вносит
        /// в список подключённых. Номер должен быть уникальным.
        /// </summary>
        public void ConnectSubscriber(Subscriber subscriber,
            string phoneNumber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
            if (FindSubscriber(phoneNumber) != null)
            {
                throw new InvalidOperationException(
                    "Номер " + phoneNumber + " уже назначен абоненту.");
            }
            // Connect проверяет номер и состояние абонента; если он
            // выбросит исключение, в список абонент не попадёт
            subscriber.Connect(phoneNumber);
            _subscribers.Add(subscriber);
        }

        /// <summary>
        /// Ищет подключённого абонента по номеру телефона.
        /// Возвращает null, если абонент не найден.
        /// </summary>
        public Subscriber FindSubscriber(string phoneNumber)
        {
            foreach (Subscriber subscriber in _subscribers)
            {
                if (subscriber.PhoneNumber == phoneNumber)
                {
                    return subscriber;
                }
            }
            return null;
        }

        /// <summary>
        /// Блокирует абонента из списка подключённых.
        /// </summary>
        public void BlockSubscriber(Subscriber subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
            if (!_subscribers.Contains(subscriber))
            {
                throw new InvalidOperationException(
                    "Абонент не подключён этим администратором.");
            }
            subscriber.Block();
        }

        /// <summary>
        /// Возвращает количество подключённых абонентов.
        /// </summary>
        public int GetSubscribersCount()
        {
            return _subscribers.Count;
        }

        /// <summary>
        /// Формирует список неоплаченных счетов всех абонентов.
        /// </summary>
        public List<Bill> GetUnpaidBills()
        {
            List<Bill> unpaid = new List<Bill>();
            foreach (Subscriber subscriber in _subscribers)
            {
                foreach (Bill bill in subscriber.Bills)
                {
                    if (!bill.IsPaid)
                    {
                        unpaid.Add(bill);
                    }
                }
            }
            return unpaid;
        }

        /// <summary>
        /// Выводит в консоль список подключённых абонентов.
        /// </summary>
        public void PrintSubscribers()
        {
            if (_subscribers.Count > 0)
            {
                int index = 1;
                Console.WriteLine(Separator);
                Console.WriteLine("Список абонентов");
                Console.WriteLine("№ п/п | ФИО, телефон, статус, услуги");
                foreach (Subscriber subscriber in _subscribers)
                {
                    Console.WriteLine(index + " | " + subscriber.Info());
                    index++;
                }
                Console.WriteLine(Separator);
            }
            else
            {
                Console.WriteLine("Список абонентов пуст!");
            }
        }

        /// <summary>
        /// Выводит в консоль неоплаченные счета и общую задолженность.
        /// </summary>
        public void PrintUnpaidBills()
        {
            List<Bill> unpaid = GetUnpaidBills();
            if (unpaid.Count > 0)
            {
                int index = 1;
                decimal debt = 0;
                Console.WriteLine(Separator);
                Console.WriteLine("Неоплаченные счета");
                foreach (Bill bill in unpaid)
                {
                    Console.WriteLine(index + " | " + bill.Print());
                    debt += bill.Total;
                    index++;
                }
                Console.WriteLine(
                    $"Общая задолженность: {debt:F2} {Service.Currency}");
                Console.WriteLine(Separator);
            }
            else
            {
                Console.WriteLine("Неоплаченных счетов нет!");
            }
        }
    }
}
