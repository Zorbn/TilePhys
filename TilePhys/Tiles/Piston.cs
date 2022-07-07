namespace TilePhys.Tiles;

public class Piston : Tile
{
    public Piston() : base(TileId.Conveyor, false, false, true, TileRotation.FourDirections)
    {
        
    }

    public override void Tick(TileMap tileMap, int x, int y, TileData data,  int tickCount)
    {
        base.Tick(tileMap, x, y, data, tickCount);

        int dirX = 0;
        int dirY = 0;
        
        switch (data.Direction)
        {
            case Direction.Right:
                dirX = 1;
                break;
            case Direction.Down:
                dirY = 1;
                break;
            case Direction.Left:
                dirX = -1;
                break;
            case Direction.Up:
                dirY = -1;
                break;
        }
        
        Move(tileMap, x + dirX, y + dirY, dirX, dirY);
    }
}