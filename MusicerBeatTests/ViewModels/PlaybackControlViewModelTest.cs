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

        [Test]
        public void PlayCommand_StateTransitionTest()
        {
            var playList = new MockPlaylist();
            playList.OriginalList.Add(new SoundFile("C://test/a.mp3") { Duration = 2, });
            playList.OriginalList.Add(new SoundFile("C://test/b.mp3") { Duration = 2, });
            playList.OriginalList.Add(new SoundFile("C://test/c.mp3") { Duration = 1, });
            playList.OriginalList.Add(new SoundFile("C://test/d.mp3") { Duration = 2, });

            var ps = new List<MockSoundPlayer>
            {
                new () { Name = "p1", },
                new () { Name = "p2", },
                new () { Name = "p3", },
                new () { Name = "p4", },
            };

            var soundPlayerFactory = new DummySoundPlayerFactory { PlayerSource = ps, };

            var vm = new PlaybackControlViewmodel(playList, soundPlayerFactory);
            vm.CrossFadeDuration = TimeSpan.FromSeconds(1);
            vm.VolumeController.VolumeFadeStep = 0.5f;

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
            vm.PlayCommand.Execute(null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Playing)); // p1
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(1000));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Fading)); // p1, p2
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(1000));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Playing)); // p2 + 500ms
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(500));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Playing)); // p2 end, p3 + 500ms
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(500));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Playing)); // p3 end, p4 + 1000ms
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(1000));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Playing)); // p4 end,
            soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += TimeSpan.FromMilliseconds(1000));
            vm.Timer_Tick(null, null);

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
        }
    }
}