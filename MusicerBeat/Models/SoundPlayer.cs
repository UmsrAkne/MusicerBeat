using System;
using NAudio.Wave;

namespace MusicerBeat.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : IDisposable, ISoundPlayer
    {
        private WaveOutEvent waveOutEvent;
        private AudioFileReader audioFileReader;

        public event EventHandler SoundEnded;

        public float Volume
        {
            get => audioFileReader?.Volume ?? 1.0f;
            set
            {
                if (audioFileReader != null)
                {
                    audioFileReader.Volume = Math.Min(Math.Max(value, 0), 1.0f);
                }
            }
        }

        public TimeSpan CurrentTime => audioFileReader?.CurrentTime ?? TimeSpan.Zero;

        public TimeSpan Duration => audioFileReader?.TotalTime ?? TimeSpan.Zero;

        public bool IsPlaying { get; private set; }

        public void PlaySound(SoundFile soundFile)
        {
            if (waveOutEvent != null)
            {
                Stop();
            }

            audioFileReader = new AudioFileReader(soundFile.FullName);
            audioFileReader.Volume = 1.0f;

            waveOutEvent = new WaveOutEvent();
            waveOutEvent.Volume = 1.0f;

            waveOutEvent.Init(audioFileReader);
            waveOutEvent.Play();

            IsPlaying = true;

            waveOutEvent.PlaybackStopped += WaveOutEventOnPlaybackStopped;
        }

        public void Stop()
        {
            waveOutEvent.Stop();
            waveOutEvent.PlaybackStopped -= WaveOutEventOnPlaybackStopped;
            waveOutEvent = null;
            audioFileReader = null;
            IsPlaying = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOutEvent.Dispose();
            audioFileReader.Dispose();
        }

        private void WaveOutEventOnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (waveOutEvent.PlaybackState == PlaybackState.Stopped)
            {
                IsPlaying = false;
                SoundEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}