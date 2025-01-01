using System.Numerics;
using Raylib_cs;

namespace Game;

public unsafe class GameLights
{
    private Shader _shader;
    private int _lightPosLoc;
    private int _lightColorLoc;
    private int _ambientLoc;
    private float _ambient;
    private float[] _lightColor;
    private float[] _lightPos;
    private DateTimeOffset _currentTime;
    private float _lightSin = 0.0f;

    public GameLights()
    {
        _currentTime = DateTimeOffset.Now;
        
    }
    public void Load()
    {
        _shader = Raylib.LoadShader("Data/Shaders/lighting.glvs", "Data/Shaders/lighting.glfs");
        
        _lightPosLoc = Raylib.GetShaderLocation(_shader, "lightPos");
        _lightColorLoc = Raylib.GetShaderLocation(_shader, "lightColor");
        _ambientLoc = Raylib.GetShaderLocation(_shader, "ambient");
        _lightPos = new float[] { 0.0f, 0.5f, 0.0f };
        _lightColor = new float[] { 1.0f, 1.0f, 1.0f };
        _ambient = 0.1f;
    }

    public void StartDrawShader(Model model, Texture2D texture)
    {
        var time = DateTimeOffset.Now;
        var deltaTime = (time - _currentTime).TotalSeconds;
        _lightSin+=(float)deltaTime;
        _currentTime = time;
        _lightPos[0] = MathF.Sin(_lightSin) * 5;
        _lightPos[2] = MathF.Cos(_lightSin) * 5;
        Raylib.BeginShaderMode(_shader);
        Raylib.SetShaderValue(_shader, _lightPosLoc, _lightPos, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(_shader, _lightColorLoc, _lightColor, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(_shader, _ambientLoc, _ambient, ShaderUniformDataType.Float);
        Raylib.SetMaterialTexture(ref model,0,MaterialMapIndex.Albedo,ref texture);
    }

    public void EndDrawShader()
    {
        Raylib.EndShaderMode();
    }
    
    public void SetShaderOnModel(Model model)
    {
        model.Materials[0].Shader = _shader;
    }
}