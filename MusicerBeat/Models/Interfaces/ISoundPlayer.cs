using System;

namespace MusicerBeat.Models.Interfaces
{
    public interface ISoundPlayer
    {
        event EventHandler SoundEnded;

        float Volume { get; set; }

        TimeSpan CurrentTime { get; }

        TimeSpan Duration { get; }

        bool IsPlaying { get; }

        public SoundFile PlayingSound { get; }

        void PlaySound(SoundFile soundFile);

        void Stop();
    }
}