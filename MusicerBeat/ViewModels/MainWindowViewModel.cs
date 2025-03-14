using System.Diagnostics;
using MusicerBeat.Models;
using MusicerBeat.Models.Services;
using MusicerBeat.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private DirectoryAreaViewModel directoryAreaViewModel;
        private SoundListViewModel soundListViewModel;
        private PlaybackControlViewmodel playbackControlViewmodel;
        private IDialogService dialogService;

        public MainWindowViewModel()
        {
            directoryAreaViewModel = new DirectoryAreaViewModel(@"C:\test");
            soundListViewModel = new SoundListViewModel(directoryAreaViewModel);
            PlaybackControlViewmodel = new PlaybackControlViewmodel(soundListViewModel, new SoundPlayerFactory());
            SetDummies();
        }

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
            directoryAreaViewModel = new DirectoryAreaViewModel(@"C:\test", containerProvider);
            soundListViewModel = new SoundListViewModel(directoryAreaViewModel);

            dialogService = containerProvider.Resolve<IDialogService>();
            PlaybackControlViewmodel = new PlaybackControlViewmodel(soundListViewModel, new SoundPlayerFactory(), containerProvider);
            SetDummies();
        }

        public DelegateCommand ShowSettingPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(SettingPage), new DialogParameters(), _ => { });
        });

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