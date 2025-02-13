using System.Collections.ObjectModel;
using MusicerBeat.Models;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class SequentialSelectorTest
    {
        [Test]
        [TestCase(new[] { "C://t/file1.mp3", "C://t/file2.mp3",}, new [] { "file1.mp3", "file2.mp3", null, })]
        [TestCase(new[] { "C://t/file1.mp3", }, new [] { "file1.mp3", "file1.mp3", "file1.mp3", })]
        [TestCase(new string [] {}, new string?[] { null, null, null, })]
        public void SelectSoundFile_Test(string[] paths, string[] expected)
        {
            var list = new ObservableCollection<SoundFile>(paths.Select(p => new SoundFile(p)));
            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list));
            var results = new []
            {
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
            };

            CollectionAssert.AreEqual(expected, results);


        }
    }
}