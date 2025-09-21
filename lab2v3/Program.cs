class Program
{
    static void Main(string[] args)
    {
        Vector3D vect1 = new Vector3D(7, 2, 3);
        Vector3D vect2 = new Vector3D(3, 4, 5);
        Console.WriteLine($"Vector 1 : {vect1.X}, {vect1.Y}, {vect1.Z}, Length : {vect1.Length}");
        Console.WriteLine($"Vector 2 : {vect2.X}, {vect2.Y}, {vect2.Z}, Length : {vect2.Length}");
        Console.WriteLine();

        Vector3D vect3 = vect1 + vect2;
        Vector3D vect4 = vect1 - vect2;
        Console.WriteLine();
        Console.WriteLine($"Vector 1 + Vector 2 = {vect3.X} {vect3.Y} {vect3.Z}" );
        Console.WriteLine($"Vector 1 - Vector 2 = {vect4.X} {vect4.Y} {vect4.Z}" );
        Console.WriteLine();
        Console.WriteLine($"Vector 1 == Vector 2 : {vect1 == vect2}");
        Console.WriteLine($"Vector 1 != Vector 2 : {vect1 != vect2}");

        Console.WriteLine();
        Console.WriteLine($"Index Vector 1 [0] : {vect1[0]}");
        Console.ReadKey();
    }
}

class Vector3D
{
    private double _x, _y, _z;

    public double X
    {
        get { return _x; }
        set
        {
            _x = value;
        }
    }
    public double Y
    {
        get { return _y; }
        set
        {
            _y = value;
        }
    }
    public double Z
    {
        get { return _z; }
        set
        {
            _z = value;
        }
    }
    public double Length
    {
        get
        {
            return Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }
    }
    public Vector3D(double x, double y, double z)
    {
        this._x = x;
        this._y = y;
        this._z = z;
        ValidateLength();
    }
    private void ValidateLength()
    {
        if (Length > 100)
            throw new ArgumentOutOfRangeException("Length", "Довжина вектора не може перевищувати 100");
    }
    public static Vector3D operator +(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    public static Vector3D operator -(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    public static bool operator ==(Vector3D a, Vector3D b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }
    public static bool operator !=(Vector3D a, Vector3D b)
    {
        return !(a == b);
    }
    public double this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return _x;
                case 1:
                    return _y;
                case 2:
                    return _z;
                default:
                    throw new IndexOutOfRangeException("Індекс має бути 0, 1 або 2");
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    _x = value;
                    break;
                case 1:
                    _y = value;
                    break;
                case 2:
                    _z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Індекс має бути 0, 1 або 2");
            }
        }
    }
}
