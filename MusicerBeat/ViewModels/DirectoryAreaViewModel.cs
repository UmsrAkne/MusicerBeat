using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MusicerBeat.Models;
using MusicerBeat.Models.Commands;
using MusicerBeat.Models.Databases;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DirectoryAreaViewModel : BindableBase, ISoundCollectionSource
    {
        private readonly ObservableCollection<SoundStorage> originalSoundStorages;
        private readonly SoundFileService soundFileService;
        private readonly ConcurrentQueue<IEnumerable<SoundFile>> databaseRequestQueue = new();
        private SoundStorage selectedItem;
        private ReadOnlyObservableCollection<SoundStorage> soundStorages;
        private SoundStorage currentStorage;
        private bool isProcessing;

        public DirectoryAreaViewModel(string rootPath)
        {
            CurrentStorage = new SoundStorage() { FullPath = rootPath, };
            originalSoundStorages = new ObservableCollection<SoundStorage>(CurrentStorage.GetChildren());
            SoundStorages = new ReadOnlyObservableCollection<SoundStorage>(originalSoundStorages);
        }

        public DirectoryAreaViewModel(string rootPath, IContainerProvider containerProvider)
            : this(rootPath)
        {
            soundFileService = containerProvider.Resolve<SoundFileService>();
        }

        public event EventHandler SoundsSourceUpdated;

        public ReadOnlyObservableCollection<SoundStorage> SoundStorages
        {
            get => soundStorages;
            private set => SetProperty(ref soundStorages, value);
        }

        public SoundStorage SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != null)
                {
                    selectedItem.IsSelected = false; // 前の選択を解除
                }

                SetProperty(ref selectedItem, value);

                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true; // 新しい選択を設定
                }
            }
        }

        /// <summary>
        /// 現在作業中の Storage です。ファイルシステムでのカレントディレクトリに当たります。
        /// </summary>
        public SoundStorage CurrentStorage
        {
            get => currentStorage;
            private set
            {
                if (SetProperty(ref currentStorage, value))
                {
                    SoundsSourceUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// SelectedItem に基づいて、カレントディレクトリを変更するコマンドを実行します。
        /// </summary>
        public AsyncDelegateCommand OpenDirectoryCommand => new (async () =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            OpenDirectory(SelectedItem.FullPath);
            await EnqueueRequest(GetSounds());
        });

        /// <summary>
        /// 一段上の SoundStorage に移動します。
        /// </summary>
        public AsyncDelegateCommand DirectoryUpCommand => new (async () =>
        {
            if (CurrentStorage == null)
            {
                return;
            }

            var parent = Directory.GetParent(CurrentStorage.FullPath);
            var parentPath = parent != null ? parent.FullName : string.Empty;
            if (string.IsNullOrWhiteSpace(parentPath))
            {
                return;
            }

            OpenDirectory(parentPath);
            await EnqueueRequest(GetSounds());
        });

        /// <summary>
        /// `TreeView` の `SelectedItemChanged` にセットして実行するコマンドです。<br/>
        /// `TreeView.SelectedItem` には、プロパティを直接バインディングできないため、このコマンドで `SelectedItem` プロパティに値をセットします。
        /// </summary>
        public DelegateCommand<SoundStorage> RaiseSelectionChangedEventCommand => new (storage =>
        {
            if (storage == null)
            {
                return;
            }

            SelectedItem = storage;
        });
        
        public void AddSoundStorage(SoundStorage item)
        {
            originalSoundStorages.Add(item);
        }

        public IEnumerable<SoundFile> GetSounds()
        {
            return CurrentStorage.GetFiles();
        }

        /// <summary>
        /// 引数で指定されたディレクトリをカレントディレクトリに設定し、originalSoundStorages の中身をアップデートします。
        /// </summary>
        /// <param name="path">移動先のストレージのフルパスを入力します。</param>
        private void OpenDirectory(string path)
        {
            CurrentStorage = new SoundStorage() { FullPath = path, };
            SelectedItem.Children = SelectedItem.GetChildren().ToList();
            SelectedItem.IsExpanded = true;
        }

        private async Task EnqueueRequest(IEnumerable<SoundFile> sounds)
        {
            if (soundFileService == null)
            {
                return;
            }

            databaseRequestQueue.Enqueue(sounds);
            await ProcessQueue();
        }

        private async Task ProcessQueue()
        {
            if (isProcessing)
            {
                return;
            }

            isProcessing = true;

            while (databaseRequestQueue.TryDequeue(out var sounds))
            {
                var soundList = sounds.ToList();
                await soundFileService.AddSoundFileCollectionAsync(soundList);
                await soundFileService.LoadSoundInfo(soundList);
            }

            isProcessing = false;
        }
    }
}