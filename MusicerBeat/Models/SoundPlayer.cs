using NAudio.Wave;

namespace MusicerBeat.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundPlayer : System.IDisposable
    {
        private WaveOutEvent waveOutEvent;
        private WaveStream waveStream;

        public void PlaySound(SoundFile soundFile)
        {
            waveStream = new Mp3FileReader(soundFile.FullName);
            waveOutEvent ??= new WaveOutEvent();
            waveOutEvent.Init(waveStream);
            waveOutEvent.Play();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOutEvent.Dispose();
            waveStream.Dispose();
        }
    }
}