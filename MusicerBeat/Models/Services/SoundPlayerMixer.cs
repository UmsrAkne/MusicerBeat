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

        public float DefaultVolume { private get; set; } = 1.0f;

        public TimeSpan FrontCut { get; set; } = TimeSpan.Zero;

        public TimeSpan BackCut { get; set; } = TimeSpan.Zero;

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

            if (GetStatus() == PlayingStatus.Fading)
            {
                newPlayer.CurrentTime = FrontCut;
            }

            newPlayer.Volume = GetStatus() switch
            {
                PlayingStatus.Playing => DefaultVolume,
                PlayingStatus.Fading => 0f,
                _ => newPlayer.Volume,
            };
        }

        public void Stop()
        {
            foreach (var p in SoundPlayers)
            {
                p.SoundEnded -= RemovePlayer;
                p.Stop();
                if (p is IDisposable d)
                {
                    d.Dispose();
                }
            }

            SoundPlayers.Clear();
        }

        /// <summary>
        /// クロスフェード中に実行した時、古いプレイヤーの方を停止して、強制的に `PlayingStatus.Playing`に移行します。
        /// </summary>
        public void StopOldSound()
        {
            if (GetStatus() != PlayingStatus.Fading)
            {
                return;
            }

            var p = SoundPlayers.First();
            p.Stop();
            SoundPlayers.Remove(p);

            SoundPlayers.Last().Volume = DefaultVolume;
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