using System.Collections.ObjectModel;
using MusicerBeat.Models;
using MusicerBeat.ViewModels;

namespace MusicerBeatTests.ViewModels
{
    public class MockPlaylist : IPlaylist
    {
        public MockPlaylist()
        {
            OriginalList = new ObservableCollection<SoundFile>();
            Sounds = new ReadOnlyObservableCollection<SoundFile>(OriginalList);
            SequentialSelector = new SequentialSelector(Sounds);
        }

        public ReadOnlyObservableCollection<SoundFile> Sounds { get; set; }

        public ObservableCollection<SoundFile> OriginalList { get; private set; }

        public SequentialSelector SequentialSelector { get; }
    }
}