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

        private static IEnumerable<TestCaseData> SequentialSelectorCases()
        {
            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = false, },
                    new (@"C:\t\b.mp3"){ IsSkip = false, },
                    new (@"C:\t\c.mp3"){ IsSkip = false, },
                },
                new List<string> { "a.mp3", "b.mp3", "c.mp3", },
                false
            ).SetName("スキップなしの通常再生");

            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = true, },
                    new (@"C:\t\b.mp3"){ IsSkip = false, },
                    new (@"C:\t\c.mp3"){ IsSkip = false, },
                },
                new List<string> { "b.mp3", "c.mp3", null, },
                false
            ).SetName("最初の曲をスキップ");

            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = false, },
                    new (@"C:\t\b.mp3"){ IsSkip = true, },
                    new (@"C:\t\c.mp3"){ IsSkip = false, },
                },
                new List<string> { "a.mp3", "c.mp3", null, },
                false
            ).SetName("真ん中にある曲をスキップ");

            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = false, },
                    new (@"C:\t\b.mp3"){ IsSkip = false, },
                    new (@"C:\t\c.mp3"){ IsSkip = true, },
                },
                new List<string> { "a.mp3", "b.mp3", null, },
                false
            ).SetName("最後の曲をスキップ");

            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = false, },
                    new (@"C:\t\b.mp3"){ IsSkip = false, },
                    new (@"C:\t\c.mp3"){ IsSkip = true, },
                },
                new List<string> { "a.mp3", "b.mp3", "a.mp3", },
                true
            ).SetName("最後の曲をスキップ(ループ)");

            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\t\a.mp3"){ IsSkip = true, },
                    new (@"C:\t\b.mp3"){ IsSkip = false, },
                    new (@"C:\t\c.mp3"){ IsSkip = false, },
                },
                new List<string> { "b.mp3", "c.mp3", "b.mp3", },
                true
            ).SetName("最初の曲をスキップ(ループ)");
        }

        [TestCaseSource(nameof(SequentialSelectorCases))]
        public void SelectSoundFile_Tests(
            List<SoundFile> soundFiles,
            List<string> expectedFileNames,
            bool loopFlag)
        {
            var list = new ObservableCollection<SoundFile>(soundFiles);
            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = loopFlag,
            };

            var results = new []
            {
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
                s.SelectSoundFile()?.Name,
            };

            CollectionAssert.AreEqual(expectedFileNames, results);
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
                new SoundFile("C://t/file1.mp3") { TotalMilliSeconds = 20000, },
                new SoundFile("C://t/file2.mp3") { TotalMilliSeconds = 10000, },
                new SoundFile("C://t/file3.mp3") { TotalMilliSeconds = 5000, },
            };

            var s = new SequentialSelector(new ReadOnlyObservableCollection<SoundFile>(list))
            {
                IsLoop = false,
            };

            Assert.Multiple(() =>
            {
                // 長いサウンドか判定した後に、インデックスが動いていないことを確認するため、`SelectSoundFile()`を実行する。
                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10), TimeSpan.Zero, TimeSpan.Zero), Is.True);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file1.mp3"));

                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10), TimeSpan.Zero, TimeSpan.Zero), Is.True);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file2.mp3"));

                Assert.That(s.NextIsLongSound(TimeSpan.FromSeconds(10), TimeSpan.Zero, TimeSpan.Zero), Is.False);
                Assert.That(s.SelectSoundFile()?.Name, Is.EqualTo("file3.mp3"));
            });
        }
    }
}