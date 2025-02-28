using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicerBeat.Models;
using MusicerBeat.Models.Databases;
using Prism.Ioc;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class SoundListViewModel : BindableBase, IPlaylist, IDisposable
    {
        private readonly ISoundCollectionSource soundCollectionSource;
        private readonly ObservableCollection<SoundFile> originalSounds = new ();
        private readonly SoundFileService soundFileService;
        private CancellationTokenSource cancellationTokenSource;
        private SoundFile selectedItem;

        public SoundListViewModel(ISoundCollectionSource soundCollectionSource, IContainerProvider containerProvider)
        {
            if (containerProvider != null)
            {
                soundFileService = containerProvider.Resolve<SoundFileService>();
            }

            this.soundCollectionSource = soundCollectionSource;
            this.soundCollectionSource.SoundsSourceUpdated += async (_, _) => await UpdateSoundsAsync();

            Sounds = new ReadOnlyObservableCollection<SoundFile>(originalSounds);
            SequentialSelector = new SequentialSelector(Sounds);
        }

        public ReadOnlyObservableCollection<SoundFile> Sounds { get; set; }

        public SequentialSelector SequentialSelector { get; set; }

        public SoundFile SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public void AddSoundFile(SoundFile item)
        {
            originalSounds.Add(item);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            cancellationTokenSource?.Dispose();
        }

        private async Task UpdateSoundsAsync()
        {
            // 古い処理があればキャンセル
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            originalSounds.Clear();
            originalSounds.AddRange(soundCollectionSource.GetSounds());

            for (var i = 0; i < originalSounds.Count; i++)
            {
                originalSounds[i].Index = i + 1;
            }

            try
            {
                await LoadAdditionalDataAsync(token);
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合は無視
            }
        }

        private async Task LoadAdditionalDataAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var listenCounts = await soundFileService.LoadListenCount(originalSounds);
            token.ThrowIfCancellationRequested();

            // キャンセルされていなければデータを更新
            for (var i = 0; i < listenCounts.Count; i++)
            {
                originalSounds[i].ListenCount = listenCounts[i].ListenCount;
                originalSounds[i].TotalSeconds = listenCounts[i].TotalSec;
            }
        }
    }
}