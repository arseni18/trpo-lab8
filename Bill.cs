using System;

namespace TrpoLab
{
    /// <summary>
    /// Счёт абонента за разговоры и услуги. Суммы фиксируются в момент
    /// выставления счёта, поэтому последующие изменения набора услуг
    /// абонента на уже выставленный счёт не влияют.
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// Стоимость одной минуты разговора. Константа класса: значение
        /// задаётся один раз и не может быть изменено.
        /// </summary>
        public const decimal CallCostPerMinute = 0.05m;

        // Номер последнего выставленного счёта. static: поле общее для
        // всего класса; private: изменить его можно только внутри Bill.
        private static int _lastNumber = 0;

        /// <summary>Уникальный номер счёта.</summary>
        public int Number { get; }

        /// <summary>Абонент, которому выставлен счёт.</summary>
        public Subscriber Subscriber { get; }

        /// <summary>Дата выставления счёта.</summary>
        public DateTime IssueDate { get; }

        /// <summary>Продолжительность разговоров, минут.</summary>
        public int CallMinutes { get; }

        /// <summary>Стоимость разговоров.</summary>
        public decimal CallsCost { get; }

        /// <summary>Стоимость выбранных абонентом услуг.</summary>
        public decimal ServicesCost { get; }

        /// <summary>
        /// Признак оплаты. Читать может любой класс, а изменить только
        /// сам счёт (метод Pay), поэтому set объявлен как private.
        /// </summary>
        public bool IsPaid { get; private set; }

        /// <summary>Итого к оплате (вычисляемое свойство).</summary>
        public decimal Total => CallsCost + ServicesCost;

        /// <summary>
        /// Формирует счёт абоненту: стоимость разговоров считается по
        /// тарифу, стоимость услуг берётся по услугам абонента.
        /// </summary>
        public Bill(Subscriber subscriber, int callMinutes,
            DateTime issueDate)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
            if (callMinutes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(callMinutes),
                    "Число минут не может быть отрицательным.");
            }
            Number = ++_lastNumber;
            Subscriber = subscriber;
            IssueDate = issueDate;
            CallMinutes = callMinutes;
            CallsCost = callMinutes * CallCostPerMinute;
            ServicesCost = subscriber.GetServicesCost();
            IsPaid = false;
        }

        /// <summary>
        /// Помечает счёт оплаченным. Повторная оплата недопустима.
        /// </summary>
        public void Pay()
        {
            if (IsPaid)
            {
                throw new InvalidOperationException("Счёт уже оплачен.");
            }
            IsPaid = true;
        }

        /// <summary>
        /// Возвращает строку с полной информацией о счёте: номер, дату,
        /// абонента, суммы за разговоры и услуги, итог и статус оплаты.
        /// </summary>
        public string Print()
        {
            string status = IsPaid ? "оплачен" : "не оплачен";
            return $"Счёт №{Number} от {IssueDate:dd.MM.yyyy} | " +
                $"{Subscriber.GetFIO()} ({Subscriber.PhoneNumber}) | " +
                $"разговоры: {CallMinutes} мин. – {CallsCost:F2} | " +
                $"услуги: {ServicesCost:F2} | " +
                $"итого: {Total:F2} {Service.Currency} | {status}";
        }
    }
}
