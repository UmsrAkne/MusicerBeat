using System;
using NAudio.Wave;

namespace MusicerBeat.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : IDisposable, ISoundPlayer
    {
        private WaveOutEvent waveOutEvent;
        private WaveStream waveStream;

        public event EventHandler SoundEnded;

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

        public TimeSpan CurrentTime { get; } = TimeSpan.Zero;

        public void PlaySound(SoundFile soundFile)
        {
            if (waveOutEvent != null)
            {
                Stop();
            }

            waveStream = new Mp3FileReader(soundFile.FullName);
            waveOutEvent = new WaveOutEvent();
            waveOutEvent.Volume = 1.0f;
            waveOutEvent.Init(waveStream);
            waveOutEvent.Play();

            waveOutEvent.PlaybackStopped += WaveOutEventOnPlaybackStopped;
        }

        public void Stop()
        {
            waveOutEvent.Stop();
            waveOutEvent.PlaybackStopped -= WaveOutEventOnPlaybackStopped;
            waveOutEvent = null;
            waveStream = null;
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

        private void WaveOutEventOnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (waveOutEvent.PlaybackState == PlaybackState.Stopped)
            {
                SoundEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}