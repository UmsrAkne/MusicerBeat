using System.Collections.ObjectModel;
using MusicerBeat.Models;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SoundListViewModel : BindableBase, IPlaylist
    {
        private readonly ISoundCollectionSource soundCollectionSource;
        private readonly ObservableCollection<SoundFile> originalSounds = new ();
        private SoundFile selectedItem;

        public SoundListViewModel(ISoundCollectionSource soundCollectionSource)
        {
            this.soundCollectionSource = soundCollectionSource;
            this.soundCollectionSource.SoundsSourceUpdated += (_, _) =>
            {
                originalSounds.Clear();
                originalSounds.AddRange(soundCollectionSource.GetSounds());
            };

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
    }
}