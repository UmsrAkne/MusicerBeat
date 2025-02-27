using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using MusicerBeat.Models;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private DirectoryAreaViewModel directoryAreaViewModel;
        private SoundListViewModel soundListViewModel;
        private PlaybackControlViewmodel playbackControlViewmodel;

        public MainWindowViewModel()
        {
            directoryAreaViewModel = new DirectoryAreaViewModel(@"C:\test");
            soundListViewModel = new SoundListViewModel(directoryAreaViewModel);
            PlaybackControlViewmodel = new PlaybackControlViewmodel(soundListViewModel, new SoundPlayerFactory());
            SetDummies();
        }

        public TextWrapper Title { get; set; } = new ();

        public DirectoryAreaViewModel DirectoryAreaViewModel
        {
            get => directoryAreaViewModel;
            set => SetProperty(ref directoryAreaViewModel, value);
        }

        public SoundListViewModel SoundListViewModel
        {
            get => soundListViewModel;
            set => SetProperty(ref soundListViewModel, value);
        }

        public PlaybackControlViewmodel PlaybackControlViewmodel
        {
            get => playbackControlViewmodel;
            set => SetProperty(ref playbackControlViewmodel, value);
        }

        [Conditional("DEBUG")]
        private void SetDummies()
        {
            var firstStorage = new SoundStorage() { FullPath = $@"C:\test\testDirectory0", };
            DirectoryAreaViewModel.AddSoundStorage(firstStorage);

            for (var i = 0; i < 30; i++)
            {
                DirectoryAreaViewModel.AddSoundStorage(new SoundStorage() { FullPath = $@"C:\test\testDirectory{i + 1}", });
                SoundListViewModel.AddSoundFile(new SoundFile(@$"C:\test\soundFile{i + 1}.mp3"));
            }

            SoundListViewModel.Sounds[1].Playing = true;
            SoundListViewModel.Sounds[2].Playing = true;

            DirectoryAreaViewModel.SelectedItem = firstStorage;
        }
    }
}