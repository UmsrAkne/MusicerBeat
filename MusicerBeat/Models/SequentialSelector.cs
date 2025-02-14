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

        /// <summary>
        /// 引数に入力したオブジェクトを内部のリストから検索し、現在のインデックスをその位置の一つ後ろにセットします。 <br/>
        /// 入力したオブジェクトが内部のリストに見つからなかった場合はインデックスのセットをせずに処理を終了します。
        /// </summary>
        /// <param name="soundFile">内部のリストに含まれるオブジェクトを入力します。</param>
        public void SetIndexBySoundFile(SoundFile soundFile)
        {
            var idx = SoundFiles.IndexOf(soundFile);
            if (idx != -1)
            {
                Index = idx + 1;
            }
        }
    }
}