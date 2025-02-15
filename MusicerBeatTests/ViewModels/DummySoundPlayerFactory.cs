using MusicerBeat.Models;

namespace MusicerBeatTests.ViewModels
{
    public class DummySoundPlayerFactory : ISoundPlayerFactory
    {
        public ISoundPlayer CreateSoundPlayer()
        {
            return new MockSoundPlayer();
        }
    }
}