using System;
using System.Collections.Generic;

public static class Program
{
    static void Main()
    {
        var cache = new Cache<User>(TimeSpan.FromSeconds(10), maxSize: 3);

        cache.Set("u1", new User { Name = "Alex", Age = 25 });
        cache.Set("u2", new User { Name = "Max", Age = 19 });
        cache.Set("u3", new User { Name = "Ira", Age = 30 });
        cache.Set("u4", new User { Name = "Herold", Age = 41 });

        Console.WriteLine("Чекаємо 10 секунд..");
        Thread.Sleep(10000);

        cache.Set("u5", new User { Name = "Oleh", Age = 11 });
        cache.Set("u6", new User { Name = "Ihor", Age = 21 });

        var sorted = cache.GetSorted(u => u.Age);

        Console.WriteLine("Не застарілий КЕШ : ");
        foreach (var u in sorted)
            Console.WriteLine($"{u.Key} {u.Value.Name} ({u.Value.Age})");

        Console.ReadKey();
    }
}

public class CacheItem<T>
{
    public T Value { get; set; }
    public DateTime Created { get; set; }

    public CacheItem(T value)
    {
        Value = value;
        Created = DateTime.UtcNow;
    }
}

public class Cache<T> where T : class, new()
{
    private readonly Dictionary<string, CacheItem<T>> _storage = new();
    private readonly object _lock = new();

    public TimeSpan Ttl { get; }
    public int MaxSize { get; }

    public Cache(TimeSpan ttl, int maxSize)
    {
        Ttl = ttl;
        MaxSize = maxSize;
    }

    public void Set(string key, T value)
    {
        lock (_lock)
        {
            RemoveExpired();

            if (_storage.Count >= MaxSize)
                RemoveOldest();

            _storage[key] = new CacheItem<T>(value);
            Console.WriteLine($"[SET] Додаю ключ '{key}', {GetKeyValues(value)}");
        }
    }

    public T? Get(string key)
    {
        lock (_lock)
        {
            if (!_storage.TryGetValue(key, out var item))
                return null;

            if (DateTime.UtcNow - item.Created > Ttl)
            {
                _storage.Remove(key);
                return null;
            }

            return item.Value;
        }
    }
    private string GetKeyValues(T value)
    {
        var prop_name = typeof(T).GetProperty("Name");
        var prop_age = typeof(T).GetProperty("Age");

        string name = prop_name?.GetValue(value)?.ToString() ?? "null";
        string age = prop_age?.GetValue(value)?.ToString() ?? "null";

        return $"Name = {name}, Age = {age}";
    }
    private void RemoveExpired()
    {
        var toRemove = new List<string>();

        foreach (var kv in _storage)
        {
            if (DateTime.UtcNow - kv.Value.Created > Ttl)
            {
                Console.WriteLine($"[DELETED] Видаляю прострочений ключ: {kv.Key}");
                toRemove.Add(kv.Key);
            }
        }

        foreach (var key in toRemove)
            _storage.Remove(key);
    }
    private void RemoveOldest()
    {
        string? oldestKey = null;
        DateTime oldestTime = DateTime.MaxValue;

        foreach (var kv in _storage)
        {
            if (kv.Value.Created < oldestTime)
            {
                oldestTime = kv.Value.Created;
                oldestKey = kv.Key;
            }
        }

        if (oldestKey != null)
        {
            Console.WriteLine($"[ODLKEY] Кеш переповнений — видаляю найстаріший ключ: {oldestKey}");
            _storage.Remove(oldestKey);
        }
    }

    public List<KeyValuePair<string, T>> GetSorted(Func<T, IComparable> selector)
    {
        lock (_lock)
        {
            var list = new List<KeyValuePair<string, T>>();

            foreach (var kv in _storage)
                list.Add(new KeyValuePair<string, T>(kv.Key, kv.Value.Value));

            for (int i = 1; i < list.Count; i++)
            {
                var current = list[i];
                var keyValue = selector(current.Value);
                int j = i - 1;
                while (j >= 0 && selector(list[j].Value).CompareTo(keyValue) > 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = current;
            }
        
            return list;
        }
    }
}

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public User() { }
    }