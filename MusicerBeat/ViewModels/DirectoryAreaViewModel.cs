using System.Collections.ObjectModel;
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

        public DirectoryAreaViewModel()
        {
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
        /// SelectedItem に基づいて、カレントディレクトリを変更するコマンドを実行します。
        /// </summary>
        public DelegateCommand OpenDirectoryCommand => new DelegateCommand(() =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            var items = SelectedItem.GetChildren();
            originalSoundStorages.Clear();
            originalSoundStorages.AddRange(items);
        });

        public void AddSoundStorage(SoundStorage item)
        {
            originalSoundStorages.Add(item);
        }
    }
}