using TilePhys.Tiles;

namespace TilePhys;

public class TileMap
{
    public readonly Dictionary<TileId, Tile> Tiles = new()
    {
        { TileId.Air, new Tile(TileId.Air, false, true, false, TileRotation.None) },
        { TileId.Dirt, new Tile(TileId.Dirt, false, false, false, TileRotation.None) },
        { TileId.Sand, new Tile(TileId.Sand, true, false, false, TileRotation.None) },
        { TileId.Bedrock, new Tile(TileId.Air, false, true, false, TileRotation.None) },
        { TileId.Conveyor, new Conveyor() },
        { TileId.Piston, new Piston() }
    };

    public readonly int TileSize;
    
    private readonly TileId[] tiles;
    private readonly TileData[] tileDataSets;
    private readonly int width, height;
    private readonly HashSet<Point> movedTiles;
    private int tickCount;

    public TileMap(int width, int height, int tileSize)
    {
        this.width = width;
        this.height = height;
        TileSize = tileSize;
        
        tiles = new TileId[width * height];
        tileDataSets = new TileData[width * height];
        movedTiles = new HashSet<Point>();
    }

    public void Generate()
    {
        int groundLevel = height / 2;
        int sandX = width / 2;

        TileData defaultData = new();
        
        for (int i = 0; i < width * height; i++)
        {
            int x = i % width;
            int y = i / height;
            
            if (y > groundLevel)
            {
                TileId tileId = TileId.Dirt;

                if (x >= sandX) tileId = TileId.Sand;

                SetTile(tileId, defaultData, x, y);
            }
        }
    }

    public void Draw(TextureAtlas atlas)
    {
        for (int i = 0; i < width * height; i++)
        {
            int x = i % width;
            int y = i / height;
            TileId tileId = GetTileId(x, y);

            if (tileId == TileId.Air) continue;

            int tileTexX = (int)tileId;
            
            atlas.Draw(x * TileSize, y * TileSize, tileTexX, 0, 1, 1, false, tileDataSets[i].Direction);
        }
    }

    public void Tick()
    {
        tickCount++;
        
        for (int i = width * height - 1; i >= 0; i--)
        {
            int x = i % width;
            int y = i / height;
            TileId tileId = GetTileId(x, y);
            TileData tileData = GetTileData(x, y);

            if (tileId == TileId.Air) continue;
            if (!Tiles[tileId].DoesTick) continue;
            
            Tiles[tileId].Tick(this, x, y, tileData, tickCount);
        }
        
        for (int i = width * height - 1; i >= 0; i--)
        {
            int x = i % width;
            int y = i / height;
            TileId tileId = GetTileId(x, y);

            if (tileId == TileId.Air) continue;
            
            Tiles[tileId].ApplyGravity(this, x, y, tickCount);
        }
        
        movedTiles.Clear();
    }

    public void PlaceTile(TileId tileId, int x, int y, int fromX, int fromY)
    {
        if (tileId != TileId.Air && GetTileId(x, y) != TileId.Air) return;
        
        TileData tileData = new();
        
        do
        {
            if (Tiles[tileId].Rotation == TileRotation.None) break;
            
            if (fromX < x * TileSize) tileData.Direction = Direction.Left;
            if (Tiles[tileId].Rotation == TileRotation.TwoDirections) break;
            
            if (fromY < y * TileSize) tileData.Direction = Direction.Up;
            if (fromY > y * TileSize) tileData.Direction = Direction.Down;
        } while (false);
        
        SetTile(tileId, tileData, x, y);
    }

    public void SetTile(TileId tileId, TileData tileData, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;

        int i = x + y * width;
        tiles[i] = tileId;
        tileDataSets[i] = tileData;
    }

    public TileId GetTileId(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return TileId.Bedrock;

        return tiles[x + y * width];
    }
    
    public TileData GetTileData(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return new TileData();

        return tileDataSets[x + y * width];
    }

    public void MarkMovedTile(int x, int y)
    {
        Point movedTile = new(x, y);
        if (!movedTiles.Contains(movedTile)) movedTiles.Add(movedTile);
    }

    public bool HasTileMoved(int x, int y)
    {
        return movedTiles.Contains(new Point(x, y));
    }
}