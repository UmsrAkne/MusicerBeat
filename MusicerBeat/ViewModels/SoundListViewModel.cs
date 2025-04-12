using System;
using System.Collections.ObjectModel;
using System.Linq;
using MusicerBeat.Models;
using MusicerBeat.Models.Commands;
using MusicerBeat.Models.Databases;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundListViewModel : BindableBase, IPlaylist
    {
        private readonly ISoundCollectionSource soundCollectionSource;
        private readonly ObservableCollection<SoundFile> originalSounds = new ();
        private readonly SoundFileService soundFileService;
        private SoundFile selectedItem;

        public SoundListViewModel(ISoundCollectionSource soundCollectionSource)
        {
            this.soundCollectionSource = soundCollectionSource;
            this.soundCollectionSource.SoundsSourceUpdated += (_, _) => UpdateSoundsAsync();

            Sounds = new ReadOnlyObservableCollection<SoundFile>(originalSounds);
            SequentialSelector = new SequentialSelector(Sounds);
        }

        public SoundListViewModel(DirectoryAreaViewModel directoryAreaViewModel, IContainerProvider containerProvider)
        : this(directoryAreaViewModel)
        {
            soundFileService = containerProvider.Resolve<SoundFileService>();
        }

        public ReadOnlyObservableCollection<SoundFile> Sounds { get; set; }

        public SequentialSelector SequentialSelector { get; set; }

        public SoundFile SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        /// <summary>
        /// 選択中のサウンドの Skip プロパティをトグルします。
        /// </summary>
        public AsyncDelegateCommand ToggleSkipCommand => new AsyncDelegateCommand(async () =>
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsSkip = !SelectedItem.IsSkip;
                await soundFileService.ToggleSkip(SelectedItem);
            }
        });

        public DelegateCommand ReversePlayListCommand => new (() =>
        {
            var r = originalSounds.Reverse().ToList();
            originalSounds.Clear();
            originalSounds.AddRange(r);

            ReIndex();
        });

        public DelegateCommand ShufflePlayListCommand => new (() =>
        {
            var random = originalSounds.OrderBy(_ => Guid.NewGuid()).ToList();
            originalSounds.Clear();
            originalSounds.AddRange(random);

            ReIndex();
        });

        public DelegateCommand SortPlayListByPlayCountCommand => new (() =>
        {
            var sorted = originalSounds.OrderBy(s => s.ListenCount).ToList();
            originalSounds.Clear();
            originalSounds.AddRange(sorted);

            ReIndex();
        });

        public void AddSoundFile(SoundFile item)
        {
            originalSounds.Add(item);
        }

        private void UpdateSoundsAsync()
        {
            originalSounds.Clear();
            originalSounds.AddRange(soundCollectionSource.GetSounds());

            ReIndex();
        }

        private void ReIndex()
        {
            for (var i = 0; i < originalSounds.Count; i++)
            {
                originalSounds[i].Index = i + 1;
            }
        }
    }
}