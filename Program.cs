using System;
using System.Collections.Generic;
using System.Globalization;

namespace TrpoLab
{
    /// <summary>
    /// Текстовое интерактивное приложение "Телефонная станция".
    /// Создает объекты классов, передает их друг другу в параметрах
    /// методов и вызывает методы классов по командам пользователя.
    /// </summary>
    class Program
    {
        // Номер пункта меню "Выход". Константа: значение не меняется
        private const byte ExitItem = 8;

        static void Main(string[] args)
        {
            // Администратор создается пользователем (пункт 1 меню)
            Administrator admin = null;
            // Перечень услуг, которые предоставляет станция
            List<Service> services = CreateServices();
            byte selectItem = 0;

            while (selectItem != ExitItem)
            {
                Console.Clear();
                Console.WriteLine("Приложение \"Телефонная станция\"\r\n");
                ShowMenu(admin != null);
                selectItem = ReadMenuItem(admin != null);

                try
                {
                    // Навигация по меню: пункту соответствует действие
                    switch (selectItem)
                    {
                        case 1:
                            if (admin == null)
                            {
                                admin = CreateAdministrator();
                            }
                            else
                            {
                                ConnectSubscriber(admin);
                            }
                            break;
                        case 2:
                            admin.PrintSubscribers();
                            break;
                        case 3:
                            SelectService(admin, services);
                            break;
                        case 4:
                            IssueBill(admin);
                            break;
                        case 5:
                            PayBill(admin);
                            break;
                        case 6:
                            admin.PrintUnpaidBills();
                            break;
                        case 7:
                            BlockSubscriber(admin);
                            break;
                    }
                }
                catch (Exception ex)
                    when (ex is InvalidOperationException ||
                          ex is ArgumentException)
                {
                    // Класс отказался выполнить действие: выводим причину
                    Console.WriteLine("Ошибка: " + ex.Message);
                }

                if (selectItem != ExitItem)
                {
                    Pause();
                }
            }
            Console.WriteLine("Работа программы завершена.");
        }

        /// <summary>
        /// Формирует перечень услуг, предоставляемых станцией.
        /// </summary>
        private static List<Service> CreateServices()
        {
            return new List<Service>
            {
                new Service("Определитель номера", 3.50m),
                new Service("Антиопределитель номера", 2.00m),
                new Service("Переадресация вызовов", 4.00m),
                new Service("Голосовая почта", 5.50m)
            };
        }

        /// <summary>
        /// Выводит пункты меню. Пока администратора нет, доступны
        /// только его создание и выход.
        /// </summary>
        private static void ShowMenu(bool adminCreated)
        {
            Console.WriteLine("Выберите действие:");
            if (!adminCreated)
            {
                Console.WriteLine("1 - Создать администратора");
            }
            else
            {
                Console.WriteLine("1 - Подключить абонента");
                Console.WriteLine("2 - Показать список абонентов");
                Console.WriteLine("3 - Выбрать услугу для абонента");
                Console.WriteLine("4 - Выставить счёт абоненту");
                Console.WriteLine("5 - Оплатить счёт");
                Console.WriteLine("6 - Показать неоплаченные счета");
                Console.WriteLine("7 - Заблокировать абонента");
            }
            Console.WriteLine(ExitItem + " - Выход");
        }

        /// <summary>
        /// Считывает номер пункта меню. При недопустимом вводе выводит
        /// сообщение об ошибке и возвращает 0 (такого пункта в меню нет).
        /// </summary>
        private static byte ReadMenuItem(bool adminCreated)
        {
            byte item;
            Console.Write("> ");
            string tmpStr = Console.ReadLine();
            bool correctInput = byte.TryParse(tmpStr, out item);
            // Без администратора доступны только пункт 1 и выход
            bool available = adminCreated || item == 1 || item == ExitItem;
            if (!correctInput || item < 1 || item > ExitItem || !available)
            {
                Console.WriteLine("Ошибка ввода!");
                return 0;
            }
            return item;
        }

        /// <summary>
        /// Пункт 1 (администратора еще нет): создает администратора.
        /// </summary>
        private static Administrator CreateAdministrator()
        {
            Console.WriteLine("Создание администратора\r\n");
            string lastName = ReadText("Введите фамилию: ", true);
            string firstName = ReadText("Введите имя: ", true);
            string middleName = ReadText("Введите отчество: ", false);
            Administrator admin =
                new Administrator(lastName, firstName, middleName);
            Console.WriteLine("\r\nАдминистратор создан: " +
                admin.GetFIO());
            return admin;
        }

        /// <summary>
        /// Пункт 1: создает абонента и передает его администратору
        /// вместе с номером телефона для подключения.
        /// </summary>
        private static void ConnectSubscriber(Administrator admin)
        {
            Console.WriteLine("Подключение абонента\r\n");
            string lastName = ReadText("Введите фамилию абонента: ", true);
            string firstName = ReadText("Введите имя абонента: ", true);
            string middleName = ReadText("Введите отчество абонента: ",
                false);
            string phone = ReadText("Введите номер телефона: ", true);
            Subscriber subscriber =
                new Subscriber(lastName, firstName, middleName);
            admin.ConnectSubscriber(subscriber, phone);
            Console.WriteLine("\r\nАбонент подключён: " +
                subscriber.Info());
        }

        /// <summary>
        /// Пункт 3: абонент выбирает одну из услуг станции.
        /// </summary>
        private static void SelectService(Administrator admin,
            List<Service> services)
        {
            Console.WriteLine("Выбор услуги\r\n");
            Subscriber subscriber = ReadSubscriber(admin);
            Console.WriteLine("\r\nПредоставляемые услуги:");
            for (int i = 0; i < services.Count; i++)
            {
                Console.WriteLine((i + 1) + " - " + services[i].Info());
            }
            int number = ReadInt("Введите номер услуги: ",
                1, services.Count);
            subscriber.SelectService(services[number - 1]);
            Console.WriteLine("\r\nУслуга выбрана: " + subscriber.Info());
        }

        /// <summary>
        /// Пункт 4: формирует счёт абоненту за разговоры и услуги.
        /// </summary>
        private static void IssueBill(Administrator admin)
        {
            Console.WriteLine("Выставление счёта\r\n");
            Subscriber subscriber = ReadSubscriber(admin);
            int minutes = ReadInt("Введите число минут разговоров: ",
                0, int.MaxValue);
            DateTime date = ReadDate("Введите дату счёта (дд.мм.гггг): ");
            Bill bill = subscriber.CreateBill(minutes, date);
            Console.WriteLine("\r\nСчёт выставлен:\r\n" + bill.Print());
        }

        /// <summary>
        /// Пункт 5: абонент оплачивает один из своих неоплаченных счетов.
        /// </summary>
        private static void PayBill(Administrator admin)
        {
            Console.WriteLine("Оплата счёта\r\n");
            Subscriber subscriber = ReadSubscriber(admin);
            List<Bill> unpaid = GetUnpaidBillsOf(subscriber);
            if (unpaid.Count == 0)
            {
                Console.WriteLine("\r\nНеоплаченных счетов нет!");
                return;
            }
            Console.WriteLine("\r\nНеоплаченные счета абонента:");
            for (int i = 0; i < unpaid.Count; i++)
            {
                Console.WriteLine((i + 1) + " - " + unpaid[i].Print());
            }
            int number = ReadInt("Введите номер счёта для оплаты: ",
                1, unpaid.Count);
            subscriber.PayBill(unpaid[number - 1]);
            Console.WriteLine("\r\nСчёт оплачен!");
        }

        /// <summary>
        /// Пункт 7: администратор блокирует выбранного абонента.
        /// </summary>
        private static void BlockSubscriber(Administrator admin)
        {
            Console.WriteLine("Блокировка абонента\r\n");
            Subscriber subscriber = ReadSubscriber(admin);
            admin.BlockSubscriber(subscriber);
            Console.WriteLine("\r\nАбонент заблокирован: " +
                subscriber.Info());
        }

        /// <summary>
        /// Показывает список абонентов и находит выбранного по номеру
        /// телефона. Если абонента нет, генерирует исключение.
        /// </summary>
        private static Subscriber ReadSubscriber(Administrator admin)
        {
            if (admin.GetSubscribersCount() == 0)
            {
                throw new InvalidOperationException(
                    "Нет подключённых абонентов.");
            }
            admin.PrintSubscribers();
            string phone = ReadText(
                "\r\nВведите номер телефона абонента: ", true);
            Subscriber subscriber = admin.FindSubscriber(phone);
            if (subscriber == null)
            {
                throw new InvalidOperationException(
                    "Абонент с таким номером не найден.");
            }
            return subscriber;
        }

        /// <summary>
        /// Возвращает неоплаченные счета одного абонента.
        /// </summary>
        private static List<Bill> GetUnpaidBillsOf(Subscriber subscriber)
        {
            List<Bill> unpaid = new List<Bill>();
            foreach (Bill bill in subscriber.Bills)
            {
                if (!bill.IsPaid)
                {
                    unpaid.Add(bill);
                }
            }
            return unpaid;
        }

        /// <summary>
        /// Считывает строку. Если строка обязательна, ввод повторяется
        /// до тех пор, пока пользователь не введет непустое значение.
        /// </summary>
        private static string ReadText(string prompt, bool required)
        {
            string tmpStr = "";
            bool correctInput = false;
            do
            {
                Console.Write(prompt);
                tmpStr = (Console.ReadLine() ?? "").Trim();
                correctInput = !required || tmpStr != "";
                if (!correctInput)
                {
                    Console.WriteLine("Ошибка ввода!\r\nПовторите ввод");
                }
            }
            while (!correctInput);
            return tmpStr;
        }

        /// <summary>
        /// Считывает целое число из диапазона [min; max]. Ввод повторяется
        /// до тех пор, пока не будет введено корректное значение.
        /// </summary>
        private static int ReadInt(string prompt, int min, int max)
        {
            int value = 0;
            bool correctInput = false;
            do
            {
                Console.Write(prompt);
                string tmpStr = Console.ReadLine();
                correctInput = int.TryParse(tmpStr, out value) &&
                    value >= min && value <= max;
                if (!correctInput)
                {
                    Console.WriteLine("Ошибка ввода!\r\nПовторите ввод");
                }
            }
            while (!correctInput);
            return value;
        }

        /// <summary>
        /// Считывает дату в формате дд.мм.гггг. Ввод повторяется
        /// до тех пор, пока не будет введена корректная дата.
        /// </summary>
        private static DateTime ReadDate(string prompt)
        {
            DateTime value = DateTime.Today;
            bool correctInput = false;
            do
            {
                Console.Write(prompt);
                string tmpStr = Console.ReadLine();
                correctInput = DateTime.TryParseExact(tmpStr, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out value);
                if (!correctInput)
                {
                    Console.WriteLine("Ошибка ввода!\r\nПовторите ввод");
                }
            }
            while (!correctInput);
            return value;
        }

        /// <summary>
        /// Ожидает нажатия клавиши "Ввод", чтобы пользователь успел
        /// прочитать результат действия.
        /// </summary>
        private static void Pause()
        {
            Console.Write("\r\nНажмите \"Ввод\" для продолжения");
            Console.ReadLine();
        }
    }
}
