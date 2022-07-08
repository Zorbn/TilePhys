using System.Numerics;
using Raylib_cs;

namespace TilePhys;

// TODO:
// Queue tiles who's neighbors recently changed for an update.
// Only updated queued tiles, then remove them from the queue.
// Only tick tick-able tiles, store a list of them to reduce unnecessary iteration.

internal static class Program
{
    public static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
        Raylib.InitWindow(640, 480, "TilePhys");

        TextureAtlas atlas = new ("res/atlas.png", 8, 4);
        
        TileMap tileMap = new (10, 10, 8);
        tileMap.Generate();

        Player player = new(0, 0, atlas.TileSize, atlas.TileSize);

        const float maxFrameTime = 1 / 30f;
        const float tickRate = 1 / 5f;
        float tickTimer = 0;

        while (!Raylib.WindowShouldClose())
        {
            float frameTime = MathF.Min(Raylib.GetFrameTime(), maxFrameTime);
            
            Vector2 mouseTilePos = Raylib.GetMousePosition() / atlas.RenderScale / tileMap.TileSize;
            player.Update(tileMap, mouseTilePos, frameTime);
            
            tickTimer += frameTime;
            
            if (tickTimer > tickRate)
            {
                tickTimer = 0;
                tileMap.Tick();
            }
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);
            
            tileMap.Draw(atlas);
            player.Draw(atlas);
            
            Raylib.EndDrawing();
        }
        
        Raylib.CloseWindow();
    }
}