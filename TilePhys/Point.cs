namespace TilePhys;

public struct Point : IEquatable<Point>
{
    public int X, Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}