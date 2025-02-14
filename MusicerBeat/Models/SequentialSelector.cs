using System;
using System.Collections.ObjectModel;

namespace MusicerBeat.Models
{
    public class SequentialSelector
    {
        private bool isLoop;

        public SequentialSelector(ReadOnlyObservableCollection<SoundFile> soundFiles)
        {
            SoundFiles = soundFiles;
        }

        public int Index { get; private set; }

        public bool IsLoop { get => isLoop; set => isLoop = value; }

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
                if (!IsLoop)
                {
                    return null;
                }

                Index = 0;
                return SoundFiles[Index++];
            }

            Index = Math.Min(Index, SoundFiles.Count - 1);
            return SoundFiles[Index++];
        }
    }
}