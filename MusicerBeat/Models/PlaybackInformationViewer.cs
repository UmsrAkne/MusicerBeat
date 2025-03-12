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

        public void UpdatePlaybackInformation(IEnumerable<ISoundPlayer> players)
        {
            var list = players.ToList();
            if (list.Count == 0)
            {
                PlayingFileName = string.Empty;
                PlaybackTimeString = string.Empty;
                return;
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