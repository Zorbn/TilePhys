using System.Numerics;
using Raylib_cs;

namespace TilePhys;

public class TextureAtlas
{
    public readonly int RenderScale;
    public readonly int TileSize;
    private Texture2D texture;

    public TextureAtlas(string name, int tileSize, int renderScale)
    {
        texture = Raylib.LoadTexture(name);
        this.TileSize = tileSize;
        RenderScale = renderScale;
    }

    public void Draw(int x, int y, int texX, int texY, int texW, int texH, bool flipped = false, Direction direction = Direction.Right)
    {
        // Keep tiles in the same location despite rotating around the top left corner.
        int rotOffX = 0;
        int rotOffY = 0;

        switch (direction)
        {
            case Direction.Right:
                break;
            case Direction.Down:
                rotOffX = 1;
                break;
            case Direction.Left:
                rotOffX = 1;
                rotOffY = 1;
                break;
            case Direction.Up:
                rotOffY = 1;
                break;
        }

        rotOffX *= RenderScale * TileSize;
        rotOffY *= RenderScale * TileSize;

        int srcTexW = texW * TileSize;
        if (flipped) srcTexW *= -1;
        
        Rectangle src = new(texX * TileSize, texY * TileSize, srcTexW, texH * TileSize);
        Rectangle dst = new(rotOffX + x * RenderScale, rotOffY + y * RenderScale, texW * TileSize * RenderScale, texH * TileSize * RenderScale);
        Raylib.DrawTexturePro(texture, src, dst, Vector2.Zero, (int)direction * 90f, Color.WHITE);
    }

    ~TextureAtlas()
    {
        Raylib.UnloadTexture(texture);
    }
}