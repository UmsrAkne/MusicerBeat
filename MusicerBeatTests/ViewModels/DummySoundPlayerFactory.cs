using MusicerBeat.Models;

namespace MusicerBeatTests.ViewModels
{
    public class DummySoundPlayerFactory : ISoundPlayerFactory
    {
        public ISoundPlayer CreateSoundPlayer()
        {
            var s = new MockSoundPlayer();
            CreatedPlayers.Add(s);
            return s;
        }

        public List<MockSoundPlayer> CreatedPlayers { get; } = new ();
    }
}