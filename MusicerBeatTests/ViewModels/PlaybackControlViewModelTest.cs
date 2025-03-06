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
            playList.OriginalList.Add(new SoundFile("C://test/a.mp3") { TotalMilliSeconds = 2000, });
            playList.OriginalList.Add(new SoundFile("C://test/b.mp3") { TotalMilliSeconds = 2000, });
            playList.OriginalList.Add(new SoundFile("C://test/c.mp3") { TotalMilliSeconds = 1000, });
            playList.OriginalList.Add(new SoundFile("C://test/d.mp3") { TotalMilliSeconds = 2000, });

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
                vm.UpdatePlaybackState();
            }

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
        }

        private static IEnumerable<(
            List<(string soundFilePath, int duration, string playerName)> soundAndPlayers,
            List<(TimeSpan, double?, double?, string)> expectedTransitions)>
        VolumeTransitionTestCases()
        {
            yield return (
                new List<(string, int, string)>
                {
                    (@"C:\test\a.mp3", 2000, "p1"),
                    (@"C:\test\b.mp3", 2000, "p2"),
                    (@"C:\test\c.mp3", 2000, "p3"),
                    (@"C:\test\d.mp3", 2000, "p4"),
                },
                new List<(TimeSpan, double?, double?, string)>
                {
                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p1"),

                    (TimeSpan.FromMilliseconds(250), 0.75, 0.25, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(250), 0.5, 0.5, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(250), 0.25, 0.75, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(250), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

                    (TimeSpan.FromMilliseconds(250), 0.75, 0.25, "p2 p3 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(250), 0.5, 0.5, "p2 p3 (2)"),
                    (TimeSpan.FromMilliseconds(250), 0.25, 0.75, "p2 p3 (3)"),
                    (TimeSpan.FromMilliseconds(250), 0, 1.0, "p2 p3 クロスフェード終了"),

                    (TimeSpan.FromMilliseconds(250), 0.75, 0.25, "p3 p4 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(250), 0.5, 0.5, "p3 p4 (2)"),
                    (TimeSpan.FromMilliseconds(250), 0.25, 0.75, "p3 p4 (3)"),
                    (TimeSpan.FromMilliseconds(250), 0, 1.0, "p3 p4 クロスフェード終了"),

                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p4 end,"),
                }
            );

            yield return (
                new List<(string, int, string)>
                {
                    (@"C:\test\a.mp3", 2000, "p1"),
                    (@"C:\test\b.mp3", 2000, "p2"),
                    (@"C:\test\c.mp3", 1000, "p3"),
                    (@"C:\test\d.mp3", 2000, "p4"),
                },
                new List<(TimeSpan, double?, double?, string)>
                {
                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p1"),

                    (TimeSpan.FromMilliseconds(250), 0.75, 0.25, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(250), 0.5, 0.5, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(250), 0.25, 0.75, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(250), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p2 + 500ms"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p2 end"),
                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p3 end"),
                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p4 + 1000ms"),
                    (TimeSpan.FromMilliseconds(1000), 1.0, null, "p4 end,"),
                }
            );

            yield return (
                new List<(string, int, string)>
                {
                    (@"C:\test\a.mp3", 1000, "p1"),
                    (@"C:\test\b.mp3", 1000, "p2"),
                    (@"C:\test\c.mp3", 1000, "p3"),
                },
                new List<(TimeSpan, double?, double?, string)>
                {
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p1-1"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p1-2"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p2-1"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p2-2"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p3-1"),
                    (TimeSpan.FromMilliseconds(500), 1.0, null, "p3-2"),
                }
            );
        }

        [TestCaseSource(nameof(VolumeTransitionTestCases))]
        public void PlayCommand_VolumeTransitionTests(
            (
            List<(string soundFilePath, int duration, string playerName)> soundAndPlayers,
            List<(TimeSpan, double?, double?, string)> transitions) args
            )
        {
            var playList = new MockPlaylist();
            foreach (var data in args.soundAndPlayers)
            {
                playList.OriginalList.Add(new SoundFile(data.soundFilePath) { TotalMilliSeconds = data.duration, });
            }

            var ps =
                args.soundAndPlayers.Select(d => new MockSoundPlayer() { Name = d.playerName, }).ToList();

            var soundPlayerFactory = new DummySoundPlayerFactory { PlayerSource = ps, };

            var vm = new PlaybackControlViewmodel(playList, soundPlayerFactory);
            vm.CrossFadeDuration = TimeSpan.FromSeconds(1);
            vm.VolumeController.VolumeFadeStep = 0.25f;

            // プレイヤーが止まっているかチェック。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));

            vm.PlayCommand.Execute(null);

            foreach (var (time, expectedVolOld, expectedVolNew,description) in args.transitions)
            {
                vm.UpdatePlaybackState();

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

                var r = Enumerable.Reverse(soundPlayerFactory.PlayerSource).ToList();
                r.ForEach(p => p.CurrentTime += time);
            }

            // 全ての再生が終了したはずなので、プレイヤーが停止状態かを確認する。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
        }
    }
}