using System;
using System.Collections.Generic;
using System.Linq;

// Власний делегат 
delegate double MathOperation(double x, double y);

class Order
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; }
}

class Program
{
    
    public static event Action<Order> OnNewOrder; // Action делегат

    static void Main()
    {
        Console.WriteLine("Приклади власного делегата");
        MathOperation add = (a, b) => a + b; // Лямбда-вираз
        MathOperation multiply = delegate (double a, double b) { return a * b; }; // Анонімний метод

        Console.WriteLine($"Add 5 + 3 = {add(5, 3)}");
        Console.WriteLine($"Multiply 5 * 3 = {multiply(5, 3)}");

        Console.WriteLine("\nПриклади вбудованих делегатів");
        // Func
        Func<int, int, int> funcAdd = (x, y) => x + y;
        Console.WriteLine($"Func Add: 10 + 20 = {funcAdd(10, 20)}");

        // Action
        Action<string> greet = name => Console.WriteLine($"Hello, {name}!");
        greet("Alice");

        // Predicate
        Predicate<int> isEven = n => n % 2 == 0;
        Console.WriteLine($"Is 7 even? {isEven(7)}");

        Console.WriteLine("\nКолекція замовлень і LINQ");
        List<Order> orders = new List<Order>
        {
            new Order { Id = 1, Amount = 100, Status = "Completed" },
            new Order { Id = 2, Amount = 50, Status = "Pending" },
            new Order { Id = 3, Amount = 200, Status = "Completed" },
            new Order { Id = 4, Amount = 30, Status = "Pending" },
            new Order { Id = 5, Amount = 150, Status = "Completed" }
        };

        // Підписка на подію (Action)
        OnNewOrder += order => Console.WriteLine($"[EVENT] Нове замовлення додано: Id={order.Id}, Amount={order.Amount}");

        // Виклик події
        var newOrder = new Order { Id = 6, Amount = 120, Status = "Pending" };
        OnNewOrder?.Invoke(newOrder);
        orders.Add(newOrder);

        double totalCompleted = orders
            .Where(o => o.Status == "Completed") // Лямбда-вираз
            .Aggregate(0.0, (sum, o) => sum + o.Amount);

        Console.WriteLine($"\nЗагальна сума виконаних замовлень: {totalCompleted}");

        int pendingCount = orders
            .Count(o => o.Status == "Pending"); // Лямбда-вираз

        Console.WriteLine($"Кількість замовлень Pending: {pendingCount}");

        var sortedOrders = orders.OrderBy(o => o.Amount).ToList(); // Лямбда-вираз
        Console.WriteLine("\nЗамовлення, відсортовані за сумою:");
        foreach (var o in sortedOrders)
            Console.WriteLine($"Id={o.Id}, Amount={o.Amount}, Status={o.Status}");

        Console.WriteLine("\n Використання Predicate для фільтрації");
        Predicate<Order> highValue = o => o.Amount > 100; // Predicate
        var highValueOrders = orders.FindAll(highValue);
        foreach (var o in highValueOrders)
            Console.WriteLine($"Найбільша сума замовлення: Id={o.Id}, Amount={o.Amount}");

        Console.WriteLine("\nВикористання Action для виконання дії над кожним елементом");
        Action<Order> printOrder = o => Console.WriteLine($"Order {o.Id}: {o.Amount}, Status={o.Status}");
        orders.ForEach(printOrder); // Action
    }
}