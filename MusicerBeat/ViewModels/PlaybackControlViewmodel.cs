using System;
using MusicerBeat.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlaybackControlViewmodel : BindableBase, IDisposable
    {
        private SoundPlayer soundPlayer = new ();

        public PlaybackControlViewmodel(IPlaylist playlist)
        {
            PlayListSource = playlist;
        }

        public bool IsPlaying { get; set; }

        public DelegateCommand<SoundFile> PlayCommand => new (Play);

        private IPlaylist PlayListSource { get; init; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            soundPlayer.Dispose();
        }

        private void Play(SoundFile soundFile)
        {
            soundFile ??= PlayListSource.SequentialSelector.SelectSoundFile();
            if (soundFile == null)
            {
                return;
            }

            soundPlayer.PlaySound(soundFile);
        }

        private void Stop()
        {
        }

        private void Next()
        {
        }

        private void Previous()
        {
        }
    }
}