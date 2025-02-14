using System.Collections.ObjectModel;
using MusicerBeat.Models;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class SequentialSelectorTest
    {
        [Test]
        [TestCase(new[] { "C://t/file1.mp3", "C://t/file2.mp3",}, new [] { "file1.mp3", "file2.mp3", null, })]
        [TestCase(new[] { "C://t/file1.mp3", }, new [] { "file1.mp3", null, null, })]
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

        [Test]
        [TestCase(new[] { "C://t/file1.mp3", "C://t/file2.mp3",}, new [] { "file1.mp3", "file2.mp3", "file1.mp3", })]
        [TestCase(new[] { "C://t/file1.mp3", }, new [] { "file1.mp3", "file1.mp3", "file1.mp3", })]
        [TestCase(new string [] {}, new string?[] { null, null, null, })]
        public void SelectSoundFile_Loop_Test(string[] paths, string[] expected)
        {
            var list = new ObservableCollection<SoundFile>(paths.Select(p => new SoundFile(p)));
            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = true,
            };

            var results = new []
            {
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
        public void SetIndexBySoundFile_Test()
        {
            var list = new ObservableCollection<SoundFile>()
            {
                new SoundFile("C://t/file1.mp3"),
                new SoundFile("C://t/file2.mp3"),
                new SoundFile("C://t/file3.mp3"),
            };

            var f2 = list[1];

            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = false,
            };

            s.SetIndexBySoundFile(f2);

            var results = new []
            {
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
            };

            // 最後に file2 を取得したのと同じ状態になるわけだから、次の取得は file3 で、ループなしならばその次は null。
            CollectionAssert.AreEqual(new []{"file3.mp3", null, null, }, results);
        }
    }
}