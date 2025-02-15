using MusicerBeat.Models;
using MusicerBeat.ViewModels;

namespace MusicerBeatTests.ViewModels
{
    [TestFixture]
    public class PlaybackControlViewModelTest
    {
        [Test]
        public void PlayCommandTest()
        {
            var playList = new MockPlaylist();
            playList.OriginalList.Add(new SoundFile("C://test/a.mp3"));

            var soundPlayerFactory = new DummySoundPlayerFactory();
            var vm = new PlaybackControlViewmodel(playList, soundPlayerFactory);

            vm.PlayCommand.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(soundPlayerFactory.CreatedPlayers.First().LastPlayedSoundFile?.Name, Is.EqualTo("a.mp3"));
                Assert.That(soundPlayerFactory.CreatedPlayers.First().IsPlaying, Is.True);
            });
        }

        [Test]
        public void PlayCommandTest_playlist_is_empty()
        {
            var playList = new MockPlaylist();

            var soundPlayerFactory = new DummySoundPlayerFactory();
            var vm = new PlaybackControlViewmodel(playList, soundPlayerFactory);

            vm.PlayCommand.Execute(null);

            // プレイリストにサウンドファイルが入っていない場合は、サウンドプレイヤーは生成されない。
            Assert.That(soundPlayerFactory.CreatedPlayers, Is.Empty);
        }
    }
}