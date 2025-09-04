class Program
{
    static void Main(string[] args)
    {
        Circle circle = new Circle("MyCircle", 5);
        Console.WriteLine(circle.GetArea());
        Console.ReadKey();
    }
}
abstract class Figure
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public Figure(string name)
    {
        this._name = name;
    }
    ~Figure()
    {
        Console.WriteLine("Destructor called");
    }
    public virtual double GetArea()
    {
        return 0;
    }
}
class Circle : Figure
{
    private double _radius;
    public Circle(string name, double radius) : base(name)
    {
        this._radius = radius;
    }
    public override double GetArea()
    {
        return Math.PI * _radius * _radius;
    }
}
