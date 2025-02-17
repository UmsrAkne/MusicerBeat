using MusicerBeat.Models;

namespace MusicerBeatTests.ViewModels
{
    public class MockSoundPlayer : ISoundPlayer
    {
        public event EventHandler? SoundEnded;

        public SoundFile? LastPlayedSoundFile { get; private set; }

        public bool IsPlaying { get; private set; }

        public float Volume { get; set; }

        public TimeSpan CurrentTime { get; } = TimeSpan.Zero;

        public void PlaySound(SoundFile soundFile)
        {
            LastPlayedSoundFile = soundFile;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }
}