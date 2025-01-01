using Raylib_cs;

namespace Game;

public class GameTextures
{
    private readonly List<RenderTexture2D> _textureAtlases;
    public GameTextures()
    {
        _textureAtlases = new List<RenderTexture2D>();
    }

    public void Load()
    {
        int textureSize = 1024;
        int atlasWidth = textureSize * 2;
        int atlasHeight = textureSize * 2;
        var atlas = Raylib.LoadRenderTexture(atlasWidth, atlasHeight);
        Raylib.BeginTextureMode(atlas);
        Raylib.ClearBackground(Color.Blank);
        var walltexture = Raylib.LoadTexture("Data/Textures/sadwalltexture.png");
        var floortexture = Raylib.LoadTexture("Data/Textures/floor.png");
        var rooftexture = Raylib.LoadTexture("Data/Textures/roof.png");
        Raylib.DrawTexture(walltexture,0,textureSize,Color.White);
        Raylib.DrawTexture(floortexture,0,0,Color.White);
        Raylib.DrawTexture(rooftexture,textureSize,textureSize,Color.White);
        Raylib.EndTextureMode();
        _textureAtlases.Add(atlas);
    }

    public void Unload()
    {
        foreach (var textureAtlas in _textureAtlases)
            Raylib.UnloadRenderTexture(textureAtlas);
            
    }
    
    public RenderTexture2D GetWallTexture()
    {
        return _textureAtlases[0];
    }
}