using MusicerBeat.Models.Services;
using MusicerBeatTests.ViewModels;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class VolumeControllerTest
    {
        [Test]
        public void ChangeVolumeTest()
        {
            var p1 = new MockSoundPlayer(){ Volume = 1, };
            var p2 = new MockSoundPlayer(){ Volume = 0, };
            var vc = new VolumeController() { VolumeFadeStep = 0.4f, };
            vc.Add(p1);
            vc.Add(p2);

            Assert.Multiple(() =>
            {
                Assert.That(p1.Volume, Is.EqualTo(1));
                Assert.That(p2.Volume, Is.EqualTo(0));
            });

            vc.ChangeVolumes();

            Assert.Multiple(() =>
            {
                Assert.That(p1.Volume, Is.EqualTo(0.6f).Within(0.0001f));
                Assert.That(p2.Volume, Is.EqualTo(0.4f).Within(0.0001f));
            });

            vc.ChangeVolumes();
            vc.ChangeVolumes();

            Assert.Multiple(() =>
            {
                Assert.That(p1.Volume, Is.EqualTo(0));
                Assert.That(p2.Volume, Is.EqualTo(1f));
            });
        }

        [Test]
        public void ChangeVolumeTest_3Add()
        {
            var p1 = new MockSoundPlayer(){ Volume = 1, };
            var p2 = new MockSoundPlayer(){ Volume = 0, };
            var p3 = new MockSoundPlayer(){ Volume = 0, };

            var vc = new VolumeController() { VolumeFadeStep = 0.4f, };
            vc.Add(p1);
            vc.Add(p2);

            vc.ChangeVolumes();
            vc.ChangeVolumes();
            vc.ChangeVolumes();

            // 新しいオブジェクトを投入。
            vc.Add(p3);

            // 一度目の音量調節処理が完了。それぞれの音量は以下の状態。
            Assert.Multiple(() =>
            {
                Assert.That(p1.Volume, Is.EqualTo(0)); // 登録解除済み。
                Assert.That(p2.Volume, Is.EqualTo(1)); // 現在再生中。
                Assert.That(p3.Volume, Is.EqualTo(0)); // 再生待ち。
            });

            vc.ChangeVolumes();

            Assert.Multiple(() =>
            {
                Assert.That(p2.Volume, Is.EqualTo(0.6f).Within(0.0001f)); // 音量ダウン
                Assert.That(p3.Volume, Is.EqualTo(0.4f).Within(0.0001f)); // 音量アップ
            });

            vc.ChangeVolumes();
            vc.ChangeVolumes();

            Assert.Multiple(() =>
            {
                Assert.That(p2.Volume, Is.EqualTo(0));
                Assert.That(p3.Volume, Is.EqualTo(1f));
            });
        }


        // 内部のリストが空の状態で ChangeVolumes を実行しても例外がスローされたりしないか確認する。
        [Test]
        public void ChangeVolumeTest_Empty()
        {
            var vc = new VolumeController() { VolumeFadeStep = 0.4f, };

            vc.ChangeVolumes();
            vc.ChangeVolumes();
            vc.ChangeVolumes();
        }

        [Test]
        public void ChangeVolumeTest_OneElement()
        {
            var p1 = new MockSoundPlayer(){ Volume = 1, };
            var vc = new VolumeController() { VolumeFadeStep = 0.4f, };
            vc.Add(p1);

            Assert.That(p1.Volume, Is.EqualTo(1));

            vc.ChangeVolumes();

            Assert.That(p1.Volume, Is.EqualTo(1.0f).Within(0.0001f));

            vc.ChangeVolumes();
            vc.ChangeVolumes();

            Assert.That(p1.Volume, Is.EqualTo(1.0f).Within(0.0001f));
        }
    }
}