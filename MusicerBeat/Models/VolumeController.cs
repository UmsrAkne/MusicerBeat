using System.Collections.Generic;

namespace MusicerBeat.Models
{
    public class VolumeController
    {
        private List<ISoundPlayer> SoundPlayers { get; set; } = new ();

        /// <summary>
        /// このクラスがボリュームを調節する対象を追加します。<br/>
        /// ただし、内部で保持できるオブジェクトは2つまでです。それ以上の数になる場合は内部のリストをクリアしてから追加します。
        /// </summary>
        /// <param name="player">追加するプレイヤー。</param>
        public void Add(ISoundPlayer player)
        {
            if (SoundPlayers.Count >= 2)
            {
                SoundPlayers.Clear();
            }

            SoundPlayers.Add(player);
        }

        /// <summary>
        /// 一定のロジックに従って、このクラスが保持している SoundPlayer のボリュームを調節します。
        /// </summary>
        public void ChangeVolumes()
        {
            if (SoundPlayers.Count <= 1)
            {
                return;
            }
        }
    }
}