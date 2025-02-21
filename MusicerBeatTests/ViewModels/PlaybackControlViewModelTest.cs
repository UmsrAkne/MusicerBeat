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

            var expectedTransitions = new List<(TimeSpan, PlayingStatus)>
            {
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Playing), // p1
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Fading),  // p1, p2
                (TimeSpan.FromMilliseconds(500),  PlayingStatus.Playing), // p2 + 500ms
                (TimeSpan.FromMilliseconds(500),  PlayingStatus.Playing), // p2 end, p3 + 500ms
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Playing), // p3 end, p4 + 1000ms
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Playing), // p4 end,
            };

            foreach (var (time, expectedStatus) in expectedTransitions)
            {
                Assert.That(vm.GetStatus(), Is.EqualTo(expectedStatus));
                soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += time);
                vm.Timer_Tick(null, null);
            }

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
        }

        [Test]
        public void PlayCommand_VolumeTransitionTest()
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
            vm.VolumeController.VolumeFadeStep = 0.25f;

            // プレイヤーが止まっているかチェック。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));

            vm.PlayCommand.Execute(null);

            var expectedTransitions = new List<(TimeSpan, double?, double?, string)>
            {
                (TimeSpan.FromMilliseconds(1000), 1.0, null, "p1"),

                (TimeSpan.FromMilliseconds(250), 0.75, 0.25, "p1, p2 (1) クロスフェード開始"),
                (TimeSpan.FromMilliseconds(250), 0.5,  0.5 , "p1, p2 (2)"),
                (TimeSpan.FromMilliseconds(250), 0.25, 0.75, "p1, p2 (3)"),
                (TimeSpan.FromMilliseconds(250), 0,    1.0 , "p1, p2 (4) クロスフェード完了"),

                (TimeSpan.FromMilliseconds(500),  1.0, null, "p2 + 500ms"),
                (TimeSpan.FromMilliseconds(500),  1.0, null, "p2 end, p3 + 500ms"),
                (TimeSpan.FromMilliseconds(1000), 1.0, null, "p3 end, p4 + 1000ms"),
                (TimeSpan.FromMilliseconds(1000), 1.0, null, "p4 end,"),
            };

            foreach (var (time, expectedVolOld, expectedVolNew,description) in expectedTransitions)
            {
                vm.Timer_Tick(null, null);

                var ov = vm.GetVolumes().OldPlayerVol;
                var nv = vm.GetVolumes().NewPlayerVol;

                Assert.Multiple(() =>
                {
                    if (expectedVolOld is null)
                    {
                        Assert.That(ov, Is.Null);
                    }
                    else
                    {
                        Assert.That(ov, Is.EqualTo(expectedVolOld).Within(0.0001));
                    }

                    if (expectedVolNew is null)
                    {
                        Assert.That(nv, Is.Null);
                    }
                    else
                    {
                        Assert.That(nv, Is.EqualTo(expectedVolNew).Within(0.0001));
                    }
                });

                soundPlayerFactory.PlayerSource.ForEach(p => p.CurrentTime += time);
            }

            // 全ての再生が終了したはずなので、プレイヤーが停止状態かを確認する。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
        }
    }
}