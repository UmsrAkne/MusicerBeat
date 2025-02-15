using System;
using MusicerBeat.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlaybackControlViewmodel : BindableBase, IDisposable
    {
        private ISoundPlayer soundPlayer;

        public PlaybackControlViewmodel(IPlaylist playlist, ISoundPlayerFactory soundPlayerFactory)
        {
            soundPlayer = soundPlayerFactory.CreateSoundPlayer();
            PlayListSource = playlist;

            soundPlayer.SoundEnded += (_, _) =>
            {
                Play(null);
            };
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
            var disposable = soundPlayer as IDisposable;
            disposable?.Dispose();
        }

        private void Play(SoundFile soundFile)
        {
            soundFile ??= PlayListSource.SequentialSelector.SelectSoundFile();
            if (soundFile == null)
            {
                return;
            }

            soundPlayer.PlaySound(soundFile);
            PlayListSource.SequentialSelector.SetIndexBySoundFile(soundFile);
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