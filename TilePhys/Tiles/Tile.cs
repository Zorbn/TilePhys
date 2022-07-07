namespace TilePhys.Tiles;

public class Tile
{
    public readonly TileId Id;
    public readonly bool DoesSlide;
    public readonly bool DoesTick;
    public readonly bool IsStatic;
    public readonly TileRotation Rotation;
    
    public Tile(TileId id, bool doesSlide, bool isStatic, bool doesTick, TileRotation rotation)
    {
        Id = id;
        DoesSlide = doesSlide;
        IsStatic = isStatic;
        DoesTick = doesTick;
        Rotation = rotation;
    }

    public virtual void Tick(TileMap tileMap, int x, int y, TileData data, int tickCount)
    {
        if (IsStatic) throw new Exception($"Cannot update static tile at ({x}, {y})!");
        if (!DoesTick) throw new Exception($"Cannot tick non-ticking tile at ({x}, {y})!");
    }

    public void ApplyGravity(TileMap tileMap, int x, int y, int tickCount)
    {
        bool movedDown = Move(tileMap, x, y, 0, 1);

        if (!movedDown && DoesSlide)
        {
            Move(tileMap, x, y, tickCount % 2 * 2 - 1, 1);
        }
    }
    
    public bool Move(TileMap tileMap, int x, int y, int dirX, int dirY)
    {
        if (IsStatic) throw new Exception($"Cannot move static tile at ({x}, {y})!");

        if (tileMap.HasTileMoved(x, y)) return false;
        
        if (tileMap.GetTileId(x + dirX, y + dirY) == TileId.Air &&
            (dirX == 0 || tileMap.GetTileId(x + dirX, y) == TileId.Air))
        {
            TileId id = tileMap.GetTileId(x, y);
            TileData data = tileMap.GetTileData(x, y);
            tileMap.SetTile(TileId.Air, new TileData(), x, y);
            tileMap.SetTile(id, data, x + dirX, y + dirY);
            tileMap.MarkMovedTile(x + dirX, y + dirY);
            return true;
        }

        return false;
    }
}