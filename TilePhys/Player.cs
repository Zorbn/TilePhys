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

        float moveX = 0;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) moveX--;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) moveX++;

        direction = moveX switch
        {
            > 0 => Direction.Right,
            < 0 => Direction.Left,
            _ => direction
        };

        moveX *= frameTime * Speed;
        moveX = GetMaxMovementOfRect(tileMap, new Vector2(moveX, 0)).X;
        x += moveX;
        
        float moveY = frameTime * Gravity;
        moveY = GetMaxMovementOfRect(tileMap, new Vector2(0, moveY)).Y;
        y += moveY;
    }

    public void Draw(TextureAtlas atlas)
    {
        atlas.Draw((int)x, (int)y, 0, 1, 1, 1, direction == Direction.Left);
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

    private Vector2 GetMaxMovementOfRect(TileMap tileMap, Vector2 movement)
    {
        if (movement.X != 0 && movement.Y != 0)
            throw new ArgumentException("Max movement should only be used on one direction at once!");
        
        int tileW = width / tileMap.TileSize;
        int tileH = height / tileMap.TileSize;
    
        Vector2 moveDir = movement / Math.Max(1, movement.Length());
    
        bool isMovingHorizontally = movement.Y == 0;
        
        int tileSize = isMovingHorizontally ? tileW : tileH;
        float xOffset = isMovingHorizontally ? 0 : width / tileW;
        float yOffset = isMovingHorizontally ? height / tileH : 0;
        Vector2 offset = Vector2.Zero;
    
        // Check each tile on the rectangle's side of movement.
        for (int i = 0; i <= tileSize; i++)
        {
            offset.X = i * xOffset;
            offset.Y = i * yOffset;
    
            // Prevent the sideways offset from moving out of the player's hit-box by one pixel.
            // that behavior makes sense for the forward projection which is checking where the
            // player will be, but not for the sideways offset which should only include where
            // the player currently is.
            if (i == tileSize)
            {
                if (isMovingHorizontally) offset.Y--;
                else offset.X--;
            }
            
            Vector2 newMovement = GetMaxMovement(tileMap, movement, offset);
    
            // Returns the value that is least in the given direction.
            // This method allows the player to be moved backwards if necessary,
            // (ie: if a block falls into the player)
            float SmallerDistance(float a, float b, float dir)
            {
                if (dir > 0) return MathF.Min(a, b);
                return MathF.Max(a, b);
            }
    
            // If the player is planning to move farther than this segment of its hit-box can move
            // without colliding, reduce the amount that the player is planning to move.
            if (isMovingHorizontally) movement.X = SmallerDistance(newMovement.X, movement.X, moveDir.X);
            else movement.Y = SmallerDistance(newMovement.Y, movement.Y, moveDir.Y);
        }
        
        return movement;
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