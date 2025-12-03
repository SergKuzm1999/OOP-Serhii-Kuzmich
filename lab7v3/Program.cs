using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

class FileProcessor
{
    private int attempt = 0;

    public List<string> GetLines(string path)
    {
        attempt++;
        if (attempt <= 2) 
        {
            Console.WriteLine($"[FileProcessor] Attempt {attempt}: імітація IOException");
            throw new IOException("Помилка читання файлу.");
        }
        return new List<string> { "Line1", "Line2", "Line3" };
    }
}

// Клас для імітації мережевих запитів
class NetworkClient
{
    private int attempt = 0;

    public string GetApiResponse(string endpoint)
    {
        attempt++;
        if (attempt <= 4) 
        {
            Console.WriteLine($"[NetworkClient] Attempt {attempt}: імітація HttpRequestException");
            throw new HttpRequestException("Помилка мережі при запиті до API.");
        }

        return $"Response from {endpoint}";
    }
}

// Узагальнений хелпер для повторних спроб
public static class RetryHelper
{
    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool> shouldRetry = null)
    {
        if (initialDelay == default)
            initialDelay = TimeSpan.FromSeconds(1);

        int attempt = 0;
        while (true)
        {
            try
            {
                attempt++;
                Console.WriteLine($"[RetryHelper] Спроба {attempt}");
                return operation();
            }
            catch (Exception ex)
            {
                bool retry = shouldRetry?.Invoke(ex) ?? true;

                if (!retry || attempt >= retryCount)
                {
                    Console.WriteLine($"[RetryHelper] Остання невдача: {ex.Message}");
                    throw;
                }

                TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                Console.WriteLine($"[RetryHelper] Невдача: {ex.Message}, очікування {delay.TotalSeconds} сек перед повтором...");
                Thread.Sleep(delay);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var fileProcessor = new FileProcessor();
        var networkClient = new NetworkClient();

        // Делегат shouldRetry
        Func<Exception, bool> shouldRetry = ex => ex is IOException || ex is HttpRequestException;

        Console.WriteLine("Тест FileProcessor");
        try
        {
            var lines = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.GetLines("fakePath.txt"),
                retryCount: 5,
                initialDelay: TimeSpan.FromSeconds(1),
                shouldRetry: shouldRetry
            );

            Console.WriteLine("Файл успішно прочитано:");
            lines.ForEach(line => Console.WriteLine(line));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка FileProcessor: {ex.Message}");
        }

        Console.WriteLine("\nТест NetworkClient");
        try
        {
            var response = RetryHelper.ExecuteWithRetry(
                () => networkClient.GetApiResponse("https://google.com/"),
                retryCount: 6,
                initialDelay: TimeSpan.FromSeconds(1),
                shouldRetry: shouldRetry
            );

            Console.WriteLine($"API відповів: {response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка NetworkClient: {ex.Message}");
        }
    }
}