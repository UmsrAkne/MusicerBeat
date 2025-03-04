using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;
using MusicerBeatTests.ViewModels;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class PlaybackInformationViewerTest
    {
        private static IEnumerable<TestCaseData> ShowFileNameTestCases()
        {
            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer() { PlayingSound = new SoundFile("a.mp3"), },
                    new MockSoundPlayer() { PlayingSound = new SoundFile("b.mp3"), },
                },
                "a --> b"
            ).SetName("クロスフェード中");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer() { PlayingSound = new SoundFile("a.mp3"), },
                },
                "a"
            ).SetName("一曲だけ再生中");

            yield return new TestCaseData(
                new List<ISoundPlayer>(),
                string.Empty
            ).SetName("停止状態");
        }

        [TestCaseSource(nameof(ShowFileNameTestCases))]
        public void ShowFileNameTest(IEnumerable<ISoundPlayer> players, string expectedResult)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);
            Assert.That(viewer.PlayingFileName, Is.EqualTo(expectedResult));
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new List<ISoundPlayer>(),
                string.Empty
            ).SetName("停止状態");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.FromSeconds(1))
                    {
                        PlayingSound = new SoundFile("a.mp3"),
                    },
                },
                "00:00:01"
            ).SetName("単体再生");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.FromSeconds(4))
                    {
                        PlayingSound = new SoundFile("a.mp3"),
                    },
                    new MockSoundPlayer(TimeSpan.FromSeconds(2))
                    {
                        PlayingSound = new SoundFile("b.mp3"),
                    },
                },
                "00:00:04 --> 00:00:02"
            ).SetName("２曲再生");
        }

        [TestCaseSource(nameof(TestCases))]
        public void PlaybackTimeStringTest(IEnumerable<ISoundPlayer> players,string expectedText)
        {
            var viewer = new PlaybackInformationViewer();
            viewer.UpdatePlaybackInformation(players);
            Assert.That(viewer.PlaybackTimeString, Is.EqualTo(expectedText));
        }
    }
}