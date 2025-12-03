using System;
using System.Net.Http;
using System.Threading;
using Polly;

class Program
{
    static void Main()
    {
        Console.WriteLine("Сценарій 1: Виклик зовнішнього API");
        ApiScenario();

        Console.WriteLine("\nСценарій 2: Доступ до бази даних з Circuit Breaker");
        DatabaseScenario();
    }

    //Сценарій 1
    static void ApiScenario()
    {
        int apiAttempt = 0;

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Retry(3, (exception, retryCount) =>
            {
                Console.WriteLine($"[API Retry] Спроба {retryCount}: {exception.Message}");
            });

        try
        {
            retryPolicy.Execute(() =>
            {
                apiAttempt++;
                if (apiAttempt <= 2) 
                    throw new HttpRequestException("API тимчасово недоступний.");
                Console.WriteLine("API виклик успішний!");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API виклик завершився помилкою: {ex.Message}");
        }
    }

    //Сценарій 2 
    static void DatabaseScenario()
    {
        var dbClient = new DatabaseClient();

        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(2, (ex, retryCount) => Console.WriteLine($"[DB Retry] Спроба {retryCount}: {ex.Message}"));

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(5),
                onBreak: (ex, ts) => Console.WriteLine($"[Circuit Breaker] Відкрито через: {ex.Message}"),
                onReset: () => Console.WriteLine("[Circuit Breaker] Замкнуто."));

        try
        {
            circuitBreakerPolicy.Execute(() =>
            {
                retryPolicy.Execute(() =>
                {
                    dbClient.QueryData();
                });
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка доступу до БД: {ex.Message}");
        }
    }
}

// Імітація бази даних
class DatabaseClient
{
    private int attempt = 0;

    public void QueryData()
    {
        attempt++;
        if (attempt <= 3)
            throw new Exception("База даних тимчасово недоступна.");
        Console.WriteLine("Дані успішно отримано!");
    }
}