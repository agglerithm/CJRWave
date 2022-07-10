using System.Reflection.Metadata.Ecma335;
using BufferUtilities;
using NUnit.Framework;
using Sound.Core;

namespace CJRWave.Test;

[TestFixture]
public class WaveFilePlayerTests
{
    private IWaveFilePlayer _sut = new WaveFilePlayer(new WavePlayerService(log));

    private static FileStream _fs;

    private static void log(object? obj)
    {
        Console.WriteLine(obj);
    }

    [Test]
    public void CanPlayWaveFile()
    {
            _sut.Play("dance.wav");
    }
}