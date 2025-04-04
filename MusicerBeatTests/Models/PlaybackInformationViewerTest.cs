using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;
using MusicerBeatTests.ViewModels;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class PlaybackInformationViewerTest
    {
        [TestCaseSource(nameof(ShowFileNameTestCases))]
        public void ShowFileNameTest(IEnumerable<ISoundPlayer> players, string expectedResult)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);
            Assert.That(viewer.PlayingFileName, Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(ShowFileNameTestCases))]
        public void ResetFileNameTest(IEnumerable<ISoundPlayer> players, string expectedResult)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);

            // 空のリストをセットし、空文字が出力されるかを確認する。
            viewer.UpdatePlaybackInformation(new List<ISoundPlayer>());
            Assert.That(viewer.PlayingFileName, Is.EqualTo(string.Empty));
        }

        [TestCaseSource(nameof(CurrentTimeStringTestCases))]
        public void PlaybackTimeStringTest(IEnumerable<ISoundPlayer> players, string expectedText)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);
            Assert.That(viewer.PlaybackTimeString, Is.EqualTo(expectedText));
        }

        [TestCaseSource(nameof(CurrentTimeStringTestCases))]
        public void ResetPlaybackTimeStringTest(IEnumerable<ISoundPlayer> players, string expectedResult)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);

            // 空のリストをセットし、空文字が出力されるかを確認する。
            viewer.UpdatePlaybackInformation(new List<ISoundPlayer>());
            Assert.That(viewer.PlaybackTimeString, Is.EqualTo(string.Empty));
        }

        private static IEnumerable<TestCaseData> ShowFileNameTestCases()
        {
            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer() { PlayingSound = new SoundFile("a.mp3"), },
                    new MockSoundPlayer() { PlayingSound = new SoundFile("b.mp3"), },
                },
                "a --> b")
                .SetName("クロスフェード中");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer() { PlayingSound = new SoundFile("a.mp3"), },
                },
                "a")
                .SetName("一曲だけ再生中");

            yield return new TestCaseData(
                new List<ISoundPlayer>(),
                string.Empty)
                .SetName("停止状態");
        }

        private static IEnumerable<TestCaseData> CurrentTimeStringTestCases()
        {
            yield return new TestCaseData(
                new List<ISoundPlayer>(),
                string.Empty)
                .SetName("停止状態");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.FromSeconds(1), true)
                    {
                        PlayingSound = new SoundFile("a.mp3") { TotalMilliSeconds = 1000, },
                        Duration = TimeSpan.FromSeconds(2),
                        CurrentTime = TimeSpan.FromSeconds(1),
                    },
                },
                "00:00:01")
                .SetName("単体再生");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.FromSeconds(0), true)
                    {
                        PlayingSound = new SoundFile("a.mp3") { TotalMilliSeconds = 4000, },
                        Duration = TimeSpan.FromSeconds(5),
                        CurrentTime = TimeSpan.FromSeconds(4),
                    },
                    new MockSoundPlayer(TimeSpan.FromSeconds(0), true)
                    {
                        PlayingSound = new SoundFile("b.mp3") { TotalMilliSeconds = 2000, },
                        Duration = TimeSpan.FromSeconds(3),
                        CurrentTime = TimeSpan.FromSeconds(2),
                    },
                },
                "00:00:04 --> 00:00:02")
                .SetName("２曲再生");
        }
    }
}