using System.Numerics;
using Raylib_cs;

namespace TilePhys;

public class TextureAtlas
{
    public readonly int RenderScale;
    private Texture2D texture;
    private readonly int tileSize;

    public TextureAtlas(string name, int tileSize, int renderScale)
    {
        texture = Raylib.LoadTexture(name);
        this.tileSize = tileSize;
        RenderScale = renderScale;
    }

    public void Draw(int x, int y, int texX, int texY, int texW, int texH, Direction direction = Direction.Right)
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

        rotOffX *= RenderScale * tileSize;
        rotOffY *= RenderScale * tileSize;
        
        Rectangle src = new(texX * tileSize, texY * tileSize, texW * tileSize, texH * tileSize);
        Rectangle dst = new(rotOffX + x * RenderScale, rotOffY + y * RenderScale, texW * tileSize * RenderScale, texH * tileSize * RenderScale);
        Raylib.DrawTexturePro(texture, src, dst, Vector2.Zero, (int)direction * 90f, Color.WHITE);
    }

    ~TextureAtlas()
    {
        Raylib.UnloadTexture(texture);
    }
}