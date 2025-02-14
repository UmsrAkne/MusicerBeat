using System;
using System.Collections.ObjectModel;

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

            if (Index > SoundFiles.Count - 1)
            {
                return null;
            }

            Index = Math.Min(Index, SoundFiles.Count - 1);
            return SoundFiles[Index++];
        }
    }
}