using System.Numerics;
using Raylib_cs;

namespace Game;

public class GameLevel
{
    private readonly LevelDefinition _levelDefinition;
    private Model? _model;
    private Mesh? _mesh;
    private readonly GameTextures _gameTextures;
    private readonly GameLights _lights;
    private LevelDefinitionPoints _levelDefinitionPoints;
    private Texture2D _texture;
    private List<BoundingBox> _boundingBox;
    private readonly float _maxZ;
    private readonly float _maxX;
    private readonly float _minZ;
    private readonly float _minX;

    public GameLevel(LevelDefinition levelDefinition)
    {
        
        _levelDefinition = levelDefinition;
        _gameTextures = new GameTextures();
        _minX = -10f;
        _minZ = -10f;
        _maxX = 10f;
        _maxZ = 10f;
        
        _lights = new GameLights();
        _boundingBox = new List<BoundingBox>();
    }

    public GameLevel(float maxX)
    {
        _maxX = maxX;
    }

    public void Load()
    {
        _gameTextures.Load();
        _lights.Load();
        _model = CreateWallModel();
    }

    public void Unload()
    {
        _gameTextures.Unload();
        if (_mesh == null) return;
        var mesh = (Mesh)_mesh;
        Raylib.UnloadMesh(mesh);
    }
    
    private Model CreateWallModel()
    {
        
        _levelDefinitionPoints = _levelDefinition.Parse(_minX, _minZ, _maxX, _maxZ);
        _boundingBox.AddRange(_levelDefinitionPoints.BoundingBoxes);
        var mesh = new Mesh() {TriangleCount = _levelDefinitionPoints.Indices.Count/3, VertexCount = _levelDefinitionPoints.Points.Count};
        mesh.AllocIndices();
        mesh.AllocVertices();
        mesh.AllocNormals();
        mesh.AllocColors();
        mesh.AllocTexCoords();
        for (var i = 0; i < _levelDefinitionPoints.Indices.Count; i++)
        {
            mesh.IndicesAs<short>()[i]=_levelDefinitionPoints.Indices[i];
        }

        for (var i = 0; i < _levelDefinitionPoints.Points.Count; i++)
        {
            mesh.VerticesAs<Vector3>()[i]=_levelDefinitionPoints.Points[i];
            mesh.NormalsAs<Vector3>()[i]=_levelDefinitionPoints.Normals[i];
            mesh.TexCoordsAs<Vector2>()[i]=_levelDefinitionPoints.TextureCoordinates[i];
            mesh.ColorsAs<Color>()[i]=Color.White;
        }
        _mesh = mesh;
        Raylib.UploadMesh(ref mesh, false);
        var model= Raylib.LoadModelFromMesh(mesh);
        _texture = _gameTextures.GetWallTexture().Texture;
        _lights.SetShaderOnModel(model);
        return model;
        
    }

    public void CheckCollision(Player player)
    {
        player.CheckCollision(_boundingBox);
    }
    
    public void Draw()
    {
        if (_model == null) return;
        var model = (Model)_model;
        // Raylib.DrawModelWires(model,new Vector3(0,0,0),1,Color.Black);
        _lights.StartDrawShader(model,_texture);
        Raylib.DrawModel(model,new Vector3(0,0,0),1,Color.White);
        
        /*foreach (var point in _levelDefinitionPoints.Points.Zip(_levelDefinitionPoints.Normals))
        {
            Vector3 normalEnd = point.First + (point.Second * 0.5f); // Scale normal by 0.5 for visibility
            Raylib.DrawLine3D(point.First, normalEnd, Color.Red);
        }*/
        
        _lights.EndDrawShader();
    }

    public bool CheckExit(Player player)
    {
        return player.ReachedExit(_maxX, _maxZ, _minX, _minZ);
    }
}