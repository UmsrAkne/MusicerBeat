using System;

namespace MusicerBeat.Models
{
    public interface ISoundPlayer
    {
        event EventHandler SoundEnded;

        float Volume { get; set; }

        void PlaySound(SoundFile soundFile);

        void Stop();
    }
}