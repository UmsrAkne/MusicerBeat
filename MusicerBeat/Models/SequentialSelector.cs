using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicerBeat.Models
{
    public class SequentialSelector
    {
        public SequentialSelector(ReadOnlyObservableCollection<SoundFile> soundFiles)
        {
            SoundFiles = soundFiles;
        }

        public int Index { get; private set; }

        private ReadOnlyObservableCollection<SoundFile> SoundFiles { get; set; }

        #nullable enable
        public SoundFile? SelectSoundFile()
        {
            if (SoundFiles.Count == 0)
            {
                return null;
            }

            if (SoundFiles.Count == 1)
            {
                return SoundFiles.First();
            }

            if (Index > SoundFiles.Count - 1)
            {
                return null;
            }

            Index = Math.Min(Index, SoundFiles.Count - 1);
            return SoundFiles[Index++];
        }
    }
}