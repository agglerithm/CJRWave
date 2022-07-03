// See https://aka.ms/new-console-template for more information

using BufferUtilities;
using CJRWave;
using Sound.Core;

IWaveFilePlayer sut = new WaveFilePlayer(new WavePlayerService());

sut.Play("dance.wav");

static void log(object? obj)
{
    if (obj == null) return;
    //Thread.Sleep(900);
    Console.WriteLine(obj);
}