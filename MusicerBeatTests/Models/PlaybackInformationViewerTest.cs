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
    }
}