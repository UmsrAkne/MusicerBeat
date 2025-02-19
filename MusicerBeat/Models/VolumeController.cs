using System;
using System.Collections.Generic;

namespace MusicerBeat.Models
{
    public class VolumeController
    {
        private float volumeFadeStep = 0.1f;

        public VolumeController()
        {
        }

        public VolumeController(List<ISoundPlayer> players)
        {
            SoundPlayers = players;
        }

        public float CurrentVolume { get; set; } = 1.0f;

        /// <summary>
        /// 一度の処理で変化させるボリュームの量です。セットされる値は、内部で絶対値に変換されます。<br/>
        /// デフォルトは 0.1 です。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">ゼロをセットしたときにすろーされます。</exception>
        public float VolumeFadeStep
        {
            get => volumeFadeStep;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "VolumeFadeStep cannot be 0!");
                }

                value = Math.Abs(value);
                volumeFadeStep = value;
            }
        }

        private List<ISoundPlayer> SoundPlayers { get; set; } = new ();

        /// <summary>
        /// このクラスがボリュームを調節する対象を追加します。<br/>
        /// ただし、内部で保持できるオブジェクトは2つまでです。それより多い数になる場合は古いアイテムを削除して追加します。
        /// </summary>
        /// <param name="player">追加するプレイヤー。</param>
        public void Add(ISoundPlayer player)
        {
            if (SoundPlayers.Count >= 2)
            {
                SoundPlayers.RemoveAt(0);
            }

            SoundPlayers.Add(player);
        }

        /// <summary>
        /// 現在のサウンドプレイヤーの音量をフェードさせ、
        /// フェードアウトが完了した古いプレイヤーをリストから削除します。
        /// </summary>
        /// <remarks>
        ///  - `SoundPlayers` に 2 つのプレイヤーが存在する場合にのみ動作します。<br/>
        ///  - 古いプレイヤーの音量を `VolumeFadeStep` ずつ減少させ、 0 になったらリストから削除します。<br/>
        ///  - 新しいプレイヤーの音量を `VolumeFadeStep` ずつ増加させ、`CurrentVolume` まで上げます。<br/>
        ///  - すでにフェードが完了している場合は処理をスキップします。<br/>
        /// </remarks>
        public void ChangeVolumes()
        {
            if (SoundPlayers.Count != 2)
            {
                return;
            }

            var oldPlayer = SoundPlayers[0];
            var newPlayer = SoundPlayers[1];

            if (oldPlayer.Volume == 0 && Math.Abs(newPlayer.Volume - CurrentVolume) < 0.001)
            {
                return;
            }

            if (oldPlayer.Volume > 0)
            {
                oldPlayer.Volume -= VolumeFadeStep;
                oldPlayer.Volume = Math.Max(0, oldPlayer.Volume);
            }

            if (newPlayer.Volume < CurrentVolume)
            {
                newPlayer.Volume += VolumeFadeStep;
                newPlayer.Volume = Math.Min(CurrentVolume, newPlayer.Volume);
            }

            if (IsVolumeFadeCompleted())
            {
                SoundPlayers.RemoveAt(0);
            }
        }

        /// <summary>
        /// このクラスが保持する ISoundPlayer オブジェクトのリストをクリアします。
        /// </summary>
        public void Clear()
        {
            SoundPlayers.Clear();
        }

        private bool IsVolumeFadeCompleted()
        {
            if (SoundPlayers.Count != 2)
            {
                return false;
            }

            var oldPlayer = SoundPlayers[0];
            var newPlayer = SoundPlayers[1];

            return oldPlayer.Volume == 0 && Math.Abs(newPlayer.Volume - CurrentVolume) < 0.001;
        }
    }
}