using System;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MusicerBeat.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : IDisposable, ISoundPlayer
    {
        private WaveOutEvent waveOutEvent;
        private AudioFileReader audioReader;
        private VorbisWaveReader vorbisReader;
        private VolumeSampleProvider volumeSampleProvider;

        public event EventHandler SoundEnded;

        public float Volume
        {
            get => volumeSampleProvider?.Volume ?? 1.0f;
            set
            {
                if (volumeSampleProvider != null)
                {
                    volumeSampleProvider.Volume = Math.Min(Math.Max(value, 0), 1.0f);
                }
            }
        }

        public TimeSpan CurrentTime => audioReader?.CurrentTime ?? vorbisReader?.CurrentTime ?? TimeSpan.Zero;

        public TimeSpan Duration => audioReader?.TotalTime ?? vorbisReader?.TotalTime ?? TimeSpan.Zero;

        public bool IsPlaying { get; private set; }

        public void PlaySound(SoundFile soundFile)
        {
            if (waveOutEvent != null)
            {
                Stop();
            }

            if (audioReader == null && vorbisReader == null)
            {
                if (soundFile.Extension == ".ogg")
                {
                    vorbisReader = new VorbisWaveReader(soundFile.FullName);
                    volumeSampleProvider = new VolumeSampleProvider(vorbisReader) { Volume = 1.0f, };
                }
                else
                {
                    audioReader = new AudioFileReader(soundFile.FullName);
                    volumeSampleProvider = new VolumeSampleProvider(audioReader) { Volume = 1.0f, };
                }
            }

            waveOutEvent = new WaveOutEvent();
            waveOutEvent.Volume = 1.0f;

            waveOutEvent.Init(volumeSampleProvider);
            waveOutEvent.Play();

            IsPlaying = true;

            waveOutEvent.PlaybackStopped += WaveOutEventOnPlaybackStopped;
        }

        public void Stop()
        {
            waveOutEvent.Stop();
            waveOutEvent.PlaybackStopped -= WaveOutEventOnPlaybackStopped;

            waveOutEvent = null;
            audioReader = null;
            vorbisReader = null;

            IsPlaying = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOutEvent.Dispose();
            audioReader.Dispose();
            vorbisReader.Dispose();
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