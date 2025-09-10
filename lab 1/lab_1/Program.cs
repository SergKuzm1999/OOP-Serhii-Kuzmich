class Program
{
    static void Main(string[] args)
    {
        Student st1 = new Student(3, "Kuzmych Serhii", new int[] { 5, 4, 3, 5, 4 });
        st1.PrintCard();
        Student st2 = new Student(1, "Denisevich Ivan", new int[] { 5, 5, 5, 5, 3 });
        st2.PrintCard();

        Console.ReadKey();
    }
}

class Student
{
    private int _id;
    private string _name;
    public double AverageMark { get; set; }

    public Student(int id, string name, int[] marks)
    {
        this._id = id;
        this._name = name;
        AverageMark = marks.Average();
    }
    public void PrintCard()
    {
        Console.WriteLine($"ID - {this._id}, Name - {this._name}, Average Mark - {AverageMark}");
    }
    ~Student()
    {
        Console.WriteLine($"Student {_name} is deleted");
    }
}


