using Prism.Commands;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlaybackControlViewmodel : BindableBase
    {
        public PlaybackControlViewmodel(IPlaylist playlist)
        {
            PlayListSource = playlist;
        }

        public bool IsPlaying { get; set; }

        public DelegateCommand PlayCommand => new DelegateCommand(Play);

        private IPlaylist PlayListSource { get; init; }

        private void Play()
        {
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