using System;
using NAudio.Wave;

namespace MusicerBeat.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : IDisposable
    {
        private WaveOutEvent waveOutEvent;
        private WaveStream waveStream;

        public float Volume
        {
            get => waveOutEvent?.Volume ?? 1.0f;
            set
            {
                if (waveOutEvent != null)
                {
                    waveOutEvent.Volume = Math.Min(Math.Max(value, 0), 1.0f);
                }
            }
        }

        public void PlaySound(SoundFile soundFile)
        {
            waveStream = new Mp3FileReader(soundFile.FullName);
            waveOutEvent ??= new WaveOutEvent();
            waveOutEvent.Volume = 1.0f;
            waveOutEvent.Init(waveStream);
            waveOutEvent.Play();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOutEvent.Dispose();
            waveStream.Dispose();
        }
    }
}