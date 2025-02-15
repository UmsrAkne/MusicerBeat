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

            var soundPlayer = new MockSoundPlayer();
            var vm = new PlaybackControlViewmodel(playList, soundPlayer);

            vm.PlayCommand.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(soundPlayer.LastPlayedSoundFile?.Name, Is.EqualTo("a.mp3"));
                Assert.That(soundPlayer.IsPlaying, Is.True);
            });
        }

        [Test]
        public void PlayCommandTest_playlist_is_empty()
        {
            var playList = new MockPlaylist();

            var soundPlayer = new MockSoundPlayer();
            var vm = new PlaybackControlViewmodel(playList, soundPlayer);

            vm.PlayCommand.Execute(null);

            // プレイリストにサウンドファイルが入っていない場合は、最後に再生した曲名は入っておらず、 PlaySound も実行されない。
            Assert.Multiple(() =>
            {
                Assert.That(soundPlayer.LastPlayedSoundFile?.Name, Is.EqualTo(null));
                Assert.That(soundPlayer.IsPlaying, Is.False);
            });
        }
    }
}