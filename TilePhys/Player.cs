using System.Numerics;
using Raylib_cs;
using TilePhys.Tiles;

namespace TilePhys;

public class Player
{
    private const float Speed = 10;
    private const float Gravity = 10;
    
    private float x, y;
    private readonly int width, height;
    private int selectedTile;
    private Direction direction;
    // Never make oldPos the same as currentPos, it will cause collision detection to hang
    // because the algorithm won't be able to move the player towards the oldPos
    private Vector2 oldPos;

    public Player(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        direction = Direction.Right;
    }

    public void Update(TileMap tileMap, Vector2 mouseTilePos, float frameTime)
    {
        EditTileMap(tileMap, mouseTilePos);

        // TODO: Make a class for this and oldPos, which can be used for any entity
        // make sure that class prevents oldPos from being the same as current pos
        void MoveAndCollide(float moveX, float moveY)
        {
            if (moveX != 0 && moveY != 0)
                throw new ArgumentException("Move and collide should be used in only one direction at once!");
                    
            if (moveX != 0 || moveY != 0) oldPos = new(x, y);
            x += moveX;
            y += moveY;
            CollideWithTiles(tileMap);
        }
        
        float moveX = 0;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) moveX--;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) moveX++;
        
        moveX *= frameTime * Speed;
        MoveAndCollide(moveX, 0);
        
        float moveY = frameTime * Gravity;
        MoveAndCollide(0, moveY);
    }

    public void Draw(TextureAtlas atlas)
    {
        atlas.Draw((int)x, (int)y, 0, 1, 1, 1);
        atlas.Draw(0, 0, selectedTile, 0, 1, 1);
    }

    private void EditTileMap(TileMap tileMap, Vector2 mouseTilePos)
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            selectedTile++;
            selectedTile %= 6;
        }
        
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            int mouseX = (int)mouseTilePos.X;
            int mouseY = (int)mouseTilePos.Y;
            
            tileMap.PlaceTile((TileId)selectedTile, mouseX, mouseY, (int)x, (int)y);
        }
    }

    private void CollideWithTiles(TileMap tileMap)
    {
        Vector2 offset = Vector2.Zero;
        Vector2 dirToOldPos = oldPos - new Vector2(x, y);
        if (dirToOldPos.Length() != 0) dirToOldPos /= dirToOldPos.Length();

        int tileW = width / tileMap.TileSize;
        int tileH = height / tileMap.TileSize;
        
        for (int ix = 0; ix <= tileW; ix++)
        {
            for (int iy = 0; iy <= tileH; iy++)
            {
                offset.X = ix * (width / (float)tileW);
                offset.Y = iy * (height / (float)tileH);

                if (ix == tileW) offset.X--;
                if (iy == tileH) offset.Y--;

                while (true)
                {
                    int tileX = (int)((x + offset.X) / tileMap.TileSize);
                    int tileY = (int)((y + offset.Y) / tileMap.TileSize);

                    if (tileMap.GetTileId(tileX, tileY) is TileId.Air or TileId.Bedrock) break;

                    // A collision has been encountered! Move back towards the character's previous position.
                    x += dirToOldPos.X;
                    y += dirToOldPos.Y;
                }
            }
        }
    }

    private Vector2 GetMaxMovement(TileMap tileMap, Vector2 movement, Vector2 offset)
    {
        if (movement.X != 0 && movement.Y != 0)
            throw new ArgumentException("Max movement should only be used on one direction at once!");

        float xWithOffset = x + offset.X + (movement.X > 0 ? width : 0);
        float yWithOffset = y + offset.Y + (movement.Y > 0 ? height : 0);

        int tileX = (int)((xWithOffset + movement.X) / tileMap.TileSize);
        int tileY = (int)((yWithOffset + movement.Y) / tileMap.TileSize);
        
        if (tileMap.GetTileId(tileX, tileY) == TileId.Air) return movement;
        
        float dstX = tileX * tileMap.TileSize + (movement.X < 0 ? tileMap.TileSize : 0);
        float dstY = tileY * tileMap.TileSize + (movement.Y < 0 ? tileMap.TileSize : 0);
        movement.X = MathF.Ceiling(dstX - xWithOffset);
        movement.Y = MathF.Ceiling(dstY - yWithOffset);
        
        return movement;
    }
}