using MusicerBeat.Models.Interfaces;

namespace MusicerBeat.Models.Services
{
    public class SoundPlayerFactory : ISoundPlayerFactory
    {
        public ISoundPlayer CreateSoundPlayer()
        {
            return new SoundPlayer();
        }
    }
}