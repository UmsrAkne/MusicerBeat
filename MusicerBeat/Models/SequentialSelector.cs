using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class SequentialSelector : BindableBase
    {
        private bool isLoop;

        public SequentialSelector(ReadOnlyObservableCollection<SoundFile> soundFiles)
        {
            SoundFiles = soundFiles;
        }

        public int Index { get; private set; }

        public bool IsLoop
        {
            get => isLoop;
            set => SetProperty(ref isLoop, value);
        }

        private ReadOnlyObservableCollection<SoundFile> SoundFiles { get; set; }

        #nullable enable
        public SoundFile? SelectSoundFile()
        {
            if (SoundFiles.Count == 0 && SoundFiles.All(s => s.IsSkip))
            {
                return null;
            }

            if (!IsLoop && SoundFiles.Skip(Index).All(s => s.IsSkip))
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
            }

            var idx = SoundFiles.ToList().FindIndex(Index, s => !s.IsSkip);
            if (idx == -1)
            {
                idx = SoundFiles.ToList().FindIndex(s => !s.IsSkip);
            }

            Index = idx + 1;
            return SoundFiles[idx];
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

        /// <summary>
        /// 次に選択されるサウンドの長さが、引数に与えられた値以上の長さのサウンドかを判定します。<br/>
        /// このメソッドによって内部のインデックスが変化することはありません。
        /// </summary>
        /// <param name="threshold">長いサウンドかを判定するための基準となる TimeSpan を入力します。</param>
        /// <param name="frontCut">サウンドの冒頭カットの時間を入力します。</param>
        /// <param name="backCut">サウンドの末尾のカットの時間を入力します。</param>
        /// <returns>次に選択されるサウンドの長さが、引数に入力された TimeSpan にカットの時間を加えたものよりも長いかどうか。<br/>
        /// ただし、次に選択されるサウンドが null の場合は false を返します。
        /// </returns>
        public bool NextIsLongSound(TimeSpan threshold, TimeSpan frontCut, TimeSpan backCut)
        {
            var originalIndex = Index;
            var sound = SelectSoundFile();
            Index = originalIndex;
            threshold += frontCut + backCut;

            return sound != null && TimeSpan.FromMilliseconds(sound.TotalMilliSeconds) >= threshold;
        }

        /// <summary>
        /// `CrossFadeSetting` の情報から、次に選択されるサウンドの長さがクロスフェードに必要な長さかどうかを取得します。
        /// </summary>
        /// <param name="setting">クロスフェードの情報が入ったオブジェクト</param>
        /// <returns>次に選択されるサウンドの長さが、クロスフェードに必要な長さかどうか。<br/>
        /// ただし、次に選択されるサウンドが null の場合は false を返します。
        /// </returns>
        public bool NextIsLongSound(CrossFadeSetting setting)
        {
            return NextIsLongSound(setting.RequiredCrossFadeDuration, TimeSpan.Zero, TimeSpan.Zero);
        }

        /// <summary>
        /// このセレクターが保持しているインデックスをリセットします。
        /// </summary>
        public void ResetIndex()
        {
            Index = 0;
        }
    }
}