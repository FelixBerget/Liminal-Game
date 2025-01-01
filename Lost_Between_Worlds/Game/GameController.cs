﻿using System.Numerics;
using Raylib_cs;

namespace Game;

public class GameController
{
    private readonly GameConfig _config;
    private readonly List<GameLevel> _levels;
    private readonly GameLevel _currentLevel;
    private readonly Player _player;
    private readonly SoundManager _soundManager;
    private bool _hasExited;


    public GameController(GameConfig config, List<GameLevel> levels)
    {
        _config = config;
        _levels = levels;
        _currentLevel = levels[0];
        _soundManager = new SoundManager();
        _player = new Player(_soundManager);
        _hasExited = false;
    }
    public void Init()
    {
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(_config.Width, _config.Height, "Lost worlds");
        if (_config.IsFullscreen)
        {
            Raylib.SetWindowSize(Raylib.GetMonitorWidth(0), Raylib.GetMonitorHeight(0));
            Raylib.ToggleFullscreen();
        }
            
        Raylib.SetTargetFPS(60);
        _currentLevel.Load();
        _soundManager.Load();
    }

    public void Unload()
    {
        _currentLevel.Unload();
    }
    
    
    public async Task Run()
    {
        
        //var mesh = Raylib.GenMeshCube(1,1,1);
        //var model = Raylib.LoadModelFromMesh(mesh);
        Rlgl.DisableBackfaceCulling();
        while (!Raylib.WindowShouldClose())
        {
            if (_player.hasItMoved()==false)
            {
                _player.Move();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Blue);
                Raylib.DrawText("Hello!", 100, 100, 20, Color.White);
                Raylib.DrawText("Press W,S,D, of A to move", 150, 120, 20, Color.White);
                Raylib.EndDrawing();
            }
            else if (!_hasExited)
            {
                _currentLevel.CheckCollision(_player);
                _hasExited = _currentLevel.CheckExit(_player);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                _player.AttachView();
                _player.Move();
                _soundManager.playSoundEffects();
                _currentLevel.Draw();
                _player.DetachView();
                Raylib.EndDrawing();
            }
            else
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Blue);
                Raylib.DrawText("Congratulations! You have reached the exit", 100, 100, 20, Color.White);
                Raylib.DrawText("Press escape to exit!", 150, 120, 20, Color.White);
                Raylib.EndDrawing();
                
            }
            
        }
        Unload();
    }
    
    
}