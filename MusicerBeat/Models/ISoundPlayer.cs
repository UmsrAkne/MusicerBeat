using System;

namespace MusicerBeat.Models
{
    public interface ISoundPlayer
    {
        event EventHandler SoundEnded;

        float Volume { get; set; }

        TimeSpan CurrentTime { get; }

        TimeSpan Duration { get; }

        bool IsPlaying { get; }

        void PlaySound(SoundFile soundFile);

        void Stop();
    }
}