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
        private readonly List<ISoundPlayer> soundPlayers = new ();
        private readonly SoundFileService soundFileService;
        private readonly SoundPlayerMixer soundPlayerMixer;

        private readonly DispatcherTimer timer;
        private float volume = 1.0f;
        private PlayingStatus playingStatus;

        public PlaybackControlViewmodel(IPlaylist playlist, ISoundPlayerFactory soundPlayerFactory)
        {
            PlayListSource = playlist;
            timer = new DispatcherTimer { Interval = CrossFadeSetting.FadeProcessInterval, };
            timer.Tick += Timer_Tick;
            VolumeController = new VolumeController(soundPlayers);

            soundPlayerMixer = new SoundPlayerMixer(soundPlayers, soundPlayerFactory);
            soundPlayerMixer.SoundEnded += PlayNext;

            timer.Start();
        }

        public PlaybackControlViewmodel(SoundListViewModel soundListViewModel, SoundPlayerFactory soundPlayerFactory, IContainerProvider containerProvider)
        : this(soundListViewModel, soundPlayerFactory)
        {
            soundFileService = containerProvider.Resolve<SoundFileService>();
        }

        public bool IsPlaying { get; set; }

        public VolumeController VolumeController { get; set; }

        public float Volume
        {
            get => volume;
            set
            {
                SetProperty(ref volume, value);
                VolumeController.CurrentVolume = value;
                VolumeController.SetVolume(value);
                soundPlayerMixer.DefaultVolume = value;
            }
        }

        public PlaybackInformationViewer PlaybackInformationViewer { get; set; } = new ();

        public CrossFadeSetting CrossFadeSetting { get; set; } = new ();

        public DelegateCommand<SoundFile> PlayCommand => new (Play);

        public DelegateCommand PlayNextCommand => new (Next);

        public DelegateCommand StopCommand => new (Stop);

        public PlayingStatus PlayingStatus
        {
            get => playingStatus;
            set => SetProperty(ref playingStatus, value);
        }

        public IPlaylist PlayListSource { get; init; }

        public PlayingStatus GetStatus()
        {
            return soundPlayerMixer.GetStatus();
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
                var nextIsLongSound = PlayListSource.SequentialSelector.NextIsLongSound(CrossFadeSetting);
                var currentlyIsLongSound = p.Duration >= CrossFadeSetting.RequiredCrossFadeDuration;
                var endPoint = p.Duration - CrossFadeSetting.Duration - soundPlayerMixer.BackCut;
                if (p.CurrentTime >= endPoint && nextIsLongSound && currentlyIsLongSound)
                {
                    Play(null);
                }
            }

            var state = GetStatus();

            if (state == PlayingStatus.Fading)
            {
                VolumeController.ChangeVolumes(CrossFadeSetting);
                PlayingStatus = PlayingStatus.Fading;
                return;
            }

            PlayingStatus = state == PlayingStatus.Playing
                ? PlayingStatus.Playing
                : PlayingStatus.Stopped;
        }

        public void ApplySetting(ApplicationSetting setting)
        {
            CrossFadeSetting.FrontCut = setting.FrontCut;
            CrossFadeSetting.BackCut = setting.BackCut;
            CrossFadeSetting.Duration = setting.CrossFadeDuration;
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var d in soundPlayers.Select(p => p as IDisposable))
            {
                d?.Dispose();
            }
        }

        /// <summary>
        /// 指定された `SoundFile` を適切な手順を踏んでから再生します。
        /// </summary>
        /// <param name="soundFile">再生する `SoundFile` を指定します。<br/>
        /// `null` が入力された場合は、次に再生するべきサウンドを内部で自動取得します。</param>
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

            soundPlayerMixer.Play(soundFile);
            soundPlayerMixer.FrontCut = CrossFadeSetting.FrontCut;

            soundFileService?.AddListenHistoryAsync(soundFile);
            PlaybackInformationViewer.UpdatePlaybackInformation(soundPlayers);
        }

        private void PlayNext(object sender, EventArgs e)
        {
            if (GetStatus() == PlayingStatus.Stopped)
            {
                Play(null);
            }
        }

        private void Stop()
        {
            soundPlayerMixer.Stop();
            PlayListSource.SequentialSelector.ResetIndex();
            PlaybackInformationViewer.UpdatePlaybackInformation(soundPlayers);
        }

        private void Next()
        {
            var status = GetStatus();

            if (status == PlayingStatus.Stopped)
            {
                return;
            }

            if (status == PlayingStatus.Playing)
            {
                var file = PlayListSource.SequentialSelector.SelectSoundFile();
                Play(file);
                return;
            }

            if (status == PlayingStatus.Fading)
            {
                soundPlayerMixer.StopOldSound();
                PlaybackInformationViewer.UpdatePlaybackInformation(soundPlayers);
            }
        }

        private void Previous()
        {
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePlaybackState();
            PlaybackInformationViewer.UpdatePlaybackInformation(soundPlayers);
        }
    }
}