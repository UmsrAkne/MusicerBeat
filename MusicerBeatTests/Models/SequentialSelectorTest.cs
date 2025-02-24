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
        public void SelectSoundFile_Contains_SkipFile()
        {
            var list = new ObservableCollection<SoundFile>()
            {
                new ("C://t/a.mp3"),
                new ("C://t/b.mp3"),
                new ("C://t/c.mp3") {IsSkip = true, },
            };

            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = false,
            };

            var expected = new[] { "a.mp3", "b.mp3", null, };

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

        [Test]
        public void NextIsLongSound_Test()
        {
            var list = new ObservableCollection<SoundFile>()
            {
                new SoundFile("C://t/file1.mp3") { Duration = 20, },
                new SoundFile("C://t/file2.mp3") { Duration = 10, },
                new SoundFile("C://t/file3.mp3") { Duration = 5, },
            };

            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = false,
            };

            Assert.Multiple(() =>
            {
                // 長いサウンドか判定した後に、インデックスが動いていないことを確認するため、`SelectSoundFile()`を実行する。
                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10)), Is.True);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file1.mp3"));

                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10)), Is.True);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file2.mp3"));

                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10)), Is.False);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file3.mp3"));
            });
        }
    }
}