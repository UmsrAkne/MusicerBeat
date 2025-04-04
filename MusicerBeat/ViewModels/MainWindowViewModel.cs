﻿using System.Diagnostics;
using System.IO;
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
        private readonly IDialogService dialogService;
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

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
            ApplicationSetting = ApplicationSetting.LoadFromXml(ApplicationSetting.SettingFileName);

            var rootDirectoryPath = ApplicationSetting.RootDirectoryPath;
            if (string.IsNullOrWhiteSpace(rootDirectoryPath) || !Directory.Exists(rootDirectoryPath))
            {
                rootDirectoryPath = @"C:\";
            }

            directoryAreaViewModel = new DirectoryAreaViewModel(rootDirectoryPath, containerProvider);
            soundListViewModel = new SoundListViewModel(directoryAreaViewModel);

            dialogService = containerProvider.Resolve<IDialogService>();
            PlaybackControlViewmodel = new PlaybackControlViewmodel(soundListViewModel, new SoundPlayerFactory(), containerProvider);

            ApplicationSetting = ApplicationSetting.LoadFromXml(ApplicationSetting.SettingFileName);
            PlaybackControlViewmodel.ApplySetting(ApplicationSetting);
        }

        public DelegateCommand ShowSettingPageCommand => new (() =>
        {
            dialogService.ShowDialog(nameof(SettingPage), new DialogParameters(), _ => { });
            ApplicationSetting = ApplicationSetting.LoadFromXml(ApplicationSetting.SettingFileName);
            PlaybackControlViewmodel.ApplySetting(ApplicationSetting);
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

        private ApplicationSetting ApplicationSetting { get; set; } = new ();

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