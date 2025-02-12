using System.Collections.ObjectModel;
using MusicerBeat.Models;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SoundListViewModel : BindableBase
    {
        private ObservableCollection<SoundFile> originalSounds = new ObservableCollection<SoundFile>();
        private SoundFile selectedItem;

        public SoundListViewModel()
        {
            Sounds = new ReadOnlyObservableCollection<SoundFile>(originalSounds);
        }

        public ReadOnlyObservableCollection<SoundFile> Sounds { get; set; }

        public SoundFile SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
    }
}