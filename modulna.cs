using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryServiceApp
{
    public class Order
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }    // у кг
        public double Distance { get; set; }  // у км

        public Order(string number, string description, double weight, double distance)
        {
            Number = number;
            Description = description;
            Weight = weight;
            Distance = distance;
        }

        public override string ToString() => 
            $"Замовлення №{Number}: \"{Description}\" (Вага: {Weight} кг, Відстань: {Distance} км)";
    }
    public interface IPayable
    {
        void Pay(string paymentMethod, decimal amount);
    }
    public abstract class Transport : IPayable
    {
        public string Id { get; set; }
        public bool IsBusy { get; set; } = false;
        public double MaxWeight { get; set; } // Максимальне обмеження ваги

        protected Transport(string id, double maxWeight)
        {
            Id = id;
            MaxWeight = maxWeight;
        }
        public abstract decimal CalculateCost(Order order);
        public virtual bool CanTake(Order order)
        {
            return !IsBusy && order.Weight <= MaxWeight;
        }

        public virtual void Pay(string paymentMethod, decimal amount)
        {
            Console.WriteLine($"[Оплата] Транспорт {Id} прийняв оплату ({paymentMethod}): {amount} грн.");
        }

        public override string ToString() => 
            $"{GetType().Name} [{Id}] (Макс. вага: {MaxWeight} кг, Статус: {(IsBusy ? "Зайнятий" : "Вільний")})";
    }

    public class Bike : Transport
    {
        public Bike(string id) : base(id, 15.0) 
        {
        }

        public override decimal CalculateCost(Order order)
        {
            return 20m + 5m * (decimal)order.Distance;
        }

        public override bool CanTake(Order order)
        {
            return base.CanTake(order) && order.Distance <= 10.0;
        }
    }

    public class Car : Transport
    {
        public Car(string id) : base(id, 500.0) 
        {
        }

        public override decimal CalculateCost(Order order)
        {
            return 50m + 8m * (decimal)order.Distance + 1m * (decimal)order.Weight;
        }
    }

    // 3. Вантажівка
    public class Truck : Transport
    {
        public Truck(string id) : base(id, 5000.0) 
        {
        }

        public override decimal CalculateCost(Order order)
        {
            return 200m + 15m * (decimal)order.Distance + 0.5m * (decimal)order.Weight;
        }
    }

    public class DeliveryHistoryRecord
    {
        public Order Order { get; set; }
        public Transport Transport { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime DeliveryTime { get; set; }

        public override string ToString() => 
            $"[Історія] {Order.Number} доставлено транспортом {Transport.Id} | Вартість: {TotalCost} грн | Оплата: {PaymentMethod} | Час: {DeliveryTime:HH:mm:ss}";
    }
    public class DeliveryService
    {
        private List<Transport> _transports = new List<Transport>();
        private List<Order> _activeOrders = new List<Order>();
        private List<DeliveryHistoryRecord> _history = new List<DeliveryHistoryRecord>();
        private Dictionary<Order, Transport> _assignedTransports = new Dictionary<Order, Transport>();
        public void AddTransport(Transport transport)
        {
            _transports.Add(transport);
        }

        public void AddOrder(Order order)
        {
            _activeOrders.Add(order);
            Console.WriteLine($"[Створено замовлення] {order}");
        }

        public IReadOnlyList<Transport> Transports => _transports.AsReadOnly();
        public IReadOnlyList<Order> ActiveOrders => _activeOrders.AsReadOnly();
        public void ShowAvailableCostsForOrder(Order order)
        {
            Console.WriteLine($"\n Розрахунок вартості для замовлення {order.Number}");
            foreach (var transport in _transports)
            {
                if (transport.CanTake(order))
                {
                    decimal cost = transport.CalculateCost(order);
                    Console.WriteLine($"  ✓ {transport.GetType().Name} ({transport.Id}): {cost} грн (Доступний)");
                }
                else
                {
                    string reason = transport.IsBusy ? "Зайнятий" : "Не відповідає обмеженням (вага/відстань > 10км)";
                    Console.WriteLine($"  ✗ {transport.GetType().Name} ({transport.Id}): Не може виконати ({reason})");
                }
            }
        }

        public bool AssignOrderToTransport(Order order, Transport transport)
        {
            if (!_activeOrders.Contains(order))
            {
                Console.WriteLine($"[Помилка] Замовлення {order.Number} не знайдено в активних.");
                return false;
            }

            if (transport.CanTake(order))
            {
                transport.IsBusy = true;
                _assignedTransports[order] = transport;
                Console.WriteLine($"[Призначення] Замовлення {order.Number} закріплено за транспортом {transport.GetType().Name} [{transport.Id}]. Транспорт тепер зайнятий.");
                return true;
            }
            else
            {
                Console.WriteLine($"[Помилка] Транспорт {transport.Id} не може взяти замовлення {order.Number}.");
                return false;
            }
        }
        public bool DeliverOrder(Order order, string paymentMethod)
        {
            if (!_activeOrders.Contains(order) || !_assignedTransports.ContainsKey(order))
            {
                Console.WriteLine($"[Помилка] Замовлення {order.Number} не має призначеного транспорту.");
                return false;
            }

            Transport transport = _assignedTransports[order];
            decimal cost = transport.CalculateCost(order);

            transport.Pay(paymentMethod, cost);
            transport.IsBusy = false;
            _activeOrders.Remove(order);
            _assignedTransports.Remove(order);

            _history.Add(new DeliveryHistoryRecord
            {
                Order = order,
                Transport = transport,
                TotalCost = cost,
                PaymentMethod = paymentMethod,
                DeliveryTime = DateTime.Now
            });

            Console.WriteLine($"[Доставка завершена] Замовлення {order.Number} успішно доставлено. Транспорт звільнено.\n");
            return true;
        }
        public void ShowStatus()
        {
            Console.WriteLine("\n СТАТУС СЛУЖБИ ДОСТАВКИ ");
            
            Console.WriteLine("Парк транспорту:");
            foreach (var t in _transports)
            {
                Console.WriteLine($"  • {t}");
            }

            Console.WriteLine($"\nАктивні замовлення ({_activeOrders.Count}):");
            if (_activeOrders.Count == 0)
            {
                Console.WriteLine("  (немає активних замовлень)");
            }
            else
            {
                foreach (var o in _activeOrders)
                {
                    string assigned = _assignedTransports.ContainsKey(o) 
                        ? $" [Виконує: {_assignedTransports[o].GetType().Name} {_assignedTransports[o].Id}]" 
                        : " [Очікує призначення]";
                    Console.WriteLine($"  • {o}{assigned}");
                }
            }

            Console.WriteLine($"\nІсторія доставок ({_history.Count}):");
            if (_history.Count == 0)
            {
                Console.WriteLine("  (історія порожня)");
            }
            else
            {
                foreach (var h in _history)
                {
                    Console.WriteLine($"  • {h}");
                }
            }
            Console.WriteLine("=\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("СИМУЛЯЦІЯ РОБОТИ СЛУЖБИ ДОСТАВКИ\n");

            DeliveryService service = new DeliveryService();
            service.AddTransport(new Bike("Bike-01"));
            service.AddTransport(new Car("Car-01"));
            service.AddTransport(new Truck("Truck-01"));

            Order order1 = new Order("№101", "Документи", 1.5, 4.0);        
            Order order2 = new Order("№102", "Коробка з взуттям", 8.0, 12.0); 
            Order order3 = new Order("№103", "Холодильник", 85.0, 30.0);      
            Order order4 = new Order("№104", "Квіти", 2.0, 5.0);              

            Console.WriteLine();
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            service.AddOrder(order4);
            service.ShowStatus();

            service.ShowAvailableCostsForOrder(order1);
            service.ShowAvailableCostsForOrder(order2);
            service.ShowAvailableCostsForOrder(order3);
            Console.WriteLine("\n Тестування призначення та доставки ");
            var bike = service.Transports.OfType<Bike>().First();
            service.AssignOrderToTransport(order1, bike);
            var anotherBikeCandidate = service.Transports.OfType<Bike>().First();
            service.AssignOrderToTransport(order4, anotherBikeCandidate);
            var car = service.Transports.OfType<Car>().First();
            service.AssignOrderToTransport(order2, car);
            var truck = service.Transports.OfType<Truck>().First();
            service.AssignOrderToTransport(order3, truck);
            service.ShowStatus();
            service.DeliverOrder(order1, "Картка");
            service.DeliverOrder(order2, "Готівка");
            service.AssignOrderToTransport(order4, bike);
            service.DeliverOrder(order4, "Картка");
            service.DeliverOrder(order3, "Безготівковий розрахунок");
            service.ShowStatus();
        }
    }
}