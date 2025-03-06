using System;
using System.Collections.Generic;
using System.Linq;
using MusicerBeat.Models.Interfaces;

namespace MusicerBeat.Models.Services
{
    public class SoundPlayerMixer
    {
        /// <summary>
        /// SoundPlayerMixer をインスタンス化するコンストラクタです。
        /// </summary>
        /// <param name="soundPlayers">入力されたリストは内部で参照が保持され、`SoundPlayer` が生成された際に新しいプレイヤーを追加します。</param>
        /// <param name="soundPlayerFactory">このクラスの内部で `ISoundPlayer` を生成するファクトリークラスを入力します。</param>
        public SoundPlayerMixer(List<ISoundPlayer> soundPlayers, ISoundPlayerFactory soundPlayerFactory)
        {
            SoundPlayers = soundPlayers;
            SoundPlayerFactory = soundPlayerFactory;
        }

        /// <summary>
        /// このインスタンスが保持するプレイヤーの再生が終了した時に発生するイベントです。
        /// </summary>
        public event EventHandler SoundEnded;

        private List<ISoundPlayer> SoundPlayers { get; }

        private ISoundPlayerFactory SoundPlayerFactory { get; }

        public PlayingStatus GetStatus()
        {
            if (SoundPlayers.Count == 0)
            {
                return PlayingStatus.Stopped;
            }

            if (SoundPlayers.Count == 1)
            {
                if (!SoundPlayers.First().IsPlaying)
                {
                    throw new InvalidOperationException("Invalid Status");
                }

                return PlayingStatus.Playing;
            }

            if (SoundPlayers.Count == 2)
            {
                if (SoundPlayers.All(p => p.IsPlaying))
                {
                    // リストの中のプレイヤーが両方動いている。
                    return PlayingStatus.Fading;
                }
            }

            throw new InvalidOperationException("Invalid Status");
        }

        /// <summary>
        /// 入力された `SoundFile` を使って再生を開始します。<br/>
        /// このメソッドにより再生を開始した場合、プレイヤーの初期音量はこのインスタンスの内部状態によって適宜調整されます。
        /// </summary>
        /// <param name="soundFile">再生する `SoundFile` を入力します。</param>
        public void Play(SoundFile soundFile)
        {
            var newPlayer = SoundPlayerFactory.CreateSoundPlayer();
            newPlayer.SoundEnded += RemovePlayer;
            SoundPlayers.Add(newPlayer);
            newPlayer.PlaySound(soundFile);

            newPlayer.Volume = GetStatus() switch
            {
                PlayingStatus.Playing => 1.0f,
                PlayingStatus.Fading => 0f,
                _ => newPlayer.Volume,
            };
        }

        public void Stop()
        {
            foreach (var p in SoundPlayers)
            {
                p.Stop();
                if (p is IDisposable d)
                {
                    d.Dispose();
                }
            }

            SoundPlayers.Clear();
        }

        private void RemovePlayer(object sender, EventArgs e)
        {
            if (sender is ISoundPlayer p)
            {
                SoundPlayers.Remove(p);
            }

            SoundEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}