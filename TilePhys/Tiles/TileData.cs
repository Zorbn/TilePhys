namespace TilePhys.Tiles;

// TODO: Consider converting to structure of arrays.
public struct TileData
{
    public Direction Direction = Direction.Right;

    public TileData()
    {
    }

    public TileData(Direction direction)
    {
        Direction = direction;
    }
}