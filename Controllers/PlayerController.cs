using LibVLCSharp.Shared;

namespace TGPSound.Controllers;

public class PlayerController(LibVLC libvlc)
{
    private readonly LibVLC _libvlc = libvlc;
    private readonly MediaPlayer _player = new(libvlc);

    public void SetMedia(Media media, bool playOnReady = true)
    {
        _player.Media = media;
        if (playOnReady) _player.Play();
    }

    public Media BuildMedia(string StreamUrl)
    {
        return new(_libvlc, StreamUrl, FromType.FromLocation);
    }

    public void PlayPause()
    {
        if (_player.IsPlaying)
            _player.Pause();
        else
            _player.Play();
    }

    public bool IsPlaying()
    {
        return _player.IsPlaying;
    }

    public void Backward(int seconds = 5)
    {
        _player.Time -= seconds * 1000;
    }

    public void Forward(int seconds = 5)
    {
        _player.Time += seconds * 1000;
    }

    public void Stop()
    {
        _player.Stop();
        _player.Media = null;
    }

    public double GetCurrentTime()
    {
        return _player.Time / 1000.0;
    }

    public void Dispose()
    {
        _player.Stop();
        _player.Dispose();
        _libvlc.Dispose();
    }
}
