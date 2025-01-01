// See https://aka.ms/new-console-template for more information

using Game;

Console.WriteLine("Hello, World!");
var gameConfig = new GameConfig() {Height = 480, Width = 640, IsFullscreen = true};

var levelDefinition = new LevelDefinition(@"
cbb bbbbbbbbbbbbbbbbc
l   bbbbbl     bb bbbl
l  bb    bbbbbbb     l
l  bb bbl    b    bbbl
l      lbbbb  bbbbb l
lbbbbb   lbb    bbbb l
l      bbbbb  bb  bl l
l  bbbbb      b      l   
cbbbbbbbbbbbbbbbbbbbbc
");
var gameLevel = new GameLevel(levelDefinition);
var gameLevels = new List<GameLevel>() { gameLevel };
var gameController = new GameController(gameConfig,gameLevels);
gameController.Init();
await gameController.Run();