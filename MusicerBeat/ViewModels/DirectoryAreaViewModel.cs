using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MusicerBeat.Models;
using MusicerBeat.Models.Databases;
using Prism.Ioc;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DirectoryAreaViewModel : BindableBase, ISoundCollectionSource
    {
        private readonly ObservableCollection<SoundStorage> originalSoundStorages;
        private readonly SoundFileService soundFileService;
        private SoundStorage selectedItem;
        private ReadOnlyObservableCollection<SoundStorage> soundStorages;
        private SoundStorage currentStorage;

        public DirectoryAreaViewModel(string rootPath)
        {
            CurrentStorage = new SoundStorage() { FullPath = rootPath, };
            originalSoundStorages = new ObservableCollection<SoundStorage>();
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

        public SoundStorage SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

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
            if (soundFileService != null)
            {
                await soundFileService.AddSoundFileCollectionAsync(GetSounds());
            }
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
            if (soundFileService != null)
            {
                await soundFileService.AddSoundFileCollectionAsync(GetSounds());
            }
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
            var currently = new SoundStorage() { FullPath = path, };
            CurrentStorage = currently;
            var items = currently.GetChildren();
            originalSoundStorages.Clear();
            originalSoundStorages.AddRange(items);
        }
    }
}