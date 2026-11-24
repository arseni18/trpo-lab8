using System;
using System.Collections.Generic;

namespace TrpoLab
{
    /// <summary>
    /// Абонент телефонной станции. ФИО наследует от класса Person.
    /// Состояния: создан → подключён администратором → заблокирован.
    /// Подключённый абонент выбирает услуги и оплачивает счета.
    /// </summary>
    public class Subscriber : Person
    {
        // Списки private: добавлять и удалять элементы можно только через
        // методы класса, поэтому данные абонента всегда согласованы.
        private readonly List<Service> _services;
        private readonly List<Bill> _bills;

        /// <summary>Номер телефона (задаётся при подключении).</summary>
        public string PhoneNumber { get; private set; }

        /// <summary>Подключён ли абонент к станции.</summary>
        public bool IsConnected { get; private set; }

        /// <summary>Заблокирован ли абонент.</summary>
        public bool IsBlocked { get; private set; }

        /// <summary>Выбранные услуги (только для чтения).</summary>
        public IReadOnlyList<Service> Services => _services;

        /// <summary>Выставленные счета (только для чтения).</summary>
        public IReadOnlyList<Bill> Bills => _bills;

        /// <summary>
        /// Создаёт нового абонента. Номер телефона назначается позже,
        /// при подключении (метод Connect).
        /// </summary>
        public Subscriber(string lastName, string firstName,
            string middleName)
            : base(lastName, firstName, middleName)
        {
            PhoneNumber = "";
            IsConnected = false;
            IsBlocked = false;
            _services = new List<Service>();
            _bills = new List<Bill>();
        }

        /// <summary>
        /// Подключает абонента и закрепляет за ним номер телефона.
        /// Вызывается администратором (Administrator.ConnectSubscriber).
        /// </summary>
        public void Connect(string phoneNumber)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException(
                    "Абонент уже подключён.");
            }
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException(
                    "Номер телефона не может быть пустым.",
                    nameof(phoneNumber));
            }
            PhoneNumber = phoneNumber;
            IsConnected = true;
        }

        /// <summary>
        /// Блокирует абонента. Вызывается администратором
        /// (Administrator.BlockSubscriber).
        /// </summary>
        public void Block()
        {
            EnsureConnected();
            if (IsBlocked)
            {
                throw new InvalidOperationException(
                    "Абонент уже заблокирован.");
            }
            IsBlocked = true;
        }

        /// <summary>
        /// Выбирает услугу. Доступно подключённому незаблокированному
        /// абоненту; одну услугу можно выбрать только один раз.
        /// </summary>
        public void SelectService(Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            EnsureConnected();
            if (IsBlocked)
            {
                throw new InvalidOperationException(
                    "Абонент заблокирован: выбор услуг недоступен.");
            }
            if (_services.Contains(service))
            {
                throw new InvalidOperationException(
                    "Услуга уже выбрана абонентом.");
            }
            _services.Add(service);
        }

        /// <summary>
        /// Возвращает суммарную месячную стоимость выбранных услуг.
        /// </summary>
        public decimal GetServicesCost()
        {
            decimal sum = 0;
            foreach (Service service in _services)
            {
                sum += service.MonthlyCost;
            }
            return sum;
        }

        /// <summary>
        /// Формирует счёт за разговоры и выбранные услуги и добавляет его
        /// в список счетов абонента. Возвращает созданный счёт.
        /// </summary>
        public Bill CreateBill(int callMinutes, DateTime issueDate)
        {
            EnsureConnected();
            Bill bill = new Bill(this, callMinutes, issueDate);
            _bills.Add(bill);
            return bill;
        }

        /// <summary>
        /// Оплачивает счёт. Можно оплатить только собственный
        /// неоплаченный счёт.
        /// </summary>
        public void PayBill(Bill bill)
        {
            if (bill == null)
            {
                throw new ArgumentNullException(nameof(bill));
            }
            if (!_bills.Contains(bill))
            {
                throw new InvalidOperationException(
                    "Счёт выставлен другому абоненту.");
            }
            bill.Pay();
        }

        /// <summary>
        /// Возвращает строку с информацией об абоненте: ФИО, телефон,
        /// статус и количество выбранных услуг.
        /// </summary>
        public string Info()
        {
            string phone = IsConnected ? PhoneNumber : "—";
            string status = "активен";
            if (!IsConnected)
            {
                status = "не подключён";
            }
            else if (IsBlocked)
            {
                status = "заблокирован";
            }
            return $"{GetFIO()}, тел.: {phone}, статус: {status}, " +
                $"услуг: {_services.Count}";
        }

        // Вспомогательная проверка. private: нужна только внутри класса.
        private void EnsureConnected()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Абонент не подключён к станции.");
            }
        }
    }
}
