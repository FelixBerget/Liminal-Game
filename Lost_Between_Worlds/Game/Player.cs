using System.Numerics;
using Raylib_cs;

namespace Game;

public class Player
{
    private Camera3D _camera;
    private Vector3 _oldCameraPosition;
    private DateTimeOffset _currentTime;
    private float _fovySin = 0.0f;
    private readonly SoundManager _soundManager;
    private bool _hasMoved;

    public Player(SoundManager s)
    {
        _camera = new Camera3D();
        _camera.Position = new Vector3(0.0f, 0.5f, 5.0f);
        _oldCameraPosition = _camera.Position;
        _camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        _camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        _camera.FovY = 45.0f;
        _camera.Projection = CameraProjection.Perspective;    
        _currentTime = DateTimeOffset.Now;
        _soundManager = s;
        _hasMoved = false;
    }

    public bool CheckCollision(List<BoundingBox> boxes)
    {
        var offset = 0.03f;
        var baseRay = Raylib.GetMouseRay(Raylib.GetMousePosition(), _camera);
        baseRay.Direction.Y = 0.5f;
        var rays = new[]
        {
            baseRay,
            new Ray
            {
                Position = baseRay.Position,
                Direction = Vector3.Transform(baseRay.Direction, Matrix4x4.CreateRotationY(offset))
            },
            new Ray
            {
                Position = baseRay.Position,
                Direction = Vector3.Transform(baseRay.Direction, Matrix4x4.CreateRotationY(-offset))
            },            
        };
        
        var collisionThreshold = Math.Max(1.0f, Vector3.Distance(_camera.Position, _oldCameraPosition));
        var minDistance = 100f;
        foreach (var ray in rays)
        {
            foreach (var box in boxes)
            {
                var collision = Raylib.GetRayCollisionBox(ray, box);
                if (collision.Hit)
                {
                    if (collision.Distance < minDistance) 
                        minDistance = collision.Distance;
            
                    if (collision.Distance < 1f)
                    {
                        _camera.Position = _oldCameraPosition;
                        return true;
                    }
                }            
            }
        }
        return false;
    }
    
    public void Move()
    {
        var newTime = DateTimeOffset.Now;
        var deltaTime = (newTime - _currentTime).TotalSeconds;
        _fovySin+=(float)deltaTime;        
        var sin = MathF.Sin(_fovySin) * 5;
        _camera.FovY = 45+sin;
        _currentTime = newTime;
        if (_oldCameraPosition != _camera.Position)
        {
            _soundManager.PlayWalkingSound();
            _hasMoved = true;
        }
        _oldCameraPosition = _camera.Position;
        Raylib.UpdateCamera(ref _camera, CameraMode.FirstPerson);
    }

    public bool ReachedExit(float maxX, float maxZ, float minX, float minZ)
    {
        if (_camera.Position.X > maxX || _camera.Position.X < minX || _camera.Position.Z > maxZ ||
            _camera.Position.Z < minZ)
            return true;
        return false;
    }
    
    public void AttachView()
    {
        Raylib.BeginMode3D(_camera);
    }

    public void DetachView()
    {
        Raylib.EndMode3D();
    }

    public bool hasItMoved()
    {
        return _hasMoved;
    }
}