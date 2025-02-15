namespace MusicerBeat.Models
{
    public class SoundPlayerFactory : ISoundPlayerFactory
    {
        public ISoundPlayer CreateSoundPlayer()
        {
            return new SoundPlayer();
        }
    }
}