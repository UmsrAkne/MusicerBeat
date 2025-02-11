using System.Collections.ObjectModel;
using System.IO;
using MusicerBeat.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DirectoryAreaViewModel : BindableBase
    {
        private SoundStorage selectedItem;
        private ReadOnlyObservableCollection<SoundStorage> soundStorages;
        private ObservableCollection<SoundStorage> originalSoundStorages;
        private SoundStorage currentStorage;

        public DirectoryAreaViewModel(string rootPath)
        {
            CurrentStorage = new SoundStorage() { FullPath = rootPath, };
            originalSoundStorages = new ObservableCollection<SoundStorage>();
            SoundStorages = new ReadOnlyObservableCollection<SoundStorage>(originalSoundStorages);
        }

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
            set => SetProperty(ref currentStorage, value);
        }

        /// <summary>
        /// SelectedItem に基づいて、カレントディレクトリを変更するコマンドを実行します。
        /// </summary>
        public DelegateCommand OpenDirectoryCommand => new DelegateCommand(() =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            CurrentStorage = new SoundStorage() { FullPath = SelectedItem.FullPath, };
            var items = SelectedItem.GetChildren();
            originalSoundStorages.Clear();
            originalSoundStorages.AddRange(items);
        });

        /// <summary>
        /// 一段上の SoundStorage に移動します。
        /// </summary>
        public DelegateCommand DirectoryUpCommand => new DelegateCommand(() =>
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

            var currently = new SoundStorage() { FullPath = parentPath, };
            CurrentStorage = currently;
            var items = currently.GetChildren();
            originalSoundStorages.Clear();
            originalSoundStorages.AddRange(items);
        });

        public void AddSoundStorage(SoundStorage item)
        {
            originalSoundStorages.Add(item);
        }
    }
}