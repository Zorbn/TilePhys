namespace TilePhys.Tiles;

public class Conveyor : Tile
{
    public Conveyor() : base(TileId.Conveyor, false, false, true, TileRotation.TwoDirections)
    {
        
    }

    public override void Tick(TileMap tileMap, int x, int y, TileData data, int tickCount)
    {
        base.Tick(tileMap, x, y, data, tickCount);
        Move(tileMap, x, y - 1, data.Direction == Direction.Right ? 1 : -1, 0);
    }
}