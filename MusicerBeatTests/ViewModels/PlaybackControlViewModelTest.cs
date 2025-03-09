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
            vm.CrossFadeSetting.Duration = TimeSpan.FromSeconds(1);
            vm.VolumeController.VolumeFadeStep = 0.5f;

            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
            vm.PlayCommand.Execute(null);

            var expectedTransitions = new List<(TimeSpan, PlayingStatus)>
            {
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Playing), // p1
                (TimeSpan.FromMilliseconds(1000), PlayingStatus.Fading), // p1, p2
                (TimeSpan.FromMilliseconds(500), PlayingStatus.Playing), // p2 + 500ms
                (TimeSpan.FromMilliseconds(500), PlayingStatus.Playing), // p2 end, p3 + 500ms
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

                    (TimeSpan.FromMilliseconds(200), 0.8, 0.2, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(200), 0.6, 0.4, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(200), 0.4, 0.6, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(200), 0.2, 0.8, "p1, p2 (4) "),
                    (TimeSpan.FromMilliseconds(200), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

                    (TimeSpan.FromMilliseconds(200), 0.8, 0.2, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(200), 0.6, 0.4, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(200), 0.4, 0.6, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(200), 0.2, 0.8, "p1, p2 (4) "),
                    (TimeSpan.FromMilliseconds(200), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

                    (TimeSpan.FromMilliseconds(200), 0.8, 0.2, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(200), 0.6, 0.4, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(200), 0.4, 0.6, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(200), 0.2, 0.8, "p1, p2 (4) "),
                    (TimeSpan.FromMilliseconds(200), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

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

                    (TimeSpan.FromMilliseconds(200), 0.8, 0.2, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(200), 0.6, 0.4, "p1, p2 (1) クロスフェード開始"),
                    (TimeSpan.FromMilliseconds(200), 0.4, 0.6, "p1, p2 (2)"),
                    (TimeSpan.FromMilliseconds(200), 0.2, 0.8, "p1, p2 (3)"),
                    (TimeSpan.FromMilliseconds(200), 0, 1.0, "p1, p2 (4) クロスフェード完了"),

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
            vm.CrossFadeSetting.Duration = TimeSpan.FromSeconds(1);
            vm.VolumeController.VolumeFadeStep = 0.25f;

            // プレイヤーが止まっているかチェック。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));

            vm.PlayCommand.Execute(null);

            foreach (var (time, expectedVolOld, expectedVolNew, description) in args.transitions)
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

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\test\a.mp3") { TotalMilliSeconds = 3000, },
                    new (@"C:\test\b.mp3") { TotalMilliSeconds = 3000, },
                },
                new List<MockSoundPlayer>
                {
                    new MockSoundPlayer() { Name = "p1", },
                    new MockSoundPlayer() { Name = "p2", },
                },
                new List<(TimeSpan, TimeSpan, TimeSpan, PlayingStatus, string)>
                {
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(0), PlayingStatus.Playing, "t1"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(0), PlayingStatus.Playing, "t2-1"),
                    (TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2500), TimeSpan.FromMilliseconds(1000), PlayingStatus.Fading, "t2-2"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(2000), PlayingStatus.Playing, "t2-3"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(3000), PlayingStatus.Stopped, "t3"),
                },
                new CrossFadeSetting()
                {
                    FrontCut = TimeSpan.FromMilliseconds(500),
                    BackCut = TimeSpan.Zero,
                }
            ).SetName("冒頭カット");

            // 末尾のカットは片方のプレイヤーの音量を指定秒数早く 0 にしているだけであり、各プレイヤーの状態変遷は冒頭カットのみの場合と全く同じであるはず。
            yield return new TestCaseData(
                new List<SoundFile>
                {
                    new (@"C:\test\a.mp3") { TotalMilliSeconds = 3000, },
                    new (@"C:\test\b.mp3") { TotalMilliSeconds = 3000, },
                },
                new List<MockSoundPlayer>
                {
                    new MockSoundPlayer() { Name = "p1", },
                    new MockSoundPlayer() { Name = "p2", },
                },
                new List<(TimeSpan, TimeSpan, TimeSpan, PlayingStatus, string)>
                {
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(0), PlayingStatus.Playing, "t1"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(0), PlayingStatus.Playing, "t2-1"),
                    (TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2500), TimeSpan.FromMilliseconds(1000), PlayingStatus.Fading, "t2-2"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(2000), PlayingStatus.Playing, "t2-3"),
                    (TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(3000), PlayingStatus.Stopped, "t3"),
                },
                new CrossFadeSetting()
                {
                    FrontCut = TimeSpan.FromMilliseconds(500),
                    BackCut = TimeSpan.FromMilliseconds(500),
                }
            ).SetName("冒頭カット + 末尾カット");
        }

        [TestCaseSource(nameof(TestCases))]
        public void CrossFade_WithTrimmedSound_Test(
            List<SoundFile> soundFiles,
            List<MockSoundPlayer> players,
            List<(TimeSpan, TimeSpan, TimeSpan, PlayingStatus, string)> transitions,
            CrossFadeSetting setting)
        {
            var playList = new MockPlaylist();
            foreach (var sf in soundFiles)
            {
                playList.OriginalList.Add(sf);
            }

            var soundPlayerFactory = new DummySoundPlayerFactory { PlayerSource = players, };

            var vm = new PlaybackControlViewmodel(playList, soundPlayerFactory);
            vm.CrossFadeSetting = setting;
            vm.CrossFadeSetting.Duration = TimeSpan.FromSeconds(1);

            // プレイヤーが止まっているかチェック。
            Assert.That(vm.GetStatus(), Is.EqualTo(PlayingStatus.Stopped));
            vm.PlayCommand.Execute(null);
            var counter = 1;

            foreach (var (forwardTime, p1CurrentTime, p2CurrentTime, status, description) in transitions)
            {
                vm.UpdatePlaybackState();
                var r = Enumerable.Reverse(soundPlayerFactory.PlayerSource).ToList();
                r.ForEach(p => p.CurrentTime += forwardTime);

                Assert.Multiple(() =>
                {
                    Assert.That(vm.GetStatus(), Is.EqualTo(status));
                    Assert.That(soundPlayerFactory.PlayerSource[0].CurrentTime, Is.EqualTo(p1CurrentTime));
                    Assert.That(soundPlayerFactory.PlayerSource[1].CurrentTime, Is.EqualTo(p2CurrentTime));
                });

                counter++;
            }
        }
    }
}