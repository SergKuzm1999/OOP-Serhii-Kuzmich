class Program
{
    static void Main(string[] args)
    {
        List<Animal> animals = new List<Animal>();
        animals.Add(new Dog("Buddy", 25.0, 1.6));
        animals.Add(new Cat("Whiskers", 5.25, 1.2));
        animals.Add(new Dog("Max", 30.0, 2.0));
        animals.Add(new Cat("Luna", 4.5, 1.4));
        
        foreach (Animal animal in animals)
        {
            Console.WriteLine($"{animal.Name} says: {animal.Speak()}");
            Console.WriteLine($"{animal.Name} eat per day : {animal.Eat():F0} calories");
            Console.WriteLine();
        }
        for (int i = 0; i < animals.Count; i++)
        {
            for (int j = i + 1; j < animals.Count; j++)
            {
                CompareFood(animals[i], animals[j]);
            }
        }
    }
    static void CompareFood(Animal a1, Animal a2)//порівняння потреби в їжі
    {
        double eat1 = a1.Eat();
        double eat2 = a2.Eat();
        if (eat1 > eat2)
            Console.WriteLine($"{a1.Name} needs more food than {a2.Name}");
        else if (eat1 < eat2)
            Console.WriteLine($"{a2.Name} needs more food than {a1.Name}");
        else
            Console.WriteLine($"{a1.Name} and {a2.Name} need the same amount of food");
    }
    abstract class Animal
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public double CoefficientActivity { get; set; } // коефіцієнт активності
        public Animal(string name, double weight, double coefficient_activity)
        {
            Name = name;
            Weight = weight;
            CoefficientActivity = coefficient_activity;
        }
        public abstract string Speak();
        public virtual double Eat()
        {
            double RER = 70 * Math.Pow(Weight, 0.75);  // базова потреба
            double MER = RER * CoefficientActivity;   // добова норма з активністю
            return MER;
        }
        ~Animal()
        {
            Console.WriteLine("Destroy Animal");
        }
    }
    class Dog : Animal
    {
        public Dog(string name, double weight, double coefficient_activity) : base(name, weight, coefficient_activity)
        {}
        public override string Speak()
        {
            return "Woof!";
        }
    }
    class Cat : Animal
    {
        public Cat(string name, double weight, double coefficient_activity) : base(name, weight, coefficient_activity)
        {}
        public override string Speak()
        {
            return "Meow!";
        }
    }
}
