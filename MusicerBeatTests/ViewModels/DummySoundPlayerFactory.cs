using MusicerBeat.Models;
using MusicerBeat.Models.Interfaces;

namespace MusicerBeatTests.ViewModels
{
    public class DummySoundPlayerFactory : ISoundPlayerFactory
    {
        /// <summary>
        /// `MockSoundPlayer` を取得します。<br/>
        /// `PlayerSource` に要素が入っている場合、実行のたびに `PlayerSource` から順番にアイテムを取り出して取得します。<br/>
        /// それ以外の場合は MockSoundPlayer を新しく生成して取得します。<br/>
        /// いずれの場合も、返したインスタンスを `CreatedPlayers` に追加します。
        /// </summary>
        /// <returns></returns>
        public ISoundPlayer CreateSoundPlayer()
        {
            if (PlayerSource.Count != 0)
            {
                var p = PlayerSource[CreatedCount++];
                CreatedPlayers.Add(p);
                return p;
            }

            var s = new MockSoundPlayer();
            CreatedPlayers.Add(s);
            return s;
        }

        public List<MockSoundPlayer> CreatedPlayers { get; } = new ();

        public List<MockSoundPlayer> PlayerSource { get; set; } = new ();

        public int CreatedCount { get; set; }
    }
}