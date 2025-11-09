using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;
using MusicerBeat.Models.Services;
using MusicerBeatTests.ViewModels;

namespace MusicerBeatTests.Models.Service
{
    [TestFixture]
    public class SoundPlayerMixerTest
    {
        [TestCaseSource(nameof(GetStatusTestCases))]
        public void GetStatusTest(List<ISoundPlayer> players, PlayingStatus expectedStatus)
        {
            var mixer = new SoundPlayerMixer(players, new DummySoundPlayerFactory());
            Assert.That(mixer.GetStatus(), Is.EqualTo(expectedStatus));
        }

        private static IEnumerable<TestCaseData> GetStatusTestCases()
        {
            yield return new TestCaseData(
                new List<ISoundPlayer>(),
                PlayingStatus.Stopped)
                .SetName("停止中");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.Zero, true),
                },
                PlayingStatus.Playing)
                .SetName("通常の再生中");

            yield return new TestCaseData(
                new List<ISoundPlayer>
                {
                    new MockSoundPlayer(TimeSpan.Zero, true),
                    new MockSoundPlayer(TimeSpan.Zero, true),
                },
                PlayingStatus.Fading)
                .SetName("曲のフェード中");
        }
    }
}