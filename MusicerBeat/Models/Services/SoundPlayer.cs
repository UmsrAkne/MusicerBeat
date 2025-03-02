using System;
using MusicerBeat.Models.Interfaces;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MusicerBeat.Models.Services
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : IDisposable, ISoundPlayer
    {
        private WaveOutEvent waveOutEvent;
        private AudioFileReader audioReader;
        private VorbisWaveReader vorbisReader;
        private VolumeSampleProvider volumeSampleProvider;
        private SoundFile sound;

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

        public SoundFile PlayingSound { get; set; }

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
            sound = soundFile;
            soundFile.Playing = true;
            PlayingSound = soundFile;

            waveOutEvent.PlaybackStopped += WaveOutEventOnPlaybackStopped;
        }

        public void Stop()
        {
            waveOutEvent.PlaybackStopped -= WaveOutEventOnPlaybackStopped;
            waveOutEvent.Stop();

            IsPlaying = false;
            if (sound != null)
            {
                sound.Playing = false;
            }

            PlayingSound = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOutEvent?.Dispose();
            audioReader?.Dispose();
            vorbisReader?.Dispose();

            waveOutEvent = null;
            audioReader = null;
            vorbisReader = null;
        }

        private void WaveOutEventOnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (waveOutEvent.PlaybackState == PlaybackState.Stopped)
            {
                IsPlaying = false;
                if (sound != null)
                {
                    sound.Playing = false;
                }

                PlayingSound = null;
                SoundEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}