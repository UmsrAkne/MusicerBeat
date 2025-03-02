using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;

namespace MusicerBeatTests.ViewModels
{
    public class MockSoundPlayer : ISoundPlayer
    {
        private TimeSpan currentTime = TimeSpan.Zero;

        public event EventHandler? SoundEnded;

        public SoundFile? LastPlayedSoundFile { get; private set; }

        public bool IsPlaying { get; private set; }

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
                    SoundEnded?.Invoke(this, EventArgs.Empty);
                    IsPlaying = false;
                }
            }
        }

        public TimeSpan Duration { get; set; }

        public string Name { get; set; } = string.Empty;

        public void PlaySound(SoundFile soundFile)
        {
            if (soundFile.TotalSeconds > 0)
            {
                Duration = TimeSpan.FromSeconds(soundFile.TotalSeconds);
            }

            LastPlayedSoundFile = soundFile;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }
}