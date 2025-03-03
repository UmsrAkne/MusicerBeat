using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using MusicerBeat.Models;
using MusicerBeat.Models.Databases;
using MusicerBeat.Models.Interfaces;
using MusicerBeat.Models.Services;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlaybackControlViewmodel : BindableBase, IDisposable
    {
        private readonly ISoundPlayerFactory soundPlayerFactory;
        private readonly List<ISoundPlayer> soundPlayers = new ();
        private readonly SoundFileService soundFileService;

        private readonly DispatcherTimer timer;
        private TimeSpan crossFadeDuration = TimeSpan.FromSeconds(10);
        private string soundFileName = string.Empty;

        public PlaybackControlViewmodel(IPlaylist playlist, ISoundPlayerFactory soundPlayerFactory)
        {
            this.soundPlayerFactory = soundPlayerFactory;
            PlayListSource = playlist;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200), };
            timer.Tick += Timer_Tick;
            VolumeController = new VolumeController(soundPlayers);
            CrossFadeDuration = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        public PlaybackControlViewmodel(SoundListViewModel soundListViewModel, SoundPlayerFactory soundPlayerFactory, IContainerProvider containerProvider)
        : this(soundListViewModel, soundPlayerFactory)
        {
            soundFileService = containerProvider.Resolve<SoundFileService>();
        }

        public bool IsPlaying { get; set; }

        public VolumeController VolumeController { get; set; }

        public PlayingFileNameViewer PlayingFileNameViewer { get; set; } = new ();

        public DelegateCommand<SoundFile> PlayCommand => new (Play);

        /// <summary>
        /// クロスフェード時、音量の変化が完了するまでの時間を設定します。<br/>
        /// このプロパティに値をセットすると、時間あたりの音量の変更量が変化するため、`VolumeController.VolumeFadeStep`も自動で変更されます。
        /// </summary>
        public TimeSpan CrossFadeDuration
        {
            get => crossFadeDuration;
            set
            {
                SetProperty(ref crossFadeDuration, value);
                var dur = 1.0 / (TimeSpan.FromSeconds(1).TotalMilliseconds / timer.Interval.TotalMilliseconds * crossFadeDuration.TotalSeconds);
                VolumeController.VolumeFadeStep = (float)dur;
            }
        }

        private IPlaylist PlayListSource { get; init; }

        public PlayingStatus GetStatus()
        {
            if (soundPlayers.Count == 0)
            {
                return PlayingStatus.Stopped;
            }

            if (soundPlayers.Count == 1)
            {
                if (!soundPlayers.First().IsPlaying)
                {
                    throw new InvalidOperationException("Invalid Status");
                }

                return PlayingStatus.Playing;
            }

            if (soundPlayers.Count == 2)
            {
                if (soundPlayers.All(p => p.IsPlaying))
                {
                    // リストの中のプレイヤーが両方動いている。
                    return PlayingStatus.Fading;
                }
            }

            throw new InvalidOperationException("Invalid Status");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 再生中のプレイヤーの音量を取得します。
        /// </summary>
        /// <returns>プレイヤーの音量を取得します。再生中のプレイヤーが一つ、または存在しない場合は、該当の箇所に null が入ります。</returns>
        public (double? OldPlayerVol, double? NewPlayerVol) GetVolumes()
        {
            return GetStatus() switch
            {
                PlayingStatus.Stopped => (null, null),
                PlayingStatus.Playing => (soundPlayers.First().Volume, null),
                PlayingStatus.Fading => (soundPlayers.First().Volume, soundPlayers.Last().Volume),
                _ => (0, 0),
            };
        }

        public void UpdatePlaybackState()
        {
            if (GetStatus() == PlayingStatus.Playing)
            {
                var p = soundPlayers.First();
                var nextIsLongSound = PlayListSource.SequentialSelector.NextIsLongSound(CrossFadeDuration * 2);
                var currentlyIsLongSound = p.Duration >= CrossFadeDuration * 2;
                if (p.CurrentTime >= p.Duration - CrossFadeDuration && nextIsLongSound && currentlyIsLongSound)
                {
                    Play(null);
                }
            }

            if (GetStatus() == PlayingStatus.Fading)
            {
                VolumeController.ChangeVolumes();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var d in soundPlayers.Select(p => p as IDisposable))
            {
                d?.Dispose();
            }
        }

        private void Play(SoundFile soundFile)
        {
            if (soundFile != null)
            {
                // 再生する曲が直接選択されて指定されたケース
                // 全てのプレイヤーを停止する。
                Stop();
                PlayListSource.SequentialSelector.SetIndexBySoundFile(soundFile);
            }

            soundFile ??= PlayListSource.SequentialSelector.SelectSoundFile();
            if (soundFile == null)
            {
                return;
            }

            var newPlayer = soundPlayerFactory.CreateSoundPlayer();
            newPlayer.SoundEnded += RemoveAndPlay;
            soundPlayers.Add(newPlayer);
            newPlayer.PlaySound(soundFile);

            soundFileService?.AddListenHistoryAsync(soundFile);

            newPlayer.Volume = GetStatus() switch
            {
                PlayingStatus.Playing => 1.0f,
                PlayingStatus.Fading => 0f,
                _ => newPlayer.Volume,
            };

            PlayingFileNameViewer.UpdatePlayingFileName(soundPlayers);
        }

        private void RemoveAndPlay(object sender, EventArgs e)
        {
            if (sender is ISoundPlayer p)
            {
                soundPlayers.Remove(p);
                PlayingFileNameViewer.UpdatePlayingFileName(soundPlayers);
            }

            if (GetStatus() == PlayingStatus.Stopped)
            {
                Play(null);
            }
        }

        private void Stop()
        {
            foreach (var p in soundPlayers)
            {
                p.Stop();
                if (p is IDisposable d)
                {
                    d.Dispose();
                }
            }

            soundPlayers.Clear();
            PlayingFileNameViewer.UpdatePlayingFileName(soundPlayers);
        }

        private void Next()
        {
        }

        private void Previous()
        {
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePlaybackState();
            PlayingFileNameViewer.UpdatePlayingFileName(soundPlayers);
        }
    }
}