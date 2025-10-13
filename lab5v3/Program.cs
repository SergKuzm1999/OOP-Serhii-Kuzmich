using System;
using System.Collections.Generic;
using System.Linq;
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var accounts = new Repository<Account>();
        var acc1 = new Account("Сергій Кузьмич");
        try
        {
            accounts.Add(acc1);

            Console.WriteLine($"\nБаланс {acc1.Owner}: {acc1.Balance:C}");
            // Поповнення рахунку
            var depositResult = acc1.Deposit(1500);;
            Console.WriteLine(depositResult.Message);

            // Спроба зняти гроші
            var withdrawResult = acc1.Withdraw(500);
            Console.WriteLine(withdrawResult.Message);

            // Спроба перевищити баланс
            var failWithdraw = acc1.Withdraw(2000);
            Console.WriteLine(failWithdraw.Message);

            // Поточний баланс
            Console.WriteLine($"\nПоточний баланс: {acc1.Balance:C}");

            // Обчислення за місяць
            var monthly = acc1.GetMonthlyTotal(DateTime.Now.Month, DateTime.Now.Year);
            Console.WriteLine($"Підсумок за поточний місяць: {monthly:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        // Виведення історії транзакцій
        acc1.PrintTransactions();

        Console.WriteLine("\nРоботу завершено.");
        Console.ReadLine();
    }
}
public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}
public class InvalidAmountException : Exception
{
    public InvalidAmountException(string message) : base(message) { }
}
public class Result<T>
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public T? Data { get; }

    private Result(bool success, string message, T? data)
    {
        IsSuccess = success;
        Message = message;
        Data = data;
    }

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>(true, message, data);
    }
    public static Result<T> Failure(string message)
    {
        return new Result<T>(false, message, default);
    }
        
}
public class Transaction
{
    public DateTime Date { get; }
    public decimal Amount { get; }
    public string Type { get; } // Deposit, Withdraw, Transfer

    public Transaction(decimal amount, string type)
    {
        if (amount <= 0)
            throw new InvalidAmountException("Сума має бути більшою за 0.");

        Date = DateTime.Now;
        Amount = amount;
        Type = type;
    }

    public override string ToString()
    {
        string sign = Type == "Withdraw" ? "-" : Type == "Deposit" ? "+" : "";
        return $"{Date:g} | {Type,-10} | {sign} {Amount:0.00} ₴";
    }
}
public class Account
{
    public string Owner { get; }
    public decimal Balance { get; private set; } 
    private List<Transaction> Transactions { get; }

    public Account(string owner)
    {
        Owner = owner;
        Transactions = new List<Transaction>();
        Balance = 0m;
    }

    public Result<decimal> Deposit(decimal amount)
    {
        try
        {
            if (amount <= 0)
                throw new InvalidAmountException("Сума поповнення має бути більшою за 0.");

            Transactions.Add(new Transaction(amount, "Deposit"));
            Balance += amount;
            return Result<decimal>.Success(Balance, $"Поповнено на {amount:C}");
        }
        catch (InvalidAmountException ex)
        {
            return Result<decimal>.Failure(ex.Message);
        }
    }

    public Result<decimal> Withdraw(decimal amount)
    {
        try
        {
            if (amount <= 0)
                throw new InvalidAmountException("Сума зняття має бути більшою за 0.");

            if (amount > Balance)
                throw new InsufficientFundsException("Недостатньо коштів на рахунку.");

            Transactions.Add(new Transaction(amount, "Withdraw"));
            Balance -= amount;
            return Result<decimal>.Success(Balance, $"Знято {amount:C}");
        }
        catch (Exception ex) when (ex is InsufficientFundsException or InvalidAmountException)
        {
            return Result<decimal>.Failure(ex.Message);
        }
    }

    public decimal GetMonthlyTotal(int month, int year)
    {
        return Transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .Sum(t => t.Type == "Deposit" ? t.Amount : -t.Amount);
    }

    public void PrintTransactions()
    {
        Console.WriteLine($"\nТранзакції користувача {Owner}:");
        foreach (var t in Transactions)
            Console.WriteLine(t);
    }
}
public interface IRepository<T>
{
    void Add(T item);
    bool Remove(T item);
    T? Find(Func<T, bool> predicate);
    IEnumerable<T> All();
    IEnumerable<T> Where(Func<T, bool> predicate);
}
public class Repository<T> : IRepository<T>
{
    private readonly List<T> _items = new();
    public void Add(T item)
    {
        _items.Add(item);
    }
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }
    public T? Find(Func<T, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }
    public IEnumerable<T> All()
    {
        return _items;
    }
    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
        return _items.Where(predicate);
    }
}
