class Program
{
    static void Main(string[] args)
    {
        string text = "This is a bad text with some bad words and spaces.";
        int totalChanges = 0;

        // Створюємо фільтри
        RemoveSpacesFilter removeSpaces = new RemoveSpacesFilter();
        CensorWordsFilter censorWords = new CensorWordsFilter(new string[] { "bad"});

        // Створюємо об'єкт Text і додаємо фільтри
        Text txt = new Text(removeSpaces, censorWords);

        // Обробляємо текст
        string filteredText = txt.Process(text);
        totalChanges = removeSpaces.ChangesCount + censorWords.ChangesCount;
        
        Console.WriteLine($"Початковий текст: {text}");
        Console.WriteLine("Результат:");
        Console.WriteLine(filteredText);
        Console.WriteLine("Кількість змін" + totalChanges);
    }
}
// Базовий інтерфейс
interface ITextFilter
{
    string ActionByText(string text);
    int ChangesCount { get; set; }
}
// Базовий клас для фільтрів
abstract class TextFilterBase : ITextFilter
{
    protected int changesCount = 0;
    public int ChangesCount { get; set; }

    public abstract string ActionByText(string text);
}
// Фільтр для видалення пробілів
class RemoveSpacesFilter : TextFilterBase
{
    public override string ActionByText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
                ChangesCount++;
        }
        return text.Replace(" ", "");
    }
}
// Фільтр для заміни заборонених слів
class CensorWordsFilter : TextFilterBase
{
    private string[] _bannedWords;
    public CensorWordsFilter(string[] bannedWords)
    {
        _bannedWords = bannedWords;
    }
    public override string ActionByText(string text)
    {
        string result = text;
        foreach (var word in _bannedWords)
        {
            // Заміна слова на зірочки
            result = result.Replace(word, new string('*', word.Length));
            ChangesCount += word.Length;
        }
        return result;
    }
}
// Клас для обробки тексту з використанням фільтрів
class Text
{
    private RemoveSpacesFilter _removeSpacesFilter;
    private CensorWordsFilter _censorWordsFilter;
    public Text(RemoveSpacesFilter removeSpaces, CensorWordsFilter censorWords)
    {
        _removeSpacesFilter = removeSpaces;
        _censorWordsFilter = censorWords;
    }
    public string Process(string text)
    {
        // Застосовуємо спочатку заміну заборонених слів
        text = _censorWordsFilter.ActionByText(text);
        // Потім видалення пробілів
        text = _removeSpacesFilter.ActionByText(text);
        return text;
    }
}