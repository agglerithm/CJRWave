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
        if (obj == null) return;
        using var fs = new FileStream("op.txt",FileMode.Append);
        var str = $"{obj.GetType().Name};{obj}\r\n";
        var buff = str.StringToByteArray();
        fs.Write(buff,0,buff.Length);
    }

    [Test]
    public void CanPlayWaveFile()
    {
            _sut.Play("dance.wav");
    }
}