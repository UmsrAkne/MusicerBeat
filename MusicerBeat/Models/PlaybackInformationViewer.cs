using System.Collections.Generic;
using System.Linq;
using MusicerBeat.Models.Interfaces;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class PlaybackInformationViewer : BindableBase
    {
        private string playingFileName = string.Empty;

        public string PlayingFileName
        {
            get => playingFileName;
            private set => SetProperty(ref playingFileName, value);
        }

        public void UpdatePlaybackInformation(IEnumerable<ISoundPlayer> players)
        {
            var list = players.ToList();
            if (list.Count == 0)
            {
                PlayingFileName = string.Empty;
                return;
            }

            if (list.Count == 1)
            {
                PlayingFileName = list.First().PlayingSound?.NameWithoutExtension;
                PlayingFileName ??= string.Empty;
                return;
            }

            var before = list.First().PlayingSound?.NameWithoutExtension;
            before ??= string.Empty;

            var after = list.Last().PlayingSound?.NameWithoutExtension;
            after ??= string.Empty;

            PlayingFileName = $"{before} --> {after}";
        }
    }
}