using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;

namespace MusicerBeatTests.ViewModels
{
    public class MockSoundPlayer : ISoundPlayer
    {
        private TimeSpan currentTime = TimeSpan.Zero;

        public MockSoundPlayer()
        {
        }

        public MockSoundPlayer(TimeSpan defaultCurrentTime, bool defaultPlaying = false)
        {
            IsPlaying = true;
            CurrentTime = defaultCurrentTime;
            IsPlaying = false;
            IsPlaying = defaultPlaying;
        }

        public event EventHandler? SoundEnded;

        public SoundFile? LastPlayedSoundFile { get; private set; }

        public bool IsPlaying { get; private set; }

        public SoundFile? PlayingSound { get; set; }

        public float Volume { get; set; }

        public TimeSpan CurrentTime
        {
            get => currentTime;
            set
            {
                if (!IsPlaying)
                {
                    return;
                }

                currentTime = value;

                if (currentTime >= Duration)
                {
                    currentTime = Duration;
                    SoundEnded?.Invoke(this, EventArgs.Empty);
                    IsPlaying = false;
                }
            }
        }

        public TimeSpan Duration { get; set; }

        public string Name { get; set; } = string.Empty;

        public void PlaySound(SoundFile soundFile, TimeSpan startPosition = default)
        {
            if (soundFile.TotalMilliSeconds > 0)
            {
                Duration = TimeSpan.FromMilliseconds(soundFile.TotalMilliSeconds);
            }

            LastPlayedSoundFile = soundFile;
            IsPlaying = true;
            PlayingSound = soundFile;
        }

        public void Stop()
        {
            IsPlaying = false;
            PlayingSound = null;
        }
    }
}