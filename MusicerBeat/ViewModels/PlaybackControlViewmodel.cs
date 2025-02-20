using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using MusicerBeat.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlaybackControlViewmodel : BindableBase, IDisposable
    {
        private readonly ISoundPlayerFactory soundPlayerFactory;
        private readonly List<ISoundPlayer> soundPlayers = new ();

        private readonly DispatcherTimer timer;

        public PlaybackControlViewmodel(IPlaylist playlist, ISoundPlayerFactory soundPlayerFactory)
        {
            this.soundPlayerFactory = soundPlayerFactory;
            PlayListSource = playlist;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200), };
            timer.Tick += Timer_Tick;
            VolumeController = new VolumeController();
        }

        public bool IsPlaying { get; set; }

        public VolumeController VolumeController { get; set; }

        public DelegateCommand<SoundFile> PlayCommand => new (Play);

        public TimeSpan CrossFadeDuration { get; set; } = TimeSpan.FromSeconds(10);

        private IPlaylist PlayListSource { get; init; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                foreach (var p in soundPlayers)
                {
                    p.Stop();
                    var d = p as IDisposable;
                    d?.Dispose();
                }

                soundPlayers.Clear();
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
        }

        private void RemoveAndPlay(object sender, EventArgs e)
        {
            if (sender is ISoundPlayer p)
            {
                soundPlayers.Remove(p);
            }

            Play(null);
        }

        private void Stop()
        {
        }

        private void Next()
        {
        }

        private void Previous()
        {
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
        }
    }
}