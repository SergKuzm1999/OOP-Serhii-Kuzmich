using System;
using System.Collections.Generic;
using System.Linq;
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var account = new Account("Сергій Кузьмич");

        try
        {
            // Поповнення рахунку
            var depositResult = account.Deposit(1000);
            Console.WriteLine(depositResult.Message);

            // Спроба зняти гроші
            var withdrawResult = account.Withdraw(100);
            Console.WriteLine(withdrawResult.Message);

            // Спроба перевищити баланс
            var failWithdraw = account.Withdraw(2000);
            Console.WriteLine(failWithdraw.Message);

            // Поточний баланс
            Console.WriteLine($"\nПоточний баланс: {account.Balance:C}");

            // Обчислення за місяць
            var monthly = account.GetMonthlyTotal(DateTime.Now.Month, DateTime.Now.Year);
            Console.WriteLine($"Підсумок за поточний місяць: {monthly:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        // Виведення історії транзакцій
        account.PrintTransactions();

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
        if (Type == "Withdraw")
            return $"{Date:g} | {Type} | - {Amount:0.00} ₴";
        if (Type == "Deposit")
            return $"{Date:g} | {Type} | + {Amount:0.00} ₴";
        else
            return $"{Date:g} | {Type} | {Amount:0.00} ₴";
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

