using System.Numerics;

namespace TilePhys;

public static class DirectionUtils
{
    public static readonly List<Direction> Directions = new()
    {
        Direction.Right,
        Direction.Down,
        Direction.Left,
        Direction.Up
    };
    
    public static Vector2 DirectionToVector(Direction direction)
    {
        return direction switch
        {
            Direction.Right => new Vector2(1, 0),
            Direction.Down => new Vector2(0, -1),
            Direction.Left => new Vector2(-1, 0),
            Direction.Up => new Vector2(0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}