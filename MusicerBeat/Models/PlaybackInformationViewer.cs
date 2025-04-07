using System.Collections.Generic;
using System.Linq;
using MusicerBeat.Models.Interfaces;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class PlaybackInformationViewer : BindableBase
    {
        private readonly string shortTimeFormat = @"hh\:mm\:ss";
        private string playingFileName = string.Empty;
        private string playbackTimeString = string.Empty;
        private int currentSoundLength;
        private int currentSoundPosition;

        public string PlayingFileName
        {
            get => playingFileName;
            private set => SetProperty(ref playingFileName, value);
        }

        public string PlaybackTimeString
        {
            get => playbackTimeString;
            set => SetProperty(ref playbackTimeString, value);
        }

        /// <summary>
        /// 現在再生中のサウンドの長さを秒数で取得します。
        /// </summary>
        public int CurrentSoundLength
        {
            get => currentSoundLength;
            private set => SetProperty(ref currentSoundLength, value);
        }

        /// <summary>
        /// 現在再生中のサウンドの再生位置を秒数で取得します。
        /// </summary>
        public int CurrentSoundPosition
        {
            get => currentSoundPosition;
            set => SetProperty(ref currentSoundPosition, value);
        }

        public void UpdatePlaybackInformation(IEnumerable<ISoundPlayer> players)
        {
            var list = players.ToList();
            if (list.Count == 0)
            {
                PlayingFileName = string.Empty;
                PlaybackTimeString = string.Empty;
                CurrentSoundLength = 0;
                CurrentSoundPosition = 0;
                return;
            }

            if (list.Count >= 1)
            {
                CurrentSoundLength = (int)list.First().Duration.TotalSeconds;
                CurrentSoundPosition = (int)list.First().CurrentTime.TotalSeconds;
                System.Diagnostics.Debug.WriteLine($"{CurrentSoundLength}(PlaybackInformationViewer : 57)");
                System.Diagnostics.Debug.WriteLine($"{CurrentSoundPosition}(PlaybackInformationViewer : 58)");
            }

            if (list.Count == 1)
            {
                PlaybackTimeString = list.First().CurrentTime.ToString(shortTimeFormat);
                PlayingFileName = list.First().PlayingSound?.NameWithoutExtension;
                PlayingFileName ??= string.Empty;
                return;
            }

            var beforeTime = list.First().CurrentTime.ToString(shortTimeFormat);
            var before = list.First().PlayingSound?.NameWithoutExtension;
            before ??= string.Empty;

            var afterTime = list.Last().CurrentTime.ToString(shortTimeFormat);
            var after = list.Last().PlayingSound?.NameWithoutExtension;
            after ??= string.Empty;

            PlayingFileName = $"{before} --> {after}";
            PlaybackTimeString = $"{beforeTime} --> {afterTime}";
        }
    }
}