using Raylib_cs;

namespace Game;
using static Raylib_cs.Raylib;
using NAudio.Wave;
using NAudio.Lame;
using NAudio.Vorbis;
public class SoundManager
{
    
    private string _eerie;
    private DateTimeOffset _newTime;
    private DateTimeOffset _currentTime;
    private bool _firstTime;
    private string _uneven;
    private Random _rand;
    private string _walk;
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;

    public void Load()
    {
        InitAudioDevice();
        _walk = "Data/Sounds/walk.ogg";
        var walkPath = "Data/Sounds/Felix Berget (Zoraxia) - walk.m4a";
        convert(walkPath, _walk);
        var eeriePath = "Data/Sounds/Felix Berget (Zoraxia) - knirking.m4a";
        _eerie = "Data/Sounds/knirking.ogg";
        convert(eeriePath, _eerie);
        var unevenPath = "Data/Sounds/Felix Berget (Zoraxia) - uneven sound.m4a";
        _uneven = "Data/Sounds/uneven sound.ogg";
        convert(unevenPath, _uneven);
        _currentTime = DateTimeOffset.Now;
        _firstTime = true;
        _rand = new Random();

    }

    public void playSoundEffects()
    {
        var newTime = DateTimeOffset.Now;
        var deltaTime = (newTime - _currentTime).TotalSeconds;
        var r = _rand.Next(0, 2);
        if (deltaTime > 15)
        {
            _currentTime = newTime;
            if ( r == 0)
            {
                PlaySound(_uneven);
            }

            if (r == 1)
            {
                PlaySound(_eerie);
            }
            
        }
    }

    public void PlayWalkingSound()
    {
        PlaySound(_walk);
        
    }

    public static void convert(string inputPath, string outputPath)
    {
        using (var reader = new MediaFoundationReader(inputPath))
        using (var writer = new WaveFileWriter(outputPath, reader.WaveFormat))
        {
            reader.CopyTo(writer);
        }
    }

// For playing WAV files
    public void PlaySound(string wavPath)
    {
        // Clean up previous instances
        outputDevice?.Dispose();
        audioFile?.Dispose();
    
        audioFile = new AudioFileReader(wavPath);
        outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFile);
        outputDevice.Play();
    }
}